using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class MemoryCounterData : BaseCounterData
	{
		public const int MEMORY_DIVIDER = 1048576;

		private const string COROUTINE_NAME = "UpdateMemoryCounter";

		private const string TEXT_START = "<color=#{0}><b>";

		private const string LINE_START_TOTAL = "MEM TOTAL: ";

		private const string LINE_START_ALLOCATED = "MEM ALLOC: ";

		private const string LINE_START_MONO = "MEM MONO: ";

		private const string LINE_END = " MB";

		private const string TEXT_END = "</b></color>";

		public uint lastTotalValue;

		public uint lastAllocatedValue;

		public long lastMonoValue;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float updateInterval = 1f;

		[SerializeField]
		private bool preciseValues;

		[SerializeField]
		private bool totalReserved = true;

		[SerializeField]
		private bool allocated = true;

		[SerializeField]
		private bool monoUsage;

		private bool inited;

		public float UpdateInterval
		{
			get
			{
				return updateInterval;
			}
			set
			{
				if (!(Math.Abs(updateInterval - value) < 0.001f) && Application.isPlaying)
				{
					updateInterval = value;
					if (enabled)
					{
						RestartCoroutine();
					}
				}
			}
		}

		public bool PreciseValues
		{
			get
			{
				return preciseValues;
			}
			set
			{
				if (preciseValues != value && Application.isPlaying)
				{
					preciseValues = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool TotalReserved
		{
			get
			{
				return totalReserved;
			}
			set
			{
				if (totalReserved != value && Application.isPlaying)
				{
					totalReserved = value;
					if (!totalReserved)
					{
						lastTotalValue = 0u;
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool Allocated
		{
			get
			{
				return allocated;
			}
			set
			{
				if (allocated != value && Application.isPlaying)
				{
					allocated = value;
					if (!allocated)
					{
						lastAllocatedValue = 0u;
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool MonoUsage
		{
			get
			{
				return monoUsage;
			}
			set
			{
				if (monoUsage != value && Application.isPlaying)
				{
					monoUsage = value;
					if (!monoUsage)
					{
						lastMonoValue = 0L;
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		internal MemoryCounterData()
		{
			color = new Color32(234, 238, 101, byte.MaxValue);
		}

		protected override void CacheCurrentColor()
		{
			colorCached = $"<color=#{AFPSCounter.Color32ToHex(color)}><b>";
		}

		internal override void Activate()
		{
			if (!enabled || inited || !HasData())
			{
				return;
			}
			base.Activate();
			inited = true;
			lastTotalValue = 0u;
			lastAllocatedValue = 0u;
			lastMonoValue = 0L;
			if (main.OperationMode == OperationMode.Normal)
			{
				if (colorCached == null)
				{
					colorCached = $"<color=#{AFPSCounter.Color32ToHex(color)}><b>";
				}
				if (text == null)
				{
					text = new StringBuilder(200);
				}
				else
				{
					text.Length = 0;
				}
				text.Append(colorCached);
				if (totalReserved)
				{
					if (preciseValues)
					{
						text.Append("MEM TOTAL: ").Append("0.00").Append(" MB");
					}
					else
					{
						text.Append("MEM TOTAL: ").Append(0).Append(" MB");
					}
				}
				if (allocated)
				{
					if (text.Length > 0)
					{
						text.Append('\n');
					}
					if (preciseValues)
					{
						text.Append("MEM ALLOC: ").Append("0.00").Append(" MB");
					}
					else
					{
						text.Append("MEM ALLOC: ").Append(0).Append(" MB");
					}
				}
				if (monoUsage)
				{
					if (text.Length > 0)
					{
						text.Append('\n');
					}
					if (preciseValues)
					{
						text.Append("MEM MONO: ").Append("0.00").Append(" MB");
					}
					else
					{
						text.Append("MEM MONO: ").Append(0).Append(" MB");
					}
				}
				text.Append("</b></color>");
				dirty = true;
			}
			main.StartCoroutine("UpdateMemoryCounter");
		}

		internal override void Deactivate()
		{
			if (inited)
			{
				base.Deactivate();
				if (text != null)
				{
					text.Length = 0;
				}
				main.StopCoroutine("UpdateMemoryCounter");
				main.MakeDrawableLabelDirty(anchor);
				inited = false;
			}
		}

		internal override void UpdateValue(bool force)
		{
			if (!enabled)
			{
				return;
			}
			if (force)
			{
				if (!inited && HasData())
				{
					Activate();
					return;
				}
				if (inited && !HasData())
				{
					Deactivate();
					return;
				}
			}
			if (totalReserved)
			{
				uint totalReservedMemory = Profiler.GetTotalReservedMemory();
				uint num = 0u;
				bool flag;
				if (preciseValues)
				{
					flag = lastTotalValue != totalReservedMemory;
				}
				else
				{
					num = totalReservedMemory / 1048576;
					flag = lastTotalValue != num;
				}
				if (flag || force)
				{
					if (preciseValues)
					{
						lastTotalValue = totalReservedMemory;
					}
					else
					{
						lastTotalValue = num;
					}
					dirty = true;
				}
			}
			if (allocated)
			{
				uint totalAllocatedMemory = Profiler.GetTotalAllocatedMemory();
				uint num2 = 0u;
				bool flag2;
				if (preciseValues)
				{
					flag2 = lastAllocatedValue != totalAllocatedMemory;
				}
				else
				{
					num2 = totalAllocatedMemory / 1048576;
					flag2 = lastAllocatedValue != num2;
				}
				if (flag2 || force)
				{
					if (preciseValues)
					{
						lastAllocatedValue = totalAllocatedMemory;
					}
					else
					{
						lastAllocatedValue = num2;
					}
					dirty = true;
				}
			}
			if (monoUsage)
			{
				long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
				long num3 = 0L;
				bool flag3;
				if (preciseValues)
				{
					flag3 = lastMonoValue != totalMemory;
				}
				else
				{
					num3 = totalMemory / 1048576;
					flag3 = lastMonoValue != num3;
				}
				if (flag3 || force)
				{
					if (preciseValues)
					{
						lastMonoValue = totalMemory;
					}
					else
					{
						lastMonoValue = num3;
					}
					dirty = true;
				}
			}
			if (!dirty || main.OperationMode != OperationMode.Normal)
			{
				return;
			}
			bool flag4 = false;
			text.Length = 0;
			text.Append(colorCached);
			if (totalReserved)
			{
				text.Append("MEM TOTAL: ");
				if (preciseValues)
				{
					text.Append(((float)lastTotalValue / 1048576f).ToString("F"));
				}
				else
				{
					text.Append(lastTotalValue);
				}
				text.Append(" MB");
				flag4 = true;
			}
			if (allocated)
			{
				if (flag4)
				{
					text.Append('\n');
				}
				text.Append("MEM ALLOC: ");
				if (preciseValues)
				{
					text.Append(((float)lastAllocatedValue / 1048576f).ToString("F"));
				}
				else
				{
					text.Append(lastAllocatedValue);
				}
				text.Append(" MB");
				flag4 = true;
			}
			if (monoUsage)
			{
				if (flag4)
				{
					text.Append('\n');
				}
				text.Append("MEM MONO: ");
				if (preciseValues)
				{
					text.Append(((float)lastMonoValue / 1048576f).ToString("F"));
				}
				else
				{
					text.Append(lastMonoValue);
				}
				text.Append(" MB");
			}
			text.Append("</b></color>");
		}

		private void RestartCoroutine()
		{
			main.StopCoroutine("UpdateMemoryCounter");
			main.StartCoroutine("UpdateMemoryCounter");
		}

		private bool HasData()
		{
			return totalReserved || allocated || monoUsage;
		}
	}
}
