namespace ExpenditureTrackerWeb.Shared.Dto.Agent
{
    public class BillDetailsExtractor
    {
        public DateTime BillDate { get; set; }
        public double BillAmount { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
