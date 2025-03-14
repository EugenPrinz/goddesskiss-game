namespace Spine
{
	public class RotateTimeline : CurveTimeline
	{
		protected const int PREV_FRAME_TIME = -2;

		protected const int FRAME_VALUE = 1;

		internal int boneIndex;

		internal float[] frames;

		public int BoneIndex
		{
			get
			{
				return boneIndex;
			}
			set
			{
				boneIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public RotateTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount << 1];
		}

		public void SetFrame(int frameIndex, float time, float angle)
		{
			frameIndex *= 2;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = angle;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
		{
			float[] array = frames;
			if (time < array[0])
			{
				return;
			}
			Bone bone = skeleton.bones.Items[boneIndex];
			float num;
			if (time >= array[array.Length - 2])
			{
				for (num = bone.data.rotation + array[array.Length - 1] - bone.rotation; num > 180f; num -= 360f)
				{
				}
				for (; num < -180f; num += 360f)
				{
				}
				bone.rotation += num * alpha;
				return;
			}
			int num2 = Animation.binarySearch(array, time, 2);
			float num3 = array[num2 - 1];
			float num4 = array[num2];
			float num5 = 1f - (time - num4) / (array[num2 + -2] - num4);
			num5 = GetCurvePercent((num2 >> 1) - 1, (num5 < 0f) ? 0f : ((!(num5 > 1f)) ? num5 : 1f));
			for (num = array[num2 + 1] - num3; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			for (num = bone.data.rotation + (num3 + num * num5) - bone.rotation; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			bone.rotation += num * alpha;
		}
	}
}
