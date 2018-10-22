﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientComponent : MonoBehaviour {

	[SerializeField]
	private GenericEventMultipleParameter m_SoundAmbience;
	// Use this for initialization
	void Start ()
	{
		FmodManager.instance.CreateGenericEventMultipleParameter(ref m_SoundAmbience);
		FmodManager.instance.StartEvent(m_SoundAmbience);
	}
	
	// Update is called once per frame
	void Update () {
	}
	public void ChangeAmbientParameter(int index, float nextValue)
	{
		FmodManager.instance.ChangeParameter(ref m_SoundAmbience.eventParameter[index],nextValue);	
	}
}