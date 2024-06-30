using System.Text;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.Labels
{
	internal class DrawableLabel
	{
		public LabelAnchor anchor;

		public GUIText guiText;

		public StringBuilder newText;

		public bool dirty;

		private Vector2 pixelOffset;

		private Font font;

		private int fontSize;

		private float lineSpacing;

		public DrawableLabel(LabelAnchor anchor, Vector2 pixelOffset, Font font, int fontSize, float lineSpacing)
		{
			this.anchor = anchor;
			this.pixelOffset = pixelOffset;
			this.font = font;
			this.fontSize = fontSize;
			this.lineSpacing = lineSpacing;
			NormalizeOffset();
			newText = new StringBuilder(1000);
		}

		internal void CheckAndUpdate()
		{
			if (newText.Length > 0)
			{
				if (guiText == null)
				{
					GameObject gameObject = new GameObject(anchor.ToString());
					guiText = gameObject.AddComponent<GUIText>();
					if (anchor == LabelAnchor.UpperLeft)
					{
						gameObject.transform.position = new Vector3(0f, 1f);
						guiText.anchor = TextAnchor.UpperLeft;
						guiText.alignment = TextAlignment.Left;
					}
					else if (anchor == LabelAnchor.UpperRight)
					{
						gameObject.transform.position = new Vector3(1f, 1f);
						guiText.anchor = TextAnchor.UpperRight;
						guiText.alignment = TextAlignment.Right;
					}
					else if (anchor == LabelAnchor.LowerLeft)
					{
						gameObject.transform.position = new Vector3(0f, 0f);
						guiText.anchor = TextAnchor.LowerLeft;
						guiText.alignment = TextAlignment.Left;
					}
					else if (anchor == LabelAnchor.LowerRight)
					{
						gameObject.transform.position = new Vector3(1f, 0f);
						guiText.anchor = TextAnchor.LowerRight;
						guiText.alignment = TextAlignment.Right;
					}
					else if (anchor == LabelAnchor.UpperCenter)
					{
						gameObject.transform.position = new Vector3(0.5f, 1f);
						guiText.anchor = TextAnchor.UpperCenter;
						guiText.alignment = TextAlignment.Center;
					}
					else if (anchor == LabelAnchor.LowerCenter)
					{
						gameObject.transform.position = new Vector3(0.5f, 0f);
						guiText.anchor = TextAnchor.LowerCenter;
						guiText.alignment = TextAlignment.Center;
					}
					else
					{
						anchor = LabelAnchor.UpperLeft;
						gameObject.transform.position = new Vector3(0f, 1f);
						guiText.anchor = TextAnchor.UpperLeft;
						guiText.alignment = TextAlignment.Left;
					}
					guiText.pixelOffset = pixelOffset;
					guiText.font = font;
					guiText.fontSize = fontSize;
					guiText.lineSpacing = lineSpacing;
					gameObject.layer = AFPSCounter.Instance.gameObject.layer;
					gameObject.tag = AFPSCounter.Instance.gameObject.tag;
					gameObject.transform.parent = AFPSCounter.Instance.transform;
				}
				if (dirty)
				{
					guiText.text = newText.ToString();
					dirty = false;
				}
				newText.Length = 0;
			}
			else if (guiText != null)
			{
				Object.DestroyImmediate(guiText.gameObject);
			}
		}

		internal void Clear()
		{
			newText.Length = 0;
			if (guiText != null)
			{
				Object.Destroy(guiText.gameObject);
			}
		}

		internal void Dispose()
		{
			Clear();
			newText = null;
		}

		internal void ChangeFont(Font labelsFont)
		{
			font = labelsFont;
			if (guiText != null)
			{
				guiText.font = font;
			}
		}

		internal void ChangeFontSize(int newSize)
		{
			fontSize = newSize;
			if (guiText != null)
			{
				guiText.fontSize = fontSize;
			}
		}

		internal void ChangeOffset(Vector2 newPixelOffset)
		{
			pixelOffset = newPixelOffset;
			NormalizeOffset();
			if (guiText != null)
			{
				guiText.pixelOffset = pixelOffset;
			}
		}

		private void NormalizeOffset()
		{
			if (anchor == LabelAnchor.UpperLeft)
			{
				pixelOffset.y = 0f - pixelOffset.y;
			}
			else if (anchor == LabelAnchor.UpperRight)
			{
				pixelOffset.x = 0f - pixelOffset.x;
				pixelOffset.y = 0f - pixelOffset.y;
			}
			else if (anchor != LabelAnchor.LowerLeft)
			{
				if (anchor == LabelAnchor.LowerRight)
				{
					pixelOffset.x = 0f - pixelOffset.x;
				}
				else if (anchor == LabelAnchor.UpperCenter)
				{
					pixelOffset.y = 0f - pixelOffset.y;
					pixelOffset.x = 0f;
				}
				else if (anchor == LabelAnchor.LowerCenter)
				{
					pixelOffset.x = 0f;
				}
				else
				{
					pixelOffset.y = 0f - pixelOffset.y;
				}
			}
		}

		internal void ChangeLineSpacing(float lineSpacing)
		{
			this.lineSpacing = lineSpacing;
			if (guiText != null)
			{
				guiText.lineSpacing = lineSpacing;
			}
		}
	}
}
