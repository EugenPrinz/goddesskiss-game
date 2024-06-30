using System.Collections;
using UnityEngine;

public class UISelectResearchPopup : UIPopup
{
	public SelectResearchListItem weapon;

	public SelectResearchListItem equip;

	private bool _open;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	public void OpenPopup()
	{
		if (!_open)
		{
			_open = true;
			Open();
		}
	}

	public void ClosePopup()
	{
		if (_open)
		{
			_open = false;
			StartCoroutine(OnEventHidePopup());
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return null;
		Close();
	}

	public override void OnClick(GameObject sender)
	{
		if (_open)
		{
			string text = sender.name;
			if (text == "EquipItem")
			{
				base.uiWorld.laboratory.InitAndOpen();
			}
			else if (text == "WeaponItem")
			{
				base.network.RequestGetWeaponProgressList();
			}
			ClosePopup();
		}
	}
}
