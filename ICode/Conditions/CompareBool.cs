using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Variable)]
	[Tooltip("Compares if a bool parameter equals true or false.")]
	public class CompareBool : Condition
	{
		[Shared]
		[Tooltip("Parameter to test.")]
		public FsmBool variable;

		[Tooltip("Does the result equals this condition.")]
		public FsmBool equals;

		public override bool Validate()
		{
			return variable.Value == equals.Value;
		}
	}
}
