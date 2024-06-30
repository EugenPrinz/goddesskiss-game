using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Crosstales.BWF.Model;
using UnityEngine;

namespace Crosstales.BWF.Provider
{
	[ExecuteInEditMode]
	public abstract class BaseProvider : MonoBehaviour
	{
		[Tooltip("Name to identify the provider.")]
		public string Name = string.Empty;

		[Header("Regex Options")]
		[Tooltip("Option1 (default: RegexOptions.IgnoreCase).")]
		public RegexOptions RegexOption1 = RegexOptions.IgnoreCase;

		[Tooltip("Option2 (default: RegexOptions.CultureInvariant).")]
		public RegexOptions RegexOption2 = RegexOptions.CultureInvariant;

		[Tooltip("Option3 (default: RegexOptions.None).")]
		public RegexOptions RegexOption3;

		[Tooltip("Option4 (default: RegexOptions.None).")]
		public RegexOptions RegexOption4;

		[Tooltip("Option5 (default: RegexOptions.None).")]
		public RegexOptions RegexOption5;

		[Header("Sources")]
		[Tooltip("All sources for this provider.")]
		public Source[] Sources;

		[Header("Load behaviour")]
		[Tooltip("Clears all existing bad words on 'Load' (default: on).")]
		public bool ClearOnLoad = true;

		protected List<Guid> coRoutines = new List<Guid>();

		protected static bool loggedUnsupportedPlatform;

		protected bool loading;

		public bool isReady { get; protected set; }

		public abstract void Load();

		public abstract void Save();

		protected abstract void init();

		public void Awake()
		{
			Load();
		}

		protected void logNoResourcesAdded()
		{
		}
	}
}
