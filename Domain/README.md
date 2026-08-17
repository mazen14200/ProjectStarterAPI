# Domain Layer Documentation

## 1. Layer Overview

### Purpose
The Domain layer is the core of the Clean Architecture, containing the business logic, entities, value objects, domain services, and domain-specific rules. It represents the heart of the application and is independent of infrastructure, presentation, and application concerns.

### Position
This is the innermost layer in the Clean Architecture. It has no dependencies on outer layers (Application, Infrastructure, WebApplication). Other layers depend on this layer for business entities, DTOs, enums, constants, and domain-specific utilities.

### Dependencies
- **Depends on**: No other layers in the project
- **External dependencies**: Microsoft.AspNetCore.Http (2.3.11)
- **Depended by**: Application, Infrastructure, and WebApplication layers

### Key Principles
- **Dependency Inversion**: Outer layers depend on this layer, not vice versa
- **Single Responsibility**: Each class has a single, well-defined purpose
- **Domain-Driven Design**: Contains domain models that represent business concepts
- **Localization Support**: Uses resource files for multi-language support (Arabic/English)

---

## 2. Directory Structure

```
Domain/
├── Consts/
│   ├── Errors.cs
│   └── RegexPatterns.cs
├── DTOs/
│   ├── CurrencyInfoDTO.cs
│   ├── ExcelDataDTO.cs
│   └── Role/
│       ├── ClaimSelectionDto.cs
│       ├── CreateRoleDTO.cs
│       ├── ModuleClaimsDto.cs
│       ├── RoleClaimsDto.cs
│       ├── RoleDTO.cs
│       └── UpdateRoleDTO.cs
├── Entities/
│   ├── ClaimSelection.cs
│   └── LogsHistory.cs
├── Enums/
│   ├── Gender.cs
│   ├── LangEnum.cs
│   ├── QuartersYear.cs
│   ├── Role.cs
│   └── RoleNumber.cs
├── HelperForDomain/
│   └── AppDubaiTime1.cs
├── Resources/
│   ├── Resource1.Designer.cs
│   ├── Resource1.resx
│   ├── Resource1.ar.resx
│   ├── Resource2.Designer.cs
│   ├── Resource2.resx
│   ├── Resource2.ar.resx
│   ├── Resource3.Designer.cs
│   ├── Resource3.resx
│   └── Resource3.ar.resx
└── Domain.csproj
```

---

---

## 3. Subfolder Summaries

### Consts/
- **Folder Name**: Consts
- **Purpose**: Centralized constants for validation and error messages
- **Contents**: Static classes containing error messages (Errors.cs) and regex patterns (RegexPatterns.cs)
- **Relationships**: Used by ViewModels, DTOs, and validation logic across all layers for consistent validation

### DTOs/
- **Folder Name**: DTOs
- **Purpose**: Data Transfer Objects for moving data between layers
- **Contents**: General DTOs (CurrencyInfoDTO, ExcelDataDTO) and Role-specific DTOs in Role/ subfolder
- **Relationships**: Used by Application layer for services, Infrastructure for mapping, and WebApplication for API models

### DTOs/Role/
- **Folder Name**: Role (subfolder of DTOs)
- **Purpose**: DTOs specific to role management functionality
- **Contents**: ClaimSelectionDto, CreateRoleDTO, ModuleClaimsDto, RoleClaimsDto, RoleDTO, UpdateRoleDTO
- **Relationships**: Used by Application layer role services and WebApplication role management controllers

### Entities/
- **Folder Name**: Entities
- **Purpose**: Core domain entities that represent business concepts
- **Contents**: ClaimSelection, LogsHistory
- **Relationships**: Mapped by Infrastructure layer to database tables, used by Application layer for business logic

### Enums/
- **Folder Name**: Enums
- **Purpose**: Enumeration types for domain constants
- **Contents**: Gender, LangEnum, QuartersYear, Role, RoleNumber
- **Relationships**: Used across all layers for type-safe constant values, with localization support via Resources

### HelperForDomain/
- **Folder Name**: HelperForDomain
- **Purpose**: Domain-specific helper utilities
- **Contents**: AppDubaiTime1 (timezone conversion)
- **Relationships**: Used across all layers for consistent datetime handling in Dubai timezone

