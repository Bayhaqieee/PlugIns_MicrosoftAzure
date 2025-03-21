using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
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

        // // Create a kernel builder and configure Azure OpenAI
        // var builder = Kernel.CreateBuilder();
        // builder.Services.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

        // // Build the kernel
        // var kernel = builder.Build();

        // // Define prompt with placeholders
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

        // // Define prompt with placeholders

        // // Invoke the kernel function asynchronously
        // var result = await kernel.InvokeAsync(pluginFunction, arguments);

        // // Output the result
        // Console.WriteLine(result);
        

        // 2nd Assignment
        // Create a kernel with Azure OpenAI chat completion
        // var builder = Kernel.CreateBuilder();
        // builder.Services.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);;

        // // Build the kernel
        // Kernel kernel = builder.Build();

        // string prompt = """
        //     <message role="system">Instructions: Identify the from and to destinations 
        //     and dates from the user's request</message>

        //     <message role="user">Can you give me a list of flights from Seattle to Tokyo? 
        //     I want to travel from March 11 to March 18.</message>

        //     <message role="assistant">
        //     Origin: Seattle
        //     Destination: Tokyo
        //     Depart: 03/11/2025 
        //     Return: 03/18/2025
        //     </message>

        //     <message role="user">{{input}}</message>
        //     """;

        // string input = "I want to travel from June 1 to July 22. I want to go to Greece. I live in Chicago.";

        // // Create the kernel arguments
        // var arguments = new KernelArguments { ["input"] = input };

        // // Create the prompt template config using handlebars format
        // var templateFactory = new HandlebarsPromptTemplateFactory();
        // var promptTemplateConfig = new PromptTemplateConfig()
        // {
        //     Template = prompt,
        //     TemplateFormat = "handlebars",
        //     Name = "FlightPrompt",
        // };

        // // Invoke the prompt function
        // var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
        // var response = await kernel.InvokeAsync(function, arguments);
        // Console.WriteLine(response);
    }
}

// 1st Output
// In Barcelona, you're in for a treat as it's a city rich in art and culture. Here are some activities tailored to your interests:
// 1. Visit the Sagrada Familia: This iconic architectural masterpiece designed by Antoni Gaudí is a must-see. Marvel at the intricate details and spiritual atmosphere of this ongoing construction.
// 2. Explore Park Güell: Another Gaudí masterpiece, Park Güell offers stunning views of the city, interesting architecture, and vibrant mosaic works. Spend time wandering through the park's unique trails and open spaces.
// 3. Head to the Picasso Museum: Dive into the world of Pablo Picasso at this museum, which showcases his early works and the evolution of his artistic style. It's a true gem for art lovers.
// 4. Enjoy the Magic Fountain Show: In the evenings, head to Montjuïc to witness the mesmerizing Magic Fountain Show. Be captivated by the music, water, and light performances in this magical setting.
// 5. Experience Flamenco: Immerse yourself in the passionate world of Flamenco, a traditional Spanish dance. Visit one of Barcelona's many tablaos, such as Tablao Cordobes or Palau Dalmases, to witness an authentic Flamenco performance.
// 6. Explore the Gothic Quarter: Wander through the narrow medieval streets of the Gothic Quarter, where you'll find fascinating architecture, charming cafes, art galleries, and street performers.
// 7. Discover the Contemporary Art Scene: Barcelona boasts numerous contemporary art museums. Visit the Barcelona Museum of Contemporary Art (MACBA) or the CaixaForum, where you can explore thought-provoking exhibitions and installations.
// 8. Experience the "Dancing Fountains" at Plaza Catalunya: Every weekend, Plaza Catalunya comes alive with the "Dancing Fountains" show. Enjoy the synchronized water, light, and music performance in the heart of the city.
// Remember to check the timings and availability of these activities and consider any current COVID-19 restrictions before your visit. Enjoy your artistic journey in vibrant Barcelona!

// 2nd Output

// Origin: Chicago
// Destination: Greece
// Depart: 06/01/2025
// Return: 07/22/2025