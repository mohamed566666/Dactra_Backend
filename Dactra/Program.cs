using Dactra.Services.Background;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection("RateLimiting"));

builder.Services.Configure<PaymobSetting>(
    builder.Configuration.GetSection("Paymob"));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
    options.MemoryBufferThreshold = 1 * 1024 * 1024;
});

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));


#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

#endregion

#region Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region  Firebase 
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(
        Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json")
    )

});
#endregion

#region Repositories

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();

builder.Services.AddScoped<IDoctorProfileRepository, DoctorProfileRepository>();
builder.Services.AddScoped<IMedicalTestProviderProfileRepository, MedicalTestProviderProfileRepository>();
builder.Services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();
builder.Services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();

builder.Services.AddScoped<IMajorsRepository, MajorsRepository>();
builder.Services.AddScoped<ITestServiceRepository, TestServiceRepository>();
builder.Services.AddScoped<IProviderOfferingRepository, ProviderOfferingRepository>();
builder.Services.AddScoped<IDoctorQualificationRepository, DoctorQualificationRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IVitalSignRepository, VitalSignRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAllergyRepository, AllergyRepository>();
builder.Services.AddScoped<IChronicDiseaseRepository, ChronicDiseaseRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ISiteReviewRepository, SiteReviewRepository>();

builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
builder.Services.AddScoped<ISavedPostRepository, SavedPostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPostTagRepository, PostTagRepository>();

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionAnswerRepository, QuestionAnswerRepository>();
builder.Services.AddScoped<IQuestionInterestRepository, QuestionInterestRepository>();
builder.Services.AddScoped<IQuestionSaveRepository, QuestionSaveRepository>();
builder.Services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
builder.Services.AddScoped<IQuestionAnswerLikeRepository, QuestionAnswerLikeRepository>();

builder.Services.AddScoped<ISponsorshipRepository, SponsorshipRepository>();
builder.Services.AddScoped<IPatientReferralRepository, PatientReferralRepository>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IMedicalReportRepository, MedicalReportRepository>();

builder.Services.AddScoped<IAppointmentStatisticsRepository, AppointmentStatisticsRepository>();

#endregion

#region Services

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddHttpClient("OpenAI");

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileFactory, UserProfileFactory>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalTestsProviderService, MedicalTestsProviderService>();
builder.Services.AddScoped<IMajorsService, MajorsService>();
builder.Services.AddScoped<ITestServiceService, TestServiceService>();
builder.Services.AddScoped<IProviderOfferingService, ProviderOfferingService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IDoctorQualificationService, DoctorQualificationService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IVitalSignService, VitalSignService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAllergyService, AllergyService>();
builder.Services.AddScoped<IChronicDiseaseService, ChronicDiseaseService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ISiteReviewService, SiteReviewService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();  

builder.Services.AddScoped<IDoctorSlotService, DoctorSlotService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();


builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostLikeService, PostLikeService>();
builder.Services.AddScoped<ISavedPostService, SavedPostService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAITaggingService, AITaggingService>();

builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionAnswerService, QuestionAnswerService>();
builder.Services.AddScoped<IQuestionInterestService, QuestionInterestService>();
builder.Services.AddScoped<IQuestionSaveService, QuestionSaveService>();
builder.Services.AddScoped<ICommentLikeService, CommentLikeService>();
builder.Services.AddScoped<IQuestionAnswerLikeService, QuestionAnswerLikeService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton<IFcmService, FcmService>();


builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<ISponsorshipService, SponsorshipService>();
builder.Services.AddScoped<IPatientReferralService, PatientReferralService>();

builder.Services.AddScoped<IFileStorageService, CloudinaryStorageService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserImageService, UserImageService>();

builder.Services.AddScoped<IMedicalReportService, MedicalReportService>();

builder.Services.AddScoped<IVideoCallService, VideoCallService>();

builder.Services.AddScoped<IAppointmentStatisticsService, AppointmentStatisticsService>();

builder.Services.Configure<AITaggingOptions>(builder.Configuration.GetSection("AITagging"));

#endregion

#region Background Services
builder.Services.AddHostedService<CleanupExpiredTokensService>();
//builder.Services.AddHostedService<SlotReservationCleanupService>();
builder.Services.AddHostedService<SlotCleanupBackgroundService>();

builder.Services.AddHttpClient<PaymentService>();

#endregion

#region Identity & Authentication

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.CallbackPath = "/api/account/login/google/callback";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SignInKey"]!)
        )
    };
});

#endregion

#region Controllers & SignalR

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());

        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeOffsetConverter());

        options.JsonSerializerOptions.NumberHandling =
            System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.AddSignalR();

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dactra API",
        Version = "v1"
    });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost
});

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseMiddleware<RateLimitingMiddleware>();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenVersionMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<QuestionHub>("/hubs/questions");
app.MapHub<AppointmentHub>("/appointmentHub");
app.MapHub<DoctorScheduleHub>("/doctorScheduleHub");
app.MapHub<PostHub>("/hubs/posts");
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<VideoCallHub>("/hubs/videocall");
app.MapHub<SponsorshipHub>("/hubs/sponsorship");


app.UseHangfireDashboard();

#endregion

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("=== FATAL ERROR ===");
    Console.WriteLine(ex.ToString());
    Console.ReadKey();
}
