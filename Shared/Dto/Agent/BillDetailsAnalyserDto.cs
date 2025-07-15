namespace ExpenditureTrackerWeb.Shared.Dto.Agent
{
    public class BillDetailsAnalyserDto
    {

        public DateTime BillDate { get; set; }
        public double BillAmount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
