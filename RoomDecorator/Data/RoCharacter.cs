namespace RoomDecorator.Data
{
	public class RoCharacter
	{
		public string id;

		public string fno;

		public RoCommander commanderData;

		public RoDormitory.Item head;

		public RoDormitory.Item body;

		public TimeData remain;

		public static RoCharacter Create(string id)
		{
			RoCharacter roCharacter = new RoCharacter();
			roCharacter.id = id;
			roCharacter.remain = new TimeData();
			return roCharacter;
		}

		public static RoCharacter Create(RoCommander data)
		{
			RoCharacter roCharacter = Create(data.id);
			roCharacter.commanderData = data;
			return roCharacter;
		}

		public void Set(Protocols.Dormitory.FloorCharacterInfo data)
		{
			fno = data.fno;
			head = new RoDormitory.Item(EDormitoryItemType.CostumeHead, data.headId);
			body = new RoDormitory.Item(EDormitoryItemType.CostumeBody, data.bodyId);
			remain.SetByDuration(data.remain);
		}

		public RoCharacter Clone()
		{
			RoCharacter roCharacter = new RoCharacter();
			roCharacter.id = id;
			roCharacter.fno = fno;
			roCharacter.commanderData = commanderData;
			roCharacter.head = head.Clone();
			roCharacter.body = body.Clone();
			roCharacter.remain = remain;
			return roCharacter;
		}
	}
}
