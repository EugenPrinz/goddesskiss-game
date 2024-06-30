using UnityEngine;

namespace ICode
{
	public class NGUIEventHandler : MonoBehaviour
	{
		public delegate void Trigger();

		public NGUIEventType type;

		public event Trigger onTrigger;

		private void OnClick()
		{
			if (this.onTrigger != null && type == NGUIEventType.OnClick)
			{
				this.onTrigger();
			}
		}

		private void OnDoubleClick()
		{
			if (this.onTrigger != null && type == NGUIEventType.OnDoubleClick)
			{
				this.onTrigger();
			}
		}

		private void OnHover(bool isOver)
		{
			if (this.onTrigger != null && type == NGUIEventType.OnHover)
			{
				this.onTrigger();
			}
		}

		private void OnPress(bool isDown)
		{
			if (this.onTrigger != null && type == NGUIEventType.OnPress)
			{
				this.onTrigger();
			}
		}

		private void OnTooltip(bool show)
		{
			if (this.onTrigger != null && type == NGUIEventType.OnTooltip)
			{
				this.onTrigger();
			}
		}
	}
}
