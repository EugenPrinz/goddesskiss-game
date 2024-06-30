using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexagonRenderer : MonoBehaviour
{
	public float radius = 1f;

	public float edgeWidth = 0.5f;

	private void Awake()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		component.mesh = CreateMesh(radius, edgeWidth);
	}

	public static Mesh CreateMesh(float radius, float edgeWidth)
	{
		float num = (float)Math.PI / 3f;
		Vector3[] array = new Vector3[36];
		Vector2[] array2 = new Vector2[array.Length];
		int[] array3 = new int[72];
		float num2 = num;
		for (int i = 0; i < 6; i++)
		{
			int num3 = i * 6;
			float num4 = 0f;
			ref Vector3 reference = ref array[num3];
			reference = Vector3.zero;
			ref Vector3 reference2 = ref array[num3 + 1];
			reference2 = Vector3.zero;
			ref Vector2 reference3 = ref array2[num3];
			reference3 = Vector2.zero;
			ref Vector2 reference4 = ref array2[num3 + 1];
			reference4 = new Vector2(1f, 0f);
			num4 = Mathf.Max(0.1f, radius - edgeWidth);
			array[num3 + 2].x = num4 * Mathf.Cos(num2);
			array[num3 + 2].y = num4 * Mathf.Sin(num2);
			ref Vector2 reference5 = ref array2[num3 + 2];
			reference5 = new Vector2(0f, 0.5f);
			array[num3 + 3].x = num4 * Mathf.Cos(num2 + num);
			array[num3 + 3].y = num4 * Mathf.Sin(num2 + num);
			ref Vector2 reference6 = ref array2[num3 + 3];
			reference6 = new Vector2(1f, 0.5f);
			num4 = radius;
			array[num3 + 4].x = num4 * Mathf.Cos(num2);
			array[num3 + 4].y = num4 * Mathf.Sin(num2);
			ref Vector2 reference7 = ref array2[num3 + 4];
			reference7 = new Vector2(0f, 1f);
			array[num3 + 5].x = num4 * Mathf.Cos(num2 + num);
			array[num3 + 5].y = num4 * Mathf.Sin(num2 + num);
			ref Vector2 reference8 = ref array2[num3 + 5];
			reference8 = new Vector2(1f, 1f);
			int num5 = i * 12;
			array3[num5] = num3;
			array3[num5 + 1] = num3 + 3;
			array3[num5 + 2] = num3 + 2;
			array3[num5 + 3] = num3;
			array3[num5 + 4] = num3 + 1;
			array3[num5 + 5] = num3 + 3;
			array3[num5 + 6] = num3 + 2;
			array3[num5 + 7] = num3 + 5;
			array3[num5 + 8] = num3 + 4;
			array3[num5 + 9] = num3 + 2;
			array3[num5 + 10] = num3 + 3;
			array3[num5 + 11] = num3 + 5;
			num2 += num;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		return mesh;
	}
}
