using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Read values from configuration (User Secrets will override empty values)
var modelId = config["OpenAI:ModelId"] ?? "gpt-35-turbo-16k";
var endpoint = config["OpenAI:Endpoint"] ?? throw new ArgumentNullException("OpenAI:Endpoint is missing.");
var apiKey = config["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey is missing.");
var deploymentName = config["OpenAI:DeploymentName"] ?? throw new ArgumentNullException("OpenAI:DeploymentName is missing.");

// Create a kernel builder and configure Azure OpenAI
var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Add the plugins
kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");
kernel.Plugins.AddFromType<CurrencyExchangePlugin>("CurrencyExchange");
kernel.Plugins.AddFromType<WeatherPlugin>("Weather");

// Select the plugin functions
KernelFunction searchFlight = kernel.Plugins.GetFunction("FlightBooking", "search_flights");
KernelFunction convertCurrency = kernel.Plugins.GetFunction("CurrencyExchange", "convert_currency");

// Enable planning
KernelFunction getWeather = kernel.Plugins.GetFunction("Weather", "get_weather");

PromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeather]) 
};

var history = new ChatHistory();
history.AddSystemMessage("The year is 2025 and the current month is January");
AddUserMessage("What is the weather in Tokyo");
await GetReply();



// Enter the following code to a prompt to trigger the SearchFlights function:
// c#
// AddUserMessage("Find me a flight to Tokyo on the 19");
// await GetReply();
// GetInput();
// await GetReply();

//Output
// User: What is the weather in Tokyo
// Assistant: The current weather in Tokyo is rainy with a temperature of 18.3°C and a humidity level of 85%.

