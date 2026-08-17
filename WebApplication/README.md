# WebApplication Layer Documentation

## 1. Layer Overview

### Purpose
The WebApplication layer is the presentation layer of the application. It contains controllers, views, middleware, filters, attributes, and configuration for the ASP.NET Core MVC web application. It handles HTTP requests, responses, authentication, authorization, and user interface.

### Position
This is the outermost layer in the Clean Architecture. It depends on all other layers (Domain, Application, Infrastructure) and contains the entry point (Program.cs) for the application.

### Dependencies
- **Depends on**: Domain layer, Application layer, Infrastructure layer
- **External dependencies**: AutoMapper (16.2.0), Microsoft.EntityFrameworkCore.Sqlite (10.0.2), Microsoft.EntityFrameworkCore.Tools (10.0.2), NuGet.Packaging (6.12.5), NuGet.Protocol (6.12.5), SQLitePCLRaw.bundle_e_sqlite3 (3.0.3), Microsoft.VisualStudio.Azure.Containers.Tools.Targets (1.23.0), Microsoft.VisualStudio.Web.CodeGeneration.Design (10.0.2)
- **Depended by**: None (this is the presentation/entry layer)

### Key Principles
- **Separation of Concerns**: Presentation logic separated from business logic
- **Middleware Pipeline**: Request processing through middleware chain
- **Area Organization**: Features organized by areas (Admin, Identity)
- **Security-First**: Multiple security layers (authentication, authorization, headers, rate limiting)
- **Localization**: Built-in support for Arabic and English languages

---

## 2. Directory Structure

```
WebApplication/
├── Areas/
│   ├── Admin/ (14 items)
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── _ViewStart.cshtml
│   ├── Identity/ (72 items)
│   │   ├── Pages/Account/
│   │   └── _ViewStart.cshtml
│   └── CustomErrorController.cs
├── Attributes/
│   ├── AdminAuthorize.cs
│   ├── DecimalAttribute.cs
│   ├── FullNameMinPartsAttribute.cs
│   ├── IgnoreAction.cs
│   ├── InThePast.cs
│   ├── IntAttribute.cs
│   ├── LocalizedMaxLength.cs
│   ├── LocalizedMinLength.cs
│   ├── LocalizedRequired.cs
│   ├── MemberAuthorize.cs
│   ├── MemberOrAdminAuthorize.cs
│   ├── NotInThePast.cs
│   └── UniqueAttribute.cs
├── Extensions/
│   ├── MvcExtensions.cs
│   └── PipelineExtensions.cs
├── FiltersAttributes/
│   ├── AjaxOnly.cs
│   ├── ClaimRequirementAnyAttribute.cs
│   └── ClaimRequirementAttribute.cs
├── Helpers/
│   ├── SessionHelper.cs
│   ├── SelectListHelper.cs
│   └── RadioButtonHelper.cs
├── Hub/
│   └── (SignalR hubs if any)
├── Mapper/
│   └── (AutoMapper profiles if any)
├── Middleware/
│   ├── LogsHistoryMiddleware.cs
│   ├── MaintenanceMiddleware.cs
│   └── NotificationMiddleware.cs
├── ModelBinders/
│   └── (Custom model binders if any)
├── Models/
│   └── (ViewModels if any)
├── Program.cs
├── appsettings.json
├── web.config
├── WebApplication.csproj
├── Properties/
│   └── launchSettings.json
├── Resources/
│   └── (Localization resource files)
├── Views/
│   └── (Shared views)
└── wwwroot/ (34 items)
    ├── (Static assets: CSS, JS, images, etc.)
    └── uploads/
```


---

## 3. Subfolder Summaries

### Areas/
- **Folder Name**: Areas
- **Purpose**: Organizes application into functional areas (Admin, Identity)
- **Contents**: Admin area (controllers, views), Identity area (Razor Pages), CustomErrorController
- **Relationships**: Each area is self-contained with its own controllers and views. CustomErrorController handles global errors.

### Attributes/
- **Folder Name**: Attributes
- **Purpose**: Custom validation and authorization attributes
- **Contents**: Authorization attributes (AdminAuthorize, MemberAuthorize, etc.), validation attributes (Decimal, Int, Localized, etc.)
- **Relationships**: Used by controllers and view models for declarative validation and authorization.