### Resources/
- **Folder Name**: Resources
- **Purpose**: Localization resource files for multi-language support
- **Contents**: Resource1, Resource2, Resource3 (each with .resx, .ar.resx, and .Designer.cs)
- **Relationships**: Used by Enums for localized display names, referenced throughout system for UI localization

---

## 4. Cross-Layer Relationships

### What this layer exposes:
- **Entities**: ClaimSelection, LogsHistory - core business entities mapped to database
- **DTOs**: All DTO classes for data transfer between layers
- **Enums**: Gender, LangEnum, QuartersYear, Role, RoleNumber - type-safe domain constants
- **Constants**: Errors, RegexPatterns - validation and error message constants
- **Helpers**: AppDubaiTime1 - timezone conversion utilities
- **Resources**: Resource1, Resource2, Resource3 - localization strings

### What this layer consumes:
- **External Libraries**: Microsoft.AspNetCore.Http (2.3.11)
- **No internal layer dependencies**: This is the innermost layer with no dependencies on other project layers

### Data Flow:
- **Input**: Domain entities and DTOs are populated by Application layer from user input or database
- **Processing**: Domain entities contain business logic and validation rules
- **Output**: DTOs are returned to Application layer for presentation or further processing
- **Storage**: Entities are persisted by Infrastructure layer via Entity Framework

---------

## 5. Detailed File-by-File Documentation

### Consts/Errors.cs
- **Location**: `Consts/Errors.cs`
- **Type**: Static class
- **Purpose**: Centralized repository of error messages used throughout the system for validation and user feedback
- **Role**: Provides consistent, localized error messages for validation attributes and error handling
- **Key Members**:
  - `GenericError`: Generic unexpected error message
  - `NotFoundError`: Resource not found error
  - `UnauthorizedError`: Authorization failure message
  - `Required`: Required field validation (with placeholder for field name)
  - `MaxLength`: Maximum length validation (with placeholders)
  - `MinLength`: Minimum length validation (with placeholders)
  - `StringLength`: String length range validation (with placeholders)
  - `InvalidFormat`: Invalid format error
  - `AlreadyExists`: Duplicate value error
  - `MustMatch`: Field matching validation (e.g., password confirmation)
  - `EmailInvalid`: Email format validation error
  - `PhoneInvalid`, `PhoneInvalidUAE`, `PhoneInvalidEgypt`, `PhoneInvalidSaudi`: Phone validation errors for different regions
  - `PasswordMismatch`, `PasswordWeak`: Password validation errors
  - `NameInvalidCharacters`: Name character validation
  - `UsernameInvalidCharacters`: Username character validation
  - `InvalidCredentials`, `AccountLocked`, `AccountNotFound`, `EmailNotConfirmed`: Authentication-related errors
  - `FileRequired`, `FileTooLarge`, `InvalidFileType`, `ImageRequired`, `InvalidImageFormat`: File upload validation errors
- **Dependencies**: System.ComponentModel.DataAnnotations (for use with validation attributes)
- **Impact**: Changes to error messages affect user-facing validation messages across the entire application. Used in ViewModels, DTOs, and validation logic.
- **Usage Examples**:
  ```csharp
  [Required(ErrorMessage = Errors.Required)]
  [StringLength(200, ErrorMessage = Errors.MaxLength)]
  public string Name { get; set; }
  ```
- **Notes**: All messages are in Arabic, supporting the system's primary language. Uses placeholder format {0}, {1}, {2} for dynamic values.

### Consts/RegexPatterns.cs
- **Location**: `Consts/RegexPatterns.cs`
- **Type**: Static class
- **Purpose**: Centralized repository of regular expression patterns for data validation
- **Role**: Provides reusable regex patterns for validating common data formats (passwords, phones, names, usernames, emails)
- **Key Members**:
  - `StrongPassword`: 8+ chars, uppercase, lowercase, number, special character
  - `MediumPassword`: 6+ chars, letters and numbers only
  - `UAEPhone`: UAE phone number format (05XXXXXXXX or +971 format)
  - `EgyptPhone`: Egyptian phone number format (01XXXXXXXXX or +20 format)
  - `SaudiPhone`: Saudi phone number format (05XXXXXXXX or +966 format)
  - `ArabicName`: Arabic characters and spaces only
  - `EnglishName`: English characters and spaces only
  - `ArabicOrEnglishName`: Arabic or English characters and spaces
  - `SimpleUsername`: Alphanumeric, 3-50 characters
  - `Username`: Starts with letter, alphanumeric + underscore, 3-20 characters
  - `Email`: Standard email format validation
