using DemoOpentelemetry.API2;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(new ConfigurationOptions()
{
    EndPoints = { "RedisConnection" },
    AbortOnConnectFail = false
}));

builder.Services.AddSingleton<RedisUtility>();

builder.Services.AddOpenTelemetryTracing(b => {
    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
     .SetSampler(new AlwaysOnSampler())
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
     .AddRedisInstrumentation()
     .AddSqlClientInstrumentation()
     .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
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
