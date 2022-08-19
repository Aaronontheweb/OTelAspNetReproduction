using System.Net;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var resource = ResourceBuilder.CreateDefault()
                .AddService(Assembly.GetEntryAssembly().GetName().Name, serviceInstanceId: $"{Dns.GetHostName()}");

// enables OpenTelemetry for ASP.NET / .NET Core
builder.Services.AddOpenTelemetryTracing(b =>
{
    b
        .SetResourceBuilder(resource)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddJaegerExporter(opt =>
        {

        });
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b
        .SetResourceBuilder(resource)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter(opt =>
        {
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
