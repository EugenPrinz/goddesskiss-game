using System;
using UnityEngine;

public class UIAgreement : MonoBehaviour
{
	public UILabel label1;

	public UILabel label2;

	public UILabel check1Label;

	public UIToggle toggle1;

	public UIToggle toggle2;

	[SerializeField]
	private GameObject pageRoot1;

	[SerializeField]
	private GameObject pageRoot2;

	[SerializeField]
	private UIScrollView listView1;

	[SerializeField]
	private UIScrollView listView2;

	[SerializeField]
	private UILabel pageLabel1;

	[SerializeField]
	private UILabel pageLabel2;

	private int maxPage1;

	private int maxPage2;

	private int page1;

	private int page2;

	private string[] text1;

	private string[] text2;

	private void Start()
	{
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Next1")
		{
			if (page1 < maxPage1)
			{
				page1++;
				label1.text = text1[page1 - 1];
				listView1.ResetPosition();
				UISetter.SetLabel(pageLabel1, page1 + "/" + maxPage1);
			}
		}
		else if (text == "Prev1" && page1 > 1)
		{
			page1--;
			label1.text = text1[page1 - 1];
			listView1.ResetPosition();
			UISetter.SetLabel(pageLabel1, page1 + "/" + maxPage1);
		}
		if (text == "Next2")
		{
			if (page2 < maxPage2)
			{
				page2++;
				label2.text = text2[page2 - 1];
				listView2.ResetPosition();
				UISetter.SetLabel(pageLabel2, page2 + "/" + maxPage2);
			}
		}
		else if (text == "Prev2" && page2 > 1)
		{
			page2--;
			label2.text = text2[page2 - 1];
			listView2.ResetPosition();
			UISetter.SetLabel(pageLabel2, page2 + "/" + maxPage2);
		}
	}

	private void OnEnable()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		if (toggle1 != null)
		{
			toggle1.value = false;
		}
		if (toggle2 != null)
		{
			toggle2.value = false;
		}
	}

	public void SetText(string t1, string t2)
	{
		page1 = 1;
		text1 = t1.Split('#');
		label1.text = text1[page1 - 1];
		maxPage1 = text1.Length;
		UISetter.SetLabel(pageLabel1, page1 + "/" + maxPage1);
		UISetter.SetActive(pageRoot1, maxPage1 > 1);
		page2 = 1;
		text2 = t2.Split('#');
		label2.text = text2[page2 - 1];
		maxPage2 = text2.Length;
		UISetter.SetLabel(pageLabel2, page2 + "/" + maxPage2);
		UISetter.SetActive(pageRoot2, maxPage2 > 1);
	}

	public void OnClose()
	{
		SoundManager.PlaySFX("BTN_Negative_001");
		M01_Title m01_Title = UnityEngine.Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.obj_Agreement.SetActive(value: false);
		m01_Title.obj_Login.SetActive(value: true);
	}

	public void OnValueChange()
	{
		if (toggle1.value)
		{
			UISetter.SetLabel(check1Label, Localization.Get("19060") + Localization.Format("955", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute));
		}
		else
		{
			UISetter.SetLabel(check1Label, Localization.Get("19060"));
		}
	}

	public void OnOK()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		if (toggle1.value && toggle2.value)
		{
			M01_Title m01_Title = UnityEngine.Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
			m01_Title.obj_Agreement.SetActive(value: false);
			m01_Title.obj_Login.SetActive(value: true);
			PlayerPrefs.SetString("UserTermVersion", RemoteObjectManager.instance.localUser.userTermVersion.ToString());
			RemoteObjectManager.instance.localUser.bShowUserTerm = false;
			AdjustManager.Instance.SimpleEvent("9fmyhk");
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("MemberID")))
			{
				UIPopup.Create<UISelectPlatformPopup>("SelectPlatformPopup").InitAndOpen();
			}
		}
		else
		{
			UISimplePopup.CreateOK(localization: true, Localization.Get("19028"), Localization.Get("19029"), null, Localization.Get("1001"));
		}
	}
}
