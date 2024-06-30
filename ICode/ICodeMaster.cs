using System;
using System.Collections.Generic;
using UnityEngine;

namespace ICode
{
	[AddComponentMenu("ICode/ICodeMaster")]
	public class ICodeMaster : MonoBehaviour
	{
		[Serializable]
		public class ComponentModel
		{
			public ICodeBehaviour component;

			public bool show;

			public ComponentModel(ICodeBehaviour component, bool show)
			{
				this.component = component;
				this.show = show;
			}
		}

		public List<ComponentModel> components;
	}
}
