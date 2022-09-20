namespace APIM.Logger.Service.Models;

public sealed record ApimLog
{
	public string EventId { get; set; } = Guid.NewGuid().ToString();
	public string Topic { get; set; } = "";
	public string Subject { get; set; } = "";
	public string EventType { get; set; } = "";
	public string DataVersion { get; set; } = "1.0";
	public string MetadataVersion { get; set; } = "1";
	public DateTime EventTime { get; set; }
	public object Data { get; set; }

	public static ApimLog FromEventLog(EventLog eventLog) =>
		new()
		{
			EventId = eventLog.Id,
			Topic = eventLog.Topic,
			Subject = eventLog.Subject,
			EventType = eventLog.EventType,
			DataVersion = eventLog.DataVersion,
			MetadataVersion = eventLog.MetadataVersion,
			EventTime = eventLog.EventTime,
			Data = eventLog.Data
		};
}
