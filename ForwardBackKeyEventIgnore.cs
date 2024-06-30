using UnityEngine;

public class ForwardBackKeyEventIgnore : MonoBehaviour
{
	private void OnEnable()
	{
		ForwardBackKeyEvent.Lock();
	}

	private void OnDisable()
	{
		ForwardBackKeyEvent.Unlock();
	}
}
