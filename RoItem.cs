using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoItem
{
	public string id;

	public string nameIdx;

	public int level;

	public EItemSetType setType;

	public int pointType;

	public EItemStatType statType;

	public EItemStatType upgradeType;

	public EItemStatType disassembleType;

	public bool isEquip;

	public int statPoint;

	public string currEquipCommanderId;

	public int itemCount;

	public static RoItem Create(string id, int lv, int itemCnt, string commanderId)
	{
		RoItem roItem = new RoItem();
		roItem.id = id;
		roItem.level = lv;
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == id);
		roItem.nameIdx = equipItemDataRow.equipItemName;
		roItem.setType = equipItemDataRow.setItemType;
		roItem.pointType = equipItemDataRow.pointType;
		roItem.statType = equipItemDataRow.statType;
		roItem.upgradeType = equipItemDataRow.UpgradeType;
		roItem.disassembleType = equipItemDataRow.disassembleType;
		roItem.statPoint = equipItemDataRow.statBasePoint + (roItem.level - 1) * equipItemDataRow.statAddPoint;
		roItem.currEquipCommanderId = commanderId;
		roItem.itemCount = itemCnt;
		if (!string.IsNullOrEmpty(commanderId))
		{
			roItem.isEquip = true;
		}
		else
		{
			roItem.isEquip = false;
		}
		return roItem;
	}

	public void SetItemLevel(string id, int curLevel)
	{
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == id);
		if (equipItemDataRow != null)
		{
			level = curLevel;
			statPoint = equipItemDataRow.statBasePoint + (curLevel - 1) * equipItemDataRow.statAddPoint;
		}
	}
}
