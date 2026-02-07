# Deployment

## Local
- Install .NET 10 preview SDK and Windows App SDK 1.8+.
- Configure secrets using user-secrets or environment variables.
- Run EF Core migrations.

## CI
GitHub Actions runs build, tests, and format verification on push/PR to `main` and `develop`.

## Notes
- TODO: Add MSIX packaging pipeline.
- TODO: Add code signing configuration.
