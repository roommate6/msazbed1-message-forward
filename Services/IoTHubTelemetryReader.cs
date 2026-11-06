using Azure.Messaging.EventHubs.Consumer;

namespace Esp32Backend.Services;

public class IoTHubTelemetryReader : BackgroundService
{
    private readonly TelemetryMessageStore _store;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IoTHubTelemetryReader> _logger;

    public IoTHubTelemetryReader(
        TelemetryMessageStore store,
        IConfiguration configuration,
        ILogger<IoTHubTelemetryReader> logger)
    {
        _store = store;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var connectionString =
            _configuration["IoTHub:EventHubConnectionString"]
            ?? throw new InvalidOperationException(
                "Missing IoTHub Event Hub connection string.");

        await using var consumer = new EventHubConsumerClient(
            EventHubConsumerClient.DefaultConsumerGroupName,
            connectionString);

        _logger.LogInformation("Listening for IoT Hub messages...");

        await foreach (var partitionEvent in consumer.ReadEventsAsync(
            startReadingAtEarliestEvent: false,
            cancellationToken: stoppingToken))
        {
            if (partitionEvent.Data == null)
                continue;

            var eventData = partitionEvent.Data;

            var deviceId = "Unknown";

            if (eventData.SystemProperties.TryGetValue(
                "iothub-connection-device-id", out var id))
            {
                deviceId = id?.ToString() ?? "Unknown";
            }

            var telemetry = new TelemetryMessage
            {
                DeviceId = deviceId,
                Body = eventData.EventBody.ToString(),
                ReceivedAt = DateTime.UtcNow
            };

            _store.Add(telemetry);

            _logger.LogInformation(
                "Message received from {DeviceId}: {Body}",
                deviceId,
                telemetry.Body);
        }
    }
}