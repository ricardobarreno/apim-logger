namespace APIM.Logger.Service.Models;

public sealed record EventLog
{
	public string Topic { get; set; } = "";
	public string Subject { get; set; } = "";
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string EventType { get; set; } = "";
	public DateTime EventTime { get; set; }
	public object Data { get; set; }
	public string DataVersion { get; set; } = "";
	public string MetadataVersion { get; set; } = "";
}
