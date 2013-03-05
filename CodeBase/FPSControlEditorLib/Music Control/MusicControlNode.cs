using System;
using UnityEngine;

namespace FPSControlEditor
{
	public class MusicControlNode
	{
		public int type;
		public bool shown = false;
		public string name = "Unnamed Track";
		public AudioClip track;
		public float volume = 0.5f;
		public bool backgroundMusic;
		public bool loop = false;
		public float time = 0.0f;
	
		public MusicControlNode ( int type  )
		{
			this.type = type;
		}
	
	}
}