### Extensions/
- **Folder Name**: Extensions
- **Purpose**: Extension methods for service and configuration registration
- **Contents**: MvcExtensions (MVC configuration), PipelineExtensions (middleware pipeline)
- **Relationships**: Called from Program.cs for organized configuration.

### FiltersAttributes/
- **Folder Name**: FiltersAttributes
- **Purpose**: Custom action and authorization filters
- **Contents**: AjaxOnly, ClaimRequirementAttribute, ClaimRequirementAnyAttribute
- **Relationships**: Applied to controllers/actions for cross-cutting concerns.

### Helpers/
- **Folder Name**: Helpers
- **Purpose**: View and session helper classes
- **Contents**: SessionHelper, SelectListHelper, RadioButtonHelper
- **Relationships**: Used in views and controllers for common UI operations.

### Middleware/
- **Folder Name**: Middleware
- **Purpose**: Custom middleware for request processing
- **Contents**: LogsHistoryMiddleware, MaintenanceMiddleware, NotificationMiddleware
- **Relationships**: Registered in PipelineExtensions. Processes all requests.

### Hub/
- **Folder Name**: Hub
- **Purpose**: SignalR hubs for real-time communication
- **Contents**: SignalR hub classes (if any)
- **Relationships**: Used for real-time features like notifications.

### Mapper/
- **Folder Name**: Mapper
- **Purpose**: AutoMapper profiles for view model mapping
- **Contents**: AutoMapper profile classes (if any)
- **Relationships**: Configured in service registration. Maps entities to view models.

### ModelBinders/
- **Folder Name**: ModelBinders
- **Purpose**: Custom model binders for request binding
- **Contents**: Custom model binder classes (if any)
- **Relationships**: Registered in MVC configuration. Custom request parameter binding.

### Models/
- **Folder Name**: Models
- **Purpose**: View models and input models
- **Contents**: View model classes for views
- **Relationships**: Used by controllers and views to transfer data.

### Properties/
- **Folder Name**: Properties
- **Purpose**: Project properties and launch settings
- **Contents**: launchSettings.json
- **Relationships**: Configures development launch profile.

### Resources/
- **Folder Name**: Resources
- **Purpose**: Localization resource files
- **Contents**: .resx files for different languages
- **Relationships**: Used by localization middleware and validation attributes.

### Views/
- **Folder Name**: Views
- **Purpose**: Shared Razor views and layouts
- **Contents**: _ViewStart, _ViewImports, shared partials, layouts
- **Relationships**: Used by all controllers for consistent UI.

### wwwroot/
- **Folder Name**: wwwroot
- **Purpose**: Static web assets
- **Contents**: CSS, JS, images, fonts, uploads
- **Relationships**: Served directly by static file middleware.

---

## 4. Cross-Layer Relationships

### What this layer exposes:
- **HTTP Endpoints**: Controllers and Razor Pages for HTTP requests
- **UI**: Razor views for HTML rendering
- **Middleware**: Custom middleware for request processing
- **Attributes**: Custom validation and authorization attributes
- **Configuration**: appsettings.json for runtime configuration

### What this layer consumes:
- **From Domain Layer**: DTOs, Enums, Resources, Entities (for logging)
- **From Application Layer**: Services, Helpers, Interfaces
- **From Infrastructure Layer**: DbContext (for middleware), Identity entities
- **External Libraries**: ASP.NET Core MVC, Identity, Entity Framework, AutoMapper

### Data Flow:
- **Input**: HTTP requests from browsers/API clients
- **Processing**: Middleware → Controllers → Services → Repositories
- **Output**: HTML views, JSON responses, redirects
- **Storage**: Middleware logs to database via Infrastructure layer

---

## 5. Detailed File-by-File Documentation

### Program.cs
- **Location**: `Program.cs`
- **Type**: Application entry point
- **Purpose**: Application startup and configuration
- **Role**: Configures services, middleware pipeline, and starts the application
- **Key Members**:
  - Service registration via extension methods (AddMvcServices, AddSecurityServices, AddIdentityServices, AddDatabaseServices, AddApplicationServices)
  - Database seeding (SeedDatabaseAsync)
  - Pipeline configuration (ConfigurePipeline)
