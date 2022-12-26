using Microsoft.AspNetCore.Mvc;

namespace DemoOpentelemetry.API2
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _apiClient;

        public WeatherForecastController(HttpClient apiClient, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<string> Get(string param)
        {
            var tasks = new List<Task>();

            tasks.Add(Task.Run(async () => await _apiClient.GetAsync($"https://odws.hccg.gov.tw/001/Upload/25/opendataback/9059/57/320c9e83-5d8b-4754-8449-e092a55e79c6.json")));
            tasks.Add(Task.Run(async () => await _apiClient.GetAsync($"https://odws.hccg.gov.tw/001/Upload/25/opendataback/9059/57/320c9e83-5d8b-4754-8449-e092a55e79c6.json")));
            tasks.Add(Task.Run(async () => await _apiClient.GetAsync($"https://odws.hccg.gov.tw/001/Upload/25/opendataback/9059/57/320c9e83-5d8b-4754-8449-e092a55e79c6.json")));

            Task.WaitAll(tasks.ToArray());
            //var res1 = _apiClient.GetAsync($"https://odws.hccg.gov.tw/001/Upload/25/opendataback/9059/57/320c9e83-5d8b-4754-8449-e092a55e79c6.json");
            //var res2 = _apiClient.GetAsync($"https://odws.hccg.gov.tw/001/Upload/25/opendataback/9059/57/320c9e83-5d8b-4754-8449-e092a55e79c6.json");

            //Task.WaitAll(res1, res2);
            //Task.WaitAll(res1.Content.ReadAsStringAsync(), res2.Content.ReadAsStringAsync());
            //string b = await response;
            //}

            return "a " + param;
        }
    }
}