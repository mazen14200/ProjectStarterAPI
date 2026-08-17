# Application Layer Documentation

## 1. Layer Overview

### Purpose
The Application layer contains application services, business logic orchestration, use case implementations, and application-specific utilities. It acts as the intermediary between the Application layer (core business logic) and the Infrastructure/WebApplication layers (data access and presentation).

### Position
This layer sits between the Application layer (inner) and the Infrastructure/WebApplication layers (outer). It orchestrates business operations, coordinates between different components, and implements application-specific use cases.

### Dependencies
- **Depends on**: Application layer (entities, DTOs, enums, constants), Infrastructure layer (repositories, Identity, DbContext)
- **External dependencies**: AutoMapper (16.2.0), ClosedXML (0.105.1), Microsoft.AspNetCore.Identity.EntityFrameworkCore (10.0.10), Microsoft.AspNetCore.Identity.UI (10.0.10), QRCoder (1.8.0)
- **Depended by**: WebApplication layer (controllers use services from this layer)

### Key Principles
- **Service Layer Pattern**: Business logic encapsulated in service classes
- **Dependency Injection**: Services are injected via constructor injection
- **Separation of Concerns**: Helpers separated from services, interfaces for abstraction
- **Orchestration**: Coordinates between repositories, Identity, and business rules

---

## 2. Directory Structure

```
Application/
├── Exception/
│   └── ServiceException.cs
├── Helpers/
│   ├── AesEncryptionService.cs
│   ├── AppDubaiTime.cs
│   ├── ArabicDateTime.cs
│   ├── ClaimsPrincipalExtensions.cs
│   ├── CurrencyHelper.cs
│   ├── DatePickerHelper.cs
│   ├── EnumHelper.cs
│   ├── ExcelStaticReport.cs
│   ├── FileHelper.cs
│   ├── FileRootProvider.cs
│   ├── GeneratePdfHelper.cs
│   ├── HashHelper.cs
│   ├── NumberToArabic.cs
│   ├── PhoneHelper.cs
│   ├── QrCodeHelper.cs
│   ├── TafqeetHelper.cs
│   └── UploadPathHelper.cs
├── Interfaces/
│   ├── IExampleService.cs
│   ├── IRoleClaimsService.cs
│   └── IRoleService.cs
├── Mappings/
│   └── MappingProfile.cs
├── Services/
│   ├── ExampleService.cs
│   ├── RoleClaimsService.cs
│   └── RoleService.cs
├── Settings/
│   └── UploadSettings.cs
└── Application.csproj
```


---

## 3. Subfolder Summaries

### Exception/
- **Folder Name**: Exception
- **Purpose**: Custom exception classes for application-specific error handling
- **Contents**: ServiceException.cs
- **Relationships**: Used by services to wrap and communicate errors. Caught by middleware or controllers.

### Helpers/
- **Folder Name**: Helpers
- **Purpose**: Utility classes for common application tasks (encryption, file handling, formatting, etc.)
- **Contents**: 17 helper classes covering encryption, datetime, files, Excel, currency, QR codes, etc.
- **Relationships**: Used by services and controllers. Some depend on Infrastructure layer (architectural concern). Provide reusable functionality across the application.

### Interfaces/
- **Folder Name**: Interfaces
- **Purpose**: Service interface definitions for dependency inversion
- **Contents**: IExampleService, IRoleClaimsService, IRoleService
- **Relationships**: Implemented by Services classes. Used by controllers via dependency injection. Some interfaces depend on Infrastructure layer (architectural concern).

### Mappings/
- **Folder Name**: Mappings
- **Purpose**: AutoMapper configuration profiles
- **Contents**: MappingProfile.cs
- **Relationships**: Used by AutoMapper to map between entities and DTOs. Currently empty - needs configuration.

### Services/
- **Folder Name**: Services
- **Purpose**: Application service implementations containing business logic
- **Contents**: ExampleService, RoleClaimsService, RoleService
- **Relationships**: Implement interfaces from Interfaces folder. Use repositories from Infrastructure layer. Injected into controllers.

### Settings/
- **Folder Name**: Settings
- **Purpose**: Configuration POCO classes for strongly-typed configuration
- **Contents**: UploadSettings.cs
- **Relationships**: Used for configuration binding from appsettings.json. Used by FileRootProvider.

---

## 4. Cross-Layer Relationships

