using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Not using appsettings.json because it sucks
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

        // Build the kernel
        var kernel = builder.Build();

        // Define prompt with placeholders
        // string prompt = """
        //     You are a helpful travel guide.
        //     I'm visiting {{$city}}. {{$background}}. What are some activities I should do today?
        //     """;

        // string city = "Barcelona";
        // string background = "I really enjoy art and dance.";

        // // Create a function using the prompt
        // var pluginFunction = kernel.CreateFunctionFromPrompt(prompt);

        // // Set up arguments using KernelArguments
        // var arguments = new KernelArguments
        // {
        //     ["city"] = city,
        //     ["background"] = background
        // };

        string prompt = """
            <message role="system">Instructions: Identify the from and to destinations 
            and dates from the user's request</message>

            <message role="user">Can you give me a list of flights from Seattle to Tokyo? 
            I want to travel from March 11 to March 18.</message>

            <message role="assistant">
            Origin: Seattle
            Destination: Tokyo
            Depart: 03/11/2025 
            Return: 03/18/2025
            </message>

            <message role="user">{{input}}</message>
            """;
            ```

        string input = "I want to travel from June 1 to July 22. I want to go to Greece. I live in Chicago.";

        // Create the kernel arguments
        var arguments = new KernelArguments { ["input"] = input };

        // Create the prompt template config using handlebars format
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = prompt,
            TemplateFormat = "handlebars",
            Name = "FlightPrompt",
        };

        // Invoke the prompt function
        var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
        var response = await kernel.InvokeAsync(function, arguments);
        Console.WriteLine(response);

        // // Invoke the kernel function asynchronously
        // var result = await kernel.InvokeAsync(pluginFunction, arguments);

        // // Output the result
        // Console.WriteLine(result);
    }
}