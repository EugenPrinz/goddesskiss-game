using System;
using System.Text;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public abstract class BaseCounterData
	{
		[SerializeField]
		protected bool enabled = true;

		[SerializeField]
		protected LabelAnchor anchor;

		[SerializeField]
		protected Color color;

		protected string colorCached;

		internal StringBuilder text;

		internal bool dirty;

		protected AFPSCounter main;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value && Application.isPlaying)
				{
					enabled = value;
					if (enabled)
					{
						Activate();
					}
					else
					{
						Deactivate();
					}
					main.UpdateTexts();
				}
			}
		}

		public LabelAnchor Anchor
		{
			get
			{
				return anchor;
			}
			set
			{
				if (anchor != value && Application.isPlaying)
				{
					LabelAnchor labelAnchor = anchor;
					anchor = value;
					if (enabled)
					{
						dirty = true;
						main.MakeDrawableLabelDirty(labelAnchor);
						main.UpdateTexts();
					}
				}
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (!(color == value) && Application.isPlaying)
				{
					color = value;
					if (enabled)
					{
						CacheCurrentColor();
						Refresh();
					}
				}
			}
		}

		public void Refresh()
		{
			if (enabled && Application.isPlaying)
			{
				UpdateValue(force: true);
				main.UpdateTexts();
			}
		}

		protected abstract void CacheCurrentColor();

		internal virtual void UpdateValue()
		{
			UpdateValue(force: false);
		}

		internal virtual void UpdateValue(bool force)
		{
		}

		internal void Init(AFPSCounter reference)
		{
			main = reference;
		}

		internal void Dispose()
		{
			main = null;
			if (text != null)
			{
				text.Remove(0, text.Length);
				text = null;
			}
		}

		internal virtual void Activate()
		{
			if (main.OperationMode == OperationMode.Normal)
			{
				if (text == null)
				{
					text = new StringBuilder(100);
				}
				else
				{
					text.Remove(0, text.Length);
				}
			}
		}

		internal virtual void Deactivate()
		{
			if (text != null)
			{
				text.Remove(0, text.Length);
			}
			main.MakeDrawableLabelDirty(anchor);
		}
	}
}
