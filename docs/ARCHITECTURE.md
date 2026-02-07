# Architecture

## Overview
The solution follows a layered architecture with a WinUI 3 front-end and a set of domain/application/infrastructure layers.

## Layers
- **Domain**: Entities, value objects, and domain rules.
- **Application**: Use-cases, DTOs, and service interfaces.
- **Infrastructure**: EF Core, repositories, integrations (OCR/AI), and logging.
- **WinUI**: UI, MVVM view models, app bootstrap, and navigation.

## Cross-cutting
- Logging with Serilog.
- Resilience with Polly.
- Mapping with AutoMapper.
- DI with Microsoft.Extensions.Hosting.
