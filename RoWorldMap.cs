using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoWorldMap
{
	[JsonObject]
	public class Stage
	{
		public string id { get; private set; }

		public bool isOpen { get; set; }

		public bool clear { get; set; }

		public int star { get; set; }

		public bool canBattle => typeData.battleCount > clearCount;

		public bool isCleared => clear;

		public bool isProduct => !string.IsNullOrEmpty(commanderId) && time.IsValid();

		public int clearCount { get; set; }

		public int clearEnableCount => typeData.battleCount - clearCount;

		public string commanderId { get; set; }

		public TimeData time { get; set; }

		public int targetTime { get; set; }

		public int rechargeCount
		{
			get
			{
				RoLocalUser localUser = RemoteObjectManager.instance.localUser;
				if (localUser.stageRechargeList.ContainsKey(id))
				{
					return localUser.stageRechargeList[id];
				}
				return 0;
			}
		}

		public CommanderDataRow commanderData => _reg.commanderDtbl[commanderId];

		public WorldMapStageDataRow data => _reg.worldMapStageDtbl[id];

		public WorldMapStageTypeDataRow typeData => _reg.worldMapStageTypeDtbl[data.typeId];

		public static Stage Create(string id)
		{
			Stage stage = new Stage();
			stage.id = id;
			stage.isOpen = false;
			stage.clearCount = 0;
			stage.clear = false;
			stage.time = TimeData.Create();
			return stage;
		}

		public RoUser GetEnemy()
		{
			return RoUser.CreateNPC("Enemy-" + data.enemyId, "NPC", RoTroop.CreateEnemy(data.enemyId));
		}
	}

	public string id { get; private set; }

	public List<Stage> stageList { get; private set; }

	public bool rwd { get; set; }

	public string lastOpenedStageId { get; private set; }

	public Stage lastOpenedStage => FindStage(lastOpenedStageId);

	public int stageCount => stageList.Count;

	public int clearStageCount
	{
		get
		{
			int count = 0;
			stageList.ForEach(delegate(Stage row)
			{
				if (row.isCleared)
				{
					count++;
				}
			});
			return count;
		}
	}

	public int maxStarCount => stageList.Count * 3;

	public int starCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < stageList.Count; i++)
			{
				num += stageList[i].star;
			}
			return num;
		}
	}

	private static Regulation _reg => RemoteObjectManager.instance.regulation;

	public WorldMapDataRow dataRow => _reg.worldMapDtbl[id];

	public List<WorldMapStageDataRow> stageDataRowList => _reg.worldMapStageDtbl.FindAll((WorldMapStageDataRow row) => row.worldMapId == id);

	public bool isComplete
	{
		get
		{
			if (stageCount <= 0)
			{
				return false;
			}
			return stageCount == clearStageCount;
		}
	}

	public bool isActivate
	{
		get
		{
			if (stageCount <= 0)
			{
				return false;
			}
			return dataRow.unlockUserLevel <= RemoteObjectManager.instance.localUser.level;
		}
	}

	public bool canMoveNextMap
	{
		get
		{
			if (stageCount > clearStageCount)
			{
				return false;
			}
			int num = _reg.worldMapDtbl.FindIndex(id);
			if (!_reg.worldMapDtbl.IsValidIndex(num + 1))
			{
				return false;
			}
			return _reg.worldMapDtbl[num + 1].unlockUserLevel <= RemoteObjectManager.instance.localUser.level;
		}
	}

	public bool canMovePreviousMap
	{
		get
		{
			int num = _reg.worldMapDtbl.FindIndex(id);
			if (!_reg.worldMapDtbl.IsValidIndex(num - 1) || num - 1 == 0)
			{
				return false;
			}
			return _reg.worldMapDtbl[num - 1] != null;
		}
	}

	public string name => dataRow.name;

	public static List<RoWorldMap> CreateAll()
	{
		List<RoWorldMap> list = new List<RoWorldMap>();
		_reg.worldMapDtbl.ForEach(delegate(WorldMapDataRow row)
		{
			list.Add(Create(row.id));
		});
		return list;
	}

	public static RoWorldMap Create(string worldMapId)
	{
		if (!_reg.worldMapDtbl.ContainsKey(worldMapId))
		{
			return null;
		}
		RoWorldMap roWorldMap = new RoWorldMap();
		roWorldMap.id = worldMapId;
		List<Stage> stageList = new List<Stage>();
		List<WorldMapStageDataRow> list = roWorldMap.stageDataRowList;
		list.ForEach(delegate(WorldMapStageDataRow row)
		{
			stageList.Add(Stage.Create(row.id));
		});
		roWorldMap.stageList = stageList;
		if (stageList.Count > 0)
		{
			roWorldMap.OpenStage(stageList[0].id);
		}
		return roWorldMap;
	}

	public Stage FindStage(string stageId)
	{
		return stageList.Find((Stage stage) => stage.id == stageId);
	}

	public Stage FindNextStage(string stageId)
	{
		int num = stageList.FindIndex((Stage row) => row.id == stageId);
		num++;
		if (num < 0 || num >= stageList.Count)
		{
			return null;
		}
		return stageList[num];
	}

	public Stage FindPreviousStage(string stageId)
	{
		int num = stageList.FindIndex((Stage row) => row.id == stageId);
		num--;
		if (num < 0 || num >= stageList.Count)
		{
			return null;
		}
		return stageList[num];
	}

	public void OpenStage(string stageId)
	{
		Stage stage = FindStage(stageId);
		stage.isOpen = true;
		lastOpenedStageId = stageId;
		if (stage.data.type == EStageTypeIdRange.PowerPlant)
		{
			stage = FindNextStage(stageId);
			if (stage != null)
			{
				OpenStage(stage.id);
			}
		}
	}

	public void StageClear(string stageId)
	{
		FindStage(stageId).clearCount++;
		Stage stage = FindNextStage(stageId);
		if (stage != null)
		{
			OpenStage(stage.id);
		}
	}

	public void RefreshByClearCount()
	{
		for (int i = 0; i < stageList.Count; i++)
		{
			Stage stage = null;
			Stage stage2 = null;
			if (i > 0)
			{
				stage = stageList[i - 1];
			}
			Stage stage3 = stageList[i];
			if (stage == null || stage.isCleared)
			{
				stage3.isOpen = true;
			}
			else
			{
				stage3.isOpen = false;
			}
			if (stage3.isOpen)
			{
				lastOpenedStageId = stage3.id;
			}
		}
	}
}
