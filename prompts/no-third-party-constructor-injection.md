# Prompt: No Third-Party Constructor Injection for ASP.NET MVC 5 / Web API 2

Set up constructor injection in this classic ASP.NET MVC 5 / Web API 2 application without adding any third-party DI container.

Requirements:

- Keep DI composition inside the web application project, not inside `ChuA.DatabaseLegacy` or provider packages.
- Use built-in MVC/Web API resolver hooks:
  - `System.Web.Mvc.IDependencyResolver`
  - `System.Web.Http.Dependencies.IDependencyResolver`
- Add a small in-app container capable of:
  - registering singleton services by factory
  - constructing concrete controller types through public constructors
  - resolving constructor parameters from registered services
  - returning `null` for framework or unknown services it cannot resolve
- Register `IChuADatabase` as an application singleton.
- Build `IChuADatabase` through `ChuA.DatabaseLegacy` using `ChuADatabase.CreateSqlServerFromAppSettings()`.
- Configure the connection through the `ChuA.DatabaseLegacy` app-settings keys:
  - `ChuA.DatabaseLegacy.Provider`
  - `ChuA.DatabaseLegacy.ConnectionString`
  - `ChuA.DatabaseLegacy.DatabaseName`
  - `ChuA.DatabaseLegacy.CommandTimeoutSeconds`
  - `ChuA.DatabaseLegacy.EnableLogging`
- Do not register `ChuADatabaseScope` as a singleton. Controllers or services should create a short-lived scope with `IChuADatabase.CreateScope()` per operation.
- Call dependency registration from `Global.asax.Application_Start()` before MVC/Web API routes are used.
- Dispose the singleton container from `Global.asax.Application_End()`.
- Add the `ChuA.DatabaseLegacy.*` settings to `Web.config`.
- Do not add Simple Injector, Autofac, Ninject, Unity, Microsoft.Extensions.DependencyInjection, or any other DI library.

Expected project shape:

- `App_Start/DependencyConfig.cs`
- `Infrastructure/SimpleContainer.cs`
- `Infrastructure/AppDependencyResolver.cs`
- Updated `Global.asax.cs`
- Updated `Web.config`
- Updated `.csproj` compile/content entries if the project uses explicit file includes
