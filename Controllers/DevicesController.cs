
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;

namespace Esp32Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
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

        return Ok();
    }
}
