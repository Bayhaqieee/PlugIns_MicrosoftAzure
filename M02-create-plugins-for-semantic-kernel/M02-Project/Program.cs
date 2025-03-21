using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var config = new ConfigurationBuilder()
    .AddJsonFile("../../appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()  // Load from User Secrets
    .Build();

// Read values from configuration (User Secrets will override empty values)
var modelId = config["OpenAI:ModelId"] ?? "gpt-35-turbo-16k"; 
var endpoint = config["OpenAI:Endpoint"] ?? throw new ArgumentNullException("OpenAI:Endpoint is missing.");
var apiKey = config["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey is missing.");
var deploymentName = config["OpenAI:DeploymentName"] ?? throw new ArgumentNullException("OpenAI:DeploymentName is missing.");

// Create a kernel builder with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

// Build the kernel
var kernel = builder.Build();

string prompt = """
    You are a helpful travel guide. 
    I'm visiting {{$city}}. {{$background}}. What are some activities I should do today?
    """;
string city = "Barcelona";
string background = "I really enjoy art and dance.";

// Create the kernel function from the prompt
var activitiesFunction = kernel.CreateFunctionFromPrompt(prompt);

// Create the kernel arguments
var arguments = new KernelArguments { ["city"] = city, ["background"] = background };

// InvokeAsync on the kernel object
var result = await kernel.InvokeAsync(activitiesFunction, arguments);
Console.WriteLine(result);