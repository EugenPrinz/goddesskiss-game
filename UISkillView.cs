using System.Collections.Generic;
using Shared.Battle;
using UnityEngine;

public class UISkillView : Manager<UISkillView>
{
	public delegate void SelectDelegate(int index);

	protected bool bTurn;

	protected bool bInputFlag;

	protected Unit activeUnit;

	protected int selectedSkillIdx;

	public GameObject content;

	public List<BtnSkillIcon> btnSkillIcons;

	public event SelectDelegate _Click;

	protected override void Init()
	{
		base.Init();
		activeUnit = null;
		bInputFlag = false;
		selectedSkillIdx = -1;
		for (int i = 0; i < btnSkillIcons.Count; i++)
		{
			btnSkillIcons[i].Initilize(i, this);
			btnSkillIcons[i].gameObject.SetActive(value: false);
		}
	}

	public void Set(Unit unit)
	{
		if (activeUnit == unit)
		{
			if (!bTurn)
			{
				bTurn = true;
				selectedSkillIdx = -1;
				UpdateUiStatus();
			}
		}
		else
		{
			bTurn = true;
			activeUnit = unit;
			selectedSkillIdx = -1;
			UpdateUiStatus();
		}
	}

	public void SetInputFlag(bool bFlag)
	{
		bInputFlag = bFlag;
		if (!bInputFlag)
		{
			bTurn = false;
		}
	}

	public bool CanTouch()
	{
		if (!bInputFlag)
		{
			return false;
		}
		return true;
	}

	public int GetSelectedSkillIdx()
	{
		return selectedSkillIdx;
	}

	public void OnSelectSkill(int idx)
	{
		if (selectedSkillIdx == idx)
		{
			return;
		}
		for (int i = 0; i < btnSkillIcons.Count; i++)
		{
			if (i != idx)
			{
				btnSkillIcons[i].SetEnable(bEnable: false);
			}
		}
		selectedSkillIdx = idx;
		this._Click(selectedSkillIdx);
	}

	protected void UpdateUiStatus()
	{
		for (int i = 0; i < btnSkillIcons.Count; i++)
		{
			if (activeUnit == null || i >= activeUnit.skills.Count || activeUnit.skills[i] == null)
			{
				btnSkillIcons[i].gameObject.SetActive(value: false);
				continue;
			}
			btnSkillIcons[i].gameObject.SetActive(value: true);
			btnSkillIcons[i].Set(activeUnit.skills[i]);
		}
	}
}
