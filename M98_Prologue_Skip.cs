using UnityEngine;

public class M98_Prologue_Skip : MonoBehaviour
{
	public void OnBtnSkip()
	{
		AdjustManager.Instance.SimpleEvent("g401wp");
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (localUser.tutorialStep == 0)
		{
			Loading.Load(Loading.Tutorial);
		}
		else
		{
			Loading.Load(Loading.WorldMap);
		}
	}
}