- **Dependencies**: Microsoft.AspNetCore.Builder, WebApplication.Extensions
- **Impact**: Entry point for the application. Changes affect application startup and configuration.
- **Notes**: Uses alias `webApplication` to resolve namespace conflict with project name "WebApplication". Calls extension methods for organized configuration. Seeds admin user on startup.

### appsettings.json
- **Location**: `appsettings.json`
- **Type**: Configuration file
- **Purpose**: Application configuration settings
- **Role**: Stores connection strings, logging, security settings, and other configuration
- **Key Members**:
  - `ConnectionStrings`: DefaultConnection and AppDbContextConnection
  - `Logging`: Log level configuration
  - `MaintenanceMode`: Boolean for maintenance mode
  - `AllowedHosts`: "*"
  - `UploadSettings`: UploadsRootPath configuration
  - `Admin`: Admin password configuration
  - `Member`: Member password configuration
  - `Encryption`: Encryption key for AesEncryptionService
- **Dependencies**: None (JSON configuration)
- **Impact**: Runtime configuration for the application. Changes affect database connections, logging, security settings.
- **Notes**: Contains hardcoded passwords (security concern). Encryption key is stored in configuration (should be in secure storage). Uses SQL Server and LocalDB.

### WebApplication.csproj
- **Location**: `WebApplication.csproj`
- **Type**: Project file
- **Purpose**: Defines WebApplication project configuration and dependencies
- **Role**: MSBuild project file for compiling the presentation layer
- **Key Members**:
  - TargetFramework: net10.0
  - Nullable: enabled
  - ImplicitUsings: enabled
  - UserSecretsId: for development secrets
  - DockerDefaultTargetOS: Linux
  - PackageReferences: AutoMapper, Entity Framework Core, NuGet packages, Docker tools
  - ProjectReferences: Application, Domain, Infrastructure
- **Dependencies**: Application, Domain, Infrastructure projects
- **Impact**: Defines compilation settings and web-specific dependencies. Changes affect build process and runtime capabilities.
- **Notes**: Includes Docker support. References all three layers. Uses SQLite for some operations alongside SQL Server.

### Extensions/PipelineExtensions.cs
- **Location**: `Extensions/PipelineExtensions.cs`
- **Type**: Static extension methods class
- **Purpose**: Configures the HTTP request pipeline middleware
- **Role**: Organizes middleware configuration in a centralized location
- **Key Members**:
  - `ConfigurePipeline(this WebApplication)`: Configures all middleware in order
  - `SeedDatabaseAsync(this WebApplication)`: Seeds database on startup
- **Dependencies**: Application.Helpers, Application.Services.Admin, WebApplication.Helpers, WebApplication.Middleware, Infrastructure, Microsoft.Extensions.FileProviders, System.Globalization
- **Impact**: Defines request processing pipeline. Changes affect middleware order and behavior.
- **Notes**: Configures logging, error handling, security headers, rate limiting, static files, session, localization, routing, authentication, authorization. Has custom culture middleware for Arabic/English. Redirects root to Admin area.

### Extensions/MvcExtensions.cs
- **Location**: `Extensions/MvcExtensions.cs`
- **Type**: Static extension methods class
- **Purpose**: Configures MVC and Razor services
- **Role**: Adds controllers, views, and localization services
- **Key Members**:
  - `AddMvcServices(this IServiceCollection)`: Adds MVC and Razor services with localization
- **Dependencies**: Microsoft.Extensions.DependencyInjection
- **Impact**: Configures MVC framework. Changes affect controller/view functionality.
- **Notes**: Adds ControllersWithViews and RazorPages. Configures localization resources path.

### Areas/Admin/Controllers/HomeController.cs
- **Location**: `Areas/Admin/Controllers/HomeController.cs`
- **Type**: Controller class
- **Purpose**: Admin area home controller
- **Role**: Handles admin area home page, language switching, and error pages
- **Key Members**:
  - `Index()`: Admin home page
  - `Privacy()`: Privacy page
  - `ChangeLanguage(string)`: Changes language (Ar/En)
  - `Error()`: Error 500 handler
  - `HttpStatusCodeHandler(int)`: Handles various HTTP status codes (404, 403, 401, 500, 503)
  - `AccessDeniedError403()`: Access denied view
