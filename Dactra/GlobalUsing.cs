// ====================================
// System Namespaces
// ====================================
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
global using System.Globalization;
global using System.Collections.Concurrent;
global using System.Net;

// ====================================
// ASP.NET Core
// ====================================
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.Google;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.CookiePolicy;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;

// ====================================
// Entity Framework Core
// ====================================
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Metadata.Internal;
global using Microsoft.EntityFrameworkCore.Migrations;

// ====================================
// Microsoft Data & Security
// ====================================
global using Microsoft.Data.SqlClient;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;

// ====================================
// Third-Party Libraries
// ====================================
global using AutoMapper;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;
global using MimeKit.Text;
global using Google.Apis.Auth;

// ====================================
// Project Core
// ====================================
global using Dactra.Models;
global using Dactra.Enums;
global using Dactra.Helpers;

// ====================================
// DTOs
// ====================================
global using Dactra.DTOs;
global using Dactra.DTOs.AccountDTOs;
global using Dactra.DTOs.Admin;
global using Dactra.DTOs.AuthemticationDTOs;
global using Dactra.DTOs.MajorDTOs;
global using Dactra.DTOs.SiteReviewDTOs;
global using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
global using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
global using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;

// ====================================
// Services
// ====================================
global using Dactra.Services.Interfaces;
global using Dactra.Services.Implementation;

// ====================================
// Repositories
// ====================================
global using Dactra.Repositories.Interfaces;
global using Dactra.Repositories.Implementation;

// ====================================
// Factories & Mappings
// ====================================
global using Dactra.Factories.Interfaces;
global using Dactra.Factories.Implementation;
global using Dactra.Mappings;
// ====================================
//SingelR For RealTime
//=====================================
global using Microsoft.AspNetCore.SignalR;
// ====================================
//Hubs
//=====================================
global using Dactra.Hubs;
