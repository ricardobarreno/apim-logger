namespace APIM.Faker.Service.Models;

public sealed record Item
{
	public Guid Id { get; set; }
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public Item()
	{
		this.Id = Guid.NewGuid();
		this.CreatedAt = DateTime.UtcNow;
		this.UpdatedAt = DateTime.UtcNow;
	}
}
