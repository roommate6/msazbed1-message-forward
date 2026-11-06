using Microsoft.Azure.Devices;
using Esp32Backend.Services;
using Azure.Core;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TelemetryMessageStore>();
builder.Services.AddHostedService<IoTHubTelemetryReader>();

var connectionString = "HostName=iothub67.azure-devices.net;SharedAccessKeyName=Backend;SharedAccessKey=iQVoqLb20AU9LM6oA+UMx7dQ5TEAT7/42AIoTCdGc4s=";

builder.Services.AddSingleton(_ =>
    ServiceClient.CreateFromConnectionString(connectionString));

// builder.Services.AddSingleton<TokenCredential>(_ =>
// {
//     if (builder.Environment.IsDevelopment())
//         return new AzureCliCredential();

//     return new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
// });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
