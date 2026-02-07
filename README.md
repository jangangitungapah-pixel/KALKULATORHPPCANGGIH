# KALKULATORHPPCANGGIH

KALKULATORHPPCANGGIH is a WinUI 3 (.NET 10) desktop application that implements an advanced Harga Pokok Penjualan (HPP) calculator with modular architecture, MVVM, EF Core, and pluggable OCR/AI adapters.

## Architecture
- **Hpp.WinUI**: WinUI 3 UI shell, MVVM view models, and app bootstrap.
- **Hpp.Application**: Use-cases, DTOs, service interfaces, and orchestration.
- **Hpp.Domain**: Entities, value objects, domain rules.
- **Hpp.Infrastructure**: EF Core, repositories, integrations, logging.
- **Hpp.Shared**: Shared primitives.

## Local Development
> TODO: Install .NET 10 SDK preview and Windows App SDK 1.8+. Ensure Visual Studio 2022 preview components are installed.

### Commands
```bash
dotnet restore
```
```bash
dotnet build
```
```bash
dotnet ef database update --project src/Hpp.Infrastructure --startup-project src/Hpp.WinUI
```
```bash
dotnet run --project src/Hpp.WinUI
```
```bash
dotnet test
```

## Secrets & Configuration
Never commit secrets. Use `dotnet user-secrets` or environment variables.
- `OPENAI_API_KEY` (TODO)
- `FORM_RECOGNIZER_KEY` (TODO)
- `DB_CONN` (TODO)

An example configuration file is provided at `appsettings.Development.json.example` inside `src/Hpp.WinUI`.

## GitHub Repo Setup
```bash
git init
git checkout -b develop
git add . && git commit -m "chore: initial scaffold"
```
```bash
gh repo create <owner>/KALKULATORHPPCANGGIH --public --source=. --remote=origin
```
```bash
git push -u origin develop
```

## Notes
- TODO: Configure Azure OpenAI/Form Recognizer keys in user-secrets.
- TODO: Configure SQL Server connection string for EF Core.
- TODO: Finalize WinUI visuals and theme palette.
