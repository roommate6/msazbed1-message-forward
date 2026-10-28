using Microsoft.Azure.Devices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// var connectionString =
//     builder.Configuration["IoTHub:ConnectionString"]
//     ?? throw new InvalidOperationException(
//         "Missing IoTHub connection string.");

var connectionString = "HostName=iothub67.azure-devices.net;SharedAccessKeyName=Backend;SharedAccessKey=iQVoqLb20AU9LM6oA+UMx7dQ5TEAT7/42AIoTCdGc4s=";

builder.Services.AddSingleton(_ =>
    ServiceClient.CreateFromConnectionString(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
