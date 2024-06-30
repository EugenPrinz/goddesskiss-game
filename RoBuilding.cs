using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoBuilding
{
	private int _level;

	private BuildingLevelDataRow _reg;

	private BuildingLevelDataRow _nextLevelReg;

	private BuildingLevelDataRow _firstLevelReg;

	public EBuilding type { get; set; }

	public UIBuilding uiBuilding => GetUIBuilding();

	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			if (_level != value)
			{
				_level = value;
				_reg = null;
				_nextLevelReg = null;
			}
		}
	}

	public TimeData upgradeTime { get; private set; }

	public bool isMaxLevel => nextLevelReg == null;

	public bool isUpgrading => upgradeTime.IsValid() && upgradeTime.IsEnd();

	public bool isEndUpgrade => upgradeTime.IsEnd();

	public EBuildingState state { get; set; }

	public string regulationId => $"{type}-{level:00}";

	public string nextLevelRegulationId => $"{type}-{level + 1:00}";

	public BuildingLevelDataRow reg
	{
		get
		{
			if (_reg == null)
			{
				_reg = _GetReguilation(level);
			}
			return _reg;
		}
	}

	public BuildingLevelDataRow nextLevelReg
	{
		get
		{
			if (_nextLevelReg == null)
			{
				_nextLevelReg = _GetReguilation(level + 1);
			}
			return _nextLevelReg;
		}
	}

	public BuildingLevelDataRow firstLevelReg
	{
		get
		{
			if (_firstLevelReg == null)
			{
				_firstLevelReg = _GetReguilation(1);
			}
			return _firstLevelReg;
		}
	}

	private RoBuilding()
	{
	}

	public static RoBuilding Create(EBuilding type, int level = 1)
	{
		RoBuilding roBuilding = new RoBuilding();
		roBuilding.type = type;
		roBuilding.level = level;
		roBuilding.upgradeTime = TimeData.Create();
		return roBuilding;
	}

	private BuildingLevelDataRow _GetReguilation(int level)
	{
		RemoteObjectManager instance = RemoteObjectManager.instance;
		return instance.regulation.buildingLevelDtbl.Find((BuildingLevelDataRow row) => row.type == type && row.level == level);
	}

	public UIBuilding GetUIBuilding()
	{
		if (type == EBuilding.Undefined)
		{
			return null;
		}
		string id = type.ToString();
		UIItemBase uIItemBase = UIManager.instance.world.camp.buildingListView.FindItem(id);
		return uIItemBase as UIBuilding;
	}
}
