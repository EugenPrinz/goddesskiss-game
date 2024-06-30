using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Variable)]
	[Tooltip("Compares two int values.")]
	public class CompareInt : Condition
	{
		[Shared]
		[Tooltip("Parameter to test.")]
		public FsmInt variable;

		[Tooltip("Is the variable greater or less?")]
		public FloatComparer comparer;

		[Tooltip("Value to test with.")]
		public FsmInt value;

		public override bool Validate()
		{
			return FsmUtility.CompareFloat(variable.Value, value.Value, comparer);
		}
	}
}
