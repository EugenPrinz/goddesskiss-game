using UnityEngine;

public class UIInputNickName : MonoBehaviour
{
	public int curStep;

	public int nextStep;

	public UILabel nickName;

	protected string requestNickName;

	public void OnClick(GameObject item)
	{
		requestNickName = nickName.text;
		if (requestNickName.Length < 2 || requestNickName.Length > 10 || string.IsNullOrEmpty(requestNickName))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7143"));
		}
		else if (!Utility.PossibleChar(requestNickName))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
		}
		else
		{
			RemoteObjectManager.instance.RequestSetNickNameFromTutorial(nickName.text, nextStep);
		}
	}

	private void Update()
	{
		if (RemoteObjectManager.instance.localUser.tutorialStep == nextStep)
		{
			RemoteObjectManager.instance.localUser.nickname = requestNickName;
			UIManager.instance.RefreshOpenedUI();
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
