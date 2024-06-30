using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoScramble
{
	[JsonObject]
	public class Stage
	{
		public string id { get; private set; }

		public bool isOpen { get; set; }

		public bool clear { get; set; }

		public int hp { get; set; }

		public bool isCleared => clear;

		public string commanderId { get; set; }

		public GuildStruggleDataRow data => _reg.guildStruggleDtbl[id];

		public static Stage Create(string id)
		{
			Stage stage = new Stage();
			stage.id = id;
			stage.isOpen = false;
			stage.clear = false;
			return stage;
		}

		public RoUser GetEnemy()
		{
			return null;
		}
	}

	public string id { get; private set; }

	public List<Stage> stageList { get; private set; }

	public string lastOpenedStageId { get; set; }

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

	private static Regulation _reg => RemoteObjectManager.instance.regulation;

	public List<GuildStruggleDataRow> stageDataRowList => _reg.guildStruggleDtbl.FindAll((GuildStruggleDataRow row) => true);

	public bool isComplete => false;

	public string name => string.Empty;

	public static RoScramble Create()
	{
		RoScramble roScramble = new RoScramble();
		List<Stage> stageList = new List<Stage>();
		List<GuildStruggleDataRow> list = roScramble.stageDataRowList;
		list.ForEach(delegate(GuildStruggleDataRow row)
		{
			stageList.Add(Stage.Create(row.idx));
		});
		roScramble.stageList = stageList;
		if (stageList.Count > 0)
		{
			roScramble.OpenStage(stageList[0].id);
		}
		return roScramble;
	}

	public Stage FindStage(string stageId)
	{
		return stageList.Find((Stage stage) => stage.id == stageId);
	}

	public void OpenStage(string stageId)
	{
	}

	public void StageClear(string stageId)
	{
	}
}
