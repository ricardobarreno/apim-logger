// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System.Text;
using System.Text.Json;
using APIM.Logger.Service.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APIM.Logger.Service.Functions
{
	public class ApimLogger
	{
		private readonly ILogger logger;
		private readonly IConfiguration configuration;

		public ApimLogger(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			this.logger = loggerFactory.CreateLogger<ApimLogger>();
			this.configuration = configuration;
		}

		[Function("ApimLogger")]
		public async Task Run([EventGridTrigger] EventLog input, FunctionContext context)
		{
			logger.LogInformation("Start method execution: APIMLogEventGrid.Run");

			try
			{
				context.Items.TryGetValue("authToken", out var authToken);

				if (authToken == null)
					throw new NullReferenceException("Auth token does not exist.");

				string accessToken = ((AuthToken)authToken).AccessToken;

				var dataToSent = JsonSerializer.Serialize(new ApimLog[] { ApimLog.FromEventLog(input) });
				string endpoint = $"{this.configuration["LogIngestionEnpoint"]}/dataCollectionRules/{this.configuration["ImmutableId"]}/streams/Custom-{this.configuration["LogTable"]}_CL?api-version=2021-11-01-preview";
				logger.LogInformation($"Send custom log to {endpoint}");

				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
				var response = await httpClient.PostAsync(endpoint, new StringContent(dataToSent, Encoding.UTF8, "application/json"));
				response.EnsureSuccessStatusCode();
				logger.LogInformation("Successful log sending.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
			}

			logger.LogInformation("End method execution: APIMLogEventGrid.Run");
		}
	}
}
