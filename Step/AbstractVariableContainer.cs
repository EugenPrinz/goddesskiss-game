using System;
using System.Collections.Generic;

namespace Step
{
	public abstract class AbstractVariableContainer : AbstractVariable
	{
		[Serializable]
		public class ElementData
		{
			public string key;

			public int index;

			public AbstractVariable value;
		}

		public List<ElementData> datas;

		protected Dictionary<string, ElementData> dicDatas;

		public virtual void Load()
		{
		}
	}
}
