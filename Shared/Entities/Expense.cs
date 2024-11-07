using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenditureTrackerWeb.Shared.Entities
{
    public class Expense
    {
        [Key]
        public int EX_Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal EX_Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EX_DateTime { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string EX_Note { get; set; } = "";
        
        public int EX_UserU_Id { get; set; } //Foreign Key property.
        public User EX_User { get; set; }

        public int EX_TransactionCategoryTC_Id { get; set; } //Foreign Key property.
        public TransactionCategory EX_TransactionCategory { get; set; }
    }
}
