//=====
//=====


using UnityEngine;
using System;
using System.Collections;


//---
//
//---
namespace FPSControl
{
	public class BulletHoleLogic : MonoBehaviour
	{
		public GameObject		_glassBulletHole;
		public Texture2D[]	_bulletHoles;
		public Texture2D[]	_bulletHolesInGlass;
		public float			_timeOut						= 1.0F;
		public bool				_detachChildren			= false;
        Renderer renderer;
	
		//---
		//
		//---
		public void Awake ()
		{
            renderer = GetComponent<Renderer>();
            Invoke ( "DestroyNow", _timeOut );
		}
	
	
		//---
		//
		//---
		public void DestroyNow ()
		{
			if ( _detachChildren )
			{
				transform.DetachChildren ();
			}
			
			Destroy ( gameObject );
		}
	
	
		//---
		//
		//---
		public  void SurfaceType ( RaycastHit ht )
		{
			// randomly select a bullet hole texture to display
			//Debug.Log( "bullethole hit " + ht.transform.name + ", " + ht.transform.tag );
			String surfaceTag = ht.transform.tag;
			
			if ( surfaceTag == "Window" || surfaceTag == "Shard" )
			{
				if ( _bulletHolesInGlass.Length > 0 && _glassBulletHole)
				{
					//Debug.Log(surfaceTag);
					renderer.enabled = false;
                    Renderer bhr = _glassBulletHole.GetComponent<Renderer>();
					bhr.material.mainTexture = _bulletHolesInGlass [ UnityEngine.Random.Range ( 0, _bulletHolesInGlass.Length ) ];
                    bhr.enabled = true;
					if (surfaceTag == "Shard")
					{
						// send message
						ht.transform.SendMessageUpwards ( "BreakMe", ht );
					}
				}
			}
			else if ( surfaceTag == "NoHoles" )
			{
				renderer.enabled = false;
			}
			else
			{
				if ( _bulletHoles.Length > 0 ) renderer.material.mainTexture = _bulletHoles [ UnityEngine.Random.Range ( 0, _bulletHoles.Length ) ];
			}
		}
	}
}