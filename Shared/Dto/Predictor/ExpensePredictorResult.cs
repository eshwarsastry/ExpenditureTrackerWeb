using Microsoft.ML.Data;

namespace ExpenditureTrackerWeb.Shared.Dto.Predictor
{
    public class ExpensePredictorResult
    {
        [ColumnName("Score")]
        public float PredictedAmount { get; set; } // The predicted amount for the next months.
    }
}
