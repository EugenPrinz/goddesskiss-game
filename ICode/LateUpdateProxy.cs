using System;
using UnityEngine;

namespace ICode
{
	public class LateUpdateProxy : MonoBehaviour
	{
		public event Action onLateUpdate;

		private void LateUpdate()
		{
			if (this.onLateUpdate != null)
			{
				this.onLateUpdate();
			}
		}
	}
}
