namespace ExpenditureTrackerWeb.Shared.Dto
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Category_Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; } = "";
    }
}
