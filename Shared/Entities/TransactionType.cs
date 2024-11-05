using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenditureTrackerWeb.Shared.Entities
{
    public class TransactionType
    {
        [Key]
        public int TT_Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string TT_Name { get; set; } = "";

        public ICollection<TransactionCategory>? TT_TransactionCategories { get; set; }
    }
}
