import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

const BASE_URL = 'http://193.39.208.106:8080';
const ADMIN_PASSWORD = 'secret';

const errorRate = new Rate('error_rate');
const authDuration = new Trend('auth_duration');
const transactionDuration = new Trend('transaction_duration');
const successCount = new Counter('success_operations');

export const options = {
    stages: [
        { duration: '5s', target: 15 },   
        { duration: '10s', target: 30 },  
        { duration: '5s', target: 0 },    
    ],
    thresholds: {
        'http_req_duration': ['p(95)<1000'],
        'http_req_failed': ['rate<0.05'],
        'error_rate': ['rate<0.1'],
    },
    gracefulStop: '10s',
};

function registerUser(pin, password) {
    const payload = JSON.stringify({ pin: pin, password: password });
    
    const response = http.post(`${BASE_URL}/api/accounts/register`, payload, {
        headers: { 'Content-Type': 'application/json' },
        tags: { name: 'Register' }
    });
    
    const success = check(response, {
        'Register - status 200': (r) => r.status === 200,
        'Register - has id': (r) => {
            try {
                const body = JSON.parse(r.body);
                return body.id && body.id.length > 0;
            } catch {
                return false;
            }
        },
    });
    
    if (success) {
        successCount.add(1);
        const body = JSON.parse(response.body);
        return body.id;
    } else {
        errorRate.add(1);
        console.error(`Register failed: ${response.status}`);
        return null;
    }
}

function getUserTokenAtm(accountId, pin) {
    const startTime = new Date();
    const payload = JSON.stringify({ accountId: accountId, pin: pin });
    
    const response = http.post(`${BASE_URL}/api/auth/user/atm`, payload, {
        headers: { 'Content-Type': 'application/json' },
        tags: { name: 'UserAuthAtm' }
    });
    
    authDuration.add(new Date() - startTime);
    
    const success = check(response, {
        'User auth - status 200': (r) => r.status === 200,
        'User auth - has token': (r) => r.body && r.body.length > 50,
    });
    
    if (success) {
        successCount.add(1);
        return response.body;
    } else {
        errorRate.add(1);
        return null;
    }
}

function getBalance(userToken, accountId) {
    const startTime = new Date();
    const response = http.get(`${BASE_URL}/api/accounts/balance?id=${accountId}`, {
        headers: { 'Authorization': `Bearer ${userToken}` },
        tags: { name: 'GetBalance' }
    });
    
    transactionDuration.add(new Date() - startTime);
    
    const success = check(response, {
        'Get balance - status 200': (r) => r.status === 200,
    });
    
    if (success) {
        successCount.add(1);
        const body = JSON.parse(response.body);
        return body.balance;
    } else {
        errorRate.add(1);
        return null;
    }
}


function deposit(userToken, accountId, amount) {
    const startTime = new Date();
    const payload = JSON.stringify({ amount: amount });
    
    const response = http.post(`${BASE_URL}/api/accounts/deposit/atm`, payload, {
        headers: {
            'Authorization': `Bearer ${userToken}`,
            'Content-Type': 'application/json'
        },
        tags: { name: 'Deposit' }
    });
    
    transactionDuration.add(new Date() - startTime);
    
    const success = check(response, {
        'Deposit - status 204': (r) => r.status === 204,
    });
    
    if (success) {
        successCount.add(1);
        return true;
    } else {
        errorRate.add(1);
        return false;
    }
}


function withdraw(userToken, accountId, amount) {
    const startTime = new Date();
    const payload = JSON.stringify({ amount: amount });
    
    const response = http.post(`${BASE_URL}/api/accounts/withdraw/atm`, payload, {
        headers: {
            'Authorization': `Bearer ${userToken}`,
            'Content-Type': 'application/json'
        },
        tags: { name: 'Withdraw' }
    });
    
    transactionDuration.add(new Date() - startTime);
    
    const success = check(response, {
        'Withdraw - status 204': (r) => r.status === 204,
    });
    
    if (success) {
        successCount.add(1);
        return true;
    } else {
        errorRate.add(1);
        return false;
    }
}


function healthCheck() {
    const response = http.get(`${BASE_URL}/health`);
    check(response, {
        'Health check - status 200': (r) => r.status === 200,
    });
}


export default function () {
    const testPin = Math.floor(1000 + Math.random() * 9000).toString();
    const testPassword = `Pass${Math.random().toString(36).substring(7)}`;
    

    group('Health Check', function () {
        healthCheck();
        sleep(0.3);
    });
    

    let accountId = null;
    group('Register User', function () {
        accountId = registerUser(testPin, testPassword);
        if (!accountId) {
            console.error('Failed to register');
            return;
        }
        console.log(`Created account: ${accountId} with PIN: ${testPin}`);
        sleep(0.3);
    });
    
    if (!accountId) return;
    
    // Аутентификация
    let userToken = null;
    group('Authenticate User', function () {
        userToken = getUserTokenAtm(accountId, testPin);
        if (!userToken) {
            console.error('Failed to authenticate');
            return;
        }
        sleep(0.3);
    });
    
    if (!userToken) return;
    

    group('Money Operations', function () {

        const depositSuccess = deposit(userToken, accountId, 500);
        if (depositSuccess) {
            console.log('Deposit 500 OK');
        }
        sleep(0.3);
        

        const balance = getBalance(userToken, accountId);
        console.log(`Balance: ${balance}`);
        sleep(0.3);
        

        const withdrawSuccess = withdraw(userToken, accountId, 200);
        if (withdrawSuccess) {
            console.log('Withdraw 200 OK');
        }
        sleep(0.3);
        

        const finalBalance = getBalance(userToken, accountId);
        console.log(`Final balance: ${finalBalance}`);
    });
}


export function setup() {
    console.log('Starting API load test...');
    const healthResponse = http.get(`${BASE_URL}/health`);
    if (healthResponse.status !== 200) {
        console.error(`Server not available at ${BASE_URL}`);
        return { error: 'Server not available' };
    }
    console.log('Server is ready');
    return { setupData: 'OK' };
}

export function teardown(data) {
    console.log('Test completed!');
    console.log(`Total successful operations: ${successCount.values?.count || 0}`);
}
