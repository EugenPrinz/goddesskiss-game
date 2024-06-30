using System.Collections;
using UnityEngine;

public class SendMessageTimer : MonoBehaviour
{
	public float delay;

	public GameObject target;

	public string eventName;

	public GameObject param;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(delay);
		if (target == null)
		{
			BroadcastMessage(eventName, param, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			target.SendMessage(eventName, param, SendMessageOptions.DontRequireReceiver);
		}
		base.enabled = false;
	}
}
