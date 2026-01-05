// ==================================================
// System
// ==================================================
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
global using System.Collections.Concurrent;
global using System.Globalization;

// ==================================================
// ASP.NET Core
// ==================================================
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authentication.Google;
global using Microsoft.AspNetCore.HttpOverrides;

// ==================================================
// Entity Framework Core
// ==================================================
global using Microsoft.EntityFrameworkCore;

// ==================================================
// Microsoft Security & Config
// ==================================================
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

// ==================================================
// Swagger / OpenAPI
// ==================================================
global using Microsoft.OpenApi.Models;

// ==================================================
// Third-Party Libraries
// ==================================================
global using AutoMapper;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;
global using MimeKit.Text;
global using Google.Apis.Auth;

// ==================================================
// Project Core
// ==================================================
global using Dactra.Models;
global using Dactra.Enums;
global using Dactra.Helpers;

// ==================================================
// DTOs (Grouped)
// ==================================================
global using Dactra.DTOs;
global using Dactra.DTOs.AccountDTOs;
global using Dactra.DTOs.Admin;
global using Dactra.DTOs.AuthemticationDTOs;
global using Dactra.DTOs.ComplaintsDTOs;
global using Dactra.DTOs.MajorDTOs;
global using Dactra.DTOs.SiteReviewDTOs;
global using Dactra.DTOs.RatingDTOs;
global using Dactra.DTOs.VitalSignDTOs;

global using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
global using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
global using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;

// ==================================================
// Services
// ==================================================
global using Dactra.Services.Interfaces;
global using Dactra.Services.Implementation;

// ==================================================
// Repositories
// ==================================================
global using Dactra.Repositories.Interfaces;
global using Dactra.Repositories.Implementation;

// ==================================================
// Background & Middleware
// ==================================================
global using Dactra.BackgroundServices;
global using Dactra.Middlewares;

// ==================================================
// Factories & Mapping
// ==================================================
global using Dactra.Factories.Interfaces;
global using Dactra.Factories.Implementation;
global using Dactra.Mappings;

// ==================================================
// SignalR
// ==================================================
global using Microsoft.AspNetCore.SignalR;
global using Dactra.Hubs;
