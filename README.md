# Optimizely SaaS Style Manager

A Blazor Server web application for managing styles (Display Templates) in Optimizely SaaS CMS.

## Overview

This application provides a user-friendly interface for managing Display Templates in Optimizely SaaS CMS. Display Templates define the visual presentation options available to content editors when configuring sections, rows, and columns in the Visual Builder.

## Features

- **View Styles**: Browse all registered styles with search and filtering capabilities
- **Create Styles**: Register new display templates with custom settings
- **Edit Styles**: Modify existing style properties and settings
- **Delete Styles**: Remove styles with confirmation dialog
- **Settings Management**: Configure style settings with support for:
  - String inputs
  - Boolean toggles
  - Number inputs
  - Select dropdowns with custom choices
- **Import/Export**: Import and export styles as JSON for backup or migration

## Prerequisites

- .NET 10.0 SDK or later
- Node.js (for Tailwind CSS compilation)
- Optimizely SaaS CMS instance with API access

## Configuration

### API Credentials

The application requires OAuth credentials to authenticate with the Optimizely CMS API. Configure these in `appsettings.Development.json` (this file is gitignored to protect secrets):

```json
{
  "OptimizelyApi": {
    "BaseUrl": "https://api.cms.optimizely.com/preview3",
    "TokenUrl": "https://api.cms.optimizely.com/oauth/token",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

To obtain API credentials:
1. Log in to your Optimizely CMS instance
2. Navigate to Settings > API Clients
3. Create a new API client or use existing credentials

## Running the Application

### Development

```bash
# Restore dependencies
dotnet restore

# Install npm packages (for Tailwind CSS)
npm install

# Run the application
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

### Building for Production

```bash
dotnet publish -c Release
```

## Technical Notes

### Architecture

- **Framework**: Blazor Server (.NET 8.0) with interactive server-side rendering
- **Styling**: Tailwind CSS v4
- **HTTP Client**: Typed HttpClient with IHttpClientFactory pattern
- **Authentication**: OAuth 2.0 client_credentials flow with automatic token refresh

### Project Structure

```
OptimizelySaaSStyleManager/
├── Components/
│   ├── Layout/          # Main layout and navigation
│   ├── Pages/           # Razor pages (Home, StyleEdit, Import)
│   └── Shared/          # Reusable components (Toast, ConfirmDialog, etc.)
├── Configuration/       # DI setup and settings classes
├── Models/              # Data models and DTOs
├── Services/            # API services (StyleService, TokenService)
├── Styles/              # Tailwind CSS source
└── wwwroot/             # Static assets
```

### API Integration

The application communicates with the Optimizely CMS API using the following endpoints:

| Operation | Method | Endpoint |
|-----------|--------|----------|
| List Styles | GET | `/displaytemplates` |
| Get Style | GET | `/displaytemplates/{key}` |
| Create Style | POST | `/displaytemplates` |
| Update Style | PATCH | `/displaytemplates/{key}` |
| Replace Style | PUT | `/displaytemplates/{key}` |
| Delete Style | DELETE | `/displaytemplates/{key}` |

### Token Management

OAuth tokens are automatically:
- Cached to minimize API calls
- Refreshed 30 seconds before expiry
- Thread-safe with semaphore locking

### Error Handling

API errors are captured with detailed information including:
- HTTP method and URL
- Status code
- Response body

Errors are displayed to users via toast notifications.

## Troubleshooting

### Common Issues

**Cloudflare 403 Errors**
- Ensure the User-Agent header is set (handled automatically)
- Verify API credentials are correct

**Token Errors**
- Check that ClientId and ClientSecret are valid
- Ensure the API client has appropriate permissions

**Style Creation Fails**
- Key must contain only letters, numbers, hyphens, and underscores
- Display Name is required
- Settings with Select editor type must have valid choices

## License

This project is provided as-is for use with Optimizely SaaS CMS.
