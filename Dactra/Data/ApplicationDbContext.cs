using Dactra.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<PatientProfile> Patients { get; set; }
    public DbSet<DoctorProfile> Doctors { get; set; }
    public DbSet<ServiceProviderProfile> Providers { get; set; }
    public DbSet<MedicalTestProviderProfile> MedicalTestProviders { get; set; }
    public DbSet<TestService> TestServices { get; set; }
    public DbSet<ProviderOffering> ProviderOfferings { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PatientAppointment> PatientAppointments { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Majors> Majors { get; set; }
    public DbSet<Questions> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<VitalSign> VitalSigns { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<ScheduleTable> ScheduleTables { get; set; }
    public DbSet<Medicines> Medicines { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        List<ApplicationRole> roles = new List<ApplicationRole>
        {
            new ApplicationRole { Name="Admin", NormalizedName="ADMIN" },
            new ApplicationRole { Name="Doctor", NormalizedName="DOCTOR" },
            new ApplicationRole { Name="Patient", NormalizedName="PATIENT" },
            new ApplicationRole { Name="MedicalTestProvider", NormalizedName="MEDICALTESTPROVIDER" }
        };

        modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.isDeleted);
        modelBuilder.Entity<Questions>().HasQueryFilter(q => !q.isDeleted);
        modelBuilder.Entity<Post>().HasQueryFilter(p => !p.isDeleted);

        modelBuilder.Entity<ApplicationRole>().HasData(roles);

        modelBuilder.Entity<ServiceProviderProfile>()
        .ToTable("ServiceProviders");
        modelBuilder.Entity<DoctorProfile>()
        .ToTable("DoctorProfiles");
        modelBuilder.Entity<MedicalTestProviderProfile>()
        .ToTable("MedicalTestProviderProfiles");

        modelBuilder.Entity<PrescriptionWithMedicin>()
    .HasKey(pm => new { pm.PrescriptionId, pm.MedicinesId });

        modelBuilder.Entity<PrescriptionWithMedicin>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionWithMedicins)
            .HasForeignKey(pm => pm.PrescriptionId);

        modelBuilder.Entity<PrescriptionWithMedicin>()
            .HasOne(pm => pm.Medicines)
            .WithMany(m => m.PrescriptionWithMedicins)
            .HasForeignKey(pm => pm.MedicinesId);

        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.NoAction;
    }
}
