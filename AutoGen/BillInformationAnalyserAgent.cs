using AutoGen.Core;
using AutoGen.Ollama;
using AutoGen.Ollama.Extension;
using ExpenditureTrackerWeb.AutoGen.Prompts;
using ExpenditureTrackerWeb.AutoGen.Services;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;

namespace ExpenditureTrackerWeb.AutoGen
{
    public interface IBillInformationAnalyserAgent
    {
        public Task<BillDetailsAnalyserDto> InitializeAgentsAsync(string billInformationText, string listOfCategories);
    }

    public class BillInformationAnalyserAgent : IBillInformationAnalyserAgent
    {
        public async Task<BillDetailsAnalyserDto> InitializeAgentsAsync(string billInformationText, string listOfCategories)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:11434"),
            };
            
            var agentPrompt = BillsAnalyserAgentPrompt.Prompt1.Replace("[category_list]", listOfCategories);

            // Create agent with specialized role
            var categoryAgent = new OllamaAgent(
                httpClient,
                name: "BillDetailsAnalyser",
                modelName: "mistral:7b",
                systemMessage: agentPrompt
            ).RegisterMessageConnector().RegisterPrintMessage();

            var response = await categoryAgent.SendAsync(billInformationText);
            
            return await ParseResponse(response.GetContent());
        }

        private async Task<BillDetailsAnalyserDto> ParseResponse(string response)
        {
            // Parse the JSON response to extract bill details.
            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonDeserializerConverter(), new StringToDoubleConverter() }
                };

                var billDetails = JsonSerializer.Deserialize<BillDetailsAnalyserDto>(response, options)!;
                return billDetails ?? new BillDetailsAnalyserDto();
            }

            catch (Exception ex)
            {
                // Handle parsing errors
                return new BillDetailsAnalyserDto();
            }
        }

    }
}
