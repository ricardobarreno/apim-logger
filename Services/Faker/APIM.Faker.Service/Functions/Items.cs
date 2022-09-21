using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace APIM.Faker.Service;
public class Items
{
	private readonly ILogger logger;
    private readonly Bogus.Faker faker;

	public Items(ILoggerFactory loggerFactory)
	{
        this.logger = loggerFactory.CreateLogger<Items>();
        this.faker = new Bogus.Faker("es");
	}


	[Function(nameof(GetAllItemsAsync))]
	public async Task<HttpResponseData> GetAllItemsAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/items")] HttpRequestData req)
	{
        var data = Enumerable.Range(0, 100).Select(index => new Models.Item
        {
            Name = this.faker.Lorem.Sentence(),
            Description = this.faker.Lorem.Paragraph()
        });

		var response = req.CreateResponse(HttpStatusCode.OK);
		await response.WriteAsJsonAsync(data);

		return response;
	}


    [Function(nameof(GetItemByIdAsync))]
	public async Task<HttpResponseData> GetItemByIdAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/items/{id:guid}")] HttpRequestData req, string id)
	{
        var data = new Models.Item
        {
            Id = Guid.Parse(id),
            Name = this.faker.Lorem.Sentence(),
            Description = this.faker.Lorem.Paragraph()
        };

		var response = req.CreateResponse(HttpStatusCode.OK);
		await response.WriteAsJsonAsync(data);

		return response;
	}


	[Function(nameof(CreateItemAsync))]
	public async Task<HttpResponseData> CreateItemAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/items")] HttpRequestData req)
	{
		string requestBody = string.Empty;

		using (StreamReader streamReader = new StreamReader(req.Body))
		{
			requestBody = await streamReader.ReadToEndAsync();
		}

		JsonObject bodyData = JsonSerializer.Deserialize<JsonObject>(requestBody);

        var data = new Models.Item
        {
            Name = bodyData["name"].ToString(),
            Description = bodyData["description"].ToString()
        };

		var response = req.CreateResponse(HttpStatusCode.Created);
		await response.WriteAsJsonAsync(data);

		return response;
	}

	[Function(nameof(DeleteItemByIdAsync))]
	public HttpResponseData DeleteItemByIdAsync([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "v1/items/{id:guid}")] HttpRequestData req, string id)
	{
        var response = req.CreateResponse(HttpStatusCode.NoContent);
		return response;
	}
}