- **Dependencies**: Domain.Resources, Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Diagnostics, Microsoft.AspNetCore.Localization, Microsoft.AspNetCore.Mvc, WebApplication.Attributes, WebApplication.Models
- **Impact**: Main admin area controller. Changes affect admin UI and error handling.
- **Notes**: Uses [IgnoreAction] attribute to exclude from logging. Has security fixes with ResponseCache. Redirects to previous page after language change. Uses localized error messages from resources.

### Attributes/AdminAuthorize.cs
- **Location**: `Attributes/AdminAuthorize.cs`
- **Type**: Authorization attribute and filter
- **Purpose**: Custom authorization attribute for admin access
- **Role**: Ensures only authenticated admin users can access protected resources
- **Key Members**:
  - `AdminAuthorizeAttribute`: Attribute that applies AdminAuthorizeFilter
  - `AdminAuthorizeFilter`: IAsyncAuthorizationFilter implementation
- **Dependencies**: Infrastructure.Identity, Microsoft.AspNetCore.Identity, Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Protects admin controllers/actions. Changes affect admin authorization logic.
- **Notes**: Uses SignInManager to check if user is signed in. Redirects to Identity/Account/Login if not authorized.

### Middleware/LogsHistoryMiddleware.cs
- **Location**: `Middleware/LogsHistoryMiddleware.cs`
- **Type**: Middleware class
- **Purpose**: Logs all HTTP requests to database for audit trail
- **Role**: Captures request details, response status, duration, and user information
- **Key Members**:
  - `LogsHistoryMiddleware(RequestDelegate, ILogger, IServiceProvider)`: Constructor
  - `InvokeAsync(HttpContext)`: Processes request and logs to database
  - `IsStaticFileRequest(HttpContext)`: Skips logging for static files
  - `GetOperationName(HttpContext)`: Extracts controller/action name
  - `UseLogsHistory(this IApplicationBuilder)`: Extension method for middleware registration
- **Dependencies**: Domain.Entities, Infrastructure.DbContext, Microsoft.AspNetCore.Http, Microsoft.EntityFrameworkCore, Microsoft.Extensions.Logging
- **Impact**: Provides comprehensive audit logging. Changes affect logging behavior and performance.
- **Notes**: Skips static files to reduce log noise. Uses stopwatch for duration tracking. Creates service scope for DbContext. Logs to both database and console. Handles exceptions gracefully.

### Middleware/MaintenanceMiddleware.cs
- **Location**: `Middleware/MaintenanceMiddleware.cs`
- **Type**: Middleware class
- **Purpose**: Displays maintenance page when maintenance mode is enabled
- **Role**: Blocks access to application during maintenance
- **Key Members**:
  - `MaintenanceMiddleware(RequestDelegate, IConfiguration)`: Constructor
  - `InvokeAsync(HttpContext)`: Checks maintenance mode and returns maintenance page if enabled
- **Dependencies**: Microsoft.AspNetCore.Http, Microsoft.Extensions.Configuration
- **Impact**: Controls maintenance mode access. Changes affect maintenance page behavior.
- **Notes**: Reads "MaintenanceMode" from configuration. Returns maintenance view when enabled.

### Middleware/NotificationMiddleware.cs
- **Location**: `Middleware/NotificationMiddleware.cs`
- **Type**: Middleware class
- **Purpose**: Handles notifications for users (likely SignalR or session-based)
- **Role**: Manages user notifications throughout the application
- **Key Members**:
  - `NotificationMiddleware(RequestDelegate)`: Constructor
  - `InvokeAsync(HttpContext)`: Processes notifications
- **Dependencies**: Microsoft.AspNetCore.Http
- **Impact**: Manages user notifications. Changes affect notification delivery.
- **Notes**: Implementation details would need to be read from full file.

