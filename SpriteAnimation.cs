using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public sealed class SpriteAnimation : MonoBehaviour
{
	public SpriteSheet spriteSheet;

	public string packageName;

	public float fps = 1f;

	public float frame;

	private string prePackageName;

	private Mesh[] meshes;

	public bool isLoop = true;

	public int sortingOrderOffset;

	private bool _isPlaying = true;

	private float _playDir = 1f;

	public bool isPlaying => _isPlaying;

	private void Start()
	{
	}

	public void ClearMesh()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		component.mesh = null;
		component.sharedMesh = null;
	}

	public void Play(string animationName, bool forward = true, bool loop = true)
	{
		packageName = animationName;
		if (packageName != prePackageName)
		{
			meshes = spriteSheet.GetMeshes(packageName);
			prePackageName = packageName;
		}
		frame = 0f;
		isLoop = loop;
		_playDir = ((!forward) ? (-1f) : 1f);
		_isPlaying = true;
	}

	public void Stop()
	{
		_isPlaying = false;
	}

	private void Update()
	{
		if (!_isPlaying)
		{
			return;
		}
		if (packageName != prePackageName)
		{
			meshes = spriteSheet.GetMeshes(packageName);
			prePackageName = packageName;
		}
		if (meshes == null || meshes.Length == 0)
		{
			return;
		}
		frame += Time.deltaTime * fps;
		if ((int)frame > meshes.Length - 1)
		{
			if (!isLoop)
			{
				_isPlaying = false;
				return;
			}
			frame = 0f;
		}
		int num = ((!(_playDir >= 0f)) ? (meshes.Length - 1 - (int)frame) : ((int)frame));
		MeshFilter component = GetComponent<MeshFilter>();
		component.sharedMesh = meshes[num];
		MeshRenderer component2 = GetComponent<MeshRenderer>();
		int num2 = (int)(base.transform.position.z * -10f) + 3000;
		if (component.sharedMesh != null)
		{
			int length = component.sharedMesh.name.Length;
			switch (component.sharedMesh.name[length - 1])
			{
			case '1':
				num2++;
				break;
			case '2':
				num2 += 2;
				break;
			case '3':
				num2 += 3;
				break;
			case '4':
				num2 += 4;
				break;
			case '5':
				num2 += 5;
				break;
			case '6':
				num2 += 6;
				break;
			case '7':
				num2 += 7;
				break;
			case '8':
				num2 += 8;
				break;
			case '9':
				num2 += 9;
				break;
			}
		}
		component2.sortingOrder = num2 + sortingOrderOffset;
		component2.sharedMaterial = spriteSheet.atlasMaterial;
	}

	private void OnDrawGizmos()
	{
		Update();
	}
}
