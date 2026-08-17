# ASP.NET Core + IIS Security Implementation Guide

This guide documents all security fixes implemented to address vulnerabilities DB-001, DB-002, WEB-019 and other security concerns.

## ✅ Implemented Security Fixes

### 🔴 Priority 1: HTTPS Enforcement

**Files Modified:** `Program.cs`

**Changes:**
- Added `app.UseHttpsRedirection()` to enforce HTTPS with 301 redirect
- Configured HSTS with strict settings (max-age=31536000, includeSubDomains, preload)
- HSTS is automatically enabled in production environment

**Code Location:** Program.cs lines 15-22, 55-57, 60-62

**Vulnerabilities Addressed:** DB-001, DB-002, WEB-019

---

### 🔴 Priority 1: Hide Server Information

**Files Modified:** `Program.cs`, `web.config`

**Changes:**
- Removed `Server` header in middleware
- Removed `X-Powered-By` header in middleware
- Removed `X-AspNet-Version` and `X-AspNetMvc-Version` headers
- Added IIS-level header removal in web.config

**Code Location:** Program.cs lines 72-81, web.config lines 15-18

**Vulnerabilities Addressed:** Information disclosure

---

### 🟠 Content Security Policy (CSP)

**Files Modified:** `Program.cs`

**Changes:**
- Implemented strict CSP with no unsafe-inline or wildcards
- Policy: `default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self'`

**Code Location:** Program.cs lines 83-94

**Vulnerabilities Addressed:** XSS attacks

---

### 🟠 Additional Security Headers

**Files Modified:** `Program.cs`, `web.config`

**Headers Added:**
- `X-Content-Type-Options: nosniff` - Prevents MIME type sniffing
- `Referrer-Policy: strict-origin-when-cross-origin` - Controls referrer information
- `Permissions-Policy` - Restricts browser features (geolocation, camera, microphone, etc.)
- `X-Frame-Options: DENY` - Prevents clickjacking
- `X-XSS-Protection: 1; mode=block` - Enables XSS filter

**Code Location:** Program.cs lines 96-121, web.config lines 20-33

**Vulnerabilities Addressed:** XSS, Clickjacking, Feature abuse

---

### 🟠 Cookie Security

**Files Modified:** `Program.cs`

**Changes:**
- All cookies configured with `Secure` flag (HTTPS only)
- All cookies configured with `HttpOnly` flag (prevents JavaScript access)
- SameSite policy set to `Strict`
- Cookie policy middleware enabled

**Code Location:** Program.cs lines 6-13, 64-66

**Vulnerabilities Addressed:** Session hijacking, CSRF

---

### 🟠 CSRF Protection

**Files Modified:** `Views/Shared/_Layout.cshtml`, `Controllers/HomeController.cs`

**Changes:**
- Added `@Html.AntiForgeryToken()` to layout
- Added example `[ValidateAntiForgeryToken]` attribute usage in controller
- All forms that modify data should include anti-forgery tokens

**Code Location:** _Layout.cshtml lines 13-15, HomeController.cs lines 25-29

**Vulnerabilities Addressed:** CSRF attacks

---

### 🟠 CORS Configuration

**Files Modified:** `Program.cs`

**Changes:**
- Created "ProductionCors" policy with restricted origins
- Policy allows specific domains only (replace "https://yourdomain.com" with actual domains)
- Disabled `AllowAnyOrigin` for production

**Code Location:** Program.cs lines 24-36, 132-134

**Vulnerabilities Addressed:** Unauthorized cross-origin requests

---

### 🟠 Session Security

**Files Modified:** `Program.cs`

**Changes:**
- Session timeout set to 30 minutes
- Session cookies configured with Secure, HttpOnly, and SameSite=Strict
- Session middleware enabled in pipeline

**Code Location:** Program.cs lines 38-47, 126-128

**Vulnerabilities Addressed:** Session fixation, session hijacking

---

### 🟠 Cache Control

**Files Modified:** `Controllers/HomeController.cs`

**Changes:**
- Added `[ResponseCache]` attribute to sensitive pages
- Cache set to `no-store` for Index, Privacy, and Error pages
- Prevents caching of user-sensitive data

**Code Location:** HomeController.cs lines 9-23, 31-35

**Vulnerabilities Addressed:** Information disclosure via cache

---

### 🟠 Input Validation

**Files Created:** `Models/UserInputModel.cs`

**Changes:**
- Created comprehensive input validation models
- Email validation with regex pattern
- Password complexity requirements
- Name, phone, username validation
- URL validation
- Query parameter validation

**Code Location:** Models/UserInputModel.cs

**Vulnerabilities Addressed:** SQL injection, XSS, injection attacks

---

### 🟠 IIS Security Configuration

**Files Created:** `web.config`

**Changes:**
- Request filtering to limit URL length
- File extension restrictions
- Hidden segment protection
- HTTP method restrictions
- SSL/TLS enforcement (commented, enable as needed)

**Code Location:** web.config

**Vulnerabilities Addressed:** Path traversal, file inclusion, request flooding

---

## 📋 Pre-Deployment Checklist

Before deploying to production, complete this checklist:

### ✅ HTTPS Configuration
- [ ] SSL certificate installed and configured
- [ ] HTTPS redirection tested (HTTP → HTTPS)
- [ ] HSTS enabled in production
- [ ] HSTS preload submitted (optional but recommended)

