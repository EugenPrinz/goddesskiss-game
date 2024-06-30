using System;
using UnityEngine;

namespace ICode
{
	public class OnGUIProxy : MonoBehaviour
	{
		public event Action onGUI;

		private void OnGUI()
		{
			if (this.onGUI != null)
			{
				this.onGUI();
			}
		}
	}
}
