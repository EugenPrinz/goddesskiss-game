using System;
using CodeStage.AdvancedFPSCounter.CountersData;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	public class APITester : MonoBehaviour
	{
		private int selectedTab;

		private readonly string[] tabs = new string[4] { "Common", "FPS Counter", "Memory Counter", "Device info" };

		private FPSLevel currentFPSLevel;

		private void Awake()
		{
			AFPSCounter.AddToScene();
			FPSCounterData fpsCounter = AFPSCounter.Instance.fpsCounter;
			fpsCounter.onFPSLevelChange = (Action<FPSLevel>)Delegate.Combine(fpsCounter.onFPSLevelChange, new Action<FPSLevel>(OnFPSLevelChanged));
		}

		private void OnFPSLevelChanged(FPSLevel newLevel)
		{
			currentFPSLevel = newLevel;
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(40f, 110f, Screen.width - 80, Screen.height - 140));
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.UpperCenter;
			gUIStyle.richText = true;
			GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.label);
			gUIStyle2.richText = true;
			GUILayout.Label("<b>Public API usage examples</b>", gUIStyle);
			selectedTab = GUILayout.Toolbar(selectedTab, tabs);
			if (selectedTab == 0)
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Operation Mode:", GUILayout.MaxWidth(100f));
				int operationMode = (int)AFPSCounter.Instance.OperationMode;
				operationMode = GUILayout.Toolbar(operationMode, new string[3]
				{
					OperationMode.Disabled.ToString(),
					OperationMode.Background.ToString(),
					OperationMode.Normal.ToString()
				});
				AFPSCounter.Instance.OperationMode = (OperationMode)operationMode;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Hot Key:", GUILayout.MaxWidth(100f));
				int selected = (int)((AFPSCounter.Instance.hotKey == KeyCode.BackQuote) ? ((KeyCode)1) : AFPSCounter.Instance.hotKey);
				selected = GUILayout.Toolbar(selected, new string[2] { "None (disabled)", "BackQoute (`)" });
				if (selected == 1)
				{
					AFPSCounter.Instance.hotKey = KeyCode.BackQuote;
				}
				else
				{
					AFPSCounter.Instance.hotKey = KeyCode.None;
				}
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.keepAlive = GUILayout.Toggle(AFPSCounter.Instance.keepAlive, "Keep Alive");
				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.ForceFrameRate = GUILayout.Toggle(AFPSCounter.Instance.ForceFrameRate, "Force FPS", GUILayout.Width(100f));
				AFPSCounter.Instance.ForcedFrameRate = (int)SliderLabel(AFPSCounter.Instance.ForcedFrameRate, -1f, 100f);
				GUILayout.EndHorizontal();
				float x = AFPSCounter.Instance.AnchorsOffset.x;
				float y = AFPSCounter.Instance.AnchorsOffset.y;
				GUILayout.BeginHorizontal();
				GUILayout.Label("Pixel offset X", GUILayout.Width(100f));
				x = (int)SliderLabel(x, 0f, 100f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Pixel offset Y", GUILayout.Width(100f));
				y = (int)SliderLabel(y, 0f, 100f);
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.AnchorsOffset = new Vector2(x, y);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Font Size", GUILayout.Width(100f));
				AFPSCounter.Instance.FontSize = (int)SliderLabel(AFPSCounter.Instance.FontSize, 0f, 100f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Line spacing", GUILayout.Width(100f));
				AFPSCounter.Instance.LineSpacing = SliderLabel(AFPSCounter.Instance.LineSpacing, 0f, 10f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Counters spacing", GUILayout.Width(120f));
				AFPSCounter.Instance.CountersSpacing = (int)SliderLabel(AFPSCounter.Instance.CountersSpacing, 0f, 10f);
				GUILayout.EndHorizontal();
			}
			else if (selectedTab == 1)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.fpsCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Enabled, "Enabled");
				GUILayout.Space(10f);
				AFPSCounter.Instance.fpsCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.fpsCounter.Anchor, new string[6] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Update Interval", GUILayout.Width(100f));
				AFPSCounter.Instance.fpsCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.fpsCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.fpsCounter.ShowMilliseconds = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.ShowMilliseconds, "Milliseconds");
				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.fpsCounter.ShowAverage = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.ShowAverage, "Average FPS", GUILayout.Width(100f));
				if (AFPSCounter.Instance.fpsCounter.ShowAverage)
				{
					GUILayout.Label("Samples", GUILayout.ExpandWidth(expand: false));
					AFPSCounter.Instance.fpsCounter.AverageFromSamples = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.AverageFromSamples, 0f, 100f);
					GUILayout.Space(10f);
					AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene, "Reset Average On Load", GUILayout.ExpandWidth(expand: false));
					if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(expand: false)))
					{
						AFPSCounter.Instance.fpsCounter.ResetAverage();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.fpsCounter.ShowMinMax = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.ShowMinMax, "MinMax FPS", GUILayout.Width(100f));
				if (AFPSCounter.Instance.fpsCounter.ShowMinMax)
				{
					GUILayout.Label("Delay", GUILayout.ExpandWidth(expand: false));
					AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip, 0f, 10f);
					GUILayout.Space(10f);
					AFPSCounter.Instance.fpsCounter.MinMaxOnNewLine = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMaxOnNewLine, "On new line");
					AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene, "Reset MinMax On Load", GUILayout.ExpandWidth(expand: false));
					if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(expand: false)))
					{
						AFPSCounter.Instance.fpsCounter.ResetMinMax();
					}
				}
				GUILayout.EndHorizontal();
			}
			else if (selectedTab == 2)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.memoryCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Enabled, "Enabled");
				GUILayout.Space(10f);
				AFPSCounter.Instance.memoryCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.memoryCounter.Anchor, new string[6] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Update Interval", GUILayout.Width(100f));
				AFPSCounter.Instance.memoryCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.memoryCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.memoryCounter.PreciseValues = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.PreciseValues, "Precise (uses more system resources)");
				AFPSCounter.Instance.memoryCounter.TotalReserved = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.TotalReserved, "Show total reserved memory size");
				AFPSCounter.Instance.memoryCounter.Allocated = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Allocated, "Show allocated memory size");
				AFPSCounter.Instance.memoryCounter.MonoUsage = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.MonoUsage, "Show mono memory usage");
			}
			else if (selectedTab == 3)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.Enabled, "Enabled");
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.deviceInfoCounter.Anchor, new string[6] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.CpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.CpuModel, "Show CPU model and maximum threads count");
				AFPSCounter.Instance.deviceInfoCounter.GpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuModel, "Show GPU model and total VRAM count");
				AFPSCounter.Instance.deviceInfoCounter.RamSize = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.RamSize, "Show total RAM size");
				AFPSCounter.Instance.deviceInfoCounter.ScreenData = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.ScreenData, "Show resolution, window size and DPI (if possible)");
			}
			GUILayout.Label("<b>Raw counters values</b> (read using API)", gUIStyle2);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.ExpandWidth(expand: true));
			GUILayout.Label(string.Concat("  FPS: ", AFPSCounter.Instance.fpsCounter.lastValue, "  [", AFPSCounter.Instance.fpsCounter.lastMillisecondsValue, " MS]  AVG: ", AFPSCounter.Instance.fpsCounter.lastAverageValue, "\n  MIN: ", AFPSCounter.Instance.fpsCounter.lastMinimumValue, "  MAX: ", AFPSCounter.Instance.fpsCounter.lastMaximumValue, "\n  Level (direct / callback): ", AFPSCounter.Instance.fpsCounter.currentFPSLevel, " / ", currentFPSLevel));
			if (AFPSCounter.Instance.memoryCounter.PreciseValues)
			{
				GUILayout.Label("  Memory (Total, Allocated, Mono):\n  " + (float)AFPSCounter.Instance.memoryCounter.lastTotalValue / 1048576f + ", " + (float)AFPSCounter.Instance.memoryCounter.lastAllocatedValue / 1048576f + ", " + (float)AFPSCounter.Instance.memoryCounter.lastMonoValue / 1048576f);
			}
			else
			{
				GUILayout.Label("  Memory (Total, Allocated, Mono):\n  " + AFPSCounter.Instance.memoryCounter.lastTotalValue + ", " + AFPSCounter.Instance.memoryCounter.lastAllocatedValue + ", " + AFPSCounter.Instance.memoryCounter.lastMonoValue);
			}
			GUILayout.EndVertical();
			if (AFPSCounter.Instance.deviceInfoCounter.Enabled)
			{
				GUILayout.Label(AFPSCounter.Instance.deviceInfoCounter.lastValue);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private float SliderLabel(float sliderValue, float sliderMinValue, float sliderMaxValue)
		{
			GUILayout.BeginHorizontal();
			sliderValue = GUILayout.HorizontalSlider(sliderValue, sliderMinValue, sliderMaxValue);
			GUILayout.Space(10f);
			GUILayout.Label($"{sliderValue:F2}", GUILayout.ExpandWidth(expand: false));
			GUILayout.EndHorizontal();
			return sliderValue;
		}
	}
}
