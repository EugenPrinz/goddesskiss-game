using System;

namespace Spine
{
	public class Skeleton
	{
		internal SkeletonData data;

		internal ExposedList<Bone> bones;

		internal ExposedList<Slot> slots;

		internal ExposedList<Slot> drawOrder;

		internal ExposedList<IkConstraint> ikConstraints;

		internal ExposedList<TransformConstraint> transformConstraints;

		private ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();

		internal Skin skin;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal float time;

		internal bool flipX;

		internal bool flipY;

		internal float x;

		internal float y;

		public SkeletonData Data => data;

		public ExposedList<Bone> Bones => bones;

		public ExposedList<Slot> Slots => slots;

		public ExposedList<Slot> DrawOrder => drawOrder;

		public ExposedList<IkConstraint> IkConstraints
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

		public Skin Skin
		{
			get
			{
				return skin;
			}
			set
			{
				skin = value;
			}
		}

		public float R
		{
			get
			{
				return r;
			}
			set
			{
				r = value;
			}
		}

		public float G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public float B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		public float A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		public float Time
		{
			get
			{
				return time;
			}
			set
			{
				time = value;
			}
		}

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public bool FlipX
		{
			get
			{
				return flipX;
			}
			set
			{
				flipX = value;
			}
		}

		public bool FlipY
		{
			get
			{
				return flipY;
			}
			set
			{
				flipY = value;
			}
		}

