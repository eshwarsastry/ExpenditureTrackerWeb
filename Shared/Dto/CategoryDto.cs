using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenditureTrackerWeb.Shared.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int TransactionType_Id { get; set; }
        public string TransactionType_Name { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
