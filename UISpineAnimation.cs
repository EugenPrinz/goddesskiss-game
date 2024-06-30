using Cache;
using Spine.Unity;
using UnityEngine;

[ExecuteInEditMode]
public class UISpineAnimation : UIWidget
{
	public GameObject target;

	public bool clipping;

	public bool usePanelClip;

	public Rect clipRange;

	public Vector2 softness;

	public Vector3 meshPositionOffset = Vector3.zero;

	public UIPanel _panel;

	private string _spinePrefabName;

	private string _spinePrefabName_ForKissAnim;

	public string spinePrefabName
	{
		get
		{
			return _spinePrefabName;
		}
		set
		{
			if (!(_spinePrefabName == value))
			{
				_spinePrefabName = value;
				_ChangeSpineData(value);
			}
		}
	}

	public string spinePrefabName_ForKissAnim
	{
		get
		{
			return _spinePrefabName_ForKissAnim;
		}
		set
		{
			if (!(_spinePrefabName_ForKissAnim == value))
			{
				_spinePrefabName_ForKissAnim = value;
				_ChangeSpineData_OnlyNewKissAnim(value);
			}
		}
	}

	public GameObject TartgetParent { get; set; }

	public SkeletonAnimation skeletonAnimation
	{
		get
		{
			if (target != null)
			{
				return target.GetComponent<SkeletonAnimation>();
			}
			return null;
		}
	}

	public MeshFilter meshFilter
	{
		get
		{
			if (target != null)
			{
				return target.GetComponent<MeshFilter>();
			}
			return null;
		}
	}

	public MeshRenderer targetRenderer
	{
		get
		{
			if (target != null)
			{
				return target.GetComponent<MeshRenderer>();
			}
			return null;
		}
	}

	public override Material material
	{
		get
		{
			if (targetRenderer != null)
			{
				if (Application.isPlaying)
				{
					return targetRenderer.sharedMaterial;
				}
				return targetRenderer.sharedMaterial;
			}
			return null;
		}
		set
		{
			if (!(targetRenderer == null))
			{
				targetRenderer.material = value;
			}
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y, 0f));
		verts.Add(new Vector3(vector.x, vector.w, 0f));
		verts.Add(new Vector3(vector.z, vector.y, 0f));
		verts.Add(new Vector3(vector.z, vector.w, 0f));
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.zero);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		cols.Add(Color.clear);
		if (onPostFill != null)
		{
			onPostFill(this, verts.size, verts, uvs, cols);
		}
	}

	private void _ChangeSpineData(string name)
	{
		if (target != null)
		{
			Object.DestroyImmediate(target);
			target = null;
			Resources.UnloadUnusedAssets();
		}
		SpineCache spineCache = null;
		spineCache = ((!Application.isPlaying) ? Object.FindObjectOfType<SpineCache>() : CacheManager.instance.SpineCache);
		if (spineCache == null)
		{
		}
		SkeletonAnimation skeletonAnimation = spineCache.Create<SkeletonAnimation>(name, base.transform);
		if (!(skeletonAnimation == null))
		{
			target = skeletonAnimation.gameObject;
			SetAnimation("a_01_idle1");
		}
	}

	public void _ChangeSpineData_OnlyNewKissAnim(string name)
	{
		if (target != null)
		{
			Object.DestroyImmediate(target);
			target = null;
			Resources.UnloadUnusedAssets();
		}
		string path = "Prefabs/SpineData/" + name + "/" + name + "_sk";
		target = Object.Instantiate(Resources.Load(path)) as GameObject;
		target.transform.parent = TartgetParent.transform;
	}

	public void SetSkin(string skinName)
	{
		if (skeletonAnimation != null && skeletonAnimation.skeleton.data.FindSkin(skinName) != null)
		{
			skeletonAnimation.skeleton.SetSkin(skinName);
			skeletonAnimation.skeleton.SetSlotsToSetupPose();
		}
	}

	public void SetAnimation(string aniName)
	{
		skeletonAnimation.AnimationName = aniName;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (usePanelClip)
		{
			_FindClipPanel();
		}
	}

	private void _FindClipPanel()
	{
		Transform transform = base.transform.parent;
		while (transform != null)
		{
			UIPanel component = transform.GetComponent<UIPanel>();
			if (component != null && component.clipping == UIDrawCall.Clipping.SoftClip)
			{
				_panel = component;
				break;
			}
			transform = transform.parent;
		}
	}

	public void UpdateMaterial()
	{
		Material[] sharedMaterials = targetRenderer.sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			if (!(sharedMaterials[i] == null))
			{
				Material material = new Material(sharedMaterials[i]);
				if (clipping)
				{
					_UpdateClipValue(material);
				}
				else
				{
					material.SetVector("_ClipRange0", new Vector4(0f, 0f, 2000f, 2000f));
					material.SetColor("_Color", base.color);
				}
				if (drawCall != null)
				{
					material.renderQueue = drawCall.finalRenderQueue;
				}
				sharedMaterials[i] = material;
			}
		}
		targetRenderer.sharedMaterials = sharedMaterials;
	}

	private void _UpdateClipValue(Material mat)
	{
		if (mat == null)
		{
			return;
		}
		Vector4 value = new Vector4(clipRange.x, clipRange.y, clipRange.width, clipRange.height);
		Vector4 vector = new Vector4(softness.x, softness.y);
		if (usePanelClip)
		{
			if (_panel == null)
			{
				_FindClipPanel();
			}
			if (_panel != null)
			{
				Vector3 vector2 = base.cachedTransform.InverseTransformPoint(_panel.cachedTransform.position);
				vector.x = _panel.clipSoftness.x;
				vector.y = _panel.clipSoftness.y;
				value = _panel.drawCallClipRange;
				float num = _panel.cachedTransform.lossyScale.x / base.transform.lossyScale.x;
				float num2 = _panel.cachedTransform.lossyScale.y / base.transform.lossyScale.y;
				value.z *= num;
				value.w *= num2;
				value.x += vector2.x;
				value.y += vector2.y;
				value.x -= target.transform.localPosition.x * num;
				value.y -= target.transform.localPosition.y * num2;
				clipRange.x = value.x;
				clipRange.y = value.y;
				clipRange.width = value.z;
				clipRange.height = value.w;
				softness = _panel.clipSoftness;
			}
		}
		else
		{
			_panel = null;
			value.x -= target.transform.localPosition.x;
			value.y -= target.transform.localPosition.y;
		}
		if (vector.x > 0f)
		{
			vector.x = value.z / vector.x;
		}
		else
		{
			vector.x = 0f;
		}
		if (vector.y > 0f)
		{
			vector.y = value.w / vector.y;
		}
		else
		{
			vector.y = 0f;
		}
		mat.SetVector("_ClipRange0", value);
		mat.SetColor("_Color", base.color);
	}

	private void LateUpdate()
	{
		if (!(targetRenderer == null))
		{
			UpdateMaterial();
		}
	}

	public override void MakePixelPerfect()
	{
		if (meshFilter == null)
		{
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		Mesh sharedMesh = meshFilter.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			if (vector.x < num)
			{
				num = vector.x;
			}
			else if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.y < num3)
			{
				num3 = vector.y;
			}
			else if (vector.y > num4)
			{
				num4 = vector.y;
			}
		}
		base.width = Mathf.RoundToInt(num2 - num);
		base.height = Mathf.RoundToInt(num4 - num3);
	}
}
