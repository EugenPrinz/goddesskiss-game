using System.Collections;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIBuyItem : UIPopup
	{
		public GEAnimNGUI AnimBG;

		public GEAnimNGUI AnimBlock;

		public UISprite costIcon;

		public UILabel costValue;

		public UILabel okName;

		public UILabel cancelName;

		private bool _isOpen;

		private bool _isClose = true;

		private void Start()
		{
			SetAutoDestroy(autoDestory: true);
			Open();
		}

		public override void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.Open();
				AnimBG.Reset();
				AnimBlock.Reset();
				AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			}
		}

		public override void Close()
		{
			if (!_isClose)
			{
				_isClose = true;
				AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				StartCoroutine("WaitClose");
			}
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			base.Close();
			_isOpen = false;
		}

		public void Set(string title, string message, string costSpriteName, string costValue, string okName, string cancelName)
		{
			UISetter.SetLabel(base.title, title);
			UISetter.SetLabel(base.message, message);
			UISetter.SetSprite(costIcon, costSpriteName);
			UISetter.SetLabel(this.costValue, costValue);
			UISetter.SetLabel(this.okName, okName);
			UISetter.SetLabel(this.cancelName, cancelName);
		}

		public override void OnClick(GameObject sender)
		{
			if (!_isClose)
			{
				base.OnClick(sender);
			}
		}
	}
}
