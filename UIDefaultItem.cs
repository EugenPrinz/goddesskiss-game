using UnityEngine;

public class UIDefaultItem : UIItemBase
{
	public UISprite bg;

	public UISprite icon;

	public UISprite iconProgress;

	public UISprite main;

	public UISprite shadow;

	public UISprite spriteProgress;

	public UISprite branch;

	public UISprite branchIconBG;

	public GameObject newMark;

	public UILabel description;

	public UILabel time;

	public UITimer timer;

	public UILabel cost;

	public UILabel count;

	public UILabel nickname;

	public UILabel level;

	public UILabel levelOnly;

	public UILabel position;

	public GameObject positionRoot;

	public UISprite gradeBadge;

	public UIGrid gradeGrid;

	public UIProgressBar progress;

	public GameObject lockRoot;

	public GameObject selectedRoot;

	public GameObject researchRoot;

	public UIAtlas buildingAtlas;

	public UIAtlas commonAtlas;

	public void SetNickname(string nickname)
	{
		UISetter.SetLabel(this.nickname, nickname);
	}

	public void SetTime(string t)
	{
		UISetter.SetLabel(time, t);
	}

	public void SetTime(double t)
	{
		UISetter.SetLabel(time, Utility.GetTimeString(t));
	}

	public void SetTimer(double start, double end)
	{
		if (timer != null)
		{
			timer.Set(start, end);
		}
	}

	public void SetBuildingAtlas()
	{
		icon.atlas = buildingAtlas;
		main.atlas = buildingAtlas;
	}

	public void SetCommonAtlas()
	{
		icon.atlas = commonAtlas;
		main.atlas = commonAtlas;
	}

	public void SetTimer(double end)
	{
		if (timer != null)
		{
			timer.Set(end);
		}
	}

	public void SetTimerByDuration(double start, double duration)
	{
		if (timer != null)
		{
			timer.SetByDuration(start, duration);
		}
	}

	public void SetGradeBadge(string spriteName, bool useSnap = false)
	{
		UISetter.SetSprite(gradeBadge, spriteName, useSnap);
	}

	public void SetIcon(string spriteName, bool useSnap = false)
	{
		UISetter.SetSprite(icon, spriteName, useSnap);
		SetMainSprite(spriteName, useSnap);
	}

	public void SetMainSprite(string spriteName, bool useSnap = false)
	{
		UISetter.SetSprite(main, spriteName, useSnap);
		UISetter.SetSprite(shadow, spriteName, useSnap);
		UISetter.SetSprite(icon, spriteName, useSnap);
	}

	public void SetBG(string spriteName, bool useSnap = false)
	{
		UISetter.SetSprite(bg, spriteName, useSnap);
	}

	public void SetNewMark(bool active)
	{
		UISetter.SetActive(newMark, active);
	}

	public void SetLockMark(bool locked)
	{
		UISetter.SetActive(lockRoot, locked);
	}

	public void SetResearchMark(bool researching)
	{
		UISetter.SetActive(researchRoot, researching);
	}

	public void SetSelectedMark(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
	}

	public void SetBranch(EBranch branch)
	{
		UISetter.SetSprite(this.branch, "com_img_slot_" + branch.ToString().ToLower());
		UISetter.SetSprite(branchIconBG, "Branch-IconBG-" + branch);
	}

	public void SetLevel(int level)
	{
		UISetter.SetLabel(this.level, string.Format("Lv. " + level));
		UISetter.SetLabel(levelOnly, level);
	}

	public void SetCost(int cost)
	{
		UISetter.SetLabel(this.cost, cost);
	}

	public void SetCount(int count)
	{
		UISetter.SetLabel(this.count, count);
	}

	public void SetGrade(int grade)
	{
		if (gradeGrid == null || grade < 0)
		{
			return;
		}
		Transform transform = gradeGrid.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child == null)
			{
				break;
			}
			child.gameObject.SetActive(i < grade);
		}
		gradeGrid.enabled = true;
		gradeGrid.transform.localPosition = Vector3.zero;
		gradeGrid.Reposition();
	}
}
