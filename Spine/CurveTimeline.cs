namespace Spine
{
	public abstract class CurveTimeline : Timeline
	{
		protected const float LINEAR = 0f;

		protected const float STEPPED = 1f;

		protected const float BEZIER = 2f;

		protected const int BEZIER_SEGMENTS = 10;

		protected const int BEZIER_SIZE = 19;

		private float[] curves;

		public int FrameCount => curves.Length / 19 + 1;

		public CurveTimeline(int frameCount)
		{
			curves = new float[(frameCount - 1) * 19];
		}

		public abstract void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha);

		public void SetLinear(int frameIndex)
		{
			curves[frameIndex * 19] = 0f;
		}

		public void SetStepped(int frameIndex)
		{
			curves[frameIndex * 19] = 1f;
		}

		public void SetCurve(int frameIndex, float cx1, float cy1, float cx2, float cy2)
		{
			float num = 0.1f;
			float num2 = num * num;
			float num3 = num2 * num;
			float num4 = 3f * num;
			float num5 = 3f * num2;
			float num6 = 6f * num2;
			float num7 = 6f * num3;
			float num8 = (0f - cx1) * 2f + cx2;
			float num9 = (0f - cy1) * 2f + cy2;
			float num10 = (cx1 - cx2) * 3f + 1f;
			float num11 = (cy1 - cy2) * 3f + 1f;
			float num12 = cx1 * num4 + num8 * num5 + num10 * num3;
			float num13 = cy1 * num4 + num9 * num5 + num11 * num3;
			float num14 = num8 * num6 + num10 * num7;
			float num15 = num9 * num6 + num11 * num7;
			float num16 = num10 * num7;
			float num17 = num11 * num7;
			int i = frameIndex * 19;
			float[] array = curves;
			array[i++] = 2f;
			float num18 = num12;
			float num19 = num13;
			for (int num20 = i + 19 - 1; i < num20; i += 2)
			{
				array[i] = num18;
				array[i + 1] = num19;
				num12 += num14;
				num13 += num15;
				num14 += num16;
				num15 += num17;
				num18 += num12;
				num19 += num13;
			}
		}

		public float GetCurvePercent(int frameIndex, float percent)
		{
			float[] array = curves;
			int num = frameIndex * 19;
			float num2 = array[num];
			if (num2 == 0f)
			{
				return percent;
			}
			if (num2 == 1f)
			{
				return 0f;
			}
			num++;
			float num3 = 0f;
			int num4 = num;
			for (int num5 = num + 19 - 1; num < num5; num += 2)
			{
				num3 = array[num];
				if (num3 >= percent)
				{
					float num6;
					float num7;
					if (num == num4)
					{
						num6 = 0f;
						num7 = 0f;
					}
					else
					{
						num6 = array[num - 2];
						num7 = array[num - 1];
					}
					return num7 + (array[num + 1] - num7) * (percent - num6) / (num3 - num6);
				}
			}
			float num8 = array[num - 1];
			return num8 + (1f - num8) * (percent - num3) / (1f - num3);
		}

		public float GetCurveType(int frameIndex)
		{
			return curves[frameIndex * 19];
		}
	}
}
