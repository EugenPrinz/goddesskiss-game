using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TimedGameObject))]
public class UnitPositionPingPong : MonoBehaviour
{
	public bool stop;

	private TimedGameObject _tgo;

	private void OnEnable()
	{
		_tgo = GetComponent<TimedGameObject>();
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		Transform t = base.transform;
		Vector3 origin = t.position;
		float duration = Random.Range(0.8f, 1.2f);
		float durationOffset = duration * -0.5f;
		float speed = Random.Range(0.1f, 0.2f);
		float time = Random.value * durationOffset + durationOffset;
		while (base.enabled)
		{
			if (stop)
			{
				yield return null;
				continue;
			}
			time += _tgo.TimedDeltaTime();
			Vector3 pos = origin;
			pos.z += speed * (Mathf.PingPong(time, duration) + durationOffset);
			t.position = pos;
			yield return null;
		}
	}
}
