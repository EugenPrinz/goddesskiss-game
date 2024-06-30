using UnityEngine;

public class UIStatusSkillBubble : UIStatusBubble
{
	public int bubbleMaximum = 5;

	public int bubbleHeight = 30;

	protected GameObject[] bubbles;

	private void Awake()
	{
		bubbles = new GameObject[bubbleMaximum];
	}

	protected int FindEmptyBubbleIdx()
	{
		for (int i = 0; i < bubbles.Length; i++)
		{
			if (bubbles[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	protected void ReleaseBubbleIdx(int idx)
	{
		bubbles[idx] = null;
	}

	public void OnReleaseBubble(UIStatusBubbleItem bubbleItem)
	{
		ReleaseBubbleIdx(bubbleItem.idx);
	}

	protected Vector3 GetBubblePosition(int idx)
	{
		return new Vector3(0f, -(idx * bubbleHeight), 0f);
	}

	public void Add(int mode, string commanderImg, string skillName)
	{
		int num = FindEmptyBubbleIdx();
		if (num >= 0)
		{
			GameObject gameObject = _Create();
			bubbles[num] = gameObject;
			UIStatusBubbleItem component = gameObject.GetComponent<UIStatusBubbleItem>();
			component.idx = num;
			component.transform.localPosition = GetBubblePosition(num);
			component.Set(mode, commanderImg, skillName);
		}
	}
}
