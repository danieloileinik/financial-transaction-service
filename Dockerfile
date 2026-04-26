FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY FinancialTransactionService.sln .
COPY src/FinancialTransactionService.Domain/FinancialTransactionService.Domain.csproj src/FinancialTransactionService.Domain/
COPY src/FinancialTransactionService.Application/FinancialTransactionService.Application.csproj src/FinancialTransactionService.Application/
COPY src/FinancialTransactionService.Infrastructure/FinancialTransactionService.Infrastructure.csproj src/FinancialTransactionService.Infrastructure/
COPY src/FinancialTransactionService.Presentation/FinancialTransactionService.Presentation.csproj src/FinancialTransactionService.Presentation/

RUN dotnet restore src/FinancialTransactionService.Presentation/FinancialTransactionService.Presentation.csproj

COPY src/ ./src/

RUN dotnet ef migrations bundle \
    --project src/Infrastructure/ConversionReporter.Infrastructure \
    --startup-project src/Presentation/ConversionReporter.Presentation \
    -o /app/publish/efbundle
    
WORKDIR /app/src/FinancialTransactionService.Presentation
RUN dotnet publish FinancialTransactionService.Presentation.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FinancialTransactionService.Presentation.dll"]
