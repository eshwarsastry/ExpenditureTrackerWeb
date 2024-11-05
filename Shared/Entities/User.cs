using ExpenditureTrackerWeb.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenditureTrackerWeb.Shared.Models
{
    public class User
    {
        [Key]
        public int U_Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string U_FirstName { get; set; } = "";
        [Column(TypeName = "nvarchar(150)")]
        public string U_Name { get; set; } = "";
        [Column(TypeName = "nvarchar(150)")]
        public string U_Email { get; set; } = "";
        [Column(TypeName = "nvarchar(max)")]
        public string U_Password { get; set; } = "";

        public ICollection<TransactionCategory>? U_TransactionCategories { get; set; }
        public ICollection<Expense>? U_Expense { get; set; }

    }
}