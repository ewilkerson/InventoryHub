# Project: Building and Deploying the Full-Stack Integration Project

To complete this assignment, you’ll submit your code for a complete and optimized full-stack application, InventoryHub, which you worked on using Microsoft Copilot. The project integrates the work you completed in four previous activities and focuses on:

Creating seamless communication between the front-end and back-end

Resolving integration issues

Structuring JSON responses

Optimizing performance

This project combines all these activities into a comprehensive submission for review and feedback.

# SafeVault — Secure Coding Project (Activities 1–3)

ASP.NET Core MVC / SQLite training app covering:
1. Secure input validation, parameterized SQL, safe Razor output encoding, and SQL injection/XSS regression tests.
2. Password hashing, cookie authentication, and Admin/User role-based authorization.
3. Security hardening: antiforgery tokens, safe error messages, security headers, and regression tests.

## Requirements
Install the **.NET 8 SDK**. First build requires internet access to restore NuGet packages.

## Run
From this directory:
```bash
dotnet restore
dotnet run --project SafeVault
```
Open the HTTPS URL printed in the terminal. If the development certificate is not trusted, run `dotnet dev-certs https --trust`.

### Demo accounts
- Admin: `admin` / `Admin@12345!`
- User: `demo` / `User@12345!`

Override demo passwords using `SAFEVAULT_ADMIN_PASSWORD` and `SAFEVAULT_USER_PASSWORD` environment variables before first startup. These are training defaults: change them and do not deploy this sample as-is.

## Run tests
```bash
dotnet test
```

## Activity mapping
**Activity 1 — Writing Secure Code:** server-side allow-list validation, length limits, parameterized SQL, safe Razor output encoding, input/SQL injection tests.

**Activity 2 — Authentication and Authorization:** ASP.NET Core `PasswordHasher<TUser>`, cookie authentication, generic login errors, `[Authorize]`, Admin-only policy, and role tests via protected controller configuration.

**Activity 3 — Debugging and Resolving Security Issues:** removes SQL string concatenation, never uses `Html.Raw` for untrusted input, antiforgery validation on POST forms, defensive response headers, and regression tests.

## Security limitations
This is a local learning sample, not a production financial application. Before production use, add a security review, HTTPS-only deployment, rate limiting/lockout, audit logging, managed secrets, backup/restore tests, monitoring, and an established identity provider. SQL parameters prevent injection; input validation is an additional layer, not a replacement.
