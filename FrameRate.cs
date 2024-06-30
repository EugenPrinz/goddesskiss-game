using UnityEngine;

public class FrameRate : MonoBehaviour
{
	public bool shouldShowFps = true;

	private float _fpsCheckTimer;

	private int _frameCount;

	private int _nextFrameCount;

	public string prefix = "FPS:";

	public Rect viewRect = new Rect(0f, 50f, 100f, 20f);

	private void OnGUI()
	{
		_nextFrameCount++;
		_fpsCheckTimer += Time.unscaledDeltaTime;
		if (_fpsCheckTimer >= 1f)
		{
			_fpsCheckTimer = 0f;
			_frameCount = _nextFrameCount;
			_nextFrameCount = 0;
		}
		if (shouldShowFps)
		{
			GUI.contentColor = Color.red;
			if (_frameCount >= 60)
			{
				GUI.contentColor = Color.green;
			}
			else if (_frameCount >= 30)
			{
				GUI.contentColor = Color.yellow;
			}
			GUI.Label(viewRect, prefix + _frameCount);
		}
	}
}
