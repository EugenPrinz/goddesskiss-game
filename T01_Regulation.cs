using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class T01_Regulation : MonoBehaviour
{
	private void Start()
	{
		Regulation value = Regulation.FromLocalResources();
		string text = JsonConvert.SerializeObject(value, Formatting.Indented);
	}

	private void Update()
	{
	}
}
