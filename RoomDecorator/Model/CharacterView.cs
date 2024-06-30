using UnityEngine;

namespace RoomDecorator.Model
{
	public class CharacterView : MonoBehaviour
	{
		private CharacterParts _head;

		private CharacterParts _body;

		private CharacterParts _accessory;

		private int _sortingOrder;

		private string _sortingLayerName;

		public int layer
		{
			set
			{
				if (base.gameObject.layer != value)
				{
					if (_head != null)
					{
						_head.layer = value;
					}
					if (_body != null)
					{
						_body.layer = value;
					}
					if (_accessory != null)
					{
						_accessory.layer = value;
					}
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
				if (_sortingOrder != value)
				{
					_sortingOrder = value;
					if (_head != null)
					{
						_head.sortingOrder = value;
					}
					if (_body != null)
					{
						_body.sortingOrder = value;
					}
					if (_accessory != null)
					{
						_accessory.sortingOrder = value;
					}
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
				if (!(_sortingLayerName == value))
				{
					_sortingLayerName = value;
					if (_head != null)
					{
						_head.sortingLayerName = value;
					}
					if (_body != null)
					{
						_body.sortingLayerName = value;
					}
					if (_accessory != null)
					{
						_accessory.sortingLayerName = value;
					}
				}
			}
		}

		public void InitHead(string resourceName)
		{
			if (_head != null)
			{
				if (_head.name == resourceName)
				{
					return;
				}
				Object.DestroyImmediate(_head.gameObject);
			}
			GameObject gameObject = null;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceName + ".assetbundle"));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resourceName + ".assetbundle");
			if (assetBundle != null)
			{
				gameObject = assetBundle.LoadAsset(resourceName + ".prefab") as GameObject;
			}
			if (gameObject == null)
			{
				gameObject = (GameObject)Resources.Load("Prefabs/Cache/Dormitory/Characters/" + resourceName, typeof(GameObject));
			}
			_head = Object.Instantiate(gameObject).GetComponent<CharacterParts>();
			_head.name = resourceName;
			_head.Init(base.gameObject.layer, _sortingLayerName, _sortingOrder);
			if (_body != null)
			{
				_head.transform.parent = _body.GetBone("neck").transform;
				_head.transform.localPosition = Vector3.zero;
				_head.transform.localScale = Vector3.one;
				_head.transform.localRotation = Quaternion.identity;
			}
		}

		public void InitAccessory(string resourceName)
		{
			if (_accessory != null)
			{
				if (_accessory.name == resourceName)
				{
					return;
				}
				Object.DestroyImmediate(_accessory.gameObject);
			}
			if (!string.IsNullOrEmpty(resourceName))
			{
				GameObject gameObject = null;
				StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceName + ".assetbundle"));
				AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resourceName + ".assetbundle");
				if (assetBundle != null)
				{
					gameObject = assetBundle.LoadAsset(resourceName + ".prefab") as GameObject;
				}
				if (gameObject == null)
				{
					gameObject = (GameObject)Resources.Load("Prefabs/Cache/Dormitory/Accessories/" + resourceName, typeof(GameObject));
				}
				_accessory = Object.Instantiate(gameObject).GetComponent<CharacterParts>();
				_accessory.name = resourceName;
				_accessory.Init(base.gameObject.layer, _sortingLayerName, _sortingOrder);
				if (_head != null)
				{
					_accessory.transform.parent = _head.GetBone("cap").transform;
					_accessory.transform.localPosition = Vector3.zero;
					_accessory.transform.localScale = Vector3.one;
					_accessory.transform.localRotation = Quaternion.identity;
				}
			}
		}

		public void InitBody(string resourceName)
		{
			if (_body != null)
			{
				if (_body.name == resourceName)
				{
					return;
				}
				if (_head != null)
				{
					_head.transform.parent = base.transform;
				}
				Object.DestroyImmediate(_body.gameObject);
			}
			GameObject gameObject = null;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceName + ".assetbundle"));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resourceName + ".assetbundle");
			if (assetBundle != null)
			{
				gameObject = assetBundle.LoadAsset(resourceName + ".prefab") as GameObject;
			}
			if (gameObject == null)
			{
				gameObject = (GameObject)Resources.Load("Prefabs/Cache/Dormitory/Characters/" + resourceName, typeof(GameObject));
			}
			_body = Object.Instantiate(gameObject).GetComponent<CharacterParts>();
			_body.name = resourceName;
			_body.transform.parent = base.transform;
			_body.transform.localPosition = Vector3.zero;
			_body.transform.localScale = Vector3.one;
			_body.transform.localRotation = Quaternion.identity;
			_body.Init(base.gameObject.layer, _sortingLayerName, _sortingOrder);
			if (_head != null)
			{
				_head.transform.parent = _body.GetBone("neck").transform;
				_head.transform.localPosition = Vector3.zero;
				_head.transform.localScale = Vector3.one;
				_head.transform.localRotation = Quaternion.identity;
			}
		}

		public void HeadAnimation(string animationName, bool loop = false)
		{
			if (!(_head == null))
			{
				_head.AnimationLoop = loop;
				_head.AnimationName = animationName;
			}
		}

		public void BodyAnimation(string animationName, bool loop = false)
		{
			if (!(_body == null))
			{
				_body.AnimationLoop = loop;
				_body.AnimationName = animationName;
			}
		}

		public void SetToSetupPose()
		{
			if (_body != null)
			{
				_body.SetToSetupPose();
			}
			if (_head != null)
			{
				_head.SetToSetupPose();
			}
		}

		public void EnableAccessory(bool enable)
		{
			if (!(_accessory == null))
			{
				_accessory.gameObject.SetActive(enable);
			}
		}
	}
}
