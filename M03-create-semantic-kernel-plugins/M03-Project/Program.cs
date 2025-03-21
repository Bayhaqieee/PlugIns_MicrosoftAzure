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

var history = new ChatHistory();
history.AddSystemMessage("The year is 2025 and the current month is January");

// Enter the following code to a prompt to trigger the SearchFlights function:
// c#
// AddUserMessage("Find me a flight to Tokyo on the 19");
// await GetReply();
// GetInput();
// await GetReply();

//Output
// User: Find me a flight to Tokyo on the 19
// Assistant: I found a flight to Tokyo on January 19th. 

// - Airline: Air Japan
// - Price: $1200

// Would you like to book this flight?
// User: Yes
// Assistant: Congratulations! Your flight to Tokyo on January 19th with Air Japan has been successfully booked. The total price for the flight is $1200.