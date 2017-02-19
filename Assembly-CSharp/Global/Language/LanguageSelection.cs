using System;
using Memoria.Assets;
using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
[AddComponentMenu("NGUI/Interaction/Language Selection")]
public class LanguageSelection : MonoBehaviour
{
	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.Refresh();
	}

	private void Start()
	{
		EventDelegate.Add(this.mList.onChange, delegate
		{
			Localization.language = UIPopupList.current.value;
		});
	}

	public void Refresh()
	{
		if (this.mList != (UnityEngine.Object)null && Localization.knownLanguages != null)
		{
			this.mList.Clear();
			Int32 i = 0;
			Int32 num = (Int32)Localization.knownLanguages.Length;
			while (i < num)
			{
				this.mList.items.Add(Localization.knownLanguages[i]);
				i++;
			}
			this.mList.value = Localization.language;
		}
	}

	private UIPopupList mList;
}