		public Bone RootBone => (bones.Count != 0) ? bones.Items[0] : null;

		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			this.data = data;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData bone3 in data.bones)
			{
				Bone bone = ((bone3.parent != null) ? bones.Items[data.bones.IndexOf(bone3.parent)] : null);
				Bone item = new Bone(bone3, this, bone);
				bone?.children.Add(item);
				bones.Add(item);
			}
			slots = new ExposedList<Slot>(data.slots.Count);
			drawOrder = new ExposedList<Slot>(data.slots.Count);
			foreach (SlotData slot in data.slots)
			{
				Bone bone2 = bones.Items[data.bones.IndexOf(slot.boneData)];
				Slot item2 = new Slot(slot, bone2);
				slots.Add(item2);
				drawOrder.Add(item2);
			}
			ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
			foreach (IkConstraintData ikConstraint in data.ikConstraints)
			{
				ikConstraints.Add(new IkConstraint(ikConstraint, this));
			}
			transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
			foreach (TransformConstraintData transformConstraint in data.transformConstraints)
			{
				transformConstraints.Add(new TransformConstraint(transformConstraint, this));
			}
			UpdateCache();
			UpdateWorldTransform();
		}

		public void UpdateCache()
		{
			ExposedList<Bone> exposedList = bones;
			ExposedList<IUpdatable> exposedList2 = updateCache;
			ExposedList<IkConstraint> exposedList3 = ikConstraints;
			ExposedList<TransformConstraint> exposedList4 = transformConstraints;
			int count = exposedList3.Count;
			int count2 = exposedList4.Count;
			exposedList2.Clear();
			int i = 0;
			for (int count3 = exposedList.Count; i < count3; i++)
			{
				Bone bone = exposedList.Items[i];
				exposedList2.Add(bone);
				for (int j = 0; j < count; j++)
				{
					IkConstraint ikConstraint = exposedList3.Items[j];
					if (bone == ikConstraint.bones.Items[ikConstraint.bones.Count - 1])
					{
						exposedList2.Add(ikConstraint);
						break;
					}
				}
			}
			for (int k = 0; k < count2; k++)
			{
				TransformConstraint transformConstraint = exposedList4.Items[k];
				int num = exposedList2.Count - 1;
				while (k >= 0)
				{
					IUpdatable updatable = exposedList2.Items[num];
					if (updatable == transformConstraint.bone || updatable == transformConstraint.target)
					{
						exposedList2.Insert(num + 1, transformConstraint);
						break;
					}
					num--;
				}
			}
		}

		public void UpdateWorldTransform()
		{
			ExposedList<IUpdatable> exposedList = updateCache;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				exposedList.Items[i].Update();
			}
		}

		public void SetToSetupPose()
		{
			SetBonesToSetupPose();
			SetSlotsToSetupPose();
		}

		public void SetBonesToSetupPose()
		{
			ExposedList<Bone> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				exposedList.Items[i].SetToSetupPose();
			}
			ExposedList<IkConstraint> exposedList2 = ikConstraints;
			int j = 0;
			for (int count2 = exposedList2.Count; j < count2; j++)
			{
				IkConstraint ikConstraint = exposedList2.Items[j];
				ikConstraint.bendDirection = ikConstraint.data.bendDirection;
				ikConstraint.mix = ikConstraint.data.mix;
			}
			ExposedList<TransformConstraint> exposedList3 = transformConstraints;
			int k = 0;
			for (int count3 = exposedList3.Count; k < count3; k++)
			{
				TransformConstraint transformConstraint = exposedList3.Items[k];
				transformConstraint.translateMix = transformConstraint.data.translateMix;
				transformConstraint.x = transformConstraint.data.x;
				transformConstraint.y = transformConstraint.data.y;
			}
		}

		public void SetSlotsToSetupPose()
		{
			ExposedList<Slot> exposedList = slots;
			drawOrder.Clear();
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				drawOrder.Add(exposedList.Items[i]);
			}
			int j = 0;
			for (int count2 = exposedList.Count; j < count2; j++)
			{
				exposedList.Items[j].SetToSetupPose(j);
			}
		}

		public Bone FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			ExposedList<Bone> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone bone = exposedList.Items[i];
				if (bone.data.name == boneName)
				{
					return bone;
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
			ExposedList<Bone> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].data.name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Slot slot = exposedList.Items[i];
				if (slot.data.name == slotName)
				{
					return slot;
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
			ExposedList<Slot> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].data.name.Equals(slotName))
				{
					return i;
				}
			}
			return -1;
		}

		public void SetSkin(string skinName)
		{
			Skin skin = data.FindSkin(skinName);
			if (skin == null)
			{
				throw new ArgumentException("Skin not found: " + skinName);
			}
			SetSkin(skin);
		}

		public void SetSkin(Skin newSkin)
		{
			if (newSkin != null)
			{
				if (skin != null)
				{
					newSkin.AttachAll(this, skin);
				}
				else
				{
					ExposedList<Slot> exposedList = slots;
					int i = 0;
					for (int count = exposedList.Count; i < count; i++)
					{
						Slot slot = exposedList.Items[i];
						string attachmentName = slot.data.attachmentName;
						if (attachmentName != null)
						{
							Attachment attachment = newSkin.GetAttachment(i, attachmentName);
							if (attachment != null)
							{
								slot.Attachment = attachment;
							}
						}
					}
				}
			}
			skin = newSkin;
		}

		public Attachment GetAttachment(string slotName, string attachmentName)
		{
			return GetAttachment(data.FindSlotIndex(slotName), attachmentName);
		}

		public Attachment GetAttachment(int slotIndex, string attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentNullException("attachmentName cannot be null.");
			}
			if (skin != null)
			{
				Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			if (data.defaultSkin != null)
			{
				return data.defaultSkin.GetAttachment(slotIndex, attachmentName);
			}
			return null;
		}

		public void SetAttachment(string slotName, string attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Slot slot = exposedList.Items[i];
				if (!(slot.data.name == slotName))
				{
					continue;
				}
				Attachment attachment = null;
				if (attachmentName != null)
				{
					attachment = GetAttachment(i, attachmentName);
					if (attachment == null)
					{
						throw new Exception("Attachment not found: " + attachmentName + ", for slot: " + slotName);
					}
				}
				slot.Attachment = attachment;
				return;
			}
			throw new Exception("Slot not found: " + slotName);
		}

		public IkConstraint FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName cannot be null.");
			}
			ExposedList<IkConstraint> exposedList = ikConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				IkConstraint ikConstraint = exposedList.Items[i];
				if (ikConstraint.data.name == constraintName)
				{
					return ikConstraint;
				}
			}
			return null;
		}

		public TransformConstraint FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName cannot be null.");
			}
			ExposedList<TransformConstraint> exposedList = transformConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				TransformConstraint transformConstraint = exposedList.Items[i];
				if (transformConstraint.data.name == constraintName)
				{
					return transformConstraint;
				}
			}
			return null;
		}

		public void Update(float delta)
		{
			time += delta;
		}
	}
}
