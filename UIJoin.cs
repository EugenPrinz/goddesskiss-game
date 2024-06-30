using System.Text.RegularExpressions;
using UnityEngine;

public class UIJoin : MonoBehaviour
{
	public UIInput ID;

	public UIInput PW;

	public UIInput PW_Confirm;

	private void Start()
	{
	}

	public void OnClose()
	{
		SoundManager.PlaySFX("BTN_Negative_001");
		M01_Title m01_Title = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.obj_Join.SetActive(value: false);
		m01_Title.obj_Login.SetActive(value: true);
	}

	public void OnOK()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		if (ID.value.Length < 6 || ID.value.Length > 16 || string.IsNullOrEmpty(ID.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19051"));
		}
		else if (PW.value.Length < 6 || PW.value.Length > 16 || string.IsNullOrEmpty(PW.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19056"));
		}
		else if (!PossibleChar(ID.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
		}
		else if (!PossibleChar(PW.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19057"));
		}
		else if (PW.value != PW_Confirm.value)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19034"));
		}
		else if (RemoteObjectManager.instance.bChangeFullMember)
		{
			RemoteObjectManager.instance.RequestChangeMembership(ID.value, PW.value, PlayerPrefs.GetString("GuestID"));
		}
		else if (RemoteObjectManager.instance.localUser.loginType == 0)
		{
			RemoteObjectManager.instance.RequestSignUp(ID.value, PW.value);
		}
		else if (RemoteObjectManager.instance.localUser.loginType == 1)
		{
			RemoteObjectManager.instance.RequestChangeDeviceDbros(Platform.Dbros, ID.value, PW.value);
		}
	}

	private bool PossibleChar(string str)
	{
		string value = Regex.Replace(str, "[^a-zA-Z0-9]", string.Empty, RegexOptions.Singleline);
		return str.Equals(value);
	}
}
