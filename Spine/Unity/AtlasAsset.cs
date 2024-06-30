using System;
using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	public class AtlasAsset : ScriptableObject
	{
		public TextAsset atlasFile;

		public Material[] materials;

		private Atlas atlas;

		public void Reset()
		{
			atlas = null;
		}

		public Atlas GetAtlas()
		{
			if (atlasFile == null)
			{
				Reset();
				return null;
			}
			if (materials == null || materials.Length == 0)
			{
				Reset();
				return null;
			}
			if (atlas != null)
			{
				return atlas;
			}
			try
			{
				atlas = new Atlas(new StringReader(atlasFile.text), string.Empty, new MaterialsTextureLoader(this));
				atlas.FlipV();
				return atlas;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Sprite GenerateSprite(string name, out Material material)
		{
			AtlasRegion atlasRegion = atlas.FindRegion(name);
			Sprite result = null;
			material = null;
			if (atlasRegion != null)
			{
			}
			return result;
		}

		public Mesh GenerateMesh(string name, Mesh mesh, out Material material, float scale = 0.01f)
		{
			AtlasRegion atlasRegion = atlas.FindRegion(name);
			material = null;
			if (atlasRegion != null)
			{
				if (mesh == null)
				{
					mesh = new Mesh();
					mesh.name = name;
				}
				Vector3[] array = new Vector3[4];
				Vector2[] array2 = new Vector2[4];
				Color[] colors = new Color[4]
				{
					Color.white,
					Color.white,
					Color.white,
					Color.white
				};
				int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
				float num = (float)atlasRegion.width / -2f;
				float x = num * -1f;
				float num2 = (float)atlasRegion.height / 2f;
				float y = num2 * -1f;
				ref Vector3 reference = ref array[0];
				reference = new Vector3(num, y, 0f) * scale;
				ref Vector3 reference2 = ref array[1];
				reference2 = new Vector3(num, num2, 0f) * scale;
				ref Vector3 reference3 = ref array[2];
				reference3 = new Vector3(x, num2, 0f) * scale;
				ref Vector3 reference4 = ref array[3];
				reference4 = new Vector3(x, y, 0f) * scale;
				float u = atlasRegion.u;
				float v = atlasRegion.v;
				float u2 = atlasRegion.u2;
				float v2 = atlasRegion.v2;
				if (!atlasRegion.rotate)
				{
					ref Vector2 reference5 = ref array2[0];
					reference5 = new Vector2(u, v2);
					ref Vector2 reference6 = ref array2[1];
					reference6 = new Vector2(u, v);
					ref Vector2 reference7 = ref array2[2];
					reference7 = new Vector2(u2, v);
					ref Vector2 reference8 = ref array2[3];
					reference8 = new Vector2(u2, v2);
				}
				else
				{
					ref Vector2 reference9 = ref array2[0];
					reference9 = new Vector2(u2, v2);
					ref Vector2 reference10 = ref array2[1];
					reference10 = new Vector2(u, v2);
					ref Vector2 reference11 = ref array2[2];
					reference11 = new Vector2(u, v);
					ref Vector2 reference12 = ref array2[3];
					reference12 = new Vector2(u2, v);
				}
				mesh.triangles = new int[0];
				mesh.vertices = array;
				mesh.uv = array2;
				mesh.colors = colors;
				mesh.triangles = triangles;
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				material = (Material)atlasRegion.page.rendererObject;
			}
			else
			{
				mesh = null;
			}
			return mesh;
		}
	}
}
