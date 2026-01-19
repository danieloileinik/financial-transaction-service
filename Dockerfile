FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY FinancialTransactionService.sln .
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Presentation/Presentation.csproj src/Presentation/

RUN dotnet restore src/Presentation/Presentation.csproj

COPY src/ ./src/

WORKDIR /app/src/Presentation
RUN dotnet publish Presentation.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Presentation.dll"]