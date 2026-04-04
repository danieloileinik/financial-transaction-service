FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/FinancialTransactionService.Tests/FinancialTransactionService.Tests.csproj", "tests/FinancialTransactionService.Tests/"]
COPY ["src/FinancialTransactionService.Presentation/FinancialTransactionService.Presentation.csproj", "src/FinancialTransactionService.Presentation/"]
COPY ["src/FinancialTransactionService.Application/FinancialTransactionService.Application.csproj", "src/FinancialTransactionService.Application/"]
COPY ["src/FinancialTransactionService.Domain/FinancialTransactionService.Domain.csproj", "src/FinancialTransactionService.Domain/"]
COPY ["src/FinancialTransactionService.Infrastructure/FinancialTransactionService.Infrastructure.csproj", "src/FinancialTransactionService.Infrastructure/"]
RUN dotnet restore "tests/FinancialTransactionService.Tests/FinancialTransactionService.Tests.csproj"
COPY . .
WORKDIR "/src/tests/FinancialTransactionService.Tests"
RUN dotnet build "./FinancialTransactionService.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinancialTransactionService.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialTransactionService.Tests.dll"]
