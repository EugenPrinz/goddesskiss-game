using System;

namespace Spine
{
	public class SkeletonData
	{
		internal string name;

		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal ExposedList<SlotData> slots = new ExposedList<SlotData>();

		internal ExposedList<Skin> skins = new ExposedList<Skin>();

		internal Skin defaultSkin;

		internal ExposedList<EventData> events = new ExposedList<EventData>();

		internal ExposedList<Animation> animations = new ExposedList<Animation>();

		internal ExposedList<IkConstraintData> ikConstraints = new ExposedList<IkConstraintData>();

		internal ExposedList<TransformConstraintData> transformConstraints = new ExposedList<TransformConstraintData>();

		internal float width;

		internal float height;

		internal string version;

		internal string hash;

		internal string imagesPath;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public ExposedList<BoneData> Bones => bones;

		public ExposedList<SlotData> Slots => slots;

		public ExposedList<Skin> Skins
		{
			get
			{
				return skins;
			}
			set
			{
				skins = value;
			}
		}

		public Skin DefaultSkin
		{
			get
			{
				return defaultSkin;
			}
			set
			{
				defaultSkin = value;
			}
		}

		public ExposedList<EventData> Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		public ExposedList<Animation> Animations
		{
			get
			{
				return animations;
			}
			set
			{
				animations = value;
			}
		}

		public ExposedList<IkConstraintData> IkConstraints
		{
			get
			{
				return ikConstraints;
			}
			set
			{
				ikConstraints = value;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public string Hash
		{
			get
			{
				return hash;
			}
			set
			{
				hash = value;
			}
		}

		public BoneData FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			ExposedList<BoneData> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				BoneData boneData = exposedList.Items[i];
				if (boneData.name == boneName)
				{
					return boneData;
				}
			}
			return null;
		}

		public int FindBoneIndex(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			ExposedList<BoneData> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public SlotData FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			ExposedList<SlotData> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				SlotData slotData = exposedList.Items[i];
				if (slotData.name == slotName)
				{
					return slotData;
				}
			}
			return null;
		}

		public int FindSlotIndex(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			ExposedList<SlotData> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].name == slotName)
				{
					return i;
				}
			}
			return -1;
		}

		public Skin FindSkin(string skinName)
		{
			if (skinName == null)
			{
				throw new ArgumentNullException("skinName cannot be null.");
			}
			foreach (Skin skin in skins)
			{
				if (skin.name == skinName)
				{
					return skin;
				}
			}
			return null;
		}

		public EventData FindEvent(string eventDataName)
		{
			if (eventDataName == null)
			{
				throw new ArgumentNullException("eventDataName cannot be null.");
			}
			foreach (EventData @event in events)
			{
				if (@event.name == eventDataName)
				{
					return @event;
				}
			}
			return null;
		}

		public Animation FindAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName cannot be null.");
			}
			ExposedList<Animation> exposedList = animations;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Animation animation = exposedList.Items[i];
				if (animation.name == animationName)
				{
					return animation;
				}
			}
			return null;
		}

		public IkConstraintData FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName cannot be null.");
			}
			ExposedList<IkConstraintData> exposedList = ikConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				IkConstraintData ikConstraintData = exposedList.Items[i];
				if (ikConstraintData.name == constraintName)
				{
					return ikConstraintData;
				}
			}
			return null;
		}

		public TransformConstraintData FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName cannot be null.");
			}
			ExposedList<TransformConstraintData> exposedList = transformConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				TransformConstraintData transformConstraintData = exposedList.Items[i];
				if (transformConstraintData.name == constraintName)
				{
					return transformConstraintData;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return name ?? base.ToString();
		}
	}
}
