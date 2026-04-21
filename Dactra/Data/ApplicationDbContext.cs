using Dactra.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
    public DbSet<VitalSignType> VitalSignTypes { get; set; }
    public DbSet<VitalSign> VitalSigns { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Medicines> Medicines { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<ComplaintAttachment> ComplaintAttachments { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<SiteReview> SiteReviews { get; set; }
    public DbSet<DoctorQualification> DoctorQualifications { get; set; }
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<ChronicDisease> ChronicDiseases { get; set; }
    public DbSet<DoctorAvailabilitySlot> DoctorAvailabilitySlots { get; set; }
    public DbSet<Comment> comments { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<SavedPost> SavedPosts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
    public DbSet<QuestionInterest> QuestionInterests { get; set; }
    public DbSet<QuestionSave> QuestionSaves { get; set; }
    public DbSet<QuestionTag> QuestionTags { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<QuestionAnswerLike> QuestionAnswerLikes { get; set; }
    public DbSet<LabsWorkingHour> LabsWorkingHour { get; set; }
    public DbSet<PatientDoctorCare> PatientDoctorCares { get; set; }
    public DbSet<DoctorMedicalTestSponsor> DoctorMedicalTestSponsors { get; set; }
    public DbSet<PatientReferral> PatientReferrals { get; set; }
    public DbSet<PatientReferralItem> PatientReferralItems { get; set; }
    public DbSet<Notifications> Notifications { get; set; }

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

        modelBuilder.Entity<ApplicationRole>().HasData(roles);

        modelBuilder.Entity<ServiceProviderProfile>().ToTable("ServiceProviders");
        modelBuilder.Entity<DoctorProfile>().ToTable("DoctorProfiles");
        modelBuilder.Entity<MedicalTestProviderProfile>().ToTable("MedicalTestProviderProfiles");

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

        modelBuilder.Entity<Complaint>()
            .HasMany(c => c.Attachments)
            .WithOne(a => a.Complaint)
            .HasForeignKey(a => a.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Complaint>().HasIndex(c => c.UserId);
        modelBuilder.Entity<ComplaintAttachment>().HasIndex(a => a.ComplaintId);

        modelBuilder.Entity<SiteReview>().HasIndex(r => r.ReviewerUserId).IsUnique();
        modelBuilder.Entity<PatientProfile>().HasIndex(p => p.UserId).IsUnique();
        modelBuilder.Entity<DoctorProfile>().HasIndex(p => p.UserId).IsUnique();
        modelBuilder.Entity<MedicalTestProviderProfile>().HasIndex(p => p.UserId).IsUnique();
        modelBuilder.Entity<SiteReview>().HasIndex(r => r.Score);

        modelBuilder.Entity<DoctorAvailabilitySlot>()
            .HasIndex(x => new { x.DoctorId, x.SlotDateTimeUtc })
            .IsUnique();

        modelBuilder.Entity<DoctorAvailabilitySlot>()
            .HasOne(x => x.Appointment)
            .WithOne(x => x.Slot)
            .HasForeignKey<PatientAppointment>(x => x.SlotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceProviderProfile>()
            .HasOne(sp => sp.User)
            .WithMany()
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PatientAppointment>()
            .HasOne(pa => pa.Patient)
            .WithMany(p => p.Patient_Appointment)
            .HasForeignKey(pa => pa.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PatientAppointment>()
            .HasOne(pa => pa.Payment)
            .WithMany()
            .HasForeignKey(pa => pa.PaymentId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Patient)
            .WithMany(p => p.questions)
            .HasForeignKey(q => q.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId });

        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SavedPost>()
            .HasIndex(sp => new { sp.UserId, sp.PostId })
            .IsUnique();

        modelBuilder.Entity<PostLike>()
            .HasIndex(pl => new { pl.UserId, pl.PostId })
            .IsUnique();

        modelBuilder.Entity<QuestionTag>()
            .HasKey(qt => new { qt.QuestionId, qt.TagId });

        modelBuilder.Entity<DoctorProfile>()
            .Property(d => d.ConsultationPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Patient)
            .WithMany(p => p.Ratings)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Provider)
            .WithMany()
            .HasForeignKey(r => r.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CommentLike>()
            .HasIndex(cl => new { cl.CommentId, cl.UserId })
            .IsUnique();
        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.Comment)
            .WithMany(c => c.Likes)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<QuestionAnswer>()
            .HasOne(a => a.Answerer)
            .WithMany()
            .HasForeignKey(a => a.AnswererUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientDoctorCare>(entity =>
        {
            entity.HasIndex(x => new { x.PatientId, x.DoctorId })
                  .IsUnique();

            entity.HasOne(x => x.Patient)
                  .WithMany(x => x.DoctorCares)
                  .HasForeignKey(x => x.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Doctor)
                  .WithMany(x => x.PatientCares)
                  .HasForeignKey(x => x.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DoctorMedicalTestSponsor>(entity =>
        {
            entity.Property(x => x.DiscountPercentage)
                  .HasPrecision(5, 2);

            entity.HasIndex(x => new { x.DoctorId, x.ProviderType });

            entity.Property(x => x.Status)
                  .HasConversion<int>();

            entity.HasOne(x => x.Doctor)
                  .WithMany(x => x.MedicalTestSponsors)
                  .HasForeignKey(x => x.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.MedicalTestProvider)
                  .WithMany(x => x.DoctorSponsors)
                  .HasForeignKey(x => x.MedicalTestProviderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ParentOffer)
                  .WithMany(x => x.CounterOffers)
                  .HasForeignKey(x => x.ParentOfferId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PatientReferral>(entity =>
        {
            entity.HasOne(x => x.Patient)
                  .WithMany()
                  .HasForeignKey(x => x.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Doctor)
                  .WithMany()
                  .HasForeignKey(x => x.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Sponsorship)
                  .WithMany()
                  .HasForeignKey(x => x.SponsorshipId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.PatientId, x.SponsorshipId });
        });

        modelBuilder.Entity<PatientReferralItem>(entity =>
        {
            entity.HasOne(x => x.PatientReferral)
                  .WithMany(x => x.ReferralServices)
                  .HasForeignKey(x => x.PatientReferralId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ProviderOffering)
                  .WithMany()
                  .HasForeignKey(x => x.ProviderOfferingId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.PatientReferralId, x.ProviderOfferingId })
                  .IsUnique();
        });
    }
}
