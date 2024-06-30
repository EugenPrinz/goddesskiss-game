using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CutInEffect : MonoBehaviour
{
	public enum Side
	{
		Left,
		Right
	}

	internal Side _side;

	internal UnitRenderer _unitRenderer;

	[Range(-1f, 10000f)]
	public int eventDelay = -1;

	[Range(-1f, 10000f)]
	public int cutInDuration = -1;

	[HideInInspector]
	public bool isDefault = true;

	public Side side => _side;

	public UnitRenderer unitRenderer => _unitRenderer;

	public float duration { get; private set; }

	public float elapsedTime { get; private set; }

	public bool isEnded
	{
		get
		{
			if (cutInDuration >= 0)
			{
				return elapsedTime >= (float)cutInDuration * 0.001f;
			}
			return elapsedTime >= duration;
		}
	}

	public void RefreshDuration()
	{
		Animation component = GetComponent<Animation>();
		if (component == null)
		{
			duration = 0f;
		}
		float b = 0f;
		foreach (AnimationState item in component)
		{
			if (component.IsPlaying(item.name))
			{
				float num = Mathf.Max(0f, item.length - item.time);
				num /= item.speed;
				b = Mathf.Max(num, b);
			}
		}
		duration = b;
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
	}
}
