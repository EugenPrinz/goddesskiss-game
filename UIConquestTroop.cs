using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIConquestTroop : MonoBehaviour
{
	public UILabel troopNo;

	public GameObject root;

	public UITimer timer;

	[HideInInspector]
	public int number;

	private List<int> indexList;

	private int step;

	private int maxStep;

	private bool isCash;

	private float percent;

	public void Set(int number, List<int> indexList, int remain, int tRemain, int ucash)
	{
		this.number = number;
		maxStep = indexList.Count - 1;
		isCash = ucash == 1;
		percent = 1f;
		if (isCash)
		{
			percent = float.Parse(RemoteObjectManager.instance.regulation.defineDtbl["GUILD_OCCUPY_QUICKMOVE"].value) * 0.01f;
		}
		this.indexList = indexList;
		UISetter.SetGameObjectName(base.gameObject, $"ConquestTroop-{number}");
		UISetter.SetLabel(troopNo, number);
		TimeData timeData = TimeData.Create();
		timeData.SetByDuration(remain);
		timer.Set(timeData);
		timer.RegisterOnFinished(delegate
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			localUser.conquestDeck[number].remain = 0;
			localUser.conquestDeck[number].remainData.SetInvalidValue();
			localUser.conquestDeck[number].status = "S";
			DeleteTroop();
			UIManager.instance.RefreshOpenedUI();
		});
		StartMove(SetStartStep(indexList, remain, tRemain));
	}

	private float SetStartStep(List<int> indexList, int remain, int tRemain)
	{
		float num = (float)tRemain - (float)remain;
		float num2 = 0f;
		float pRemain = 0f;
		int startIndex = 0;
		step = 0;
		for (int i = 0; i < indexList.Count; i++)
		{
			GuildOccupyDataRow guildOccupyDataRow = RemoteObjectManager.instance.regulation.guildOccupyDtbl[indexList[i].ToString()];
			if (i + 1 >= indexList.Count)
			{
				break;
			}
			for (int j = 0; j < guildOccupyDataRow.next.Count; j++)
			{
				int num3 = guildOccupyDataRow.next[j];
				if (num3 == indexList[i + 1])
				{
					num2 += (float)guildOccupyDataRow.nexttime[j] * percent;
					pRemain = (float)guildOccupyDataRow.nexttime[j] * percent;
					startIndex = i;
				}
			}
			if (num >= num2)
			{
				step++;
				continue;
			}
			break;
		}
		base.gameObject.transform.localPosition = SetStartPosition(startIndex, pRemain, num2 - num);
		return num2 - num;
	}

	private Vector3 SetStartPosition(int startIndex, float pRemain, float pnRemain)
	{
		GuildOccupyDataRow guildOccupyDataRow = RemoteObjectManager.instance.regulation.guildOccupyDtbl[indexList[startIndex].ToString()];
		GuildOccupyDataRow guildOccupyDataRow2 = RemoteObjectManager.instance.regulation.guildOccupyDtbl[indexList[startIndex + 1].ToString()];
		Vector3 vector = new Vector3(guildOccupyDataRow.positionx, guildOccupyDataRow.positiony, 0f);
		Vector3 vector2 = new Vector3(guildOccupyDataRow2.positionx, guildOccupyDataRow2.positiony, 0f);
		float num = ((pRemain != 0f) ? ((pRemain - pnRemain) / pRemain) : 0f);
		vector += (vector2 - vector) * num;
		vector.y *= -1f;
		vector.z = 0f;
		return vector;
	}

	private void SetMoveTween(int posX, int posY, int time)
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("x", posX, "y", (float)posY * -1f, "islocal", true, "time", time, "easeType", iTween.EaseType.linear, "oncomplete", "StartMove", "oncompleteparams", 0f, "oncompletetarget", base.gameObject));
	}

	private void SetMoveTween(Vector3[] positionList, int time)
	{
		if (positionList.Length > 1)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("path", positionList, "islocal", true, "time", time, "easeType", iTween.EaseType.linear, "oncomplete", "EndMove", "oncompletetarget", base.gameObject));
		}
	}

	private void EndMove()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	private void StartMove(float startRemain = 0f)
	{
		if (step == maxStep)
		{
			UISetter.SetActive(root, active: false);
			return;
		}
		float num = 0f;
		num = ((startRemain != 0f) ? startRemain : GetTime());
		GuildOccupyDataRow guildOccupyDataRow = RemoteObjectManager.instance.regulation.guildOccupyDtbl[indexList[step + 1].ToString()];
		SetMoveTween(guildOccupyDataRow.positionx, guildOccupyDataRow.positiony, (int)num);
		step++;
	}

	private float GetTime()
	{
		GuildOccupyDataRow guildOccupyDataRow = RemoteObjectManager.instance.regulation.guildOccupyDtbl[indexList[step].ToString()];
		for (int i = 0; i < guildOccupyDataRow.next.Count; i++)
		{
			if (guildOccupyDataRow.next[i] == indexList[step + 1])
			{
				return (float)guildOccupyDataRow.nexttime[i] * percent;
			}
		}
		return 0f;
	}

	public void DeleteTroop()
	{
		UIManager.instance.world.conquestMap.troopList.Remove(this);
		Object.DestroyImmediate(base.gameObject);
	}
}