### ✅ Security Headers
- [ ] CSP header present and working
- [ ] No `unsafe-inline` in CSP
- [ ] No wildcards (`*`) in CSP
- [ ] X-Content-Type-Options: nosniff present
- [ ] Referrer-Policy present
- [ ] Permissions-Policy present
- [ ] X-Frame-Options: DENY present
- [ ] Server header removed
- [ ] X-Powered-By header removed

### ✅ Cookie Security
- [ ] All cookies have Secure flag
- [ ] All cookies have HttpOnly flag
- [ ] SameSite policy configured (Strict or Lax)
- [ ] Session timeout configured appropriately

### ✅ CSRF Protection
- [ ] Anti-forgery tokens in all forms
- [ ] [ValidateAntiForgeryToken] on POST actions
- [ ] Token validation working correctly

### ✅ CORS Configuration
- [ ] CORS policy restricted to specific domains
- [ ] AllowAnyOrigin NOT used in production
- [ ] CORS tested with allowed and disallowed origins

### ✅ Input Validation
- [ ] All user inputs validated server-side
- [ ] Email validation implemented
- [ ] Password complexity requirements enforced
- [ ] Query parameters validated
- [ ] File uploads validated (if applicable)

### ✅ Output Encoding
- [ ] User inputs encoded before display
- [ ] HTML encoding used for user content
- [ ] JavaScript encoding used for dynamic content
- [ ] URL encoding used for URLs

### ✅ Cache Control
- [ ] Sensitive pages have no-store cache
- [ ] Cache headers tested
- [ ] Private data not cached

### ✅ Session Management
- [ ] Session ID regenerated after login
- [ ] Session cleared after logout
- [ ] Session timeout configured
- [ ] No sensitive data in session

### ✅ External Resources
- [ ] CDN resources use SRI (Subresource Integrity)
- [ ] External scripts from trusted sources
- [ ] Consider hosting critical resources locally

### ✅ Code Cleanup
- [ ] No TODO comments in production
- [ ] No debug code in production
- [ ] No hardcoded passwords or secrets
- [ ] No internal paths in comments

### ✅ Configuration
- [ ] AllowedHosts configured (not wildcard)
- [ ] Connection strings secured
- [ ] API keys stored securely (not in code)
- [ ] Environment-specific configurations

### ✅ Testing
- [ ] Security headers tested (use security headers scanner)
- [ ] XSS vulnerabilities tested
- [ ] CSRF protection tested
- [ ] SQL injection tested
- [ ] Input validation tested
- [ ] HTTPS enforced tested

---

## 🚨 Important Notes

### Before Going Live

1. **Update AllowedHosts in appsettings.json**
   - Replace `"AllowedHosts": "*"` with your actual domain
   - Example: `"AllowedHosts": "yourdomain.com,www.yourdomain.com"`

2. **Configure CORS Origins**
   - Update line 31 in Program.cs with your actual allowed domains
   - Remove or comment out CORS if not needed

3. **Enable IIS SSL Enforcement**
   - Uncomment SSL section in web.config if using IIS
   - Ensure SSL certificate is properly installed

4. **Test CSP Policy**
   - The current CSP is very strict
   - You may need to adjust based on your external resources
   - Test with CSP Report-Only mode first

5. **Review Session Timeout**
   - Current timeout is 30 minutes
   - Adjust based on your security requirements

6. **Password Requirements**
   - Current policy requires 12+ characters with complexity
   - Adjust based on your security policy

---

## 🔧 Customization Guide

### Adjusting CSP for External Resources

If you need to load external resources, modify the CSP in Program.cs:

```csharp
// Example: Allow Google Fonts
"font-src 'self' https://fonts.googleapis.com https://fonts.gstatic.com;"

// Example: Allow specific CDN
"script-src 'self' https://cdn.example.com;"
```

### Relaxing SameSite Policy

If you need to allow cross-site cookies, change:

```csharp
// From Strict to Lax
options.MinimumSameSitePolicy = SameSiteMode.Lax;
```

### Adding Custom CORS Origins

Update the CORS policy in Program.cs:

```csharp
policy.WithOrigins(
    "https://yourdomain.com",
    "https://app.yourdomain.com",
    "https://admin.yourdomain.com"
)
```

---

## 📚 Additional Security Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security Documentation](https://docs.microsoft.com/aspnet/core/security/)
- [Mozilla Observatory](https://observatory.mozilla.org/) - Test your security headers
- [Security Headers Scanner](https://securityheaders.com/) - Scan your site for security headers

---

## 🎯 Summary

All critical security vulnerabilities have been addressed:
- ✅ HTTPS enforcement with HSTS
- ✅ Server information hidden
- ✅ Content Security Policy implemented
- ✅ Security headers added
- ✅ Cookie security configured
- ✅ CSRF protection implemented
- ✅ CORS restricted
- ✅ Session security configured
- ✅ Cache control implemented
- ✅ Input validation examples provided
- ✅ IIS security configuration added

**Next Steps:**
1. Complete the pre-deployment checklist
2. Test all security measures
3. Update configuration with your actual domains
4. Deploy to staging environment
5. Perform security testing
6. Deploy to production

---

**Last Updated:** 2026-07-28
**Security Level:** High
**Framework:** ASP.NET Core 10.0
**Server:** IIS
