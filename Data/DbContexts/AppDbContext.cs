using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Z.EntityFramework.Extensions;
using Data.Entities.SMDEntities;
using System.Linq;

namespace Data.DbContexts
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            EntityFrameworkManager.ContextFactory = context => new AppDbContext(options);
        }

        #region Entities
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceForm> ServiceForms { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceUnit> ServiceUnits { get; set; }
        public virtual DbSet<WorkingCalendar> WorkingCalendars { get; set; }
        public virtual DbSet<Day> Days { get; set; }
        public virtual DbSet<Interval> Intervals { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<SchedulePlace> SchedulePlaces { get; set; }
        public virtual DbSet<ScheduleJob> ScheduleJobs { get; set; }
        public virtual DbSet<SchedulePerson> SchedulePeople { get; set; }
        public virtual DbSet<UnitDoctor> UnitDoctors { get; set; }
        public virtual DbSet<UnitType> UnitTypes { get; set; }
        public virtual DbSet<UnitImage> UnitImages { get; set; }
        public virtual DbSet<RoomWorkingCalendar> RoomWorkingCalendars { get; set; }
        public virtual DbSet<DoctorWorkingCalendar> DoctorWorkingCalendars { get; set; }
        public virtual DbSet<ServiceWorkingCalendar> ServiceWorkingCalendars { get; set; }
        public virtual DbSet<InjectionObject> InjectionObjects { get; set; }
        public virtual DbSet<InjectionObjectServiceType> InjectionObjectServiceTypes { get; set; }
        public virtual DbSet<UserInformation> UserInformation { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<LayTest> LayTests { get; set; }
        public virtual DbSet<RelatedPatient> RelatedPatients { get; set; }
        public virtual DbSet<ReferTickets> ReferTickets { get; set; }

        public virtual DbSet<ProfileLinks> ProfileLinks { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemSource> ItemSources { get; set; }
        #endregion

        #region SMD
        public virtual DbSet<Indicator> Indicators { get; set; }
        public virtual DbSet<KPI> KPIs { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ImplementPackage> ImplementPackages { get; set; }
        public virtual DbSet<Target> Targets { get; set; }
        public virtual DbSet<ReportHistory> ReportHistories { get; set; }
        public virtual DbSet<PatientInfo> PatientInfos { get; set; }
        public virtual DbSet<PatientInfoHistory> PatientInfoHistories { get; set; }
        public virtual DbSet<SMDUser> SMDUsers { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=PaymentDb;persist security info=True;Integrated Security=False;TrustServerCertificate=False;uid=sa;password=Zaq@123456;Trusted_Connection=False;MultipleActiveResultSets=true;").UseLazyLoadingProxies(); ;
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Interval>().Property(u => u.NumId).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<InjectionObjectServiceType>().HasKey(s => new { s.ServiceTypeId, s.InjectionObjectId });

            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonRole")
                .HasValue<Person>("BasePerson")
                .HasValue<Doctor>("Doctor");

            //modelBuilder.Entity<Person>().HasIndex(u => u.Code).IsUnique();
            //modelBuilder.Entity<Room>().HasIndex(u => u.Code).IsUnique();

            modelBuilder.Entity<Place>()
                .HasDiscriminator<string>("PlaceType")
                .HasValue<Place>("BasePlace")
                .HasValue<Room>("Room");

            modelBuilder.Entity<Job>()
                .HasDiscriminator<string>("JobType")
                .HasValue<Job>("BaseJob")
                .HasValue<Service>("Service");

            modelBuilder.Entity<ServiceUnit>()
                .HasIndex(su => su.Code)
                .IsUnique();

            modelBuilder.Entity<UnitType>()
                .HasIndex(ut => ut.Code)
                .IsUnique();

            #region SMD
            modelBuilder.Entity<Indicator>()
                .HasIndex(_ => _.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            modelBuilder.Entity<Package>()
                .HasIndex(_ => _.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            modelBuilder.Entity<Project>()
                .HasIndex(_ => _.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            modelBuilder.Entity<Unit>()
                .HasIndex(_ => _.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL And [IsDeleted] = 0");
            modelBuilder.Entity<Unit>()
                .HasIndex(_ => _.Name)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL And [IsDeleted] = 0");
            modelBuilder.Entity<PatientInfo>()
                .HasIndex(_ => new { _.CBOName , _.ReachCode})
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            modelBuilder.Entity<SMDUser>()
                .HasIndex(_ => _.Username)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            #endregion
        }
    }
}
