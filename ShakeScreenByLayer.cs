using System.Collections;
using UnityEngine;

public class ShakeScreenByLayer : MonoBehaviour
{
	public float startDelay;

	public float duration = 0.2f;

	private IEnumerator Start()
	{
		if (startDelay > 0f)
		{
			yield return new WaitForSeconds(startDelay);
		}
		SplitScreenManager ssm = SplitScreenManager.instance;
		if (!(ssm == null))
		{
			if (base.gameObject.layer == LayerMask.NameToLayer("Left"))
			{
				ssm.ShakeScreen(SplitScreenDrawSide.Left, duration);
			}
			else if (base.gameObject.layer == LayerMask.NameToLayer("Right"))
			{
				ssm.ShakeScreen(SplitScreenDrawSide.Right, duration);
			}
			base.enabled = false;
		}
	}
}
