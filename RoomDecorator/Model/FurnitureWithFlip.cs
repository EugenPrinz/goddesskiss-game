using System.Collections.Generic;
using RoomDecorator.Data;
using Shared.Regulation;
using Spine.Unity;
using UnityEngine;

namespace RoomDecorator.Model
{
	public class FurnitureWithFlip : Furniture
	{
		public string attachedAnimationName;

		public List<GameObject> attachObjects = new List<GameObject>();

		private SpriteRenderer[] _sprite;

		private SkeletonAnimation[] _skeletonAnimation;

		private AttachCharacterPoint[] _attachPoints;

		private int _attachCount;

		public override int sortingOrder
		{
			get
			{
				return base.sortingOrder;
			}
			set
			{
				int num = _sortingOrder;
				base.sortingOrder = value;
				for (int i = 0; i < _attachPoints.Length; i++)
				{
					_attachPoints[i].sortingOrder += _sortingOrder - num;
				}
			}
		}

		private void Awake()
		{
			_renderers = base.transform.GetComponentsInChildren<Renderer>(includeInactive: true);
			_sprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			_skeletonAnimation = GetComponentsInChildren<SkeletonAnimation>(includeInactive: true);
			_attachPoints = GetComponentsInChildren<AttachCharacterPoint>(includeInactive: true);
		}

		public override void Rotate()
		{
			Rotate((Direction)((int)(direction + 1) % 2));
		}

		public override void Rotate(Direction dir)
		{
			if (Mathf.Abs(dir - direction) % 2 == 1)
			{
				int num = width;
				width = length;
				length = num;
			}
			direction = dir;
			base.transform.localScale = new Vector3((direction == Direction.South) ? 1 : (-1), 1f, 1f);
		}

		public override void SetColor(Color color)
		{
			for (int i = 0; i < _sprite.Length; i++)
			{
				_sprite[i].color = color;
			}
			for (int j = 0; j < _skeletonAnimation.Length; j++)
			{
				if (_skeletonAnimation[j].isActiveAndEnabled)
				{
					_skeletonAnimation[j].skeleton.SetColor(color);
				}
			}
		}

		public override bool CanAttachCharacter(RoCharacter data)
		{
			if (_attachPoints.Length == 0)
			{
				return false;
			}
			string decoIdx = base.data.decoIdx;
			if (!string.IsNullOrEmpty(decoIdx) && decoIdx != "0")
			{
				string decoIdx2 = ((DormitoryBodyCostumeDataRow)data.body.data).decoIdx;
				if (decoIdx != decoIdx2)
				{
					return false;
				}
			}
			return true;
		}

		public override AttachCharacterPoint GetAttachAblePoint()
		{
			for (int i = 0; i < _attachPoints.Length; i++)
			{
				if (!_attachPoints[i].isAttached)
				{
					return _attachPoints[i];
				}
			}
			return null;
		}

		public override void AttachCharacter(AttachCharacterPoint point, Character character)
		{
			if (point.isAttached)
			{
				return;
			}
			_attachCount++;
			point.Attach(this, character);
			if (_attachCount != 1)
			{
				return;
			}
			for (int i = 0; i < attachObjects.Count; i++)
			{
				if (!(attachObjects[i] == null))
				{
					attachObjects[i].SetActive(value: true);
				}
			}
			StartAnimation(attachedAnimationName);
		}

		public override bool AttachCharacter(Character character)
		{
			AttachCharacterPoint attachAblePoint = GetAttachAblePoint();
			if (attachAblePoint == null)
			{
				return false;
			}
			AttachCharacter(attachAblePoint, character);
			return true;
		}

		public override void DetachCharacter(Character character)
		{
			for (int i = 0; i < _attachPoints.Length; i++)
			{
				if (!_attachPoints[i].isAttached || !_attachPoints[i].IsAttached(character))
				{
					continue;
				}
				_attachCount--;
				_attachPoints[i].Detach();
				if (_attachCount != 0)
				{
					break;
				}
				for (int j = 0; j < attachObjects.Count; j++)
				{
					if (!(attachObjects[j] == null))
					{
						attachObjects[j].SetActive(value: false);
					}
				}
				StartAnimation(null);
				break;
			}
		}

		protected override void DetachAllCharacter()
		{
			for (int i = 0; i < _attachPoints.Length; i++)
			{
				_attachPoints[i].Detach();
			}
			if (_attachCount > 0)
			{
				for (int j = 0; j < attachObjects.Count; j++)
				{
					if (!(attachObjects[j] == null))
					{
						attachObjects[j].SetActive(value: false);
					}
				}
				StartAnimation(null);
			}
			_attachCount = 0;
		}

		private void StartAnimation(string name)
		{
			if (string.IsNullOrEmpty(attachedAnimationName))
			{
				return;
			}
			for (int i = 0; i < _skeletonAnimation.Length; i++)
			{
				if (_skeletonAnimation[i].isActiveAndEnabled)
				{
					_skeletonAnimation[i].loop = true;
					_skeletonAnimation[i].AnimationName = name;
					_skeletonAnimation[i].skeleton.SetToSetupPose();
				}
			}
		}
	}
}
