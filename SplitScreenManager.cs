using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplitScreenManager : MonoBehaviour
{
	[Serializable]
	public class CutData
	{
		public Camera camera;

		public Transform cameraView;

		public GameObject targetRoot;

		public GameObject black;

		public TerrainScroller terrainScroller;

		public Renderer target;

		public Transform ClipLine;

		public SplitScreenDrawSide drawSide = SplitScreenDrawSide.Left;

		public Mesh originalMesh;

		public Transform ui;

		public Transform troopAnchor;

		[HideInInspector]
		public Transform subCamera;

		[HideInInspector]
		public Vector3 subCameraOffset;

		[HideInInspector]
		public bool invers;

		[HideInInspector]
		public MeshFilter mf;

		[HideInInspector]
		public Mesh clippedMesh;

		[HideInInspector]
		public Vector3[] cachedVerts;

		[HideInInspector]
		public int[] cachedTris;

		[HideInInspector]
		public Material material;

		public Texture2D maskOrginal;

		[HideInInspector]
		public Texture2D maskDynamic;

		[HideInInspector]
		public Color[] maskTexBuff;

		private Transform tfCamera;

		public void Init()
		{
			material = target.sharedMaterial;
			InitVertexClipping();
			tfCamera = camera.transform;
		}

		public void InitViewSize(int w, int h, float orthographicSize = 4.5f)
		{
			RenderTexture renderTexture = new RenderTexture(w, h, 16);
			material.mainTexture = renderTexture;
			camera.SetTargetBuffers(renderTexture.colorBuffer, renderTexture.depthBuffer);
		}

		public void InitVertexClipping()
		{
			mf = target.GetComponent<MeshFilter>();
			clippedMesh = new Mesh();
			clippedMesh.vertices = originalMesh.vertices;
			clippedMesh.uv = originalMesh.uv;
			clippedMesh.triangles = originalMesh.triangles;
			cachedVerts = clippedMesh.vertices;
			cachedTris = clippedMesh.triangles;
			mf.sharedMesh = clippedMesh;
		}

		public void CreateMask()
		{
			int num = 512;
			int num2 = 512;
			maskDynamic = new Texture2D(num, num2);
			maskTexBuff = new Color[num * num2];
			Color color = new Color(1f, 1f, 1f, 1f);
			for (int i = 0; i < maskTexBuff.Length; i++)
			{
				maskTexBuff[i] = color;
			}
			maskDynamic.SetPixels(maskTexBuff);
			maskDynamic.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		}

		public void RegisterCamera(Transform tfCamera, Vector3 offset, bool invers = false)
		{
			subCamera = tfCamera;
			subCameraOffset = offset;
			this.invers = invers;
		}

		public void UnRegisterCamera(Transform tfCamera)
		{
			if (subCamera == tfCamera)
			{
				subCamera = null;
				subCameraOffset = Vector3.zero;
				tfCamera.localPosition = Vector3.zero;
				tfCamera.localRotation = Quaternion.identity;
				invers = false;
			}
		}

		public void UpdateCamera()
		{
			if (subCamera == null)
			{
				tfCamera.localPosition = Vector3.zero;
				tfCamera.localRotation = Quaternion.identity;
				return;
			}
			Vector3 localPosition = subCamera.localPosition;
			if (invers)
			{
				localPosition.Scale(subCamera.localScale);
			}
			tfCamera.localPosition = localPosition + subCameraOffset;
			tfCamera.localRotation = subCamera.localRotation;
		}
	}

	[Serializable]
	public class PickingResult
	{
		public CutData cut;

		public GameObject target;

		public Vector3 hitPos;

		public SplitScreenDrawSide drawSide => (cut != null) ? cut.drawSide : SplitScreenDrawSide.Unknown;

		private PickingResult()
		{
		}

		public static PickingResult Create(CutData cut, GameObject go, Vector3 pos)
		{
			PickingResult pickingResult = new PickingResult();
			pickingResult.cut = cut;
			pickingResult.target = go;
			pickingResult.hitPos = pos;
			return pickingResult;
		}

		public override string ToString()
		{
			return string.Format("Cut = {0}, target = {1}", drawSide, (!(target == null)) ? target.name : "NULL");
		}
	}

	private static SplitScreenManager _singleton;

	public Camera mergeTargetCamera;

	public CutData left;

	public CutData right;

	public Transform splitLine;

	protected Transform subSplitLine;

	private List<int> _newTriList = new List<int>();

	public static SplitScreenManager instance => _singleton;

	private void Awake()
	{
		if (_singleton == null)
		{
			_singleton = this;
		}
		if (_singleton != this)
		{
			UnityEngine.Object.DestroyImmediate(this);
		}
	}

	private void OnDestroy()
	{
		if (left != null)
		{
			left = null;
		}
		if (right != null)
		{
			right = null;
		}
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	private void Start()
	{
		left.Init();
		right.Init();
		Resize(1280f, 720f);
	}

	public void ResizeBySlider(UISlider slider)
	{
		float value = slider.value;
		Resize(1280f * value, 720f * value);
	}

	public void Resize(float w, float h)
	{
		left.InitViewSize((int)w, (int)h, mergeTargetCamera.orthographicSize);
		right.InitViewSize((int)w, (int)h, mergeTargetCamera.orthographicSize);
	}

	private void Update()
	{
		UpdateCamera();
		UpdateSpline();
		SetSplitLineInfo(left);
		SetSplitLineInfo(right);
	}

	public void RegisterSpline(Transform tfSpline, bool invers = false)
	{
		subSplitLine = tfSpline;
	}

	public void UnRegisterSpline(Transform tfSpline)
	{
		if (subSplitLine == tfSpline)
		{
			subSplitLine = null;
			splitLine.localPosition = Vector3.zero;
		}
	}

	protected void UpdateSpline()
	{
		if (!(subSplitLine == null))
		{
			splitLine.localPosition = subSplitLine.localPosition;
		}
	}

	protected void UpdateCamera()
	{
		left.UpdateCamera();
		right.UpdateCamera();
	}

	public PickingResult PickObject(Vector3 screenPos, int layer = -1)
	{
		Camera camera = null;
		CutData cutData = null;
		if (IsInCut(left, screenPos))
		{
			left.target.GetComponent<Collider>().enabled = true;
			right.target.GetComponent<Collider>().enabled = false;
			camera = left.camera;
			cutData = left;
		}
		else
		{
			if (!IsInCut(right, screenPos))
			{
				return null;
			}
			left.target.GetComponent<Collider>().enabled = false;
			right.target.GetComponent<Collider>().enabled = true;
			camera = right.camera;
			cutData = right;
		}
		Ray ray = mergeTargetCamera.ScreenPointToRay(screenPos);
		if (Physics.Raycast(ray, out var hitInfo))
		{
			Vector2 textureCoord = hitInfo.textureCoord;
			ray = camera.ViewportPointToRay(textureCoord);
			if (Physics.Raycast(ray, out hitInfo))
			{
				return PickingResult.Create(cutData, hitInfo.collider.gameObject, hitInfo.point);
			}
		}
		return null;
	}

	public Vector3 ConvertPosCutToMesh(SplitScreenDrawSide side, Vector3 cutPos)
	{
		CutData cut = ((side != SplitScreenDrawSide.Left) ? right : left);
		return ConvertPosCutToMesh(cut, cutPos);
	}

	public Vector3 ConvertPosCutToMesh(CutData cut, Vector3 cutPos)
	{
		Camera camera = cut.camera;
		Vector3 result = camera.WorldToViewportPoint(cutPos);
		result.z = 0f;
		return result;
	}

	public Vector3 ConvertPosCutToMerge(SplitScreenDrawSide side, Vector3 cutPos)
	{
		CutData cut = ((side != SplitScreenDrawSide.Left) ? right : left);
		return ConvertPosCutToMerge(cut, cutPos);
	}

	public Vector3 ConvertPosCutToMerge(CutData cut, Vector3 cutPos)
	{
		Camera camera = cut.camera;
		Transform transform = cut.target.transform;
		Vector3 vector = camera.WorldToViewportPoint(cutPos);
		vector.x -= 0.5f;
		vector.y -= 0.5f;
		Vector3 lossyScale = transform.lossyScale;
		lossyScale.x *= vector.x;
		lossyScale.y *= vector.y;
		lossyScale.z = transform.localPosition.z;
		lossyScale = transform.rotation * lossyScale;
		return transform.position + lossyScale;
	}

	public Vector3 ConvertPosCutToUI(SplitScreenDrawSide side, Vector3 cutPos)
	{
		CutData cut = ((side != SplitScreenDrawSide.Left) ? right : left);
		return ConvertPosCutToUI(cut, cutPos);
	}

	public Vector3 ConvertPosCutToUI(CutData cut, Vector3 cutPos)
	{
		Vector3 position = ConvertPosCutToMerge(cut, cutPos);
		position = mergeTargetCamera.WorldToScreenPoint(position);
		position.z = 0f;
		return UICamera.mainCamera.ScreenToWorldPoint(position);
	}

	private void SetSplitLineInfo(CutData cut)
	{
		if (cut == null || cut.ClipLine == null || cut.drawSide == SplitScreenDrawSide.Unknown || (!cut.ClipLine.transform.hasChanged && !cut.target.transform.hasChanged))
		{
			return;
		}
		cut.ClipLine.transform.hasChanged = false;
		cut.target.transform.hasChanged = false;
		Transform clipLine = cut.ClipLine;
		float num = (float)cut.drawSide;
		Material material = cut.material;
		Vector3 vector = mergeTargetCamera.WorldToViewportPoint(clipLine.position);
		Vector3 vector2 = mergeTargetCamera.WorldToViewportPoint(clipLine.position + clipLine.up);
		float num2 = (vector2.y - vector.y) / (vector2.x - vector.x);
		if (float.IsInfinity(num2))
		{
			num2 = float.MaxValue;
		}
		float num3 = (0f - num2) * vector.x + vector.y;
		if (num2 > 0f)
		{
			num *= -1f;
		}
		material.SetFloat("_LineDelta", num2);
		material.SetFloat("_LineConst", num3);
		material.SetFloat("_Side", num);
		Vector3[] cachedVerts = cut.cachedVerts;
		int[] cachedTris = cut.cachedTris;
		_newTriList.Clear();
		Transform transform = cut.target.transform;
		Vector3[] array = new Vector3[3];
		for (int i = 0; i < cachedTris.Length; i += 3)
		{
			ref Vector3 reference = ref array[0];
			reference = cachedVerts[cachedTris[i]];
			ref Vector3 reference2 = ref array[1];
			reference2 = cachedVerts[cachedTris[i + 1]];
			ref Vector3 reference3 = ref array[2];
			reference3 = cachedVerts[cachedTris[i + 2]];
			bool flag = false;
			for (int j = 0; j < 3; j++)
			{
				Vector4 vector3 = transform.localToWorldMatrix * array[j];
				Vector3 vector4 = mergeTargetCamera.WorldToViewportPoint(new Vector3(vector3.x, vector3.y, vector3.z) + transform.position);
				float num4 = (num2 * vector4.x + num3 - vector4.y) * num;
				if (num4 <= 0f)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				_newTriList.Add(cachedTris[i]);
				_newTriList.Add(cachedTris[i + 1]);
				_newTriList.Add(cachedTris[i + 2]);
			}
		}
		cut.clippedMesh.triangles = _newTriList.ToArray();
	}

	public bool IsInCut(CutData cut, Vector3 screenPos)
	{
		Vector3 vector = mergeTargetCamera.ScreenToViewportPoint(screenPos);
		Transform clipLine = cut.ClipLine;
		Vector3 vector2 = mergeTargetCamera.WorldToViewportPoint(clipLine.position);
		Vector3 vector3 = mergeTargetCamera.WorldToViewportPoint(clipLine.position + clipLine.up);
		float num = (vector3.y - vector2.y) / (vector3.x - vector2.x);
		float num2 = num * (vector.x - vector2.x) - vector.y + vector2.y;
		if (cut.drawSide == SplitScreenDrawSide.Left)
		{
			return num2 < 0f != num < 0f;
		}
		if (cut.drawSide == SplitScreenDrawSide.Right)
		{
			return num2 < 0f == num < 0f;
		}
		return false;
	}

	public CutData GetCutData(SplitScreenDrawSide side)
	{
		return side switch
		{
			SplitScreenDrawSide.Left => left, 
			SplitScreenDrawSide.Right => right, 
			_ => null, 
		};
	}

	public CutData GetOtherSideCutData(SplitScreenDrawSide side)
	{
		return side switch
		{
			SplitScreenDrawSide.Left => right, 
			SplitScreenDrawSide.Right => left, 
			_ => null, 
		};
	}

	public void ShakeScreen(SplitScreenDrawSide side, float time = 1f)
	{
	}

	public void SetMask(SplitScreenDrawSide side, Vector2[] viewPosList, float halfSize = 1.5f)
	{
		if (side == SplitScreenDrawSide.Unknown || viewPosList == null || viewPosList.Length <= 0)
		{
			left.material.SetTexture("_MaskTex", left.maskOrginal);
			right.material.SetTexture("_MaskTex", right.maskOrginal);
			return;
		}
		CutData otherSideCutData = GetOtherSideCutData(side);
		otherSideCutData.material.SetTexture("_MaskTex", otherSideCutData.maskOrginal);
		otherSideCutData = GetCutData(side);
		Color[] maskTexBuff = otherSideCutData.maskTexBuff;
		Texture2D maskDynamic = otherSideCutData.maskDynamic;
		int width = maskDynamic.width;
		int height = maskDynamic.height;
		for (int i = 0; i < viewPosList.Length; i++)
		{
			viewPosList[i].x *= width;
			viewPosList[i].y *= height;
		}
		Vector2 zero = Vector2.zero;
		float num = 0.4f;
		float num2 = 1f - num;
		halfSize = halfSize / otherSideCutData.target.transform.lossyScale.x * (float)width;
		float num3 = halfSize * halfSize;
		float num4 = 1f / num3;
		float num5 = 0f;
		float num6 = 0f;
		for (int j = 0; j < width; j++)
		{
			zero.x = j;
			for (int k = 0; k < height; k++)
			{
				zero.y = k;
				num6 = num;
				for (int l = 0; l < viewPosList.Length; l++)
				{
					num5 = Vector2.SqrMagnitude(zero - viewPosList[l]);
					if (num5 <= num3)
					{
						num6 = Mathf.Max(num6, (num3 - num5) * num4 * num2 + num);
					}
				}
				maskTexBuff[width * k + j].a = num6;
			}
		}
		maskDynamic.SetPixels(maskTexBuff);
		maskDynamic.Apply();
		otherSideCutData.material.SetTexture("_MaskTex", maskDynamic);
	}
}
