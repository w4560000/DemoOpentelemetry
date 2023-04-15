using Dapper;
using DemoOpentelemetry.API;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
namespace DemoOpentelemetry.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly RedisUtility agGameGcpRedisUtility;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _apiClient;
        private readonly SqlConnection _sqlConnection;

        public WeatherForecastController(HttpClient apiClient, SqlConnection sqlConnection, ILogger<WeatherForecastController> logger, RedisUtility _agGameGcpRedisUtility)
		{
			_logger = logger;
			this.agGameGcpRedisUtility = _agGameGcpRedisUtility;
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7269");
			_sqlConnection = sqlConnection;
        }

        [HttpPost(Name = "GetWeatherForecast")]
		public async Task<IEnumerable<WeatherForecast>> Get(dynamic param)
		{
            var b = "";

            string a = _sqlConnection.Query<string>("SELECT GETDATE(), @TestParam 'TestParam'", new { TestParam = "qqqq123" }).FirstOrDefault();
            _sqlConnection.Query<string>("dbo.SP_Sample", new { QueryParam = "123" }, commandType: CommandType.StoredProcedure).ToList();

            var data = agGameGcpRedisUtility.cacheDB.StringGet("XXKey");

            var response = await _apiClient.GetAsync($"WeatherForecast?param=123");
            if (response.IsSuccessStatusCode)
            {
				b = await response.Content.ReadAsStringAsync();
            }


            DateTime startDate = new(2022,1, 1, 0, 0, 0, DateTimeKind.Utc);
			//Activity.Current?.AddEvent(new ActivityEvent("Generating random weather forecasts"));
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = startDate.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = data + a + b
            }).ToArray();
		}
	}
}