- **Dependencies**: None
- **Impact**: Changes to regex patterns affect validation logic throughout the system. Used with RegularExpressionAttribute in ViewModels and DTOs.
- **Usage Examples**:
  ```csharp
  [RegularExpression(RegexPatterns.ArabicOrEnglishName, ErrorMessage = Errors.NameInvalidCharacters)]
  public string FullName { get; set; }
  ```
- **Notes**: Patterns are designed for Middle Eastern contexts (UAE, Egypt, Saudi Arabia). All patterns are well-documented with XML comments in Arabic.

### DTOs/CurrencyInfoDTO.cs
- **Location**: `DTOs/CurrencyInfoDTO.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Represents currency information with linguistic support for Arabic grammar (singular, dual, plural forms)
- **Role**: Used for displaying currency amounts with proper grammatical forms in Arabic and English
- **Key Members**:
  - `Singular`: Singular form of currency name (e.g., "Pound", "جنيه")
  - `Dual`: Dual form (important for Arabic grammar, not used in English)
  - `Plural`: Plural form (e.g., "Pounds", "جنيهات")
  - `IsMasculine`: Gender flag for Arabic grammar (masculine/feminine)
  - `Suffix`: Currency suffix/origin (e.g., "Egyptian", "مصري")
  - `Language`: Language code ("ar" or "en")
- **Dependencies**: None
- **Impact**: Used in financial reporting and currency display logic. Changes affect how currency amounts are formatted and displayed.
- **Notes**: Specifically designed for Arabic language support where dual form and gender are grammatically important.

### DTOs/ExcelDataDTO.cs
- **Location**: `DTOs/ExcelDataDTO.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Generic container for Excel data with up to 20 columns of flexible object data
- **Role**: Used for importing/exporting Excel data where column structure may vary
- **Key Members**:
  - `t1` through `t20`: 20 object properties representing Excel columns
- **Dependencies**: None
- **Impact**: Used in Excel import/export functionality. Changes to property names would break Excel processing logic.
- **Notes**: Uses object type for maximum flexibility. Each property represents a column in Excel files.

### DTOs/Role/ClaimSelectionDto.cs
- **Location**: `DTOs/Role/ClaimSelectionDto.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Represents a single claim with selection state for role management
- **Role**: Used in role claim assignment UI to display and manage individual claims
- **Key Members**:
  - `ClaimType`: The claim type/identifier (e.g., "CanCreateUsers")
  - `Label`: Human-readable label for the claim
  - `IsSelected`: Boolean flag indicating if the claim is assigned to the role
- **Dependencies**: None
- **Impact**: Used in role management UI and claim assignment logic. Changes affect role claim management functionality.
- **Notes**: Part of the role management DTO hierarchy. Used within ModuleClaimsDto.

### DTOs/Role/CreateRoleDTO.cs
- **Location**: `DTOs/Role/CreateRoleDTO.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Data transfer object for creating new roles
- **Role**: Used when creating a new role in the system
- **Key Members**:
  - `Name`: The name of the role to create
- **Dependencies**: None
- **Impact**: Used in role creation endpoints and services. Changes affect role creation API contracts.
- **Notes**: Minimal DTO for role creation. Additional properties may be added for more complex role creation scenarios.

### DTOs/Role/ModuleClaimsDto.cs
- **Location**: `DTOs/Role/ModuleClaimsDto.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Groups claims by module/feature area for organized claim management
- **Role**: Represents a module (e.g., "User Management") with its associated claims
- **Key Members**:
  - `ModuleName`: Name of the module/feature area
  - `Claims`: List of ClaimSelectionDto objects representing claims in this module
- **Dependencies**: Domain.DTOs.Role.ClaimSelectionDto
- **Impact**: Used in role claim management UI to organize claims by module. Changes affect claim assignment UI structure.
- **Notes**: Enables hierarchical organization of claims for better UX in role management.

### DTOs/Role/RoleClaimsDto.cs
- **Location**: `DTOs/Role/RoleClaimsDto.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Complete representation of a role with all its module claims
- **Role**: Used for displaying and managing all claims assigned to a specific role
- **Key Members**:
  - `RoleId`: ID of the role
  - `RoleName`: Name of the role
  - `ModuleClaims`: List of ModuleClaimsDto representing all modules and their claims
