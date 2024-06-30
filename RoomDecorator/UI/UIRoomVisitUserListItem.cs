using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoomVisitUserListItem : UIItemBase
	{
		public UISprite thumbnail;

		public UILabel userName;

		public UILabel level;

		public UILabel lastTime;

		public UILabel world;

		public GameObject visitBtn;

		public GameObject deleteBtn;

		public void Set(Protocols.Dormitory.SearchUserInfo data, bool removeMode = false)
		{
			UISetter.SetSprite(thumbnail, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(data.thumbnail)));
			UISetter.SetLabel(userName, data.name);
			UISetter.SetLabel(level, data.level);
			UISetter.SetLabel(lastTime, Utility.GetTimeString(data.lastTime));
			UISetter.SetLabel(world, Localization.Format("81101", data.world));
			UISetter.SetActive(visitBtn, !removeMode);
			UISetter.SetActive(deleteBtn, removeMode);
		}
	}
}
