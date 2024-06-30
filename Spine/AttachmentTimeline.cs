namespace Spine
{
	public class AttachmentTimeline : Timeline
	{
		internal int slotIndex;

		internal float[] frames;

		private string[] attachmentNames;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				slotIndex = value;
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

		public string[] AttachmentNames
		{
			get
			{
				return attachmentNames;
			}
			set
			{
				attachmentNames = value;
			}
		}

		public int FrameCount => frames.Length;

		public AttachmentTimeline(int frameCount)
		{
			frames = new float[frameCount];
			attachmentNames = new string[frameCount];
		}

		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			frames[frameIndex] = time;
			attachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
		{
			float[] array = frames;
			if (time < array[0])
			{
				if (lastTime > time)
				{
					Apply(skeleton, lastTime, 2.1474836E+09f, null, 0f);
				}
				return;
			}
			if (lastTime > time)
			{
				lastTime = -1f;
			}
			int num = ((!(time >= array[array.Length - 1])) ? Animation.binarySearch(array, time) : array.Length) - 1;
			if (!(array[num] < lastTime))
			{
				string text = attachmentNames[num];
				skeleton.slots.Items[slotIndex].Attachment = ((text != null) ? skeleton.GetAttachment(slotIndex, text) : null);
			}
		}
	}
}
