namespace Spine
{
	public class TranslateTimeline : CurveTimeline
	{
		protected const int PREV_FRAME_TIME = -3;

		protected const int FRAME_X = 1;

		protected const int FRAME_Y = 2;

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

		public TranslateTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
		}

		public void SetFrame(int frameIndex, float time, float x, float y)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = x;
			frames[frameIndex + 2] = y;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
		{
			float[] array = frames;
			if (!(time < array[0]))
			{
				Bone bone = skeleton.bones.Items[boneIndex];
				if (time >= array[array.Length - 3])
				{
					bone.x += (bone.data.x + array[array.Length - 2] - bone.x) * alpha;
					bone.y += (bone.data.y + array[array.Length - 1] - bone.y) * alpha;
					return;
				}
				int num = Animation.binarySearch(array, time, 3);
				float num2 = array[num - 2];
				float num3 = array[num - 1];
				float num4 = array[num];
				float num5 = 1f - (time - num4) / (array[num + -3] - num4);
				num5 = GetCurvePercent(num / 3 - 1, (num5 < 0f) ? 0f : ((!(num5 > 1f)) ? num5 : 1f));
				bone.x += (bone.data.x + num2 + (array[num + 1] - num2) * num5 - bone.x) * alpha;
				bone.y += (bone.data.y + num3 + (array[num + 2] - num3) * num5 - bone.y) * alpha;
			}
		}
	}
}
