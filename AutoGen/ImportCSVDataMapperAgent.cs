using AutoGen.Core;
using AutoGen.Ollama;
using AutoGen.Ollama.Extension;
using ExpenditureTrackerWeb.AutoGen.Prompts;
using ExpenditureTrackerWeb.AutoGen.Services;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using System.Text.Json;

namespace ExpenditureTrackerWeb.AutoGen
{
    public interface IImportCSVDataMapperAgent
    {
        public Task<List<CategoryTransactionTypeMapperDto>> InitializeAgentsAsync(string extractedText);
    }

    public class ImportCSVDataMapperAgent: IImportCSVDataMapperAgent
    {
        public async Task<List<CategoryTransactionTypeMapperDto>> InitializeAgentsAsync(string extractedText)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:11434"),
            };

            // Create agent with specialized role
            var categoryAgent = new OllamaAgent(
                httpClient,
                name: "MasterDataImporter",
                modelName: "mistral:7b",
                systemMessage: ImportCSVDataMapperPrompt.Prompt1
            ).RegisterMessageConnector().RegisterPrintMessage();

            var response = await categoryAgent.SendAsync(extractedText);

            return await ParseResponse(response.GetContent());
        }

        private async Task<List<CategoryTransactionTypeMapperDto>> ParseResponse(string response)
        {
            try
            {
                var result = JsonSerializer.Deserialize<List<CategoryTransactionTypeMapperDto>>(response)!;
                return result ?? new List<CategoryTransactionTypeMapperDto>();
            }

            catch (Exception ex)
            {
                // Handle parsing errors
                return new List<CategoryTransactionTypeMapperDto>();
            }
        }
    }
}
