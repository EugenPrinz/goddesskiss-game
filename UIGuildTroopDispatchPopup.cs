using System.Collections;
using UnityEngine;

public class UIGuildTroopDispatchPopup : UIPopup
{
	public CommanderDispatchItem OccupationItem;

	public CommanderDispatchItem ScrambleItem;

	[SerializeField]
	private CommanderDispatchItem FirstItem;

	[SerializeField]
	private CommanderDispatchItem SecondsItem;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	[HideInInspector]
	public UISelectDispatchPopup selectPopup;

	[SerializeField]
	private GameObject contentsRoot;

	[SerializeField]
	private GameObject selectPopupPrefab;

	[SerializeField]
	private UILabel dispatchCount;

	private const int DispatchMaxCount = 2;

	public void Init()
	{
		SetAutoDestroy(autoDestory: true);
		base.network.RequestGetDispatchCommanderList();
		OpenPopup();
	}

	public void Set()
	{
	}

	public void SetDispatchList()
	{
		FirstItem.CloseSlot();
		SecondsItem.CloseSlot();
		if (base.localUser.slotDispatchInfo == null)
		{
			return;
		}
		for (int i = 0; i < base.localUser.slotDispatchInfo.Count; i++)
		{
			if (base.localUser.slotDispatchInfo[i].SlotNum == "1")
			{
				FirstItem.SetItem(base.localUser.slotDispatchInfo[i].dispatchCommanderInfo);
			}
			else
			{
				SecondsItem.SetItem(base.localUser.slotDispatchInfo[i].dispatchCommanderInfo);
			}
		}
		dispatchCount.text = $"({base.localUser.slotDispatchInfo.Count} / {2})";
	}

	public UISelectDispatchPopup GetSelectDispatchPopup()
	{
		if (selectPopup == null)
		{
			GameObject gameObject = Object.Instantiate(selectPopupPrefab);
			selectPopup = gameObject.GetComponent<UISelectDispatchPopup>();
			selectPopup.transform.parent = contentsRoot.transform;
			selectPopup.transform.localScale = new Vector3(1f, 1f, 1f);
			selectPopup.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		return selectPopup;
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
