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

        // Create a kernel builder and configure Azure OpenAI
        var builder = Kernel.CreateBuilder();
        builder.Services.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

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

        // 3rd Assignment
        // Build the kernel
        Kernel kernel = builder.Build();

        // Get chat completion service.
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Create a chat history object
        ChatHistory chatHistory = [];

        void AddMessage(string msg) {
            Console.WriteLine(msg);
            chatHistory.AddAssistantMessage(msg);
        }

        void GetInput() {
            string input = Console.ReadLine()!;
            chatHistory.AddUserMessage(input);
        }

        async Task GetReply() {
            ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                kernel: kernel
            );
            Console.WriteLine(reply.ToString());
            chatHistory.AddAssistantMessage(reply.ToString());
        }

        // Prompt the LLM
        chatHistory.AddSystemMessage("You are a helpful travel assistant.");
        chatHistory.AddSystemMessage("Recommend a destination to the traveler based on their background and preferences.");

        // Get information about the user's plans
        AddMessage("Tell me about your travel plans.");
        GetInput();
        await GetReply();

        // Offer recommendations
        AddMessage("Would you like some activity recommendations?");
        GetInput();
        await GetReply();

        // Offer language tips
        AddMessage("Would you like some helpful phrases in the local language?");
        GetInput();
        await GetReply();

        Console.WriteLine("Chat Ended.\n");
        Console.WriteLine("Chat History:");

        for (int i = 0; i < chatHistory.Count; i++)
        {
            Console.WriteLine($"{chatHistory[i].Role}: {chatHistory[i]}");
        }
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

// 3rd Output
// Chat History:
// system: You are a helpful travel assistant.
// system: Recommend a destination to the traveler based on their background and preferences.
// assistant: Tell me about your travel plans.
// user: I want a Adventorous Travel, would love to see some Rally Racing also, and Romantic where i can take my fiance to a special date    
// assistant: I have the perfect destination for you: Baja California, Mexico. Baja California offers a combination of adventure, rally racing, and romantic settings.

// For adventure, you can explore the stunning landscapes of Baja California through activities such as off-roading, hiking, and kayaking. The Baja Peninsula is known for its rugged terrain, making it the ideal location for thrilling adventures.

// In terms of rally racing, Baja California is home to the famous Baja 1000, an annual off-road race that attracts drivers from all around the world. You could either participate in the race as a driver or simply enjoy spectating and experiencing the exciting atmosphere.       

// To add a touch of romance to your trip, consider staying in one of the beautiful coastal towns such as Ensenada or Todos Santos. These picturesque locations provide a romantic setting for a special date. You can enjoy a sunset stroll along the beach, indulge in delicious seafood, or even visit local wineries for a romantic wine tasting.

// Baja California offers a unique blend of adventure, rally racing, and romantic settings, making it the perfect destination for your travel plans.
// assistant: Would you like some activity recommendations?
// user: Yes
// assistant: Certainly! Here are some activity recommendations for your trip to Baja California:

// 1. Off-Roading: Take a thrilling off-road adventure through the rugged landscapes of Baja California. The region offers a variety of off-roading trails where you can experience the excitement of driving on challenging terrains.

// 2. Rally Racing Experience: If you are a fan of rally racing, consider booking a rally racing experience in Baja California. Professional trainers will guide you through the techniques of rally driving, allowing you to get behind the wheel and feel the adrenaline rush as you navigate the rally tracks.

// 3. Baja 1000 Spectating: Plan your trip around the Baja 1000 race. You can witness the excitement as skilled drivers tackle the demanding off-road race. Find a great vantage point to watch the action unfold and immerse yourself in the rally racing atmosphere.

// 4. Sunset Beach Stroll: Head to the picturesque beaches of Baja California with your fiance and enjoy a romantic sunset beach stroll. The stunning coastal views and the sounds of the ocean create a perfect backdrop for a special date.

// 5. Wine Tasting in Valle de Guadalupe: Visit the famous Valle de Guadalupe, often referred to as Mexico's Napa Valley. Explore the vineyards and wineries, and indulge in wine tasting sessions with your partner. It's a perfect way to elevate the romance and savor delicious wines.

// 6. Scenic Hiking: Baja California offers beautiful hiking trails with diverse landscapes. Explore areas like Sierra de San Pedro Mártir National Park or Sierra La Laguna Biosphere Reserve for scenic hikes and breathtaking views.

// Remember to always check the local guidelines and restrictions before participating in any activities and make sure to prioritize safety. Enjoy your adventurous, romantic, and rally racing-filled trip to Baja California!
// assistant: Would you like some helpful phrases in the local language?
// user: Yes
// assistant: Here are some helpful phrases in Spanish, the local language of Baja California, Mexico:

// 1. Hello - Hola
// 2. Good morning - Buenos días
// 3. Good afternoon - Buenas tardes
// 4. Good evening - Buenas noches
// 5. Thank you - Gracias
// 6. Please - Por favor
// 7. Excuse me - Disculpe
// 8. Yes - Sí
// 9. No - No
// 10. I would like... - Me gustaría...
// 11. Where is...? - ¿Dónde está...?
// 12. How much does it cost? - ¿Cuánto cuesta?
// 13. Can you recommend a restaurant? - ¿Puede recomendar un restaurante?
// 14. Cheers! - ¡Salud!
// 15. Can you help me? - ¿Puede ayudarme?

// These phrases should come in handy during your visit to Baja California. It's always appreciated when travelers make an effort to communicate in the local language. Enjoy your trip!