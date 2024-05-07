using Azure.AI.OpenAI;
using Azure;
string openAIEndpoint = "https://eastus.api.cognitive.microsoft.com/";
string openAIAPIKey = "4aead22500264999a62da8de302ca90c";
string openAIDeploymentName = "HikingRecommendationTurbo";

var endpoint = new Uri(openAIEndpoint);
var credentials = new AzureKeyCredential(openAIAPIKey);
var openAIClient = new OpenAIClient(endpoint, credentials);

var completionOptions = new ChatCompletionsOptions
{
    MaxTokens=400,
    Temperature=1f,
    FrequencyPenalty=0.0f,
    PresencePenalty=0.0f,
    NucleusSamplingFactor = 0.95f, // Top P
    DeploymentName = openAIDeploymentName
};
var systemPrompt = 
    """
    You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly. You introduce yourself when first saying hello. When helping people out, you always ask them for this information to inform the hiking recommendation you provide:
    
    1. Where they are located
    2. What hiking intensity they are looking for
    
    You will then provide three suggestions for nearby hikes that vary in length after you get that information. You will also share an interesting fact about the local nature on the hikes when making a recommendation.
    """;
    
ChatRequestSystemMessage systemMessage = new(systemPrompt);

completionOptions.Messages.Add(systemMessage);

string userGreeting = """
                      Hi there hiking recommendation bot!
                      Can't wait to hear what you have in store for me!
                      """;

ChatRequestUserMessage userGreetingMessage = new (userGreeting);
completionOptions.Messages.Add(userGreetingMessage);

Console.WriteLine($"User >>> {userGreeting}");

ChatCompletions response = await openAIClient.GetChatCompletionsAsync(completionOptions);

ChatResponseMessage assistantResponse = response.Choices[0].Message;

Console.WriteLine($"AI >>> {assistantResponse.Content}");

completionOptions.Messages.Add(new ChatRequestSystemMessage(assistantResponse.Content));

var hikeRequest = 
    """
    I would like a strenous hike near where I live that ends with
    a view that is amazing.
    """;

Console.WriteLine($"User >>> {hikeRequest}");

ChatRequestUserMessage hikeMessage = new (hikeRequest);

completionOptions.Messages.Add(hikeMessage);

response = await openAIClient.GetChatCompletionsAsync(completionOptions); 

assistantResponse = response.Choices[0].Message;

Console.WriteLine($"AI >>> {assistantResponse.Content}");