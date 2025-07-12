using AutoGen;
using AutoGen.Core;
using AutoGen.Ollama;
using AutoGen.Ollama.Extension;
using ExpenditureTrackerWeb.AutoGen.Prompts;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using System.Buffers.Text;

namespace ExpenditureTrackerWeb.AutoGen
{
    public interface IBillInformationExtractorAgent
    {
        public Task<BillDetailsExtractor> InitializeAgentsAsync(string base64Image);
    }

    public class BillInformationExtractorAgent : IBillInformationExtractorAgent
    {
        public async Task<BillDetailsExtractor> InitializeAgentsAsync(string base64Image)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:11434"),
            };

            // Create agent with specialized role
            var categoryAgent = new OllamaAgent(
                httpClient,
                name: "BillDetailsExtractor",
                modelName: "llava:7b-v1.6-mistral-q4_1",
                systemMessage: BillsExtractorAgentPrompt.prompt2
            ).RegisterMessageConnector().RegisterPrintMessage();


            var response = await categoryAgent.SendAsync(base64Image);
            
            return await ParseResponse(response.GetContent());
        }

        private async Task<BillDetailsExtractor> ParseResponse(string response)
        {
            // Parse the JSON response to extract bill details.
            try
            {
                var billDetails = System.Text.Json.JsonSerializer.Deserialize<BillDetailsExtractor>(response);
                return billDetails ?? new BillDetailsExtractor();
            }
            catch (Exception ex)
            {
                // Handle parsing errors
                Console.WriteLine($"Error parsing response: {ex.Message}");
                return new BillDetailsExtractor();
            }
        }
    }
}
