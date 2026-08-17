# InitialProjectWithSecurity - Project Summary

## Project Structure

### Domain Layer
- Domain.csproj
- Entities/LogsHistory.cs
- Interfaces/IGenericRepository.cs
- Interfaces/IUnitOfWork.cs

### Infrastructure Layer
- Infrastructure.csproj
- DbContext/AppDbContext.cs
- Identity/ApplicationUser.cs
- Identity/ApplicationRole.cs
- Seeder.cs
- Repositories/GenericRepository.cs
- Repositories/UnitOfWork.cs
- Migrations/
  - 20260729084110_ConfigureIdentityCookie.cs
  - 20260729144857_AddLogsHistoryTable.cs
  - AppDbContextModelSnapshot.cs

### Application Layer
- Application.csproj
- Class1.cs
- Mappings/MappingProfile.cs

### WebApplication Layer
- WebApplication.csproj
- Program.cs
- appsettings.json
- web.config
- Properties/launchSettings.json

#### Extensions
- DatabaseExtensions.cs
- IdentityExtensions.cs
- MvcExtensions.cs
- PipelineExtensions.cs
- SecurityExtensions.cs
- ServiceRegistrationExtensions.cs

#### Middleware
- Middleware/LogsHistoryMiddleware.cs

#### Models
- Models/UserInputModel.cs

#### Areas/Identity
- Pages/Account/
  - AccessDenied.cshtml.cs
  - ConfirmEmail.cshtml.cs
  - ExternalLogin.cshtml.cs
  - ForgotPassword.cshtml.cs
  - Login.cshtml.cs
  - Logout.cshtml.cs
  - Register.cshtml.cs
  - ResetPassword.cshtml.cs

- Pages/Account/Manage/
  - ChangePassword.cshtml.cs
  - Email.cshtml.cs
  - EnableAuthenticator.cshtml.cs
  - ExternalLogins.cshtml.cs
  - Index.cshtml.cs
  - PersonalData.cshtml.cs
  - SetPassword.cshtml.cs
  - TwoFactorAuthentication.cshtml.cs

#### Controllers
- Controllers/HomeController.cs
- Controllers/AccountController.cs

#### Views
- Views/Home/
- Views/Shared/
  - _Layout.cshtml
  - _ValidationScriptsPartial.cshtml

#### wwwroot
- wwwroot/css/
- wwwroot/js/
- wwwroot/lib/
  - bootstrap/
  - jquery/
  - jquery-validation/

## Documentation
- Security-Features.md
- SECURITY_GUIDE.md

## System Files
- .gitignore
- .dockerignore
- InitialProjectWithSecurity.slnx