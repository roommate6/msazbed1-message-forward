
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI;
using OpenAI.Responses;

namespace Esp32Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    const string endpoint = "https://fplaystefan-4447-resource.services.ai.azure.com/api/projects/fplaystefan-4447";
    const string agentName = "emotion-number";
    const string userMessage = "Hi emotion-number";

    private readonly ServiceClient _serviceClient;

    public DevicesController(ServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }

    [HttpPost("{deviceId}/message")]
    public async Task<IActionResult> ForwardMessageAsync(
        string deviceId,
        [FromBody] string messageText)
    {
        using Message message = new(Encoding.UTF8.GetBytes(messageText))
        {
            ContentType = "text/plain",
            ContentEncoding = "utf-8"
        };

        await _serviceClient.SendAsync(deviceId, message);

#pragma warning disable OPENAI001

        AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new AzureCliCredential());

        ProjectResponsesClient agentClient =
            projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(agentName);

        ProjectConversationsClient conversationClient = new(
            agentClient.Pipeline,
            new OpenAIClientOptions { Endpoint = agentClient.Endpoint });
        ProjectConversation conversation = conversationClient.CreateProjectConversation();

        ProjectResponsesClient responseClient =
            projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgentEndpoint(
                agentName, defaultConversationId: conversation.Id);

        ResponseResult response = responseClient.CreateResponse(userMessage);

        Console.WriteLine(response.GetOutputText());

        return Ok(response.GetOutputText());
    }
}
