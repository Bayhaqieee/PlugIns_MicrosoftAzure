using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string filePath = Path.GetFullPath("../../appsettings.json");
var config = new ConfigurationBuilder()
    .AddJsonFile(filePath)
    .Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};

var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();

string prompt = """
    <message role="system">Instructions: Provide the user with destination recommendations relevant to their background information.
    Ask the user their budget and purpose of their trip before providing recommendations</message>
    <message role="user">Can you give me some travel suggestions?</message>
    <message role="assistant">Sure, do you have a budget in mind?</message>
    <message role="user">{{input}}</message>
    <message role="assistant">Great! Can you tell me if the trip is for business or leisure?</message>
    """;

history.AddSystemMessage(prompt);
AddUserMessage("Can you give me some destination suggestions for a company event? The event budget is $10,000.");
await SyncPreviousChat();

async Task SyncPreviousChat() {
    kernel.Plugins.AddFromType<PreviousChatPlugin>("PreviousChatPlugin");

    var chatPrompt = @"{{PreviousChatPlugin.get_previous_conversation}}";
    var promptConfig = new PromptTemplateConfig(chatPrompt)
    {
        AllowDangerouslySetContent = true
    };

    var function = KernelFunctionFactory.CreateFromPrompt(promptConfig);
    var result = await kernel.InvokeAsync(function, []);
    Console.WriteLine(result);
}

var promptConfig = new PromptTemplateConfig(chatPrompt)
{
    AllowDangerouslySetContent = true
};

void GetInput() {
    Console.Write("User: ");
    string input = Console.ReadLine()!;
    history.AddUserMessage(input);
}

async Task GetReply() {
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );
    Console.WriteLine("Assistant: " + reply.ToString());
    history.AddAssistantMessage(reply.ToString());
}

void AddUserMessage(string msg) {
    Console.WriteLine("User: " + msg);
    history.AddUserMessage(msg);
}