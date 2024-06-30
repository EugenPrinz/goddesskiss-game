using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace RoomDecorator.Model
{
	public class CharacterParts : MonoBehaviour
	{
		private SkeletonAnimation _skeletonAnimation;

		private Renderer[] _renderers;

		private Dictionary<string, GameObject> _bones;

		private int _sortingOrder;

		private string _sortingLayerName;

		public int layer
		{
			set
			{
				for (int i = 0; i < _renderers.Length; i++)
				{
					_renderers[i].gameObject.layer = value;
				}
			}
		}

		public int sortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				int num = _sortingOrder;
				_sortingOrder = value;
				for (int i = 0; i < _renderers.Length; i++)
				{
					_renderers[i].sortingOrder += _sortingOrder - num;
				}
			}
		}

		public string sortingLayerName
		{
			get
			{
				return _sortingLayerName;
			}
			set
			{
				_sortingLayerName = value;
				for (int i = 0; i < _renderers.Length; i++)
				{
					_renderers[i].sortingLayerName = _sortingLayerName;
				}
			}
		}

		public string AnimationName
		{
			get
			{
				return _skeletonAnimation.AnimationName;
			}
			set
			{
				_skeletonAnimation.AnimationName = value;
			}
		}

		public bool AnimationLoop
		{
			get
			{
				return _skeletonAnimation.loop;
			}
			set
			{
				_skeletonAnimation.loop = value;
			}
		}

		private void Awake()
		{
			_bones = new Dictionary<string, GameObject>();
			_skeletonAnimation = GetComponent<SkeletonAnimation>();
			_renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
			BoneFollower componentInChildren = GetComponentInChildren<BoneFollower>();
			if (componentInChildren != null)
			{
				_bones.Add(componentInChildren.name, componentInChildren.gameObject);
			}
		}

		private void OnDestroy()
		{
			AssetBundleManager.DeleteAssetBundle(base.name + ".assetbundle");
		}

		public GameObject GetBone(string name)
		{
			if (!_bones.ContainsKey(name))
			{
				return null;
			}
			return _bones[name];
		}

		public void Init(int layer, string sortingLayerName, int sortingOrder)
		{
			base.gameObject.layer = layer;
			_sortingLayerName = sortingLayerName;
			int num = _sortingOrder;
			_sortingOrder = sortingOrder;
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].gameObject.layer = layer;
				_renderers[i].sortingLayerName = _sortingLayerName;
				_renderers[i].sortingOrder += _sortingOrder - num;
			}
		}

		public void SetToSetupPose()
		{
			_skeletonAnimation.skeleton.SetToSetupPose();
		}
	}
}
