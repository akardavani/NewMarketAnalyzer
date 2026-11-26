# Dependency injection and provider registration

## Host composition order

`backend/MarketAnalyzer.Api/Program.cs` calls the MarketData registration extensions in this order:

```csharp
builder.Services.AddMarketDataStores();
builder.Services.AddMarketDataIngestion();
builder.Services.AddMarketDataProviders(builder.Configuration);
builder.Services.AddMarketDataOperations(builder.Configuration);
```

Stores/ingestor/settings/resolver are scoped because they depend on scoped persistence. Calendar, validator, aggregator, drift detector, default settings, and `TimeProvider.System` are singleton where appropriate. `MarketDataJobRunner` and gate are singleton and create scopes for database work; `MarketDataScheduler` is a hosted service.

## Provider registrations

`ProvidersRegistration.AddMarketDataProviders` is the central registration file. It configures typed `HttpClient` timeouts, registers `IProviderSettingDefaults` as singleton, and calls the local `AddProvider<TProvider>` helper. That helper registers the concrete provider and a corresponding `IMarketDataProvider` factory as scoped. The `IMarketDataProvider` collection is enumerated by resolver and bootstrap. Registration is guarded by a marker to be idempotent.

The providers are registered independently from selecting an active provider. Selection is stored in `MarketSettings` and resolved by capability/market. `MarketBootstrapService` inserts a default choice only when multiple candidates exist and exactly one declares itself default. It never overwrites an operator-selected DB value.

## Representative registration excerpt

```csharp
services.AddHttpClient<NahayatnegarCandleClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(30));
services.AddSingleton<IProviderSettingDefaults, NahayatnegarSettingDefaults>();
AddProvider<NahayatnegarStockCandleProvider>(services);

services.AddHttpClient<FipiranClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(60));
AddProvider<FipiranFundCatalogProvider>(services);
AddProvider<FipiranFundNavProvider>(services);

services.AddHttpClient<KucoinClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(30));
AddProvider<KucoinCatalogProvider>(services);
AddProvider<KucoinCandleProvider>(services);
```

This excerpt is taken from `ProvidersRegistration.cs`; the exact complete file is copied into `code-samples/registration/`. The registration helper is:

```csharp
private static void AddProvider<TProvider>(IServiceCollection services)
    where TProvider : class, IMarketDataProvider
{
    services.AddScoped<TProvider>();
    services.AddScoped<IMarketDataProvider>(sp => sp.GetRequiredService<TProvider>());
}
```

## Provider resolution and settings

`MarketProviderResolver` is scoped. It filters the injected registered provider list by capability interface and `Descriptor.MarketKey`, then reads `Provider:<Capability>` from `market:<market-key>`. If a stored choice no longer maps to a provider, it logs and returns null. When there is exactly one candidate and no choice, it returns that candidate; multiple candidates without a selection are an error and do not trigger fallback.

`ProviderSettings` is scoped and caches `(providerKey, settingKey)` values for the job scope. It reads the DB value before code defaults. `IProviderSettingDefaults` registrations are singleton. Provider descriptors themselves are exposed from provider instances and are consumed during bootstrap.

## Scoped parallel candle workers

The candle runner resolves the selected provider to determine the provider key, then creates a fresh scope for each parallel symbol. Within that scope it locates the concrete matching provider from `IEnumerable<IMarketDataProvider>` and invokes the scoped ingestor. This keeps scoped provider dependencies and `DbContext` from being shared across workers.
