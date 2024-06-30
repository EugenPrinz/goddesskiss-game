using System.Collections.Generic;
using UnityEngine;

public class UIImageListPopup : UISimplePopup
{
	public GameObject profileListItem;

	public GameObject guildEmblemListItem;

	public GameObject btnCancel;

	public GameObject btnOK;

	public GameObject btnExtra;

	public int emblemIdx;

	private List<GameObject> guildEmblemList;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
		profileListItem.SetActive(value: false);
		guildEmblemListItem.SetActive(value: false);
	}

	public override void OnClick(GameObject sender)
	{
		if (btnExtra.activeSelf)
		{
			string text = sender.name;
			if (text.StartsWith("Emblem_"))
			{
				emblemIdx = int.Parse(text.Replace("Emblem_", string.Empty));
				SelectEmblem(emblemIdx);
			}
			else
			{
				base.OnClick(sender);
			}
		}
		else if (onClick != null)
		{
			base.OnClick(sender);
			if (_autoDestory && _waitRoutineCount <= 0)
			{
				Close();
			}
		}
		else
		{
			base.OnClick(sender);
		}
	}

	public void SetItemProfileImage()
	{
		SetButton();
		List<RoCommander> commanderList = base.localUser.commanderList;
		commanderList.ForEach(delegate(RoCommander row)
		{
			if (row.state == ECommanderState.Nomal)
			{
				GameObject gameObject = Object.Instantiate(profileListItem);
				gameObject.transform.SetParent(profileListItem.transform.parent);
				gameObject.name = $"thumbnail_{row.id}";
				gameObject.transform.localScale = Vector3.one;
				UISetter.SetSprite(gameObject.transform.Find("Thumnail").GetComponent<UISprite>(), row.getCurrentCostumeThumbnailName());
			}
		});
	}

	public void SetItemGuildEmblem(int selectIdx = -1)
	{
		if (selectIdx > -1)
		{
			guildEmblemList = new List<GameObject>();
		}
		SetButton();
		int num = int.Parse(base.regulation.defineDtbl["GUILD_EMBLEM_COUNT"].value);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(guildEmblemListItem);
			gameObject.transform.SetParent(guildEmblemListItem.transform.parent);
			gameObject.name = "Emblem_" + i;
			gameObject.transform.localScale = Vector3.one;
			UISprite component = gameObject.transform.Find("Thumnail").GetComponent<UISprite>();
			component.spriteName = "union_mark_" + $"{i:00}";
			component.MakePixelPerfect();
			if (selectIdx > -1)
			{
				if (i == selectIdx)
				{
					gameObject.transform.Find("Select").GetComponent<UISprite>().gameObject.SetActive(value: true);
					emblemIdx = selectIdx;
				}
				guildEmblemList.Add(gameObject);
			}
		}
	}

	private void SelectEmblem(int idx)
	{
		if (guildEmblemList == null)
		{
			return;
		}
		for (int i = 0; i < guildEmblemList.Count; i++)
		{
			if (idx == i)
			{
				guildEmblemList[i].transform.Find("Select").GetComponent<UISprite>().gameObject.SetActive(value: true);
			}
			else
			{
				guildEmblemList[i].transform.Find("Select").GetComponent<UISprite>().gameObject.SetActive(value: false);
			}
		}
	}

	private void SetButton()
	{
		UISetter.SetActive(btnOK, !string.IsNullOrEmpty(okName.text));
		UISetter.SetActive(btnExtra, !string.IsNullOrEmpty(extraName.text));
		UISetter.SetActive(btnCancel, !string.IsNullOrEmpty(cancelName.text));
	}
}
