# Dactra Backend

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-purple)](https://dotnet.microsoft.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Realtime-orange)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Firebase](https://img.shields.io/badge/Firebase-Notifications-yellow)](https://firebase.google.com/)
[![Jitsi](https://img.shields.io/badge/Jitsi-Meeting-blue)](https://jitsi.org/)
[![Cloudinary](https://img.shields.io/badge/Cloudinary-Storage-lightblue)](https://cloudinary.com/)

**Dactra** is a graduation project – a medical consultation platform that connects patients with doctors through real-time chat, video meetings, and instant notifications. This repository contains the **backend** built with **.NET Core Web API**.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Environment Variables](#environment-variables)
- [Running the Project](#running-the-project)
- [API Documentation](#api-documentation)
- [Contributors](#contributors)
- [License](#license)

## Features

- **JWT Authentication** – Secure user login/register for patients & doctors.
- **Real-time Chat** – Using **SignalR** for Real-Time.
- **Video Consultations** – Integrated **Jitsi Meet** API for secure, in-app video calls.
- **Push Notifications** – **Firebase Cloud Messaging (FCM)** for real-time alerts on new messages or appointments.
- **Cloud Media Storage** – **Cloudinary** for uploading and serving profile images, prescriptions, and reports.
- **Appointment Management** – Schedule, reschedule, and cancel appointments with status tracking.
- **Prescription & Lab Results** – Digital prescriptions and lab report sharing.

## Tech Stack

| Technology               | Purpose                               |
| ------------------------ | ------------------------------------- |
| .NET Core 8 Web API      | RESTful backend services              |
| Entity Framework Core    | ORM for database operations           |
| SQL Server               | Primary database                      |
| SignalR                  | Real-time bidirectional communication |
| Jitsi Meet API           | Video conferencing                    |
| Firebase Cloud Messaging | Push notifications                    |
| Cloudinary               | Cloud image & file storage            |
| JWT                      | Authentication & authorization        |

## Architecture

The project follows a **layered architecture** (N-tier) to separate concerns:
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│ Controllers │ ←→ │ Services │ ←→ │ Repositories │
│
└─────────────────┘ └─────────────────┘ └─────────────────┘
↓ ↓ ↓
┌─────────────────────────────────────────────────────────────────┐
│ DTOs & Models │
└─────────────────────────────────────────────────────────────────┘
↓ ↓ ↓
┌─────────────────────────────────────────────────────────────────┐
│ External Services │
│ (SignalR, Firebase, Jitsi, Cloudinary, SQL Server) │
└─────────────────────────────────────────────────────────────────┘

text

- **Controllers** – Handle HTTP requests/responses.
- **Services** – Implement business logic and orchestrate repositories.
- **Repositories** – Encapsulate data access (Entity Framework).
- **Hubs** – SignalR hubs for real-time messaging.
- **External Service Wrappers** – Isolate third-party SDKs (Firebase, Jitsi, Cloudinary).

## Project Structure

Dactra_Backend/
├── Dactra/ # Main project
│ ├── Controllers/ # API endpoints
│ ├── Services/ # Business logic
│ ├── Repositories/ # Data access layer
│ ├── Hubs/ # SignalR hubs (chat, notifications)
│ ├── Models/ # Domain entities
│ ├── DTOs/ # Data transfer objects
│ ├── Data/ # DbContext & migrations
│ ├── Helpers/ # Utility classes
│ ├── Middlewares/ # Custom middleware (e.g., error handling)
│ ├── Extensions/ # Service extensions (DI configuration)
│ └── appsettings.json # Configuration (connection strings, API keys)
├── .gitignore
├── README.md
└── Dactra.sln

text

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Git](https://git-scm.com/)
- Firebase project (for FCM) – [Firebase Console](https://console.firebase.google.com/)
- Cloudinary account – [Cloudinary](https://cloudinary.com/)
- Jitsi Meet API access (optional for self-hosting)

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/mohamed566666/Dactra_Backend.git
   cd Dactra_Backend
   Restore dependencies
   ```

bash
dotnet restore
Configure appsettings.json
Copy appsettings.example.json (if exists) or create your own with the structure below (see Environment Variables section).

Apply database migrations

bash
dotnet ef database update -p Dactra -s Dactra
Run the project

bash
dotnet run --project Dactra
The API will be available at https://localhost:5001 (or configured port).

Environment Variables
Add the following sections to your appsettings.json:

json
{
"ConnectionStrings": {
"DefaultConnection": "Server=YOUR_SERVER; Database=YOUR_DATABASE; User Id=YOUR_USER; Password=YOUR_PASSWORD; Encrypt=False; MultipleActiveResultSets=True;"
},
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft.AspNetCore": "Warning"
}
},
"AllowedHosts": "\*",
"JWT": {
"Issuer": "YOUR_API_BASE_URL",
"Audience": "YOUR_API_BASE_URL",
"SignInKey": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARS_LONG"
},
"EmailSettings": {
"EmailFrom": "your-email@gmail.com",
"Password": "your-app-password",
"Host": "smtp.gmail.com",
"Port": 587
},
"Authentication": {
"Google": {
"ClientId": "your-google-client-id.apps.googleusercontent.com",
"ClientSecret": "your-google-client-secret"
}
},
"CloudinarySettings": {
"CloudName": "your_cloud_name",
"ApiKey": "your_api_key",
"ApiSecret": "your_api_secret",
"DefaultFolder": "Dactra",
"MaxFileSizeMB": 10
},
"AllowedOrigins": [
"https://your-frontend-domain.vercel.app",
"http://localhost:5173",
"http://localhost:3000"
],
"AITagging": {
"ApiKey": "YOUR_GEMINI_API_KEY",
"Model": "gemini-3.1-flash-lite-preview"
},
"Jitsi": {
"Domain": "8x8.vc",
"AppId": "your-jitsi-app-id",
"KeyId": "your-jitsi-key-id",
"PrivateKeyPath": "/path/to/your/private.pem",
"TokenExpiryMinutes": 60
},
"SwaggerAuth": {
"msk": "your_swagger_username",
"submsk": "1",
"Password": "your_swagger_password"
},
"RateLimiting": {
"EnableRateLimiting": true,
"RequestLimit": 200,
"TimeWindowInSeconds": 60,
"BlockMessage": "Too many requests, Please try again later",
"WhitelistedIPs": [ "127.0.0.1" ]
},
"Paymob": {
"ApiKey": "your_paymob_api_key",
"IntegrationId": "your_integration_id",
"IframeId": "https://accept.paymob.com/api/acceptance/iframes/YOUR_IFRAME_ID?payment_token={payment_key_obtained_previously}",
"HmacSecret": "your_hmac_secret"
},
"Firebase": {
"project_id": "your_firebase_project_id"
}
}
Never commit real secrets. Use User Secrets for development (dotnet user-secrets init) and environment variables in production.

# Running the Project

Development: dotnet run --project Dactra

Watch mode: dotnet watch run --project Dactra

Build for production: dotnet publish -c Release -o ./publish

# API Documentation

Once running, interactive API docs are available at:

Swagger UI: https://localhost:5001/swagger

Notification Flow (Firebase)
Server triggers FCM when an event occurs (new message, appointment reminder).

Client receives push notification via Firebase SDK.

User clicks to open the relevant screen.

Media Upload (Cloudinary)
All images (profile pictures, prescriptions) are uploaded directly to Cloudinary from the client using a secure token or via backend proxy.

URLs are stored in the database.

# Video Calls (Jitsi)

The backend generates a unique room name per appointment.

Returns a Jitsi meeting URL to the client.

Client loads Jitsi iframe/widget – no extra installation.

# Contributors

Mohamed Gamal (@mohamed566666)
Rashad Mostafa (@rashadmo8)

# License

This project is for educational purposes as a graduation project. No explicit license – contact the authors for permission.
