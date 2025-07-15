using ExpenditureTrackerWeb.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User -> Expense (Restrict)
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.EX_User)
                .WithMany(u => u.U_Expense)
                .HasForeignKey(e => e.EX_UserU_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> TransactionCategory (Cascade is OK)
            modelBuilder.Entity<TransactionCategory>()
                .HasOne(tc => tc.TC_User)
                .WithMany(u => u.U_TransactionCategories)
                .HasForeignKey(tc => tc.TC_UserU_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // TransactionCategory -> Expense (Cascade is OK)
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.EX_TransactionCategory)
                .WithMany(tc => tc.TC_Expenses)
                .HasForeignKey(e => e.EX_TransactionCategoryTC_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // TransactionCategory -> TransactionType (Restrict or Cascade, doesn't affect Expense)
            modelBuilder.Entity<TransactionCategory>()
                .HasOne(tc => tc.TC_TransactionType)
                .WithMany(tt => tt.TT_TransactionCategories)
                .HasForeignKey(tc => tc.TC_TransactionTypeTT_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionType>().HasData(
                GetDefaultTransactionTypes()
            );
        }

        private static List<TransactionType> GetDefaultTransactionTypes()
        {
            return new List<TransactionType>
            {
                new TransactionType
                {
                    TT_Id = 1,
                    TT_Name = "Income"
                },
                new TransactionType
                {
                    TT_Id = 2,
                    TT_Name = "Expenditure"
                },
                new TransactionType
                {
                    TT_Id = 3,
                    TT_Name = "Transfer"
                },
                new TransactionType
                {
                    TT_Id = 4,
                    TT_Name = "Investment"
                },
                new TransactionType
                {
                    TT_Id = 5,
                    TT_Name = "Loan"
                },
                new TransactionType
                {
                    TT_Id = 6,
                    TT_Name = "Refund"
                },
                new TransactionType
                {
                    TT_Id = 7,
                    TT_Name = "Other"
                }
            };
        }
    }
}
