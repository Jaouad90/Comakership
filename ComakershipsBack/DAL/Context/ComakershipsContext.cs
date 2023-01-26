using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DAL {
    public class ComakershipsContext : DbContext {


        protected override void OnConfiguring(DbContextOptionsBuilder options) {

            string connectionString = Environment.GetEnvironmentVariable("DBConnectionString");

            if (connectionString == null) {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=Comakerships;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

            options.UseSqlServer(connectionString);
        }

        public DbSet<UserBody> Users { get; set; }
        public DbSet<StudentUser> StudentUsers { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<University> University { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<ComakershipStatus> ComakershipStatuses { get; set; }
        public DbSet<Comakership> Comakerships { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Deliverable> Deliverables { get; set; }
        public DbSet<ComakershipSkill> ComakershipSkills { get; set; }
        public DbSet<StudentTeam> StudentTeams { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ComakershipProgram> ComakershipPrograms { get; set; }
        public DbSet<PurchaseKey> PurchaseKeys { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }
        public DbSet<TeamInvite> TeamInvites { get; set; }
        public DbSet<Review> Review { get; set; }
        //public DbSet<TeamComakership> TeamComakerships { get; set; }

        //voeg hier je objecten toe

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<UserBody>()
                .HasDiscriminator<string>("user_type")
                .HasValue<StudentUser>("student_user")
                .HasValue<CompanyUser>("company_user");

            builder.Entity<UserBody>().HasAlternateKey(u => u.Email);
            builder.Entity<UserBody>().HasQueryFilter(p => !p.Deleted); // makes sure a user that is deleted will never be automtically included in results

            builder.Entity<Comakership>().Property(c => c.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Entity<Comakership>().Property(c => c.ComakershipStatusId).HasDefaultValue(1);

            // Setting key on the linktable to a composite key
            builder.Entity<ComakershipSkill>().HasKey(x => new { x.ComakershipId, x.SkillId });

            // Setting relation from comakership
            builder.Entity<ComakershipSkill>()
                .HasOne(x => x.Comakership)
                .WithMany(y => y.LinkedSkills)
                .HasForeignKey(x => x.ComakershipId);

            // Setting relation from skill
            builder.Entity<ComakershipSkill>()
                .HasOne(x => x.Skill)
                .WithMany(y => y.LinkedComakerships)
                .HasForeignKey(x => x.SkillId);


            // Setting key on the linktable to a composite key
            builder.Entity<StudentTeam>().HasKey(x => new { x.StudentUserId, x.TeamId });

            // Setting relation from students
            builder.Entity<StudentTeam>()
                .HasOne(x => x.StudentUser)
                .WithMany(y => y.LinkedTeams)
                .HasForeignKey(x => x.StudentUserId);

            // Setting relation from teams
            builder.Entity<StudentTeam>()
                .HasOne(x => x.Team)
                .WithMany(y => y.LinkedStudents)
                .HasForeignKey(x => x.TeamId);


            //many to many for Users and Comakerships
            builder.Entity<UserComakership>().HasKey(x => new { x.ComakershipId, x.StudentUserId });

            builder.Entity<UserComakership>()
                .HasOne(x => x.StudentUser)
                .WithMany(y => y.Comakerships)
                .HasForeignKey(x => x.StudentUserId);

            builder.Entity<UserComakership>()
                .HasOne(x => x.Comakership)
                .WithMany(y => y.StudentUsers)
                .HasForeignKey(x => x.ComakershipId);


            //many to many for teams and comakerships
            builder.Entity<TeamComakership>().HasKey(x => new { x.ComakershipId, x.TeamId });

            builder.Entity<TeamComakership>()
                .HasOne(x => x.Comakership)
                .WithMany(y => y.Applications)
                .HasForeignKey(x => x.ComakershipId);

            builder.Entity<TeamComakership>()
                .HasOne(x => x.Team)
                .WithMany(y => y.AppliedComakerships)
                .HasForeignKey(x => x.TeamId);


            // Setting key on the linktable to a composite key
            builder.Entity<ComakershipProgram>().HasKey(x => new { x.ComakershipId, x.ProgramId });

            // Setting relation from comakership
            builder.Entity<ComakershipProgram>()
                .HasOne(x => x.Comakership)
                .WithMany(y => y.LinkedPrograms)
                .HasForeignKey(x => x.ComakershipId);

            // Setting relation from skill
            builder.Entity<ComakershipProgram>()
                .HasOne(x => x.Program)
                .WithMany(y => y.LinkedComakerships)
                .HasForeignKey(x => x.ProgramId);


            // Setting key on the linktable to a composite key
            builder.Entity<JoinRequest>().HasKey(x => new { x.StudentUserId, x.TeamId });

            // Setting relation from students
            builder.Entity<JoinRequest>()
                .HasOne(x => x.StudentUser)
                .WithMany(y => y.JoinRequests)
                .HasForeignKey(x => x.StudentUserId);

            // Setting relation from teams
            builder.Entity<JoinRequest>()
                .HasOne(x => x.Team)
                .WithMany(y => y.JoinRequests)
                .HasForeignKey(x => x.TeamId);


            // Setting key on the linktable to a composite key
            builder.Entity<TeamInvite>().HasKey(x => new { x.StudentUserId, x.TeamId });

            // Setting relation from students
            builder.Entity<TeamInvite>()
                .HasOne(x => x.StudentUser)
                .WithMany(y => y.TeamInvites)
                .HasForeignKey(x => x.StudentUserId);

            // Setting relation from teams
            builder.Entity<TeamInvite>()
                .HasOne(x => x.Team)
                .WithMany(y => y.TeamInvites)
                .HasForeignKey(x => x.TeamId);
            // Setting relation from Review
            builder.Entity<Review>()
                .HasOne<Company>()
                .WithMany(y => y.Reviews)
                .HasForeignKey(x => x.CompanyId);

            builder.Entity<Review>()
                .HasOne<StudentUser>()
                .WithMany(y => y.Reviews)
                .HasForeignKey(x => x.StudentUserId);

        }
    }
}
