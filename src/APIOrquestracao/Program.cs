using APIOrquestracao.Tracing;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using APIOrquestracao.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ContagemClient>();

builder.Services.AddOpenTelemetry().WithTracing(traceProvider =>
{
    traceProvider
        .AddSource(OpenTelemetryExtensions.ServiceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                    serviceVersion: OpenTelemetryExtensions.ServiceVersion))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddJaegerExporter(exporter =>
        {
            exporter.AgentHost = builder.Configuration["Jaeger:AgentHost"];
            exporter.AgentPort = Convert.ToInt32(builder.Configuration["Jaeger:AgentPort"]);
        })
        .AddConsoleExporter();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
