using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.BWF.Model;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF.Provider
{
	[HelpURL("http://www.crosstales.com/en/assets/badwordfilter/api/class_crosstales_1_1_bad_word_1_1_domain_provider_text.html")]
	public class DomainProviderText : DomainProvider
	{
		public override void Load()
		{
			base.Load();
			if (Sources == null)
			{
				return;
			}
			loading = true;
			if (Helper.isEditorMode)
			{
				return;
			}
			Source[] sources = Sources;
			foreach (Source source in sources)
			{
				if (source.Resource != null)
				{
					StartCoroutine(loadResource(source));
				}
				if (!string.IsNullOrEmpty(source.URL))
				{
					StartCoroutine(loadWeb(source));
				}
			}
		}

		public override void Save()
		{
		}

		private IEnumerator loadWeb(Source src)
		{
			Guid uid = Guid.NewGuid();
			coRoutines.Add(uid);
			if (!string.IsNullOrEmpty(src.URL))
			{
				WWW www = new WWW(src.URL.Trim());
				do
				{
					yield return www;
				}
				while (!www.isDone);
				if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
				{
					List<string> list = Helper.SplitStringToLines(www.text);
					yield return null;
					if (list.Count > 0)
					{
						domains.Add(new Domains(src, list));
					}
				}
				www.Dispose();
			}
			coRoutines.Remove(uid);
			if (loading && coRoutines.Count == 0)
			{
				loading = false;
				init();
			}
		}

		private IEnumerator loadResource(Source src)
		{
			Guid uid = Guid.NewGuid();
			coRoutines.Add(uid);
			if (src.Resource != null)
			{
				List<string> list = Helper.SplitStringToLines(src.Resource.text);
				yield return null;
				if (list.Count > 0)
				{
					domains.Add(new Domains(src, list));
				}
			}
			coRoutines.Remove(uid);
			if (loading && coRoutines.Count == 0)
			{
				loading = false;
				init();
			}
		}
	}
}
