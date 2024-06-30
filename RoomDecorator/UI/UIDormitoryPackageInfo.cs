using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryPackageInfo : MonoBehaviour
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UILabel packageName;

		public UIListViewBase itemListView;

		private bool _isOpen;

		private bool _isClose = true;

		public void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.gameObject.SetActive(value: true);
				itemListView.ResetPosition();
				animBG.Reset();
				animBlock.Reset();
				animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			}
		}

		private void Close()
		{
			if (!_isClose)
			{
				_isClose = true;
				animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				StartCoroutine("WaitClose");
			}
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.2f);
			base.gameObject.SetActive(value: false);
			_isOpen = false;
		}

		public void Set(List<DormitoryThemeDataRow> items)
		{
			UISetter.SetLabel(packageName, Localization.Get(items[0].name));
			itemListView.ResizeItemList(items.Count);
			for (int i = 0; i < items.Count; i++)
			{
				UIDormitoryPackageListItem uIDormitoryPackageListItem = itemListView.itemList[i] as UIDormitoryPackageListItem;
				uIDormitoryPackageListItem.gameObject.name = itemListView.itemIdPrefix + items[i].id;
				uIDormitoryPackageListItem.Set(items[i]);
			}
		}

		public void OnClick(GameObject sender)
		{
			Close();
		}
	}
}
