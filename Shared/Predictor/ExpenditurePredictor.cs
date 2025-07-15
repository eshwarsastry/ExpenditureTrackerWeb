using ExpenditureTrackerWeb.Shared.Dto.Predictor;
using ExpenditureTrackerWeb.Shared.Services;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ExpenditureTrackerWeb.Shared.Predictor
{
    public interface IExpenditurePredictor
    {
        Task<ExpensePredictorResult> PredictExpenses(int userId, int monthOfPrediction, int yearOfPrediction);
    }
    public class ExpenditurePredictor : IExpenditurePredictor
    {
        private readonly MLContext mlContext = new();
        private readonly IExpensesService expensesService;
        public ExpenditurePredictor(IExpensesService _expensesService)
        {
            expensesService = _expensesService;
        }

        public async Task<ExpensePredictorResult> PredictExpenses(int userId, int monthOfPrediction, int yearOfPrediction)
        {
            var data = await expensesService.GetExpensesOfUserforPrediction(userId);

            var trainingDataOfUser = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms
                .Conversion.ConvertType("Month_f", "Month", DataKind.Single)
                .Append(mlContext.Transforms.Conversion.ConvertType("Year_f", "Year", DataKind.Single))
                .Append(mlContext.Transforms.Concatenate("Features", "Month_f", "Year_f"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Amount"));

            var model = pipeline.Fit(trainingDataOfUser);

            // Make predictions for next month
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ExpensePredictorData, ExpensePredictorResult>(model);

            var prediction = predictionEngine.Predict(new ExpensePredictorData { Month = monthOfPrediction, Year = yearOfPrediction });

            return prediction;
        }
    }

}