- **Dependencies**: Domain.DTOs.Role.ModuleClaimsDto
- **Impact**: Used in role detail views and claim assignment operations. Changes affect role claim management functionality.
- **Notes**: Top-level DTO for role claim management. Contains the complete claim structure for a role.

### DTOs/Role/RoleDTO.cs
- **Location**: `DTOs/Role/RoleDTO.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Basic role information for listing and display purposes
- **Role**: Represents a role in role listing and selection scenarios
- **Key Members**:
  - `Id`: Unique identifier of the role
  - `Name`: Display name of the role
  - `IsDeleted`: Soft delete flag indicating if the role is deleted
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Used in role listing, selection dropdowns, and basic role display. Changes affect role listing APIs and UI.
- **Notes**: Lightweight DTO for basic role information. Does not include claims or detailed permissions.

### DTOs/Role/UpdateRoleDTO.cs
- **Location**: `DTOs/Role/UpdateRoleDTO.cs`
- **Type**: Data Transfer Object (class)
- **Purpose**: Data transfer object for updating existing roles
- **Role**: Used when updating role information
- **Key Members**:
  - `Id`: ID of the role to update
  - `Name`: New name for the role
- **Dependencies**: None
- **Impact**: Used in role update endpoints and services. Changes affect role update API contracts.
- **Notes**: Similar to CreateRoleDTO but includes Id for identifying the role to update.

### Entities/ClaimSelection.cs
- **Location**: `Entities/ClaimSelection.cs`
- **Type**: Entity class
- **Purpose**: Entity representing a claim selection (note: currently lacks primary key - needs Id property)
- **Role**: Used for storing claim selections in the database
- **Key Members**:
  - `ClaimType`: Unique identifier for the claim type
  - `Label`: Human-readable label for the claim
  - `IsSelected`: Flag indicating if the claim is selected
- **Dependencies**: None
- **Impact**: Mapped to database table. Changes affect database schema and claim storage logic.
- **Notes**: Currently missing a primary key property (Id). This will cause Entity Framework errors. Needs an Id property added for proper EF Core mapping. Namespace has typo: "Domain.Entites" should be "Domain.Entities".

### Entities/LogsHistory.cs
- **Location**: `Entities/LogsHistory.cs`
- **Type**: Entity class
- **Purpose**: Entity for storing audit logs of HTTP requests and operations
- **Role**: Provides comprehensive logging of system operations for audit and debugging
- **Key Members**:
  - `Id`: Primary key (int)
  - `OperationType`: Type of operation (GET, POST, PUT, DELETE, etc.)
  - `OperationName`: Controller/Action name or operation description
  - `Path`: Request URL path
  - `Method`: HTTP method used
  - `StatusCode`: HTTP response status code
  - `UserName`: User who made the request
  - `IpAddress`: Client IP address
  - `UserAgent`: Browser/user agent string
  - `CreatedAt`: Timestamp of the operation (UTC)
  - `ErrorMessage`: Error message if operation failed
  - `DurationMs`: Request duration in milliseconds
- **Dependencies**: System
- **Impact**: Mapped to database table. Used for audit trails, debugging, and monitoring. Changes affect logging infrastructure.
- **Notes**: Comprehensive audit logging entity. Supports performance monitoring (DurationMs) and security auditing (UserName, IpAddress).

### Enums/Gender.cs
- **Location**: `Enums/Gender.cs`
- **Type**: Enum
- **Purpose**: Represents gender options with localized display names
- **Role**: Used for gender selection in user profiles and forms
- **Key Members**:
  - `Male = 1`: Male gender with localized display name from Resource2
  - `Female`: Female gender with localized display name from Resource2
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Used in user entities and forms. Changes affect gender selection UI and data storage.
- **Notes**: Uses Display attribute with ResourceType for localization support via Resource2.

### Enums/LangEnum.cs
- **Location**: `Enums/LangEnum.cs`
- **Type**: Enum
- **Purpose**: Represents supported languages in the system
- **Role**: Used for language selection and localization
- **Key Members**:
  - `Ar = 1`: Arabic language with localized display from Resource1
  - `En`: English language with localized display from Resource1
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Used throughout the system for language switching and localization. Changes affect multi-language support.
- **Notes**: Primary system languages are Arabic and English. Uses Resource1 for localized display names.

### Enums/QuartersYear.cs
- **Location**: `Enums/QuartersYear.cs`
- **Type**: Enum
- **Purpose**: Represents time periods: quarters and full year for reporting
- **Role**: Used in financial and operational reporting to select time periods
- **Key Members**:
  - `Quareter1 = 1`: First quarter (Q1)
  - `Quareter2 = 2`: Second quarter (Q2)
  - `Quareter3 = 3`: Third quarter (Q3)
  - `Quareter4 = 4`: Fourth quarter (Q4)
  - `yearFull = 5`: Full year
- **Dependencies**: System.ComponentModel.DataAnnotations
- **Impact**: Used in reporting and date range selection. Changes affect reporting functionality.
- **Notes**: Note the typo "Quareter" instead of "Quarter" - this is intentional in the codebase. Uses Resource1 for localization.

### Enums/Role.cs
- **Location**: `Enums/Role.cs`
- **Type**: Enum
- **Purpose**: Represents high-level system roles (administrative roles)
- **Role**: Defines top-level administrative roles in the system
- **Key Members**:
  - `SuperAdmin = 1`: Super administrator with full system access
  - `Master`: Master administrator role
- **Dependencies**: None
- **Impact**: Used for role-based access control at the highest level. Changes affect authorization logic.
- **Notes**: Separate from RoleNumber enum. These are administrative/super user roles.

### Enums/RoleNumber.cs
- **Location**: `Enums/RoleNumber.cs`
- **Type**: Enum
- **Purpose**: Represents functional/operational roles in the system
- **Role**: Defines specific user roles for different job functions
- **Key Members**:
  - `NormalUser = 1`: Regular user with basic permissions
  - `Manager = 2`: Manager with elevated permissions
  - `ActivitiesSupervisor = 3`: Supervisor for activities
  - `Accountant = 4`: Accountant with financial permissions
- **Dependencies**: None
- **Impact**: Used for role-based access control for operational roles. Changes affect authorization and permissions.
- **Notes**: Different from Role enum which is for administrative roles. These are functional business roles.

### HelperForDomain/AppDubaiTime1.cs
- **Location**: `HelperForDomain/AppDubaiTime1.cs`
- **Type**: Static helper class
- **Purpose**: Provides Dubai timezone conversion utilities for consistent time handling
- **Role**: Ensures all datetime operations use Dubai timezone (Asia/Dubai) for business consistency
- **Key Members**:
  - `Now`: Returns current time in Dubai timezone (DateTime)
  - `NowOffset`: Returns current time in Dubai timezone (DateTimeOffset)
  - `ConvertToDubaiDateTime(DateTime)`: Converts UTC DateTime to Dubai DateTime
  - `ConvertToDubaiDateOnly(DateOnly)`: Converts UTC DateOnly to Dubai DateOnly
- **Dependencies**: System
- **Impact**: Used throughout the system for consistent datetime handling. Changes affect all datetime operations and displays.
- **Notes**: Critical for business operations in Dubai/Middle East region. All UTC times should be converted through this helper for display and storage consistency.

### Resources/Resource1.Designer.cs, Resource1.resx, Resource1.ar.resx
- **Location**: `Resources/Resource1.*`
- **Type**: Resource files (auto-generated Designer + .resx files)
- **Purpose**: Localization resources for general system strings (language, quarters, etc.)
- **Role**: Provides Arabic/English translations for common UI elements
- **Key Members**: Auto-generated properties for each resource string
- **Dependencies**: None
- **Impact**: Used by LangEnum and QuartersYear for localized display names. Changes affect UI language display.
- **Notes**: Supports both English (Resource1.resx) and Arabic (Resource1.ar.resx). Auto-generated by Visual Studio.

### Resources/Resource2.Designer.cs, Resource2.resx, Resource2.ar.resx
- **Location**: `Resources/Resource2.*`
- **Type**: Resource files (auto-generated Designer + .resx files)
- **Purpose**: Localization resources for gender and other specific UI elements
- **Role**: Provides Arabic/English translations for gender and related UI elements
- **Key Members**: Auto-generated properties for each resource string
- **Dependencies**: None
- **Impact**: Used by Gender enum for localized display names. Changes affect gender display in UI.
- **Notes**: Supports both English and Arabic. Auto-generated by Visual Studio.

### Resources/Resource3.Designer.cs, Resource3.resx, Resource3.ar.resx
- **Location**: `Resources/Resource3.*`
- **Type**: Resource files (auto-generated Designer + .resx files)
- **Purpose**: Localization resources for additional system strings
- **Role**: Provides Arabic/English translations for other UI elements not covered by Resource1/2
- **Key Members**: Auto-generated properties for each resource string
- **Dependencies**: None
- **Impact**: Used for various UI localizations throughout the system. Changes affect localized UI elements.
- **Notes**: Supports both English and Arabic. Auto-generated by Visual Studio.

### Domain.csproj
- **Location**: `Domain.csproj`
- **Type**: Project file
- **Purpose**: Defines the Domain layer project configuration and dependencies
- **Role**: MSBuild project file for compiling the Domain layer
- **Key Members**:
  - TargetFramework: net10.0
  - ImplicitUsings: enabled
  - Nullable: enabled
  - PackageReference: Microsoft.AspNetCore.Http (2.3.11)
  - Resource files configuration for Resource1, Resource2, Resource3
- **Dependencies**: None (project-level)
- **Impact**: Defines compilation settings and external dependencies. Changes affect build process and available APIs.
- **Notes**: Uses .NET 10.0 (preview/future version). Configured for public resource file generation.

---

## 6. Patterns and Best Practices

### Design Patterns Used:
- **Data Transfer Object Pattern**: DTOs for moving data between layers without exposing domain entities
- **Value Object Pattern**: Enums and constant classes represent domain values
- **Resource File Pattern**: Localization using .resx files for multi-language support
- **Static Helper Pattern**: AppDubaiTime1 as a utility class for cross-cutting concerns

### Coding Standards:
- **Namespace Organization**: Clear namespace structure (Domain.Consts, Domain.DTOs, Domain.Entities, etc.)
- **XML Documentation**: Arabic comments for business context, English for technical documentation
- **Nullable Reference Types**: Enabled (Nullable: enable) for null safety
- **Implicit Usings**: Enabled for cleaner code
- **Localization**: All user-facing strings use resource files for Arabic/English support

### Common Patterns:
- **Validation Attributes**: Errors and RegexPatterns used with DataAnnotations for declarative validation
- **Enum Localization**: Display attribute with ResourceType for localized enum values
- **DTO Naming**: Clear naming convention (EntityName + DTO suffix)
- **Resource Organization**: Multiple resource files (Resource1, 2, 3) for logical grouping

---

## 7. Configuration and Setup

### Configuration Files:
- **Domain.csproj**: Project configuration with .NET 10.0 target framework
- **Resource files**: .resx files configured for public code generation with PublicResXFileCodeGenerator

### Setup Requirements:
- **.NET 10.0 SDK**: Required to build the project
- **No database setup**: Domain layer is database-agnostic
- **No appsettings**: No configuration files needed in this layer

### Environment-Specific Considerations:
- **Timezone**: AppDubaiTime1 assumes Dubai timezone (Asia/Dubai) - may need adjustment for other regions
- **Localization**: Resource files support Arabic (ar) and English (en) - additional languages can be added
- **Validation Patterns**: Regex patterns are tailored for Middle Eastern phone formats (UAE, Egypt, Saudi)

---

## Known Issues and Notes:

1. **ClaimSelection Entity Missing Primary Key**: The ClaimSelection entity lacks an Id property, which will cause Entity Framework to throw an InvalidOperationException. This needs to be fixed by adding an Id property.

2. **Namespace Typo**: ClaimSelection.cs uses namespace "Domain.Entites" (typo) instead of "Domain.Entities".

3. **Enum Typo**: QuartersYear enum has "Quareter" instead of "Quarter" in member names - this appears intentional but should be noted.

4. **.NET 10.0**: The project targets .NET 10.0, which may be a preview or future version. Ensure compatibility with the development environment.
