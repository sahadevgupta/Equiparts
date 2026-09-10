# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Equiparts is a .NET MAUI mobile app (single project, no `.sln`, no test project). It's an e-commerce/catalog app for an industrial parts business: browse categories/products, cart, checkout/orders, auth, and a profile section. Root namespace: `Equiparts`.

Only `net10.0-android` is currently enabled in `Equiparts.csproj`; iOS/MacCatalyst/Windows target frameworks are commented out.

## Commands

```bash
# Restore + build (Android is the only active target framework)
dotnet build Equiparts.csproj -f net10.0-android

# Build a specific configuration
dotnet build Equiparts.csproj -f net10.0-android -c Release

# Run on a connected device/emulator
dotnet build Equiparts.csproj -f net10.0-android -t:Run
```

There is no test project and no solution file in this repo — don't assume `dotnet test` works.

## Architecture

### DI composition root

`MauiProgram.CreateMauiApp()` wires everything up by calling four extension methods, each in `Extensions/`:
- `RegisterAppServices` (`AppServiceInitializer`) — services, handlers, connectivity/auth singletons, platform-specific `INativeLoadingOverlay`.
- `RegisterViewModels` (`ViewModelInitializer`) — every `*ViewModel` as transient.
- `RegisterViews` (`ViewInitializer`) — only `AppShell` is registered (resolved once in `App.CreateWindow`); individual `Page`s are NOT in DI — Shell resolves them via `ContentTemplate="{DataTemplate views:XPage}"` in `AppShell.xaml`.
- `RegisterRefitClients` (`RefitClientInitializer`) — Refit source-generated API clients (see below).

When adding a new service/viewmodel/API client, register it in the matching `Extensions/*Initializer.cs` file rather than inline in `MauiProgram`.

### Navigation (Shell)

`AppShell.xaml` defines the routes: a `loading` splash route, a `LoginPage` route, and a `TabBar` ("app") with Home/Catalog/Cart/Profile tabs (Orders tab is commented out/disabled). Pages pushed on top of a tab (`ProductPage`, `ProductDetailPage`, `RegisterPage`, `ChangePasswordPage`) need an explicit `Routing.RegisterRoute` call in `AppShell.xaml.cs` — tab-root pages don't.

App startup/session routing lives in `AppLifeCycleCoordinator` (`Services/AppLifeCycleCoordinator.cs`), invoked from `App.CreateWindow`'s `window.Created` handler: it shows `//loading` first, then `NavigateToInitialDestinationAsync()` checks `ITokenService.HasStoredSessionAsync()` to decide between `//app/home` and `LoginPage`.

Use `INavigationService` (`Services/NavigationService.cs`) from view models rather than calling `Shell.Current` directly — it wraps `GoToAsync` with root (`//`), replace (`../`), and back-navigation (`..`, `../..`) route conventions.

### MVVM pattern

- Every view model extends `BaseViewModel` (CommunityToolkit.Mvvm `ObservableObject`), which exposes `IsBusy`/`Title` and a `LoadDataOnNavigatedTo()` hook.
- Every page extends `BasePage` (not `ContentPage` directly): it injects a shared `OfflineBannerView` above page content via the `PageContent` bindable property (`<views:BasePage.PageContent>...</views:BasePage.PageContent>` in XAML, not plain `Content`), and calls `LoadDataOnNavigatedTo()` on the bound view model from `OnNavigatedTo` when the page was pushed/replaced.
- `[ObservableProperty]` / `[RelayCommand]` (CommunityToolkit.Mvvm source generators) are used throughout instead of hand-written `INotifyPropertyChanged`/`ICommand` boilerplate.
- View models that need navigation parameters implement `IQueryAttributable.ApplyQueryAttributes` (e.g. `ProductViewModel` reads `CategoryId`/`Title`).

### API layer (Refit) and data mapping

- API contracts are Refit interfaces in `Interfaces/` (`IAuthApi`, `ICatalogApi`, `ICartApi`, `IOrderApi`, `IProfileApi`), registered via `AddRefitGeneratedClient` (compile-time source-generated — AOT/trim friendly — not the reflection-based `AddRefitClient`). All share `ApiConstants.BaseUrl`.
  - `IAuthApi` and `ICatalogApi` are unauthenticated (no `AuthHandler`) — catalog browsing is anonymous, and wiring `AuthHandler` onto auth endpoints would recurse into itself during refresh.
  - `IProfileApi`, `IOrderApi`, `ICartApi` go through `AuthHandler` (`Handlers/AuthHandler.cs`), which attaches the bearer token, and on a 401 forces a token refresh and retries the request once (via a cloned/rebuilt `HttpRequestMessage`).
  - In `DEBUG`, all clients also get `HttpMessageLogHandler` (defined inline in `RefitClientInitializer`) for verbose request/response logging.
- Backend responses use dedicated `*Request`/`*Response` DTOs under `Models/<Area>/` (e.g. `Models/Catalog/ProductResponse.cs`), separate from the app-facing domain models directly under `Models/` (e.g. `Models/Product.cs`). Responses are wrapped in `ApiResult<T>` (`Success`/`Data`), and paged endpoints return `PagedResult<T>`.
- Conversion from `*Response` DTOs to domain models goes through `Configuration/Mapper/BackendToAppModelMapper` (static facade), which delegates to one `IConverter` implementation per type under `Configuration/Mapper/Converters/`. Add a new converter there (implementing `ConverterBase`/`IConverter`) and a corresponding `Get*` method on `BackendToAppModelMapper` when mapping a new response type — don't inline mapping logic in services.
- Services (`Services/*Service.cs`, one per `Interfaces/I*Service.cs`) are the layer view models call; they call the Refit API, call `IConnectivityService.CheckInternetAccessAsync()` first, map via `BackendToAppModelMapper`, and swallow `ApiException` into empty/default results with a logged warning rather than throwing into the view model.

### Auth/session/connectivity

- `TokenService` (`Services/TokenService.cs`) stores access/refresh tokens + expiry in `SecureStorage`, single-flights concurrent refreshes behind a `SemaphoreSlim`, and treats a missing/unparseable expiry as expired. It raises `SessionExpired` when a refresh is rejected by the server or has no refresh token to use.
- `ConnectivityService` (`Services/ConnectivityService.cs`) doesn't trust OS-reported connectivity alone — it also polls an unauthenticated reachability probe URL every 20s (and on OS connectivity change events) to detect networks with no real internet route.
- `CurrentUserService` holds the in-memory authenticated user profile (id/name/email/roles) set on login from a `UserSession`, cleared on logout.
- The offline banner (`Controls/OfflineBannerView.xaml`) is wired into every page automatically via `BasePage`, not added per-page.

### Platform-specific code

`Platforms/{Android,iOS,MacCatalyst,Windows}/` hold per-platform `INativeLoadingOverlay` implementations (registered conditionally by `#if ANDROID`/`#if IOS`/etc. in `AppServiceInitializer`) and other platform entry points (`MainActivity.cs`, `AndroidManifest.xml`, platform `Handlers/`, `Services/`).

### Styling/theming

Shared design tokens live in `Resources/Styles/Colors.xaml` and `Resources/Styles/Styles.xaml`, and fonts are registered in `MauiProgram` as `OpenSansRegular`/`OpenSansSemibold`/`FontAwesome` (`Resources/Fonts/`). New pages/controls should reuse these rather than defining new colors/styles/fonts inline.
