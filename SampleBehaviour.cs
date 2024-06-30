using System.Collections;
using System.IO;
using UnityEngine;

public class SampleBehaviour : MonoBehaviour
{
	public void OnClickGlinkButton()
	{
		GLink.sharedInstance().setWidgetStartPosition(isLeft: false, 60);
		GLink.sharedInstance().executeHome();
	}

	public void OnClickScreenShotButton()
	{
		StartCoroutine(SaveScreenShot());
	}

	private IEnumerator SaveScreenShot()
	{
		yield return new WaitForEndOfFrame();
		string filePath = Application.persistentDataPath + "/GLShareImage.png";
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipmap: false);
		texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
		yield return 0;
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(filePath, bytes);
		Object.DestroyObject(texture);
		GLink.sharedInstance().executeArticlePostWithImage(filePath);
	}
}
