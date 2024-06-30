using System;
using System.Collections;
using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIInventorySelectObject : MonoBehaviour
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UILabel itemName;

		public UISprite thumbnail;

		public UILabel tileSize;

		public Action<GameObject> onClick;

		private bool _isOpen;

		private bool _isClose = true;

		public void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.gameObject.SetActive(value: true);
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

		public void Set(RoDormitory.Item item)
		{
			switch (item.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
			{
				DormitoryDecorationDataRow dormitoryDecorationDataRow = (DormitoryDecorationDataRow)item.data;
				itemName.text = Localization.Get(dormitoryDecorationDataRow.name);
				thumbnail.SetAtlasImage("DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
				tileSize.text = $"{dormitoryDecorationDataRow.xSize}x{dormitoryDecorationDataRow.ySize}";
				tileSize.gameObject.SetActive(value: true);
				break;
			}
			case EDormitoryItemType.Wallpaper:
			{
				DormitoryWallpaperDataRow dormitoryWallpaperDataRow = (DormitoryWallpaperDataRow)item.data;
				itemName.text = Localization.Get(dormitoryWallpaperDataRow.name);
				thumbnail.SetAtlasImage("DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
				tileSize.gameObject.SetActive(value: false);
				break;
			}
			}
		}

		public void OnClick(GameObject sender)
		{
			SoundManager.PlaySFX("BTN_Positive_001");
			if (onClick != null)
			{
				onClick(sender);
			}
			Close();
		}
	}
}
