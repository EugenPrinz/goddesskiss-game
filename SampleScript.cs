using UnityEngine;
using UnityEngine.UI;

public class SampleScript : MonoBehaviour
{
	[SerializeField]
	private Text text;

	public void RequestPermission()
	{
		if (UniAndroidPermission.IsPermitted(AndroidPermission.WRITE_EXTERNAL_STORAGE))
		{
			text.text = "WRITE_EXTERNAL_STORAGE is already permitted!!";
			return;
		}
		UniAndroidPermission.RequestPremission(AndroidPermission.WRITE_EXTERNAL_STORAGE, delegate
		{
			text.text = "WRITE_EXTERNAL_STORAGE is permitted NOW!!";
		}, delegate
		{
			text.text = "WRITE_EXTERNAL_STORAGE id NOT permitted...";
		});
	}
}
