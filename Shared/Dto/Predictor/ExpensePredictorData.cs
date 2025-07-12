namespace ExpenditureTrackerWeb.Shared.Dto.Predictor
{
    public class ExpensePredictorData
    {
        public int UserId { get; set; } // User ID for whom the expenses are predicted
        public int Month { get; set; } // 1 for January, 2 for February, etc.
        public int Year { get; set; } // Year of the expenses
        public float Amount { get; set; } // Amount spent in that month (changed from decimal to float for ML.NET compatibility)
    }
}
