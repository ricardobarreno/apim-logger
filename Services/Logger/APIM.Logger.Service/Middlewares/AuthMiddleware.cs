using System.Text.Json;
using APIM.Logger.Service.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APIM.Logger.Service.Middlewares;

public sealed class AuthMiddleware : IFunctionsWorkerMiddleware
{
	private readonly IConfiguration configuration;
	private readonly ILogger<AuthMiddleware> logger;

	public AuthMiddleware(IConfiguration configuration, ILogger<AuthMiddleware> logger)
	{
		this.configuration = configuration;
		this.logger = logger;
	}


	public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
	{
		logger.LogInformation($"Start method execution: {nameof(AuthMiddleware)}.{nameof(Invoke)}");

		AuthToken authToken = await this.GetAuthTokenAsync();
		context.Items.Add("authToken", authToken);

		await next(context);

		logger.LogInformation($"End method execution: {nameof(AuthMiddleware)}.{nameof(Invoke)}");
	}


	private async Task<AuthToken> GetAuthTokenAsync()
	{
		logger.LogInformation($"Start method execution: {nameof(AuthMiddleware)}.{nameof(GetAuthTokenAsync)}");

		try
		{
			HttpResponseMessage response;

			if (IsProduction())
			{
				string endpoint = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://management.azure.com/";
				logger.LogInformation($"Get access token from: {endpoint}");

				using HttpClient httpClient = new();
				httpClient.DefaultRequestHeaders.Add("Metadata", "true");
				response = await httpClient.GetAsync(endpoint);
			}
			else
			{
				string endpoint = $"https://login.microsoftonline.com/{this.configuration["TenantID"]}/oauth2/v2.0/token";
				logger.LogInformation($"Get access token from: {endpoint}");

				var dataToSent = new[]
				{
					new KeyValuePair<string, string>("client_id", this.configuration["ClientID"]),
					new KeyValuePair<string, string>("scope", "https://monitor.azure.com//.default"),
					new KeyValuePair<string, string>("client_secret", this.configuration["ClientSecret"]),
					new KeyValuePair<string, string>("grant_type", "client_credentials"),
				};

				using HttpClient httpClient = new();
				response = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(dataToSent));
			}

			response.EnsureSuccessStatusCode();
			var responseBody = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<AuthToken>(responseBody);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ex.Message);
		}

		logger.LogInformation($"End method execution: {nameof(AuthMiddleware)}.{nameof(GetAuthTokenAsync)}");

		return null;
	}


	private bool IsProduction()
	{
		string environment = configuration.GetValue<string>("AZURE_FUNCTIONS_ENVIRONMENT");
		return environment?.ToLower() == "production";
	}
}
