using BusinessOperationsSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskEntity = BusinessOperationsSaaS.Domain.Entities.Task;

namespace BusinessOperationsSaaS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Industry)
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .HasMaxLength(150);

            entity.Property(x => x.Phone)
                .HasMaxLength(30);

            entity.Property(x => x.Address)
                .HasMaxLength(250);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Phone)
                .HasMaxLength(30);

            entity.Property(x => x.Position)
                .HasMaxLength(100);

            entity.Property(x => x.Department)
                .HasMaxLength(100);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Priority)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AssignedToEmployee)
                .WithMany()
                .HasForeignKey(x => x.AssignedToEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.CompanyId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.Property(x => x.SKU)
                .HasMaxLength(100);

            entity.Property(x => x.Category)
                .HasMaxLength(100);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.CompanyId);

            entity.HasIndex(x => x.SKU);
        });

        modelBuilder.Entity<FinancialTransaction>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.CompanyId);

            entity.HasIndex(x => x.TransactionDate);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.MonthlyPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.YearlyPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.MaxEmployees)
                .IsRequired();

            entity.Property(x => x.MaxProducts)
                .IsRequired();

            entity.Property(x => x.MaxTasks)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.StripeCustomerId)
                .HasMaxLength(200);

            entity.Property(x => x.StripeSubscriptionId)
                .HasMaxLength(200);

            entity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(x => x.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.CompanyId);

            entity.HasIndex(x => x.SubscriptionPlanId);

            entity.HasIndex(x => x.StripeCustomerId);

            entity.HasIndex(x => x.StripeSubscriptionId);
        });
    }
}

