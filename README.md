# Equiparts

Equiparts is a .NET MAUI mobile app for an industrial parts business: browse categories/products, manage a cart, check out and view orders, authenticate, and manage a profile.

- **Application Id:** `com.equipartsgroup.equiparts`
- **Root namespace:** `Equiparts`
- **Active target framework:** `net10.0-android` only (iOS/MacCatalyst/Windows are commented out in `Equiparts.csproj`)

## Prerequisites

- .NET SDK with the MAUI Android workload (`dotnet workload install maui-android`)
- Android SDK/emulator or a connected device for running/deploying

There is no `.sln` file and no test project in this repo — it's a single MAUI project.

## Building

```bash
# Restore + build (Android is the only active target framework)
dotnet build Equiparts.csproj -f net10.0-android

# Build a specific configuration
dotnet build Equiparts.csproj -f net10.0-android -c Release

# Run on a connected device/emulator
dotnet build Equiparts.csproj -f net10.0-android -t:Run
```

## Architecture overview

See [CLAUDE.md](CLAUDE.md) for a detailed architecture guide (DI composition root, Shell navigation, MVVM conventions, the Refit API layer, auth/session/connectivity services, and theming). In short:

- **DI composition root** — `MauiProgram.CreateMauiApp()` wires services, view models, views, and Refit clients via extension methods in `Extensions/`.
- **Navigation** — Shell-based (`AppShell.xaml`), routed through `INavigationService` rather than `Shell.Current` directly.
- **MVVM** — `BaseViewModel`/`BasePage` base classes, CommunityToolkit.Mvvm source generators (`[ObservableProperty]`, `[RelayCommand]`).
- **API layer** — Refit interfaces in `Interfaces/`, backend DTOs under `Models/<Area>/` mapped to app-facing models via `Configuration/Mapper/BackendToAppModelMapper`.
- **Auth/session** — `TokenService`, `ConnectivityService`, `CurrentUserService` under `Services/`.

## Release signing (Android)

This app is signed for release using an Android keystore. **The keystore file and its passwords are not stored in this repository** and must never be committed to source control or written into documentation — a signing key compromise cannot be undone (Google Play ties app updates to it permanently).

To build a signed release locally, supply the signing values via MSBuild properties (e.g. a local, gitignored `local.properties`/`*.pubxml`, environment variables, or your CI secret store) rather than editing `Equiparts.csproj` directly:

```bash
dotnet build Equiparts.csproj -f net10.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=/path/to/your.keystore \
  -p:AndroidSigningKeyAlias=<key-alias> \
  -p:AndroidSigningStorePass=<store-password> \
  -p:AndroidSigningKeyPass=<key-password>
```

Store the actual keystore file and passwords in a password manager or your CI/CD secrets store (e.g. GitHub Actions secrets), and share access with teammates through that channel — not via chat, README files, or commits.