### Attributes/MemberAuthorize.cs
- **Location**: `Attributes/MemberAuthorize.cs`
- **Type**: Authorization attribute and filter
- **Purpose**: Custom authorization attribute for member access
- **Role**: Ensures only authenticated member users can access protected resources
- **Key Members**: Similar structure to AdminAuthorize
- **Dependencies**: Infrastructure.Identity, Microsoft.AspNetCore.Identity, Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Protects member-specific controllers/actions.
- **Notes**: Similar implementation to AdminAuthorize but for member role.

### Attributes/MemberOrAdminAuthorize.cs
- **Location**: `Attributes/MemberOrAdminAuthorize.cs`
- **Type**: Authorization attribute and filter
- **Purpose**: Custom authorization attribute for member or admin access
- **Role**: Allows access to both member and admin roles
- **Key Members**: Similar structure to AdminAuthorize
- **Dependencies**: Infrastructure.Identity, Microsoft.AspNetCore.Identity, Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Protects resources accessible to both members and admins.
- **Notes**: Checks for either member or admin role.

### Attributes/IgnoreAction.cs
- **Location**: `Attributes/IgnoreAction.cs`
- **Type**: Action filter attribute
- **Purpose**: Marks actions to be ignored by logging middleware
- **Role**: Prevents certain actions from being logged (e.g., language change, error pages)
- **Key Members**: Simple marker attribute
- **Dependencies**: Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Controls which actions are logged. Changes affect audit trail completeness.
- **Notes**: Used by HomeController for ChangeLanguage and error handlers.

### Attributes/DecimalAttribute.cs
- **Location**: `Attributes/DecimalAttribute.cs`
- **Type**: Validation attribute
- **Purpose**: Validates decimal input values
- **Role**: Ensures decimal fields contain valid decimal numbers
- **Key Members**: Validation logic for decimal values
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Validates decimal input in forms. Changes affect decimal validation behavior.
- **Notes**: Custom validation attribute for decimal-specific validation.

### Attributes/FullNameMinPartsAttribute.cs
- **Location**: `Attributes/FullNameMinPartsAttribute.cs`
- **Type**: Validation attribute
- **Purpose**: Validates full name has minimum number of parts
- **Role**: Ensures full name contains at least specified number of name parts (first name, last name, etc.)
- **Key Members**: Validation logic for name parts
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Validates full name input. Changes affect name validation rules.
- **Notes**: Useful for Arabic/English name validation requiring multiple parts.

### Attributes/InThePast.cs
- **Location**: `Attributes/InThePast.cs`
- **Type**: Validation attribute
- **Purpose**: Validates date is in the past
- **Role**: Ensures date fields are not future dates
- **Key Members**: Validation logic comparing date to current date
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Validates date input for past dates only. Changes affect date validation.
- **Notes**: Used for birth dates, hire dates, etc.

### Attributes/NotInThePast.cs
- **Location**: `Attributes/NotInThePast.cs`
- **Type**: Validation attribute
- **Purpose**: Validates date is not in the past
- **Role**: Ensures date fields are current or future dates
- **Key Members**: Validation logic comparing date to current date
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Validates date input for current/future dates. Changes affect date validation.
- **Notes**: Used for expiry dates, appointment dates, etc.

### Attributes/IntAttribute.cs
- **Location**: `Attributes/IntAttribute.cs`
- **Type**: Validation attribute
- **Purpose**: Validates integer input values
- **Role**: Ensures integer fields contain valid integers
- **Key Members**: Validation logic for integer values
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Validates integer input in forms. Changes affect integer validation behavior.
- **Notes**: Custom validation attribute for integer-specific validation.

### Attributes/LocalizedMaxLength.cs
- **Location**: `Attributes/LocalizedMaxLength.cs`
- **Type**: Validation attribute
- **Purpose**: Localized maximum length validation
- **Role**: Validates string length with localized error messages
- **Key Members**: Validation logic with localized error messages
- **Dependencies**: System.ComponentModel.DataAnnotations, Domain.Resources
- **Impact**: Validates string length with Arabic/English error messages. Changes affect validation UX.
- **Notes**: Uses resource files for localized error messages.

