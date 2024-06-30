using System;
using UnityEngine;

namespace ICode
{
	public class FixedUpdateProxy : MonoBehaviour
	{
		public event Action onFixedUpdate;

		private void FixedUpdate()
		{
			if (this.onFixedUpdate != null)
			{
				this.onFixedUpdate();
			}
		}
	}
}
