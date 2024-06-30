using System;
using System.Collections.Generic;
using RoomDecorator.Data;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryCharacter : UICommander
	{
		[Serializable]
		public class DormitoryData
		{
			public GameObject use;

			public UILabel floor;

			public GameObject lockObject;
		}

		public DormitoryData dormiotoryData;

		public void Set(RoCharacter data)
		{
			Set(data.commanderData);
			UISetter.SetActive(dormiotoryData.use, !string.IsNullOrEmpty(data.fno) && data.fno != "0");
			UISetter.SetLabel(dormiotoryData.floor, Localization.Format("81069", data.fno));
			UISetter.SetActive(dormiotoryData.lockObject, data.commanderData.state != ECommanderState.Nomal);
		}

		public void Set(Protocols.Dormitory.FloorCommanderInfo data)
		{
			Set(RoCommander.Create(data.id, data.level, data.grade, data.cls, data.costume, 0, 0, new List<int>()));
			UISetter.SetActive(dormiotoryData.use, active: false);
			UISetter.SetActive(dormiotoryData.floor, active: false);
		}
	}
}