### Attributes/LocalizedMinLength.cs
- **Location**: `Attributes/LocalizedMinLength.cs`
- **Type**: Validation attribute
- **Purpose**: Localized minimum length validation
- **Role**: Validates string length with localized error messages
- **Dependencies**: System.ComponentModel.DataAnnotations, Domain.Resources
- **Impact**: Validates string length with Arabic/English error messages.
- **Notes**: Similar to LocalizedMaxLength but for minimum length.

### Attributes/LocalizedRequired.cs
- **Location**: `Attributes/LocalizedRequired.cs`
- **Type**: Validation attribute
- **Purpose**: Localized required field validation
- **Role**: Validates required fields with localized error messages
- **Dependencies**: System.ComponentModel.DataAnnotations, Domain.Resources
- **Impact**: Validates required fields with Arabic/English error messages.
- **Notes**: Uses resource files for localized "required" messages.

### Attributes/UniqueAttribute.cs
- **Location**: `Attributes/UniqueAttribute.cs`
- **Type**: Validation attribute
- **Purpose**: Validates field uniqueness in database
- **Role**: Ensures field values are unique across database records
- **Key Members**: Validation logic checking database for uniqueness
- **Dependencies**: System.ComponentModel.DataAnnotations, Infrastructure
- **Impact**: Validates unique constraints. Changes affect data integrity validation.
- **Notes**: Likely uses repository to check database for existing values.

### FiltersAttributes/AjaxOnly.cs
- **Location**: `FiltersAttributes/AjaxOnly.cs`
- **Type**: Action filter attribute
- **Purpose**: Restricts actions to AJAX requests only
- **Role**: Ensures actions can only be called via AJAX (not direct browser navigation)
- **Key Members**: Filter logic checking X-Requested-With header
- **Dependencies**: Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Protects AJAX endpoints from direct access. Changes affect AJAX endpoint security.
- **Notes**: Returns 404 or error if not AJAX request.

### FiltersAttributes/ClaimRequirementAttribute.cs
- **Location**: `FiltersAttributes/ClaimRequirementAttribute.cs`
- **Type**: Authorization filter attribute
- **Purpose**: Requires specific claim for access
- **Role**: Implements claims-based authorization at action level
- **Key Members**: Claim type and value requirements
- **Dependencies**: Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Enforces claim-based authorization. Changes affect permission system.
- **Notes**: Used for fine-grained permission control beyond roles.

### FiltersAttributes/ClaimRequirementAnyAttribute.cs
- **Location**: `FiltersAttributes/ClaimRequirementAnyAttribute.cs`
- **Type**: Authorization filter attribute
- **Purpose**: Requires any of multiple claims for access
- **Role**: Allows access if user has any of the specified claims
- **Key Members**: Multiple claim type/value requirements
- **Dependencies**: Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Mvc.Filters
- **Impact**: Flexible claim-based authorization. Changes affect permission system.
- **Notes**: More flexible than ClaimRequirementAttribute (OR logic instead of AND).

### Areas/Identity/Pages/Account/
- **Location**: `Areas/Identity/Pages/Account/`
- **Type**: Razor Pages (scaffolded)
- **Purpose**: ASP.NET Core Identity authentication pages
- **Role**: Provides login, register, password management, and 2FA pages
- **Contents**: Standard Identity scaffolded pages (Login, Register, ForgotPassword, ResetPassword, Manage, etc.)
- **Dependencies**: Microsoft.AspNetCore.Identity.UI
- **Impact**: Provides authentication UI. Changes affect user authentication experience.
- **Notes**: Scaffolded pages - should not be heavily modified to allow easy updates. Includes 2FA support, external login, email confirmation.

### Areas/Admin/Views/
- **Location**: `Areas/Admin/Views/`
- **Type**: Razor Views
- **Purpose**: Admin area UI views
- **Role**: Renders HTML for admin area controllers
- **Contents**: Views for HomeController and other admin controllers
- **Dependencies**: ASP.NET Core MVC Razor
- **Impact**: Admin area user interface. Changes affect admin UI appearance and behavior.
- **Notes**: Uses shared layouts and partials. Supports localization.

