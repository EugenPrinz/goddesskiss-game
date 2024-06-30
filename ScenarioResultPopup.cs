using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioResultPopup : UIPopup
{
	public UIDefaultListView listView;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public static readonly string itemIdPrefix = "GetItem-";

	[SerializeField]
	private UILabel resultTitle;

	[SerializeField]
	private UISpineAnimation charSpine;

	[SerializeField]
	private GameObject WinObj;

	[SerializeField]
	private GameObject FailObj;

	[SerializeField]
	private UILabel failTxt;

	[SerializeField]
	private UICommander commander;

	[SerializeField]
	private Animation resultAnim;

	private string strKey;

	private string aniKey;

	public void Init(List<Protocols.RewardInfo.RewardData> rewardList, bool isAgainClear = false)
	{
		if (listView != null)
		{
			if (rewardList != null)
			{
				listView.InitRewardList(rewardList, itemIdPrefix);
			}
			listView.scrollView.ResetPosition();
			if (UIManager.instance.battle != null)
			{
				BattleData battleData = BattleData.Get();
				battleData.move = EBattleResultMove.MyTown;
				BattleData.Set(battleData);
				SetPopup(battleData.isWin, isAgainClear);
			}
			else
			{
				SetPopup(isWin: true, isAgainClear);
			}
			SetAutoDestroy(autoDestory: true);
		}
	}

	public void SetPopup(bool isWin, bool isAgainClear)
	{
		int commanderId = base.localUser.currScenario.commanderId;
		UISetter.SetActive(WinObj, isWin);
		UISetter.SetActive(FailObj, !isWin);
		if (isWin)
		{
			strKey = Localization.Get("20022");
			aniKey = "e_04_pleasure";
		}
		else
		{
			strKey = Localization.Get("20024");
			UISetter.SetLabel(failTxt, Localization.Get("20025"));
			aniKey = "a_05_lose";
		}
		if (isAgainClear)
		{
			UISetter.SetActive(WinObj, !isAgainClear);
			UISetter.SetActive(FailObj, isAgainClear);
			UISetter.SetLabel(failTxt, Localization.Get("20065"));
		}
		UISetter.SetLabel(resultTitle, strKey);
		RoCommander roCommander = base.localUser.FindCommander(commanderId.ToString());
		if (roCommander != null)
		{
			StartCoroutine(SetCommander(roCommander));
		}
	}

	private IEnumerator SetCommander(RoCommander commanderData)
	{
		yield return StartCoroutine(commander.SetCommander(commanderData));
		if (commander.spine != null)
		{
			UISetter.SetActive(commander.spine, active: false);
			if (commander.spine.target != null)
			{
				commander.spine.SetAnimation(aniKey);
			}
			UISetter.SetActive(commander.spine, active: true);
		}
		OpenPopupShow();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "BG" && !bBackKeyEnable)
		{
			Close();
			if (UIManager.instance.battle != null)
			{
				Loading.Load(Loading.WorldMap);
			}
		}
	}

	public void OpenPopupShow()
	{
		base.Open();
		StartCoroutine(ResultAnimation());
	}

	public override void Open()
	{
		base.Open();
	}

	public override void Close()
	{
		RemoteObjectManager.instance.waitingScenarioComplete = false;
		bBackKeyEnable = true;
		HidePopup();
	}

	private void HidePopup()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		StartCoroutine(OnEventHidePopup());
	}

	private IEnumerator ResultAnimation()
	{
		resultAnim.Play("Story_Result");
		while (!resultAnim.isPlaying)
		{
			yield return null;
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}
}
