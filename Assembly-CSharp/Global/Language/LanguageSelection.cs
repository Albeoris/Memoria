﻿using System;
using System.Collections.Generic;
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
			Localization.CurrentLanguage = UIPopupList.current.value;
		});
	}

	public void Refresh()
	{
	    if (this.mList == null)
            return;

	    ICollection<String> knownLanguages = Localization.KnownLanguages;
	    if (knownLanguages == null)
	        return;

        this.mList.Clear();

        foreach (var language in knownLanguages)
            this.mList.items.Add(language);
        
	    this.mList.value = Localization.CurrentLanguage;
	}

	private UIPopupList mList;
}
