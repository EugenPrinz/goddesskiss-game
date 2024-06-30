using UnityEngine;

public class UIReceiveUserString : UISimplePopup
{
	public UILabel inputLabel;

	public UIInput input;

	public GameObject xButton;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		if (Application.loadedLevelName == Loading.Title)
		{
			SetAutoDestroy(autoDestory: true);
			UISetter.SetActive(xButton, active: true);
		}
		else
		{
			SetAutoDestroy(autoDestory: true);
			UISetter.SetActive(xButton, active: true);
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = base.gameObject.name;
		if (text == "InputUserString")
		{
			if (input.value.Length <= 0)
			{
				input.value = input.defaultText;
				return;
			}
		}
		else if (text == "Cancel")
		{
			ClosePopup();
		}
		base.OnClick(sender);
	}

	public void SetDefault(string str)
	{
		UISetter.SetLabel(inputLabel, str);
		if (input != null)
		{
			input.defaultText = str;
			input.value = str;
		}
	}

	public void SetLimitLength(int limit)
	{
		input.characterLimit = limit;
	}
}
