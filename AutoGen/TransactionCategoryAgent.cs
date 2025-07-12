using AutoGen;
using AutoGen.Core;
using AutoGen.Ollama;
using AutoGen.Ollama.Extension;
using ExpenditureTrackerWeb.Shared.Dto;
using Google.Cloud.AIPlatform.V1;

namespace ExpenditureTrackerWeb.AutoGen
{
    public interface ITransactionCategoryAgent
    {
        public Task<List<CategoryDto>> InitializeAgentsAsync();
    }

    public class TransactionCategoryAgent : ITransactionCategoryAgent
    {
        public async Task<List<CategoryDto>> InitializeAgentsAsync()
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:11434"),
            };

            // Create agent with specialized role
            var categoryAgent = new OllamaAgent(
                httpClient,
                name: "CategoryExtractor",
                modelName: "mistral",
                systemMessage: """
            You are a financial data analyst specialized in transaction categorization.
            Your task is to analyze transaction data and extract distinct categories.
            
            Rules:
            1. Identify all unique transaction categories from the CSV file.
            2. Normalize variations (e.g., "Grocery" and "Groceries" should become "Grocery")
            3. Distinguish the transaction type of transactions as "Income" or "Expense"
            4. If the Amount is negative then it might be an expense, otherwise it is income
            4. Standardize capitalization (Title Case)
            5. Return only a comma-separated list of distinct categories
            6. Do NOT include any explanations or additional text
            
            Example output: "Category1-Income, Category2-Expense, Category3-Income"
            """
            ).RegisterMessageConnector().RegisterPrintMessage();

            string csvContent = "";

            var response = await categoryAgent.SendAsync($"Identify and return the dictincet transaction categories and their types from this file:{csvContent}");
            return ParseResponse(response.GetContent());
        }

        private List<CategoryDto> ParseResponse(string response)
        {
            // Extract the comma-separated list
            return response.Split(',')
                .Select(c => c.Trim())
                .Select(c => new CategoryDto
                {
                    Name = c.Split('-')[0].Trim(),
                    TransactionType_Name = c.Split('-').Length > 1 ? c.Split('-')[1].Trim() : "Unknown",
                    TransactionType_Id = c.Split('-').Length > 1 && c.Split('-')[1].Trim().Equals("Income", StringComparison.OrdinalIgnoreCase) ? 1 : 2, // Assume 1 as Income and 2 as Expense.
                })
                .ToList();
        }
    }
}
