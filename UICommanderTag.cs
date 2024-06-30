using System.Collections;
using UnityEngine;

public class UICommanderTag : UIDefaultItem
{
	public UIDefaultListView subGrade;

	public GameObject fxBoard;

	public UIShake shake;

	private string _CurrentCommanderId = string.Empty;

	private const string _emptyCommanderRes = "c_000";

	public void InitUI()
	{
		SetIcon("c_000");
		UISetter.SetActive(fxBoard, active: false);
	}

	public void Shake()
	{
		if (shake != null)
		{
			shake.Begin();
		}
	}

	public void SetRank(int grade)
	{
		if (subGrade != null)
		{
			subGrade.InitGrade(grade);
		}
		SetGradeBadge("class_star_" + grade, useSnap: true);
	}

	public void EnableCommander(bool enable)
	{
		if (enable && !string.IsNullOrEmpty(_CurrentCommanderId))
		{
			UISetter.SetActive(fxBoard, active: true);
			SetState("Idle");
		}
		else
		{
			SetIcon("c_000");
		}
	}

	public void SetCommander(string commanderId)
	{
		_CurrentCommanderId = commanderId;
		if (!string.IsNullOrEmpty(commanderId))
		{
			UISetter.SetActive(icon.gameObject, active: true);
		}
	}

	public void Set(RoCommander commander)
	{
		SetCommander(commander.resourceId);
		SetRank(commander.rank);
	}

	public void Set(string commanderId, int grade)
	{
		SetCommander(commanderId);
		SetRank(grade);
	}

	public void SetState(string state)
	{
		switch (state)
		{
		case "Idle":
			_Idle();
			break;
		case "Behit":
			StopAllCoroutines();
			StartCoroutine(_PlayBeHit());
			break;
		case "Dead":
			_Dead();
			break;
		}
	}

	private void _Idle()
	{
		StopAllCoroutines();
		SetIcon(_CurrentCommanderId + "_t2");
		if (bg != null)
		{
			bg.enabled = false;
		}
	}

	private void _Dead()
	{
		StopAllCoroutines();
		SetIcon(_CurrentCommanderId + "_t3");
		if (bg != null)
		{
			bg.enabled = true;
		}
	}

	private IEnumerator _PlayBeHit()
	{
		SetIcon(_CurrentCommanderId + "_t3");
		if (bg != null)
		{
			bg.enabled = true;
		}
		yield return new WaitForSeconds(0.4f);
		SetIcon(_CurrentCommanderId + "_t2");
		if (bg != null)
		{
			bg.enabled = false;
		}
	}
}
