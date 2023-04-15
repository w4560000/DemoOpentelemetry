using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryTracing(b => {
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
     .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

static bool Filter(HttpContext httpContext)
{
	Console.WriteLine(httpContext.Request.Path.ToString());
	if (httpContext.Request.Path.StartsWithSegments("/"))
	{
		return true;
	}
	return false;
}
