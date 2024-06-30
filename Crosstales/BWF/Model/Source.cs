using System;
using System.Text;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF.Model
{
	[Serializable]
	public class Source
	{
		[Header("Information")]
		[Tooltip("Name of the source.")]
		public string Name = string.Empty;

		[Tooltip("Description for the source (optional).")]
		public string Description = string.Empty;

		[Tooltip("Icon to represent the source (e.g. country flag, optional)")]
		public Sprite Icon;

		[Header("Settings")]
		[Tooltip("URL of a text file containing all regular expressions for this source. Add also the protocol-type ('http://', 'file://' etc.).")]
		public string URL = string.Empty;

		[Tooltip("Text file containing all regular expressions for this source.")]
		public TextAsset Resource;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name);
			stringBuilder.Append(Constants.TEXT_TOSTRING_START);
			stringBuilder.Append("Name='");
			stringBuilder.Append(Name);
			stringBuilder.Append(Constants.TEXT_TOSTRING_DELIMITER);
			stringBuilder.Append("Description='");
			stringBuilder.Append(Description);
			stringBuilder.Append(Constants.TEXT_TOSTRING_DELIMITER);
			stringBuilder.Append("Icon='");
			stringBuilder.Append(Icon);
			stringBuilder.Append(Constants.TEXT_TOSTRING_DELIMITER_END);
			stringBuilder.Append(Constants.TEXT_TOSTRING_END);
			return stringBuilder.ToString();
		}
	}
}
