using System.Collections.Generic;
using UnityEngine;

public class UIScenarioLog : UIPopup
{
	private bool isAnimation;

	[SerializeField]
	private GameObject Block;

	[SerializeField]
	private UIScrollWrap scrollWrap;

	[SerializeField]
	private ClassicRpgManager rpgMgr;

	private int wrapOffset = 6;

	[SerializeField]
	private UIDefaultListView listView;

	private void Start()
	{
		isAnimation = false;
	}

	public void InitAndOpen()
	{
		OpenLog();
		InitLogList();
	}

	public void OnInitializeItem(GameObject go, int wrapIndex, int realIndex)
	{
		ScenarioLogItem componentInChildren = go.GetComponentInChildren<ScenarioLogItem>();
		List<ScenarioLogInfo> logInfoList = rpgMgr.GetLogInfoList();
		int num = realIndex + wrapOffset;
		if (num < 0 || num >= logInfoList.Count)
		{
			componentInChildren.SetActive(isActive: false);
			return;
		}
		componentInChildren.Set(logInfoList[logInfoList.Count - 1 - num]);
		componentInChildren.SetActive(isActive: true);
	}

	public void InitLogList()
	{
		List<ScenarioLogInfo> logInfoList = rpgMgr.GetLogInfoList();
		listView.InitScenarioLogList(logInfoList);
		listView.ResetPosition();
		listView.transform.localPosition = new Vector3(0f, 185f, 0f);
		listView.GetComponent<UIPanel>().clipOffset = new Vector2(-25f, -185f);
		if ((float)(logInfoList.Count * 160) > listView.GetComponent<UIPanel>().GetViewSize().y)
		{
			listView.scrollView.SetDragAmount(0f, 1f, updateScrollbars: true);
			float num = 150 + 160 * (logInfoList.Count - 3);
			listView.transform.localPosition = new Vector3(0f, num, 0f);
			listView.GetComponent<UIPanel>().clipOffset = new Vector2(-25f, 0f - num);
		}
	}

	private void OpenAnimation()
	{
		iTween.MoveTo(root, iTween.Hash("x", -1278, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear, "oncomplete", "OpenAnimaionEnd", "oncompletetarget", base.gameObject));
	}

	public void CloseAnimation()
	{
		isAnimation = true;
		iTween.MoveTo(root, iTween.Hash("x", -358, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear, "oncomplete", "CloseAnimaionEnd", "oncompletetarget", base.gameObject));
		CloseAnimaionEnd();
	}

	private void OpenLog()
	{
		UISetter.SetActive(root, active: true);
	}

	public void CloseLog()
	{
		UISetter.SetActive(root, active: false);
	}

	public void OpenAnimaionEnd()
	{
		isAnimation = false;
	}

	public void CloseAnimaionEnd()
	{
		isAnimation = false;
	}
}
