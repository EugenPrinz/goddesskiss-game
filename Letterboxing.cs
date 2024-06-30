using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Letterboxing : MonoBehaviour
{
	private void Start()
	{
		float num = 1.7777778f;
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = num2 / num;
		Camera component = GetComponent<Camera>();
		Rect rect = component.rect;
		if (num3 < 1f)
		{
			rect.x = 0f;
			rect.y = (1f - num3) / 2f;
			rect.width = 1f;
			rect.height = num3;
		}
		else
		{
			float num4 = 1f / num3;
			rect.x = (1f - num4) / 2f;
			rect.y = 0f;
			rect.width = num4;
			rect.height = 1f;
		}
		component.rect = rect;
	}
}
