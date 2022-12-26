using DemoOpentelemetry.API;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;
using OpenTelemetry.Exporter;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(_ =>
    new SqlConnection("DBConnection")
);


// Add services to the container.
var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions()
{
    EndPoints = { "RedisConnect" },
    AbortOnConnectFail = false
});
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddSingleton<RedisUtility>();

builder.Services.AddOpenTelemetryTracing(b =>
{

    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
     .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation(options =>
    {
        options.Enrich = (activity, eventName, rawObject) =>
        {
            if (eventName.Equals("OnStartActivity"))
            {
                if (rawObject is HttpRequestMessage httpRequest)
                {
                    var request = "empty";
                    if (httpRequest.Content != null)
                        request = httpRequest.Content.ReadAsStringAsync().Result;
                    activity.SetTag("http.request_content", request);
                }
            }

            if (eventName.Equals("OnStopActivity"))
            {

                if (rawObject is HttpResponseMessage httpResponse)
                {
                    var response = "empty";
                    if (httpResponse.Content != null)
                        response = httpResponse.Content.ReadAsStringAsync().Result;
                    activity.SetTag("http.response_content", response);
                }
            }

        };
    })
     .AddRedisInstrumentation(connectionMultiplexer, o => o.SetVerboseDatabaseStatements = true)
     .AddSqlClientInstrumentation(options =>
     {
         options.EnableConnectionLevelAttributes = true;
         options.SetDbStatementForStoredProcedure = true;
         options.SetDbStatementForText = true;
         options.RecordException = true;
         options.Enrich = (activity, x, y) => activity.SetTag("db.type", "sql");
     })
     .AddOtlpExporter(otlpOptions => 
     {
         otlpOptions.Endpoint = new Uri("http://localhost:4317"); 
     });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();