using System;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class FPSCounterData : BaseCounterData
	{
		private const string COROUTINE_NAME = "UpdateFPSCounter";

		private const string FPS_TEXT_START = "<color=#{0}><b>FPS: ";

		private const string FPS_TEXT_END = "</b></color>";

		private const string MS_TEXT_START = " <color=#{0}><b>[";

		private const string MS_TEXT_END = " MS]</b></color>";

		private const string MIN_TEXT_START = "<color=#{0}><b>MIN: ";

		private const string MIN_TEXT_END = "</b></color> ";

		private const string MAX_TEXT_START = "<color=#{0}><b>MAX: ";

		private const string MAX_TEXT_END = "</b></color>";

		private const string AVG_TEXT_START = " <color=#{0}><b>AVG: ";

		private const string AVG_TEXT_END = "</b></color>";

		public int warningLevelValue = 50;

		public int criticalLevelValue = 20;

		public bool resetAverageOnNewScene;

		public bool resetMinMaxOnNewScene;

		[Range(0f, 10f)]
		public int minMaxIntervalsToSkip = 3;

		public int lastValue;

		public float lastMillisecondsValue;

		public int lastAverageValue;

		public int lastMinimumValue = -1;

		public int lastMaximumValue = -1;

		[NonSerialized]
		public FPSLevel currentFPSLevel;

		[NonSerialized]
		public Action<FPSLevel> onFPSLevelChange;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float updateInterval = 0.5f;

		[SerializeField]
		private bool showMilliseconds = true;

		[SerializeField]
		private bool showAverage = true;

		[SerializeField]
		[Range(0f, 100f)]
		private int averageFromSamples = 50;

		[SerializeField]
		private bool showMinMax;

		[SerializeField]
		private bool minMaxOnNewLine;

		[SerializeField]
		private Color colorWarning = new Color32(236, 224, 88, byte.MaxValue);

		[SerializeField]
		private Color colorCritical = new Color32(249, 91, 91, byte.MaxValue);

		internal float newValue;

		private string colorCachedMs;

		private string colorCachedMin;

		private string colorCachedMax;

		private string colorCachedAvg;

		private string colorWarningCached;

		private string colorWarningCachedMs;

		private string colorWarningCachedMin;

		private string colorWarningCachedMax;

		private string colorWarningCachedAvg;

		private string colorCriticalCached;

		private string colorCriticalCachedMs;

		private string colorCriticalCachedMin;

		private string colorCriticalCachedMax;

		private string colorCriticalCachedAvg;

		private bool inited;

		private int currentAverageSamples;

		private float currentAverageRaw;

		private float[] accumulatedAverageSamples;

		private int minMaxIntervalsSkipped;

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

		public bool MinMaxOnNewLine
		{
			get
			{
				return minMaxOnNewLine;
			}
			set
			{
				if (minMaxOnNewLine != value && Application.isPlaying)
				{
					minMaxOnNewLine = value;
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool ShowMilliseconds
		{
			get
			{
				return showMilliseconds;
			}
			set
			{
				if (showMilliseconds != value && Application.isPlaying)
				{
					showMilliseconds = value;
					if (!showMilliseconds)
					{
						lastMillisecondsValue = 0f;
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public bool ShowAverage
		{
			get
			{
				return showAverage;
			}
			set
			{
				if (showAverage != value && Application.isPlaying)
				{
					showAverage = value;
					if (!showAverage)
					{
						ResetAverage();
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public int AverageFromSamples
		{
			get
			{
				return averageFromSamples;
			}
			set
			{
				if (averageFromSamples == value || !Application.isPlaying)
				{
					return;
				}
				averageFromSamples = value;
				if (!enabled)
				{
					return;
				}
				if (averageFromSamples > 0)
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageFromSamples];
					}
					else if (accumulatedAverageSamples.Length != averageFromSamples)
					{
						Array.Resize(ref accumulatedAverageSamples, averageFromSamples);
					}
				}
				else
				{
					accumulatedAverageSamples = null;
				}
				ResetAverage();
				Refresh();
			}
		}

		public bool ShowMinMax
		{
			get
			{
				return showMinMax;
			}
			set
			{
				if (showMinMax != value && Application.isPlaying)
				{
					showMinMax = value;
					if (!showMinMax)
					{
						ResetMinMax();
					}
					if (enabled)
					{
						Refresh();
					}
				}
			}
		}

		public Color ColorWarning
		{
			get
			{
				return colorWarning;
			}
			set
			{
				if (!(colorWarning == value) && Application.isPlaying)
				{
					colorWarning = value;
					if (enabled)
					{
						CacheWarningColor();
						Refresh();
					}
				}
			}
		}

		public Color ColorCritical
		{
			get
			{
				return colorCritical;
			}
			set
			{
				if (!(colorCritical == value) && Application.isPlaying)
				{
					colorCritical = value;
					if (enabled)
					{
						CacheCriticalColor();
						Refresh();
					}
				}
			}
		}

		internal FPSCounterData()
		{
			color = new Color32(85, 218, 102, byte.MaxValue);
		}

		public void ResetAverage()
		{
			if (Application.isPlaying)
			{
				lastAverageValue = 0;
				currentAverageSamples = 0;
				currentAverageRaw = 0f;
				if (averageFromSamples > 0 && accumulatedAverageSamples != null)
				{
					Array.Clear(accumulatedAverageSamples, 0, accumulatedAverageSamples.Length);
				}
			}
		}

		public void ResetMinMax()
		{
			if (Application.isPlaying)
			{
				lastMinimumValue = -1;
				lastMaximumValue = -1;
				minMaxIntervalsSkipped = 0;
				UpdateValue(force: true);
				dirty = true;
			}
		}

		internal override void Activate()
		{
			if (!enabled || inited)
			{
				return;
			}
			base.Activate();
			inited = true;
			lastValue = 0;
			lastMinimumValue = -1;
			if (main.OperationMode == OperationMode.Normal)
			{
				if (colorCached == null)
				{
					CacheCurrentColor();
				}
				if (colorWarningCached == null)
				{
					CacheWarningColor();
				}
				if (colorCriticalCached == null)
				{
					CacheCriticalColor();
				}
				text.Append(colorCriticalCached).Append("0").Append("</b></color>");
				dirty = true;
			}
			main.StartCoroutine("UpdateFPSCounter");
		}

		internal override void Deactivate()
		{
			if (inited)
			{
				base.Deactivate();
				main.StopCoroutine("UpdateFPSCounter");
				ResetMinMax();
				ResetAverage();
				lastValue = 0;
				currentFPSLevel = FPSLevel.Normal;
				inited = false;
			}
		}

		internal override void UpdateValue(bool force)
		{
			if (!enabled)
			{
				return;
			}
			int num = (int)newValue;
			if (lastValue != num || force)
			{
				lastValue = num;
				dirty = true;
			}
			if (lastValue <= criticalLevelValue)
			{
				if (lastValue != 0 && currentFPSLevel != FPSLevel.Critical)
				{
					currentFPSLevel = FPSLevel.Critical;
					if (onFPSLevelChange != null)
					{
						onFPSLevelChange(currentFPSLevel);
					}
				}
			}
			else if (lastValue < warningLevelValue)
			{
				if (lastValue != 0 && currentFPSLevel != FPSLevel.Warning)
				{
					currentFPSLevel = FPSLevel.Warning;
					if (onFPSLevelChange != null)
					{
						onFPSLevelChange(currentFPSLevel);
					}
				}
			}
			else if (lastValue != 0 && currentFPSLevel != 0)
			{
				currentFPSLevel = FPSLevel.Normal;
				if (onFPSLevelChange != null)
				{
					onFPSLevelChange(currentFPSLevel);
				}
			}
			if (dirty && showMilliseconds)
			{
				lastMillisecondsValue = 1000f / newValue;
			}
			int num2 = 0;
			if (showAverage)
			{
				if (averageFromSamples == 0)
				{
					currentAverageSamples++;
					currentAverageRaw += ((float)lastValue - currentAverageRaw) / (float)currentAverageSamples;
				}
				else
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageFromSamples];
						ResetAverage();
					}
					accumulatedAverageSamples[currentAverageSamples % averageFromSamples] = lastValue;
					currentAverageSamples++;
					currentAverageRaw = GetAverageFromAccumulatedSamples();
				}
				num2 = Mathf.RoundToInt(currentAverageRaw);
				if (lastAverageValue != num2 || force)
				{
					lastAverageValue = num2;
					dirty = true;
				}
			}
			if (showMinMax)
			{
				if (minMaxIntervalsSkipped < minMaxIntervalsToSkip)
				{
					if (!force)
					{
						minMaxIntervalsSkipped++;
					}
				}
				else if (dirty)
				{
					if (lastMinimumValue == -1)
					{
						lastMinimumValue = lastValue;
					}
					else if (lastValue < lastMinimumValue)
					{
						lastMinimumValue = lastValue;
						dirty = true;
					}
					if (lastMaximumValue == -1)
					{
						lastMaximumValue = lastValue;
					}
					else if (lastValue > lastMaximumValue)
					{
						lastMaximumValue = lastValue;
						dirty = true;
					}
				}
			}
			if (!dirty || main.OperationMode != OperationMode.Normal)
			{
				return;
			}
			string value = ((lastValue >= warningLevelValue) ? colorCached : ((lastValue > criticalLevelValue) ? colorWarningCached : colorCriticalCached));
			text.Length = 0;
			text.Append(value).Append(lastValue).Append("</b></color>");
			if (showMilliseconds)
			{
				value = ((lastValue >= warningLevelValue) ? colorCachedMs : ((lastValue > criticalLevelValue) ? colorWarningCachedMs : colorCriticalCachedMs));
				text.Append(value).Append(lastMillisecondsValue.ToString("F")).Append(" MS]</b></color>");
			}
			if (showAverage)
			{
				value = ((num2 >= warningLevelValue) ? colorCachedAvg : ((num2 > criticalLevelValue) ? colorWarningCachedAvg : colorCriticalCachedAvg));
				text.Append(value).Append(num2).Append("</b></color>");
			}
			if (showMinMax)
			{
				if (minMaxOnNewLine)
				{
					text.Append('\n');
				}
				else
				{
					text.Append(' ');
				}
				value = ((lastMinimumValue >= warningLevelValue) ? colorCachedMin : ((lastMinimumValue > criticalLevelValue) ? colorWarningCachedMin : colorCriticalCachedMin));
				text.Append(value).Append(lastMinimumValue).Append("</b></color> ");
				value = ((lastMaximumValue >= warningLevelValue) ? colorCachedMax : ((lastMaximumValue > criticalLevelValue) ? colorWarningCachedMax : colorCriticalCachedMax));
				text.Append(value).Append(lastMaximumValue).Append("</b></color>");
			}
		}

		protected override void CacheCurrentColor()
		{
			string arg = AFPSCounter.Color32ToHex(color);
			colorCached = $"<color=#{arg}><b>FPS: ";
			colorCachedMs = $" <color=#{arg}><b>[";
			colorCachedMin = $"<color=#{arg}><b>MIN: ";
			colorCachedMax = $"<color=#{arg}><b>MAX: ";
			colorCachedAvg = $" <color=#{arg}><b>AVG: ";
		}

		protected void CacheWarningColor()
		{
			string arg = AFPSCounter.Color32ToHex(colorWarning);
			colorWarningCached = $"<color=#{arg}><b>FPS: ";
			colorWarningCachedMs = $" <color=#{arg}><b>[";
			colorWarningCachedMin = $"<color=#{arg}><b>MIN: ";
			colorWarningCachedMax = $"<color=#{arg}><b>MAX: ";
			colorWarningCachedAvg = $" <color=#{arg}><b>AVG: ";
		}

		protected void CacheCriticalColor()
		{
			string arg = AFPSCounter.Color32ToHex(colorCritical);
			colorCriticalCached = $"<color=#{arg}><b>FPS: ";
			colorCriticalCachedMs = $" <color=#{arg}><b>[";
			colorCriticalCachedMin = $"<color=#{arg}><b>MIN: ";
			colorCriticalCachedMax = $"<color=#{arg}><b>MAX: ";
			colorCriticalCachedAvg = $" <color=#{arg}><b>AVG: ";
		}

		private void RestartCoroutine()
		{
			main.StopCoroutine("UpdateFPSCounter");
			main.StartCoroutine("UpdateFPSCounter");
		}

		private float GetAverageFromAccumulatedSamples()
		{
			float num = 0f;
			for (int i = 0; i < averageFromSamples; i++)
			{
				num += accumulatedAverageSamples[i];
			}
			if (currentAverageSamples < averageFromSamples)
			{
				return num / (float)currentAverageSamples;
			}
			return num / (float)averageFromSamples;
		}
	}
}