### wwwroot/
- **Location**: `wwwroot/`
- **Type**: Static files directory
- **Purpose**: Serves static assets (CSS, JS, images, fonts)
- **Role**: Contains client-side resources
- **Contents**: CSS, JavaScript, images, fonts, uploads folder
- **Dependencies**: None (static files)
- **Impact**: Client-side assets. Changes affect UI styling and behavior.
- **Notes**: uploads subfolder for user-uploaded files. Configured with StaticFileOptions in pipeline.

---

## 6. Patterns and Best Practices

### Design Patterns Used:
- **Area Pattern**: Organizes features into areas (Admin, Identity)
- **Middleware Pattern**: Request processing through middleware chain
- **Filter Pattern**: Cross-cutting concerns via action filters
- **Attribute Pattern**: Declarative validation and authorization
- **Extension Method Pattern**: Organized service and pipeline configuration
- **Repository Pattern**: Controllers use services which use repositories

### Coding Standards:
- **Area Organization**: Features organized by area for separation
- **Authorization Attributes**: Custom attributes for role-based access
- **Validation Attributes**: Custom attributes for localized validation
- **Middleware Pipeline**: Ordered middleware configuration
- **Localization**: Resource files for Arabic/English support

### Common Patterns:
- **IgnoreAction Attribute**: Marks actions to exclude from logging
- **Localized Validation**: Validation attributes use resource files
- **Custom Authorization**: Role and claim-based authorization attributes
- **Audit Logging**: Middleware logs all requests to database
- **Maintenance Mode**: Middleware blocks access during maintenance

---

## 7. Configuration and Setup

### Configuration Files:
- **appsettings.json**: Connection strings, logging, security settings, passwords
- **WebApplication.csproj**: Project configuration and dependencies
- **launchSettings.json**: Development launch profile
- **web.config**: IIS configuration (if deployed to IIS)

### Setup Requirements:
- **.NET 10.0 SDK**: Required to build and run
- **SQL Server**: Database server for production
- **LocalDB**: Database for development (optional)
- **Package Restore**: NuGet packages must be restored
- **Database Migration**: Migrations must be applied
- **Excel Template**: ExcelFormula.xlsx in wwwroot/ReportExcel/Source

### Environment-Specific Considerations:
- **Connection Strings**: Different for dev/staging/production
- **Maintenance Mode**: Configured via appsettings
- **Upload Path**: Configured via UploadSettings
- **Logging Levels**: Different for development and production
- **HTTPS**: Required in production, optional in development

---

## Known Issues and Architectural Concerns:

1. **Namespace Conflict**:
   - Project name "WebApplication" conflicts with ASP.NET Core's WebApplication class
   - Resolved with alias `using webApplication = Microsoft.AspNetCore.Builder.WebApplication;` in Program.cs

2. **Security Issues in appsettings.json**:
   - Hardcoded passwords in configuration (Admin:password, Member:password)
   - Encryption key stored in configuration (should be in key vault)
   - Connection strings with Trusted_Connection=True (may not be suitable for production)

3. **Database Provider Inconsistency**:
   - Infrastructure uses SQL Server (Microsoft.EntityFrameworkCore.SqlServer)
   - WebApplication uses SQLite (Microsoft.EntityFrameworkCore.Sqlite)
   - Should use consistent database provider across layers

4. **Scaffolded Identity Pages**:
   - Identity pages are scaffolded and should not be heavily modified
   - Customizations should be done via partial views or extensions

5. **Middleware Order**:
   - LogsHistoryMiddleware is called twice in pipeline (line 18 and 112)
   - Should be called only once to avoid duplicate logging

6. **Commented Code**:
   - HomeController has commented code for alternative routes
   - Should be removed or properly managed with feature flags

7. **Hardcoded Redirects**:
   - Root redirects to /Admin/Home/Index hardcoded
   - Should be configurable based on user role

8. **Missing Documentation**:
   - Many middleware and filter files not fully documented
   - Would need to read full files for complete understanding

9. **Localization Middleware**:
   - Custom culture middleware in pipeline (lines 76-101)
   - May duplicate functionality with UseAppLocalization

10. **Static File Configuration**:
    - Static files configured twice (UseStaticFiles and MapStaticAssets)
    - May cause conflicts or redundant processing