### What this layer exposes:
- **Services**: RoleService, RoleClaimsService, ExampleService - business logic implementations
- **Interfaces**: IRoleService, IRoleClaimsService, IExampleService - service contracts
- **Helpers**: 17 utility classes for common operations
- **Exception**: ServiceException - custom exception type
- **Mappings**: MappingProfile - AutoMapper configuration

### What this layer consumes:
- **From Application Layer**: DTOs (RoleDTO, CreateRoleDTO, etc.), Enums, Resources, Entities (LogsHistory, ClaimSelection)
- **From Infrastructure Layer**: Repositories (IRoleRepository), Identity (RoleManager, UserManager, ApplicationRole, ApplicationUser), DbContext (IUnitOfWork), ClaimStore
- **External Libraries**: AutoMapper, ClosedXML, QRCoder, ASP.NET Core Identity

### Data Flow:
- **Input**: Controllers call service methods with DTOs from Application layer
- **Processing**: Services orchestrate business logic, validate, call repositories, use Identity managers
- **Output**: Services return DTOs or primitive types to controllers
- **Storage**: Services use repositories and Identity managers to persist data via Infrastructure layer

---
## 5. Detailed File-by-File Documentation

### Exception/ServiceException.cs
- **Location**: `Exception/ServiceException.cs`
- **Type**: Exception class
- **Purpose**: Custom exception for service-layer errors
- **Role**: Provides a specific exception type for service-related errors, enabling better error handling and logging
- **Key Members**:
  - Constructor accepting message and optional inner exception
- **Dependencies**: System (base Exception class)
- **Impact**: Used throughout services to wrap and communicate service-level errors. Changes affect error handling in the application.
- **Notes**: Namespace is "Application.Exceptions" (note the 's'). File comment references "ClientSatisfaction project" - legacy comment.

### Helpers/AesEncryptionService.cs
- **Location**: `Helpers/AesEncryptionService.cs`
- **Type**: Sealed class (service)
- **Purpose**: Provides AES-256-GCM encryption/decryption services for sensitive data
- **Role**: Encrypts and decrypts strings using modern AES-GCM encryption with authenticated encryption
- **Key Members**:
  - `Encrypt(string plainText, int? maxOutputChars)`: Encrypts plaintext to Base64 string
  - `EncryptToBytes(string plainText)`: Encrypts plaintext to byte array
  - `Decrypt(string cipherText)`: Decrypts Base64 cipher string to plaintext
  - `DecryptFromBytes(byte[] fullCipher)`: Decrypts byte array to plaintext
  - `GenerateKeyBase64()`: Static method to generate a random 32-byte key in Base64
  - `MaxPlainTextUtf8BytesForMaxBase64Chars(int)`: Calculates max plaintext size for given Base64 output
- **Dependencies**: System.Security.Cryptography, System.Text
- **Impact**: Used for encrypting sensitive data throughout the application. Critical for security. Changes affect all encrypted data.
- **Usage Examples**:
  ```csharp
  var service = new AesEncryptionService(key);
  var encrypted = service.Encrypt("sensitive data");
  var decrypted = service.Decrypt(encrypted);
  ```
- **Notes**: Uses AES-GCM (Galois/Counter Mode) for authenticated encryption. Includes version byte for future compatibility. Key must be 32 bytes (Base64 or UTF-8).

