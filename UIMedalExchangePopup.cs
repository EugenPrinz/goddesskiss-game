using System.Collections;
using UnityEngine;

public class UIMedalExchangePopup : UISimplePopup
{
	public UISprite commanderIcon;

	public UILabel commonMedal;

	public UILabel commanderMedal;

	public UILabel exchangeMedal;

	private int exchangeCount;

	private RoCommander commander;

	private bool isPress;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public void Set(RoCommander comm)
	{
		commander = comm;
		exchangeCount = 0;
		UISetter.SetSprite(commanderIcon, commander.resourceId + "_1");
		UISetter.SetLabel(subMessage, Localization.Format("8036", commander.nickname));
		setCount();
	}

	private void setCount()
	{
		UISetter.SetLabel(commonMedal, base.localUser.medal - exchangeCount);
		UISetter.SetLabel(commanderMedal, exchangeCount);
		UISetter.SetLabel(exchangeMedal, exchangeCount);
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "MaxBtn")
		{
			exchangeCount = base.localUser.medal;
			setCount();
		}
		else if (text == "OK" && exchangeCount > 0)
		{
			base.network.RequestExchangeMedal(exchangeCount, int.Parse(commander.id));
			Close();
		}
	}

	public void AddItemStart()
	{
		StartCoroutine(ItemCalculation(1));
	}

	public void AddItemEnd()
	{
		isPress = false;
		if (ItemCheck(1))
		{
			exchangeCount++;
			setCount();
		}
	}

	public void DecreaseItemStart()
	{
		StartCoroutine(ItemCalculation(-1));
	}

	public void DecreaseItemEnd()
	{
		isPress = false;
		if (ItemCheck(-1))
		{
			exchangeCount--;
			setCount();
		}
	}

	private IEnumerator ItemCalculation(int value)
	{
		float speed = 0.05f;
		isPress = true;
		yield return new WaitForSeconds(1f);
		while (ItemCheck(value) && isPress)
		{
			exchangeCount += value;
			setCount();
			yield return new WaitForSeconds(speed);
		}
		yield return true;
	}

	private bool ItemCheck(int value)
	{
		if (value > 0)
		{
			if (exchangeCount < base.localUser.medal)
			{
				return true;
			}
		}
		else if (exchangeCount > 1)
		{
			return true;
		}
		return false;
	}
}
