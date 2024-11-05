using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.DbContexts
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

    }
}
