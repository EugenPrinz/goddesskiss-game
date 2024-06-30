using UnityEngine;

namespace Step
{
	public abstract class AbstractVariable : MonoBehaviour
	{
		public new string name;

		public abstract bool Set(AbstractVariable val);

		public abstract bool Copy(AbstractVariable val);
	}
}
