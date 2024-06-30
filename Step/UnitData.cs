using System;
using Shared.Battle;

namespace Step
{
	public class UnitData : AbstractVariable
	{
		[Serializable]
		public class VariableData
		{
			public int position;

			public int unitIdx;

			public Unit unit;

			public UnitRenderer unitRenderer;
		}

		public VariableData data;

		public int position
		{
			get
			{
				return data.position;
			}
			set
			{
				data.position = value;
			}
		}

		public int unitIdx
		{
			get
			{
				return data.unitIdx;
			}
			set
			{
				data.unitIdx = value;
			}
		}

		public Unit unit
		{
			get
			{
				return data.unit;
			}
			set
			{
				data.unit = value;
			}
		}

		public UnitRenderer unitRenderer
		{
			get
			{
				return data.unitRenderer;
			}
			set
			{
				data.unitRenderer = value;
			}
		}

		public override bool Set(AbstractVariable val)
		{
			if (val is UnitData)
			{
				data = ((UnitData)val).data;
				return true;
			}
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			if (val is UnitData)
			{
				position = ((UnitData)val).position;
				unitIdx = ((UnitData)val).unitIdx;
				unit = ((UnitData)val).unit;
				unitRenderer = ((UnitData)val).unitRenderer;
				return true;
			}
			return false;
		}
	}
}
