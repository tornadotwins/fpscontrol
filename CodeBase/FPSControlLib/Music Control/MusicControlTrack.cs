//=====
//=====


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//---
//
//---
namespace FPSControl
{
	[System.Serializable]
	public class MusicControlTrack
	{
		public AudioClip track;
		public string name;
		public float volume = 1.0f;
		public bool backgroundMusic = false;
		public bool loop = false;
		public float time = 0;
	}
}