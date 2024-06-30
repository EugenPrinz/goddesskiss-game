using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class RoResearch
{
	[JsonProperty]
	public string id { get; private set; }

	public static RoResearch Create(string id)
	{
		RoResearch roResearch = new RoResearch();
		roResearch.id = id;
		return roResearch;
	}
}
