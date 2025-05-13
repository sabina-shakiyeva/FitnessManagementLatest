using Fitness.Entities.Concrete;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;


namespace FitnessManagement.Data

{

    public class GymDbContext :IdentityDbContext<ApplicationUser>

    {

        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }


        public DbSet<Admin> Admins { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Trainer> Trainers { get; set; }

       // public DbSet<Workout> Workouts { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<TrainerSchedule> TrainerSchedules { get; set; }

        //public DbSet<UserWorkout> UserWorkouts { get; set; }
        public DbSet<UserBmiInfo> UserBmiInfos { get; set; }
        public DbSet<UserEquipmentUsage> UserEquipmentUsages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }

        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutDay> WorkoutDays { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<PackageWorkout> PackageWorkouts { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GlobalNotification> GlobalNotifications { get; set; }
        public DbSet<BalanceTopUp> BalanceTopUps { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);


    //        modelBuilder.Entity<Group>()
    //.HasOne(g => g.Trainer)
    //.WithMany(t => t.Groups)
    //.HasForeignKey(g => g.TrainerId)
    //.OnDelete(DeleteBehavior.SetNull);
            // WorkoutPlan -> WorkoutDay (One to Many)
            modelBuilder.Entity<WorkoutDay>()
                .HasOne(wd => wd.WorkoutPlan)
                .WithMany(wp => wp.WorkoutDays)
                .HasForeignKey(wd => wd.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutDay -> WorkoutExercise (One to Many)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.WorkoutDay)
                .WithMany(wd => wd.Exercises)
                .HasForeignKey(we => we.WorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutPlan <-> Package (Many-to-Many with join table: PackageWorkout)
            modelBuilder.Entity<PackageWorkout>()
                .HasKey(pw => new { pw.PackageId, pw.WorkoutPlanId }); // Composite key

            modelBuilder.Entity<PackageWorkout>()
                .HasOne(pw => pw.Package)
                .WithMany(p => p.PackageWorkouts)
                .HasForeignKey(pw => pw.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PackageWorkout>()
                .HasOne(pw => pw.WorkoutPlan)
                .WithMany(wp => wp.PackageWorkouts)
                .HasForeignKey(pw => pw.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
               .HasOne(ci => ci.User)  
               .WithMany(u => u.CartItems)  
               .HasForeignKey(ci => ci.UserId);  

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)  
                .WithMany(p => p.CartItems)  
                .HasForeignKey(ci => ci.ProductId);  



            modelBuilder.Entity<GroupUser>()
    .HasKey(gu => new { gu.GroupId, gu.UserId });

            modelBuilder.Entity<GroupUser>()
                .HasOne(gu => gu.Group)
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(gu => gu.GroupId);

            modelBuilder.Entity<GroupUser>()
                .HasOne(gu => gu.User)
                .WithMany(u => u.GroupUsers)
                .HasForeignKey(gu => gu.UserId);


            modelBuilder.Entity<UserEquipmentUsage>()
       .HasOne(ue => ue.User)
       .WithMany(u => u.EquipmentUsages)
       .HasForeignKey(ue => ue.UserId)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserEquipmentUsage>()
                .HasOne(ue => ue.Equipment)
                .WithMany(e => e.EquipmentUsages)
                .HasForeignKey(ue => ue.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()

           .HasOne(u => u.Package)

           .WithMany(s => s.Users)

           .HasForeignKey(u => u.PackageId)

           .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<User>()
              .HasOne(u => u.Trainer) 
              .WithMany(t => t.Users) 
              .HasForeignKey(u => u.TrainerId) 
              .OnDelete(DeleteBehavior.SetNull);


      //      modelBuilder.Entity<Workout>()

      //.HasOne(w => w.Trainer)

      //.WithMany(t => t.Workouts)

      //.HasForeignKey(w => w.TrainerId);

      //      modelBuilder.Entity<UserWorkout>()

      // .HasKey(uw => new { uw.UserId, uw.WorkoutId });

      //      modelBuilder.Entity<UserWorkout>()

      // .HasOne(uw => uw.User)

      // .WithMany(u => u.UserWorkouts)

      // .HasForeignKey(uw => uw.UserId)

      //  .OnDelete(DeleteBehavior.Cascade);

      //      modelBuilder.Entity<UserWorkout>()

      //          .HasOne(uw => uw.Workout)

      //          .WithMany(w => w.UserWorkouts)

      //          .HasForeignKey(uw => uw.WorkoutId)

      //          .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Attendance>()

    .HasOne(a => a.User)

    .WithMany()

    .HasForeignKey(a => a.UserId)

    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainerSchedule>()

    .HasOne(ts => ts.Trainer)

    .WithMany(t => t.TrainerSchedules)

    .HasForeignKey(ts => ts.TrainerId)

    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserBmiInfo>()
    .HasOne(u => u.User)
    .WithMany(u => u.BmiInfos)
    .HasForeignKey(u => u.UserId);



        }

    }

}

