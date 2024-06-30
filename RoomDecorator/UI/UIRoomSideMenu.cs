using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoomSideMenu : MonoBehaviour
	{
		public GEAnimNGUI items;

		private bool _enableMenuItem;

		public void EnableMenuItem(bool enable)
		{
			_enableMenuItem = enable;
			if (_enableMenuItem)
			{
				items.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			}
			else
			{
				items.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			}
		}

		public void Init()
		{
			EnableMenuItem(enable: false);
		}

		public void OnClick(GameObject sender)
		{
			SoundManager.PlaySFX("BTN_Positive_001");
			switch (sender.name)
			{
			case "Menu":
				EnableMenuItem(!_enableMenuItem);
				break;
			case "Edit":
				SingletonMonoBehaviour<DormitoryData>.Instance.isEditMode = true;
				break;
			case "Character":
				RemoteObjectManager.instance.RequestGetDormitoryCommanderInfo();
				break;
			case "Shop":
				RemoteObjectManager.instance.RequestGetDormitoryShopProductList();
				break;
			}
		}
	}
}
