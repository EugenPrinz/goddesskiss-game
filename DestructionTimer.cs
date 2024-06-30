using UnityEngine;

public class DestructionTimer : MonoBehaviour
{
	[Tooltip("단위는 밀리세컨드, 1초 = 1000")]
	public int time;

	private void Start()
	{
		Object.Destroy(base.gameObject, (float)time * 0.001f);
	}
}
