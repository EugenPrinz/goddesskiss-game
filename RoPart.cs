using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoPart
{
	public int count;

	public string id;

	public string nameIdx;

	public string descriptionIdx;

	public static RoPart Create(string id, int count)
	{
		RoPart roPart = new RoPart();
		roPart.id = id;
		roPart.count = count;
		roPart.nameIdx = RemoteObjectManager.instance.regulation.partDtbl.Find((PartDataRow row) => row.type == id).name;
		roPart.descriptionIdx = RemoteObjectManager.instance.regulation.partDtbl.Find((PartDataRow row) => row.type == id).description;
		return roPart;
	}

	public PartDataRow GetPartData()
	{
		return RemoteObjectManager.instance.regulation.partDtbl.Find((PartDataRow row) => row.type == id);
	}
}
