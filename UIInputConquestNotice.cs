using UnityEngine;

public class UIInputConquestNotice : UISimplePopup
{
	public UIInput input;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		input.characterLimit = 80;
		OpenPopup();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "OK")
		{
			if (input.value != input.defaultText)
			{
				SetDefault(input.value);
				base.network.RequestSetConquestNotice(input.value);
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110376"));
			}
		}
		else if (text == "Cancel")
		{
			ClosePopup();
		}
	}

	public void SetDefault(string str)
	{
		if (input != null)
		{
			input.value = str;
			input.defaultText = str;
		}
	}

	public void SetLimitLength(int limit)
	{
		input.characterLimit = limit;
	}
}
