using System.Collections.Concurrent;

namespace Esp32Backend.Services;

public class TelemetryMessage
{
    public string DeviceId { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
}

public class TelemetryMessageStore
{
    private readonly ConcurrentQueue<TelemetryMessage> _messages = new();

    public void Add(TelemetryMessage message)
    {
        _messages.Enqueue(message);

        while (_messages.Count > 100)
        {
            _messages.TryDequeue(out _);
        }
    }

    public IEnumerable<TelemetryMessage> GetAll()
    {
        return _messages.ToArray().Reverse();
    }
}