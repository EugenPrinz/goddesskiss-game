using System;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class ParticleSystemClipper : MonoBehaviour
{
	private const string ShaderName = "Bleach/Particles Additive Area Clip";

	private const float ClipInterval = 0.5f;

	private UIPanel m_targetPanel;

	private Shader m_shader;

	private void Start()
	{
		m_targetPanel = GetComponent<UIPanel>();
		if (m_targetPanel == null)
		{
			throw new ArgumentNullException("Cann't find the right UIPanel");
		}
		if (m_targetPanel.clipping != UIDrawCall.Clipping.SoftClip)
		{
			throw new InvalidOperationException("Don't need to clip");
		}
		m_shader = Shader.Find("Bleach/Particles Additive Area Clip");
		Clip();
	}

	private Vector4 CalcClipArea()
	{
		Vector4 finalClipRegion = m_targetPanel.finalClipRegion;
		Vector4 vector = default(Vector4);
		vector.x = finalClipRegion.x - finalClipRegion.z / 2f;
		vector.y = finalClipRegion.y - finalClipRegion.w / 2f;
		vector.z = finalClipRegion.x + finalClipRegion.z / 2f;
		vector.w = finalClipRegion.y + finalClipRegion.w / 2f;
		Vector4 vector2 = vector;
		UIRoot component = m_targetPanel.root.GetComponent<UIRoot>();
		Vector3 vector3 = m_targetPanel.transform.position - component.transform.position;
		float num = 2f;
		float num2 = num / (float)component.manualHeight;
		vector = default(Vector4);
		vector.x = vector3.x + vector2.x * num2;
		vector.y = vector3.y + vector2.y * num2;
		vector.z = vector3.x + vector2.z * num2;
		vector.w = vector3.y + vector2.w * num2;
		return vector;
	}

	private void Clip()
	{
		Vector4 value = CalcClipArea();
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			Material material = particleSystem.GetComponent<Renderer>().material;
			if (material.shader.name != "Bleach/Particles Additive Area Clip")
			{
				material.shader = m_shader;
			}
			material.SetVector("_Area", value);
		}
	}
}