### Helpers/AppDubaiTime.cs
- **Location**: `Helpers/AppDubaiTime.cs`
- **Type**: Static helper class
- **Purpose**: Provides Dubai timezone conversion utilities (similar to Application layer's AppDubaiTime1)
- **Role**: Ensures consistent datetime handling in Dubai timezone across the application
- **Key Members**:
  - `Now`: Returns current Dubai time as DateTime
  - `Today`: Returns current Dubai date as DateOnly
  - `NowOffset`: Returns current Dubai time as DateTimeOffset
  - `ConvertToDubaiDateTime(DateTime)`: Converts UTC DateTime to Dubai DateTime
  - `ConvertToDubaiDateOnly(DateOnly)`: Converts UTC DateOnly to Dubai DateOnly
- **Dependencies**: System
- **Impact**: Used throughout the application for datetime operations. Changes affect all datetime displays and calculations.
- **Notes**: Duplicate functionality with Application layer's AppDubaiTime1. Consider consolidating to avoid duplication.

### Helpers/ArabicDateTime.cs
- **Location**: `Helpers/ArabicDateTime.cs`
- **Type**: Static helper class
- **Purpose**: Converts DateTime to Arabic-formatted string with Arabic numerals
- **Role**: Provides localized datetime display for Arabic UI
- **Key Members**:
  - `GetArabicDateTime(DateTime?)`: Converts nullable DateTime to Arabic formatted string with Arabic numerals
- **Dependencies**: System, System.Globalization
- **Impact**: Used for displaying dates in Arabic format. Changes affect Arabic UI date displays.
- **Notes**: Converts Western numerals (0-9) to Arabic numerals (٠-٩). Format: YYYY/MM/DD hh:mm:ss ص/م (AM/PM in Arabic).

### Helpers/ClaimsPrincipalExtensions.cs
- **Location**: `Helpers/ClaimsPrincipalExtensions.cs`
- **Type**: Static extension methods class
- **Purpose**: Extension methods for ClaimsPrincipal to simplify user claim access
- **Role**: Provides convenient methods to extract user information from claims
- **Key Members**:
  - `GetUserId(this ClaimsPrincipal)`: Extracts user ID from NameIdentifier claim
  - `GetUserPhoneNumberAsync(this ClaimsPrincipal, IUnitOfWork)`: Async method to get user phone number from database
- **Dependencies**: Infrastructure.InterfacesDB, Microsoft.EntityFrameworkCore, System.Security.Claims
- **Impact**: Used throughout controllers and services for user identification. Changes affect user authentication/authorization logic.
- **Notes**: Depends on Infrastructure layer (IUnitOfWork), which violates Clean Architecture principles (Application should not depend on Infrastructure directly).

### Helpers/CurrencyHelper.cs
- **Location**: `Helpers/CurrencyHelper.cs`
- **Type**: Static helper class
- **Purpose**: Provides currency information DTOs for UAE Dirham (AED) in Arabic and English
- **Role**: Supplies currency data for number-to-words conversion and currency display
- **Key Members**:
  - `AED_Main_Ar`: AED main unit (Dirham) in Arabic with grammatical forms
  - `AED_Sub_Ar`: AED sub unit (Fils) in Arabic with grammatical forms
  - `AED_Main_En`: AED main unit (Dirham) in English
  - `AED_Sub_En`: AED sub unit (Fils) in English
- **Dependencies**: Application.DTOs
- **Impact**: Used with TafqeetHelper for currency amount conversion to words. Changes affect financial displays.
- **Notes**: Hardcoded for UAE Dirham. Supports Arabic dual form (important for Arabic grammar). Uses CurrencyInfoDTO from Application layer.

### Helpers/DatePickerHelper.cs
- **Location**: `Helpers/DatePickerHelper.cs`
- **Type**: Static helper class
- **Purpose**: Fixes reversed date strings from date pickers
- **Role**: Handles date format issues where date components might be reversed (DD/MM vs MM/DD)
- **Key Members**:
  - `FixReversedDate(string?)`: Reverses the input string if not null/whitespace
- **Dependencies**: Microsoft.AspNetCore.Http
- **Impact**: Used for date input normalization. Changes affect date parsing logic.
- **Notes**: Very simple implementation - just reverses the string. May need more sophisticated date parsing for production use.

### Helpers/EnumHelper.cs
- **Location**: `Helpers/EnumHelper.cs`
- **Type**: Static helper class
- **Purpose**: Provides utilities for working with Display attributes on enums'
- **Role**: Extracts display names from enum Display attributes for localization
- **Key Members**:
  - `GetDisplayName(Enum)`: Gets the Display attribute Name value for an enum
  - `GetDisplayKey(this Enum)`: Extension method to get Display attribute Name
- **Dependencies**: System.ComponentModel.DataAnnotations, System.Reflection
- **Impact**: Used for displaying localized enum values in UI. Changes affect enum display logic.
- **Notes**: Works with enums that have Display attributes with ResourceType (like those in Application layer). Falls back to enum ToString() if no Display attribute.

### Helpers/ExcelStaticReport.cs
- **Location**: `Helpers/ExcelStaticReport.cs`
- **Type**: Static class (namespace: Application.Services.Admin - incorrect namespace)
- **Purpose**: Generates Excel reports using ClosedXML library with template-based approach
- **Role**: Creates Excel reports from data with styling, RTL support, and optional saving
- **Key Members**:
  - `ConfigureExcel(IWebHostEnvironment, ILogger)`: Configures the service with environment and logger
  - `ExcelReportAr_withSave_<T>(List<T>, List<string>, int, string)`: Generates Arabic Excel report and saves to file
  - `ExcelReportArEn_<T>(List<T>, List<string>, int, string, string?, bool)`: Generates Excel report (Arabic/English) and returns as byte array
- **Dependencies**: Application.Exceptions, Application.Helpers, ClosedXML.Excel, Application.DTOs, Microsoft.AspNetCore.Hosting, Microsoft.Extensions.Logging
- **Impact**: Used for reporting functionality. Changes affect Excel report generation across the application.
- **Usage Examples**:
  ```csharp
  ExcelStaticReport.ConfigureExcel(env, logger);
  var (success, filePath) = ExcelStaticReport.ExcelReportAr_withSave_<MyDto>(data, titles);
  ```
- **Notes**: Namespace is incorrect (Application.Services.Admin instead of Application.Helpers). Requires configuration before use. Uses template file "ExcelFormula.xlsx" from Source folder. Supports RTL for Arabic. Can merge category subheaders for ExcelDataDTO.

### Helpers/FileHelper.cs
- **Location**: `Helpers/FileHelper.cs`
- **Type**: Static helper class
- **Purpose**: Handles file upload, validation, and storage operations
- **Role**: Provides secure file upload with content validation, size limits, and path management
- **Key Members**:
  - `SaveImageAsync(IFormFile, string)`: Saves image file with validation
  - `CheckFileIsPdf_5Mg_Async(IFormFile?)`: Validates PDF file (max 5MB)
  - `CheckFileIsImage_3Mg_Async(IFormFile?)`: Validates image file (max 3MB)
  - `DeleteImageFile(string?)`: Deletes file by relative path
  - `IsFileExist(string?)`: Checks if file exists
  - `SaveTempAsync(IFormFile)`: Saves file to temp folder
  - `MoveTempToFinal(string, out string, string)`: Moves temp file to final location
  - `ConvertToIFormFile(string?)`: Converts file path to IFormFile
- **Dependencies**: Application.Resources, Microsoft.AspNetCore.Http, System.Linq
- **Impact**: Used for all file upload operations. Changes affect file handling security and functionality.
- **Notes**: Validates file content by checking magic bytes (header) to prevent file type spoofing. Allowed extensions: .jpg, .jpeg, .png, .pdf. Uses UploadPathHelper for path resolution. Returns error messages from Application.Resources.

### Helpers/FileRootProvider.cs
- **Location**: `Helpers/FileRootProvider.cs`
- **Type**: Static class
- **Purpose**: Provides and configures the root path for file uploads
- **Role**: Manages the uploads directory path from configuration or default
- **Key Members**:
  - `Configure(IWebHostEnvironment, IConfiguration)`: Configures the uploads root path from appsettings
  - `UploadsRootPath`: Property returning the configured uploads path
- **Dependencies**: Microsoft.AspNetCore.Hosting, Microsoft.Extensions.Configuration
- **Impact**: Used by UploadPathHelper for all file path operations. Changes affect where files are stored.
- **Notes**: Reads from "UploadSettings:UploadsRootPath" configuration. Defaults to WebRootPath/uploads if not configured. Creates directory on configuration.

### Helpers/GeneratePdfHelper.cs
- **Location**: `Helpers/GeneratePdfHelper.cs`
- **Type**: Static helper class
- **Purpose**: Placeholder for PDF generation functionality
- **Role**: Intended for generating PDFs from URLs (currently not implemented)
- **Key Members**:
  - `GeneratePdfFromUrl(string)`: Empty method with Arabic comment about browser/page auto-closing
- **Dependencies**: None
- **Impact**: Currently not functional. Changes would enable PDF generation.
- **Notes**: Not implemented - empty method body. Would likely use a library like Puppeteer or wkhtmltopdf.

### Helpers/HashHelper.cs
- **Location**: `Helpers/HashHelper.cs`
- **Type**: Static helper class
- **Purpose**: Provides hashing and encryption utilities
- **Role**: Computes SHA256 hashes and provides AES encryption/decryption
- **Key Members**:
  - `ComputeSha256Hash(string)`: Computes SHA256 hash of input string
  - `Encrypt(string)`: Encrypts string using AES (hardcoded key/IV)
  - `Decrypt(string)`: Decrypts string using AES (hardcoded key/IV)
- **Dependencies**: System.Security.Cryptography, System.Text
- **Impact**: Used for data hashing and encryption. Changes affect data security.
- **Notes**: **SECURITY ISSUE**: Encrypt/Decrypt methods use hardcoded key and IV in the source code. This is a serious security vulnerability. The key and IV should be stored securely (e.g., in configuration, key vault). Use AesEncryptionService instead for proper encryption.

### Helpers/NumberToArabic.cs
- **Location**: `Helpers/NumberToArabic.cs`
- **Type**: Static helper class
- **Purpose**: Converts numbers to Arabic ordinal feminine words (1-99)
- **Role**: Provides Arabic ordinal number conversion for feminine grammatical context
- **Key Members**:
  - `NumberToArabicOrdinalFeminine(int?)`: Converts number to Arabic ordinal feminine (e.g., "الأولى", "الثانية")
- **Dependencies**: System
- **Impact**: Used for displaying ordinal numbers in Arabic feminine form. Changes affect Arabic number displays.
- **Notes**: Limited to numbers 1-99. Returns empty string for null, zero, or numbers > 99. Handles Arabic grammar rules for ordinals.

### Helpers/PhoneHelper.cs
- **Location**: `Helpers/PhoneHelper.cs`
- **Type**: Static helper class
- **Purpose**: Normalizes phone numbers to UAE format (+971 prefix)
- **Role**: Ensures phone numbers are in consistent UAE international format
- **Key Members**:
  - `CheckAndDoPhoneStart971(string?)`: Normalizes phone to +971 format
- **Dependencies**: Application.Resources, Microsoft.AspNetCore.Hosting, Microsoft.AspNetCore.Http
- **Impact**: Used for phone number normalization before storage. Changes affect phone number format consistency.
- **Notes**: Removes spaces, strips leading "0", ensures "971" prefix. Handles edge case of "9710" prefix. Returns null for empty input.

### Helpers/QrCodeHelper.cs
- **Location**: `Helpers/QrCodeHelper.cs`
- **Type**: Static helper class
- **Purpose**: Generates QR codes as Base64 strings
- **Role**: Creates QR code images from text input for display in web pages
- **Key Members**:
  - `GenerateQrBase64(string)`: Generates QR code and returns as Base64-encoded PNG
- **Dependencies**: QRCoder, System, System.Drawing, System.Drawing.Imaging
- **Impact**: Used for QR code generation throughout the application. Changes affect QR code functionality.
- **Usage Examples**:
  ```csharp
  var qrBase64 = QrCodeHelper.GenerateQrBase64("https://example.com");
  // Use in HTML: <img src="data:image/png;base64,@qrBase64" />
  ```
- **Notes**: Uses QRCoder library. Returns Base64 string for direct embedding in HTML. QR code error correction level: Q (25%).

### Helpers/TafqeetHelper.cs
- **Location**: `Helpers/TafqeetHelper.cs`
- **Type**: Static helper class
- **Purpose**: Converts decimal currency amounts to words in Arabic and English
- **Role**: Provides number-to-words conversion for financial documents (checks, invoices)
- **Key Members**:
  - `Tafqeet(decimal, CurrencyInfoDTO, CurrencyInfoDTO)`: Main entry point, routes to Arabic or English based on language
  - `TafqeetArabic(decimal, CurrencyInfoDTO, CurrencyInfoDTO)`: Arabic conversion with proper grammar
  - `TafqeetEnglish(decimal, CurrencyInfoDTO, CurrencyInfoDTO)`: English conversion
- **Dependencies**: Application.DTOs, System, System.Globalization, System.Linq
- **Impact**: Used for financial document generation. Changes affect currency word conversion.
- **Usage Examples**:
  ```csharp
  var amount = 123.45;
  var words = TafqeetHelper.Tafqeet(amount, CurrencyHelper.AED_Main_Ar, CurrencyHelper.AED_Sub_Ar);
  // Result: "مائة و ثلاثة و عشرون درهم و خمسة و أربعون فلس"
  ```
- **Notes**: Supports up to 100 billion. Handles Arabic grammar (masculine/feminine, singular/dual/plural). Handles negative numbers. Uses CurrencyInfoDTO for grammatical forms.

### Helpers/UploadPathHelper.cs
- **Location**: `Helpers/UploadPathHelper.cs`
- **Type**: Static helper class
- **Purpose**: Provides path resolution utilities for file uploads
- **Role**: Converts relative paths to absolute paths and combines paths with uploads root
- **Key Members**:
  - `Root`: Property returning the uploads root path from FileRootProvider
  - `Combine(params string[])`: Combines root path with additional path segments
  - `ResolveRelative(string)`: Converts relative path to absolute path, handling "uploads/" prefix
- **Dependencies**: None
- **Impact**: Used by FileHelper for all文件 path operations. Changes affect file storage paths.
- **Notes**: Wraps FileRootProvider for convenience. Handles both forward slashes and backslashes. Strips "uploads/" prefix from relative paths.

### Interfaces/IExampleService.cs
- **Location**: `Interfaces/IExampleService.cs`
- **Type**: Interface
- **Purpose**: Example service interface (template/placeholder)
- **Role**: Demonstrates service interface pattern
- **Key Members**:
  - `ExampleReturnText(int)`: Returns text based on ID (not implemented)
- **Dependencies**: System
- **Impact**: Minimal - example/template code. Changes affect only if this interface is used.
- **Notes**: Placeholder interface. Implementation throws NotImplementedException. Can be used as template for new service interfaces.

### Interfaces/IRoleClaimsService.cs
- **Location**: `Interfaces/IRoleClaimsService.cs`
- **Type**: Interface
- **Purpose**: Defines contract for role claims management operations
- **Role**: Abstraction for managing claims assigned to roles
- **Key Members**:
  - `GetClaimsForRoleAsync(int)`: Retrieves all claims for a role with selection state
  - `UpdateRoleClaimsAsync(int, ClaimsModel)`: Updates claims assigned to a role
- **Dependencies**: Infrastructure.Identity, Infrastructure.Identity.Claims
- **Impact**: Used by RoleClaimsService implementation and controllers. Changes affect role claim management API.
- **Notes**: Namespace is "Application.ServiceInterfaces" (different from other interfaces). Depends on Infrastructure layer (violates Clean Architecture).

### Interfaces/IRoleService.cs
- **Location**: `Interfaces/IRoleService.cs`
- **Type**: Interface
- **Purpose**: Defines comprehensive contract for role management operations
- **Role**: Abstraction for all role CRUD operations, validation, and export
- **Key Members**:
  - `CreateRoleAsync(CreateRoleDTO)`: Creates a new role
  - `UpdateRoleAsync(UpdateRoleDTO)`: Updates an existing role
  - `SoftDeleteRoleAsync(string)`: Soft deletes a role
  - `HardDeleteRoleAsync(string)`: Permanently deletes a role
  - `RestoreRoleAsync(string)`: Restores a soft-deleted role
  - `GetAllRolesAsync()`: Gets all active roles
  - `GetAllRolesWithDeletedAsync()`: Gets all roles including deleted
  - `GetRoleByIdAsync(string)`: Gets active role by ID
  - `GetDeletedRoleByIdAsync(string)`: Gets deleted role by ID
  - `RoleNameExistsAsync(string, string?)`: Checks if role name exists
  - `ExportRolesToExcelAsync()`: Exports roles to Excel
- **Dependencies**: Application.DTOs.Role
- **Impact**: Used by RoleService implementation and role management controllers. Changes affect role management functionality.
- **Notes**: Namespace is "almetsaweq.Application.ServiceInterfaces" (incorrect namespace - should be Application.ServiceInterfaces). Well-documented with XML comments. Supports soft delete pattern.

### Mappings/MappingProfile.cs
- **Location**: `Mappings/MappingProfile.cs`
- **Type**: AutoMapper Profile class
- **Purpose**: AutoMapper configuration profile for object mapping
- **Role**: Defines mapping configurations between entities and DTOs
- **Key Members**:
  - Constructor (empty): Placeholder for mapping configurations
- **Dependencies**: AutoMapper
- **Impact**: Currently empty. When configured, will affect entity-DTO mapping throughout the application.
- **Notes**: Empty profile - no mappings configured yet. Should be populated with CreateMap calls for entity-DTO mappings.

### Services/ExampleService.cs
- **Location**: `Services/ExampleService.cs`
- **Type**: Service class
- **Purpose**: Example service implementation (template/placeholder)
- **Role**: Demonstrates service implementation pattern
- **Key Members**:
  - `ExampleReturnText(int)`: Throws NotImplementedException
- **Dependencies**: Application.Interfaces
- **Impact**: Minimal - example/template code.
- **Notes**: Implements IExampleService. Throws NotImplementedException. Can be used as template for new services.

### Services/RoleClaimsService.cs
- **Location**: `Services/RoleClaimsService.cs`
- **Type**: Service class
- **Purpose**: Implements role claims management logic
- **Role**: Manages claims assigned to roles using ASP.NET Identity RoleManager
- **Key Members**:
  - `GetClaimsForRoleAsync(int)`: Gets claims for a role with selection state
  - `UpdateRoleClaimsAsync(int, ClaimsModel)`: Updates role claims
  - `Build(List<Claim>, IList<Claim>)`: Private helper to build claim selection list
- **Dependencies**: Application.ServiceInterfaces, Application.Entites (typo - should be Entities), Infrastructure.Identity, Infrastructure.Identity.Claims, Microsoft.AspNetCore.Identity, System.Security.Claims
- **Impact**: Used by role management controllers for claim assignment. Changes affect role claim functionality.
- **Notes**: Uses ClaimStore from Infrastructure for available claims. Replaces all claims on update (remove old, add new). Depends on Infrastructure layer directly.

### Services/RoleService.cs
- **Location**: `Services/RoleService.cs`
- **Type**: Service class
- **Purpose**: Implements comprehensive role management business logic
- **Role**: Orchestrates role CRUD operations, validation, and export using repositories and Identity
- **Key Members**:
  - `CreateRoleAsync(CreateRoleDTO)`: Creates role with validation
  - `UpdateRoleAsync(UpdateRoleDTO)`: Updates role with validation
  - `SoftDeleteRoleAsync(string)`: Soft deletes role with user check
  - `HardDeleteRoleAsync(string)`: Hard deletes role with user check
  - `RestoreRoleAsync(string)`: Restores soft-deleted role
  - `GetAllRolesAsync()`: Gets active roles ordered by RoleNumber
  - `GetAllRolesWithDeletedAsync()`: Gets all roles with deletion status
  - `GetRoleByIdAsync(string)`: Gets active role by ID
  - `GetDeletedRoleByIdAsync(string)`: Gets deleted role by ID
  - `RoleNameExistsAsync(string, string?)`: Validates role name uniqueness
  - `RoleHasUsersAsync(string)`: Private helper to check if role has users
  - `ExportRolesToExcelAsync()`: Exports roles to Excel using ClosedXML
- **Dependencies**: almetsaweq.Application.ServiceInterfaces (incorrect namespace), Application.ServiceInterfaces, Application.Services.Admin, ClosedXML.Excel, Application.DTOs.Role, Application.Enums, Infrastructure.Identity, Infrastructure.InterfacesDB.RemainInterfacesDB, Microsoft.AspNetCore.Identity, System.IO
- **Impact**: Core service for role management. Changes affect all role-related functionality.
- **Notes**: Namespace is "almetsaweq.Application.Services" (incorrect). Uses both repository pattern and RoleManager. Implements soft delete pattern. TODO comments indicate logging is missing. Has logical operator precedence bug in some conditions (e.g., `r.Id == id && r.isDeleted == null || r.isDeleted == false` should be `r.Id == id && (r.isDeleted == null || r.isDeleted == false)`).

### Settings/UploadSettings.cs
- **Location**: `Settings/UploadSettings.cs`
- **Type**: Configuration class
- **Purpose**: Configuration settings for file uploads
- **Role**: Provides strongly-typed configuration for upload paths
- **Key Members**:
  - `UploadsRootPath`: Root path for file uploads (from appsettings)
- **Dependencies**: None
- **Impact**: Used by FileRootProvider to configure upload paths. Changes affect file storage location.
- **Notes**: Simple POCO for configuration binding. Used in appsettings.json section "UploadSettings".

### Application.csproj
- **Location**: `Application.csproj`
- **Type**: Project file
- **Purpose**: Defines Application layer project configuration and dependencies
- **Role**: MSBuild project file for compiling the Application layer
- **Key Members**:
  - TargetFramework: net10.0
  - ImplicitUsings: enabled
  - Nullable: enabled
  - PackageReferences: AutoMapper, ClosedXML, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Identity.UI, QRCoder
  - ProjectReferences: Application, Infrastructure
- **Dependencies**: Application project, Infrastructure project
- **Impact**: Defines compilation settings and external dependencies. Changes affect build process and available APIs.
- **Notes**: References both Application and Infrastructure layers (correct for Application layer in this architecture). Uses .NET 10.0.

--------

## 6. Patterns and Best Practices

### Design Patterns Used:
- **Service Layer Pattern**: Business logic encapsulated in service classes
- **Repository Pattern**: Services use repositories for data access (via Infrastructure)
- **Dependency Injection**: Services receive dependencies via constructor injection
- **Extension Method Pattern**: ClaimsPrincipalExtensions extends ClaimsPrincipal
- **Static Helper Pattern**: Most helpers are static utility classes
- **DTO Pattern**: Data transfer objects from Application layer for data movement
- **Soft Delete Pattern**: RoleService implements soft delete with isDeleted flag

### Coding Standards:
- **Async/Await**: Service methods use async/await for database operations
- **XML Documentation**: IRoleService has comprehensive XML comments
- **Namespace Issues**: Several files have incorrect namespaces (almetsaweq.Application.ServiceInterfaces, Application.Services.Admin)
- **Error Handling**: Services catch exceptions and return false/throw ServiceException
- **Validation**: Services validate business rules before operations

### Common Patterns:
- **Service-Interface Pair**: Each service has corresponding interface
- **Constructor Injection**: Services inject dependencies in constructors
- **TODO Comments**: Several TODO comments indicate missing logging
- **Helper Classes**: Repeated use of static helper classes for utilities
- **Configuration**: Settings classes for strongly-typed configuration

---

## 7. Configuration and Setup

### Configuration Files:
- **Application.csproj**: Project configuration with .NET 10.0 and package references
- **UploadSettings.cs**: Configuration class for upload paths (binds to appsettings)

### Setup Requirements:
- **.NET 10.0 SDK**: Required to build the project
- **Package Restore**: NuGet packages must be restored (AutoMapper, ClosedXML, QRCoder, Identity)
- **Configuration**: appsettings.json must have "UploadSettings:UploadsRootPath" section
- **Excel Template**: ExcelStaticReport requires "ExcelFormula.xlsx" in wwwroot/ReportExcel/Source folder
- **FileRootProvider Configuration**: Must call FileRootProvider.Configure() on startup

### Environment-Specific Considerations:
- **Upload Path**: Configurable via appsettings for different environments
- **Excel Template**: Template file must exist in correct location
- **Timezone**: AppDubaiTime assumes Dubai timezone
- **Logging**: ExcelStaticReport requires ILogger configuration

---

## Known Issues and Architectural Concerns:

1. **Namespace Inconsistencies**: Several files have incorrect namespaces:
   - IRoleService uses "almetsaweq.Application.ServiceInterfaces" instead of "Application.ServiceInterfaces"
   - RoleService uses "almetsaweq.Application.Services" instead of "Application.Services"
   - ExcelStaticReport uses "Application.Services.Admin" instead of "Application.Helpers"
   - ServiceException uses "Application.Exceptions" (should be singular "Exception")

2. **Clean Architecture Violations**: 
   - ClaimsPrincipalExtensions depends on Infrastructure layer (IUnitOfWork)
   - IRoleClaimsService depends on Infrastructure layer (Infrastructure.Identity)
   - Application layer should not directly depend on Infrastructure layer

3. **Security Issues**:
   - HashHelper.cs has hardcoded encryption key and IV in source code - serious security vulnerability
   - Key and IV should be stored securely (configuration, key vault)

4. **Logical Operator Precedence Bugs**:
   - RoleService has conditions like `r.Id == id && r.isDeleted == null || r.isDeleted == false`
   - Should be `r.Id == id && (r.isDeleted == null || r.isDeleted == false)` due to operator precedence

5. **Duplicate Functionality**:
   - AppDubaiTime.cs duplicates Application layer's AppDubaiTime1.cs
   - Consider consolidating to avoid duplication

6. **Missing Implementation**:
   - GeneratePdfHelper.cs is empty/not implemented
   - ExampleService throws NotImplementedException
   - MappingProfile.cs has no mappings configured

7. **TODO Comments**:
   - Multiple TODO comments indicate missing logging in catch blocks
   - Logging should be added for proper error tracking

8. **Namespace Typo**:
   - RoleClaimsService uses "Application.Entites" instead of "Application.Entities"
