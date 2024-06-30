using UnityEngine;

public class UIScaleParticles : MonoBehaviour
{
	private void Start()
	{
		float num = 1.7777778f;
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = num2 / num;
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startSize *= num3;
			ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
			if ((bool)component)
			{
				component.lengthScale *= num3;
				component.velocityScale *= num3;
			}
		}
	}
}
