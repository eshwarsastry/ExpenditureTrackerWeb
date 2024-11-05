using ExpenditureTrackerWeb.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenditureTrackerWeb.Shared.Entities
{
    public class TransactionCategory
    {
        [Key]
        public int TC_Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string TC_Name { get; set; } = "";
        [Column(TypeName = "nvarchar(150)")]
        public string TC_Description { get; set; } = "";
        
        public ICollection<Expense>? TC_Expenses { get; set; }
        public User TC_User { get; set; }
        public TransactionType TC_TransactionType { get; set; }
    }
}
