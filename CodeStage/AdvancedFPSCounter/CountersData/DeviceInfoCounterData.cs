using System;
using System.Text;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class DeviceInfoCounterData : BaseCounterData
	{
		[HideInInspector]
		public string lastValue = string.Empty;

		[SerializeField]
		private bool cpuModel = true;

		[SerializeField]
		private bool gpuModel = true;

		[SerializeField]
		private bool ramSize = true;

		[SerializeField]
		private bool screenData = true;

		private bool inited;

		public bool CpuModel
		{
			get
			{
				return cpuModel;
			}
			set
			{
				if (cpuModel != value && Application.isPlaying)
				{
					cpuModel = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool GpuModel
		{
			get
			{
				return gpuModel;
			}
			set
			{
				if (gpuModel != value && Application.isPlaying)
				{
					gpuModel = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool RamSize
		{
			get
			{
				return ramSize;
			}
			set
			{
				if (ramSize != value && Application.isPlaying)
				{
					ramSize = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool ScreenData
		{
			get
			{
				return screenData;
			}
			set
			{
				if (screenData != value && Application.isPlaying)
				{
					screenData = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		internal DeviceInfoCounterData()
		{
			color = new Color32(172, 172, 172, byte.MaxValue);
			anchor = LabelAnchor.LowerLeft;
		}

		protected override void CacheCurrentColor()
		{
			colorCached = "<color=#" + AFPSCounter.Color32ToHex(color) + ">";
		}

		internal override void Activate()
		{
			if (enabled && !inited && HasData())
			{
				base.Activate();
				inited = true;
				if (main.OperationMode == OperationMode.Normal && colorCached == null)
				{
					colorCached = "<color=#" + AFPSCounter.Color32ToHex(color) + ">";
				}
				if (text == null)
				{
					text = new StringBuilder();
				}
				else
				{
					text.Remove(0, text.Length);
				}
				UpdateValue();
			}
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
				main.MakeDrawableLabelDirty(anchor);
				inited = false;
			}
		}

		internal override void UpdateValue(bool force)
		{
			if (!inited && HasData())
			{
				Activate();
			}
			else if (inited && !HasData())
			{
				Deactivate();
			}
			else
			{
				if (!enabled)
				{
					return;
				}
				bool flag = false;
				text.Remove(0, text.Length);
				if (cpuModel)
				{
					text.Append("CPU: ").Append(SystemInfo.processorType).Append(" [")
						.Append(SystemInfo.processorCount)
						.Append(" cores]");
					flag = true;
				}
				if (gpuModel)
				{
					if (flag)
					{
						text.Append('\n');
					}
					text.Append("GPU: ").Append(SystemInfo.graphicsDeviceName).Append(", API: ")
						.Append(SystemInfo.graphicsDeviceVersion);
					bool flag2 = true;
					switch (SystemInfo.graphicsShaderLevel)
					{
					case 20:
						text.Append('\n').Append("GPU: SM: 2.0");
						break;
					case 30:
						text.Append('\n').Append("GPU: SM: 3.0");
						break;
					case 40:
						text.Append('\n').Append("GPU: SM: 4.0");
						break;
					case 41:
						text.Append('\n').Append("GPU: SM: 4.1");
						break;
					case 50:
						text.Append('\n').Append("GPU: SM: 5.0");
						break;
					default:
						flag2 = false;
						break;
					}
					int graphicsMemorySize = SystemInfo.graphicsMemorySize;
					if (graphicsMemorySize > 0)
					{
						if (flag2)
						{
							text.Append(", VRAM: ");
						}
						else
						{
							text.Append('\n').Append("GPU: VRAM: ");
						}
						text.Append(graphicsMemorySize).Append(" MB");
					}
					flag = true;
				}
				if (ramSize)
				{
					if (flag)
					{
						text.Append('\n');
					}
					int systemMemorySize = SystemInfo.systemMemorySize;
					if (systemMemorySize > 0)
					{
						text.Append("RAM: ").Append(systemMemorySize).Append(" MB");
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				if (screenData)
				{
					if (flag)
					{
						text.Append('\n');
					}
					Resolution currentResolution = Screen.currentResolution;
					text.Append("SCR: ").Append(currentResolution.width).Append("x")
						.Append(currentResolution.height)
						.Append("@")
						.Append(currentResolution.refreshRate)
						.Append("Hz [window size: ")
						.Append(Screen.width)
						.Append("x")
						.Append(Screen.height);
					float dpi = Screen.dpi;
					if (dpi > 0f)
					{
						text.Append(", DPI: ").Append(dpi).Append("]");
					}
					else
					{
						text.Append("]");
					}
				}
				lastValue = text.ToString();
				if (main.OperationMode == OperationMode.Normal)
				{
					text.Insert(0, colorCached);
					text.Append("</color>");
				}
				else
				{
					text.Length = 0;
				}
				dirty = true;
			}
		}

		private bool HasData()
		{
			return cpuModel || gpuModel || ramSize || screenData;
		}
	}
}
