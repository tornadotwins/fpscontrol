//=====
//=====


using UnityEngine;
using System;
using System.Collections.Generic;
using FPSControl.States.Player;


//---
//
//---
namespace FPSControl
{
//	[RequireComponent(typeof(AudioSource))]
	
	public class ImpactControl : MonoBehaviour
	{	
	//	[SerializeField] private float stepsPerSecondWalking		= 1.0f;
	//	[SerializeField] private float stepsPerSecondRunning		= 0.5f;
	//	[SerializeField] private float stepsPerSecondCrouching	= 1.5f;		
	//	[SerializeField] private float stepsPerSecondBackwards	= 1.0f;	
		
		private FPSControlPlayer				player;	
		private ImpactControlDefinitions		def;
		private List<System.Object>				effectsLib;
        private ImpactControlDefinition         currentClip         = null;
		private float							currentTime			= 0f;
		private float							clipLength			= 1F;
		private AudioSource						_audio;
		
		//---
		//
		//---
		public void Start()
		{
            effectsLib = new List<System.Object>();
			//Get Impact Definitions
            def = (ImpactControlDefinitions)Resources.Load("ImpactControlDefinitions");
		//	foreach( ImpactControlDefinition obj in def.impacts)
		//	{
	    //		if( obj.group.Equals(gameObject.tag.ToString()))
		//		{
        //            effectsLib.Add(obj);
		//		}
		//	}
			
			//--- create an audiosource just for impacts
			_audio = gameObject.AddComponent<AudioSource>();
	    	
			//Get the Player controls
			player = GetComponent<FPSControlPlayer>();
			
			//--- check initial standing surface
		//	CheckStandingSurface();
		}
		
		//---
		//
		//---
		public void Update () 
		{
			if( player != null )
			{

			}
		}

        //---
        //
        //---
        public void ActivateImpactEffect(string name)
        {
            foreach( ImpactControlDefinition obj in def.impacts)
			{
	    		if( obj.name.Equals(name))
				{
                    effectsLib.Add(obj);
				}
			}
        }

        //---
        // Only check on new collision if we need to proccess a new impact
        //---
        public void OnImpact(RaycastHit hit)
        {
            if (CheckImpactSurface(hit) && currentClip != null)
            {
                ApplyImpactDecal(hit);

                CreateImpactEffect(hit);

                PlayImpactSound();
            }
        }

		
		//---
		// check the surface the player is hitting to see if it's charastics are in impact defs
		//---
        private bool CheckImpactSurface(RaycastHit hit)
		{
            foreach (ImpactControlDefinition obj in effectsLib)
			{
				//See if there's a texture to check on
				if( hit.collider.tag.ToString().Equals(obj.tag.ToString()) )
				{
					currentClip = obj;
		        	return true;
				}
				//Otherwise check on material. This check is a little expensive, please use tags.
				else if( ( hit.collider.gameObject.renderer != null ) && ( hit.collider.gameObject.renderer.material.mainTexture != null ) )
				{
					foreach( Texture texture in obj.textures )
					{
						if( hit.collider.gameObject.renderer.material.mainTexture == texture )
						{
							currentClip = obj;
                            return true;
						}
					}
				}
			}

            //If nothing was found check to see if we're on a terrain, this is more expensive
            if (def.terrainCheck) 
            {
                Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                if (terrain != null)
                {   // found a terrain, try to determine best texture at this point
                    Vector3 pos = terrain.transform.InverseTransformPoint(hit.point);

                    float[,,] splatmapData = terrain.terrainData.GetAlphamaps( (int) pos.x, (int) pos.z, 1, 1);

                    float maxMix = 0;
                    int maxIndex = 0;

                    int numTex = terrain.terrainData.splatPrototypes.Length;

                    // loop through each mix value and find the maximum
                    for (int i = 0; i < numTex; ++i)
                    {
                        if (splatmapData[0,0,i] > maxMix)
                        {
                            maxIndex = i;
                            maxMix = splatmapData[0, 0, i];
                        }
                    }

                    Texture terrainTex = terrain.terrainData.splatPrototypes[maxIndex].texture;

                    foreach (ImpactControlDefinition obj in effectsLib)
                    {
                        //if (obj.terrainCheck)
                        {
                            foreach (Texture texture in obj.textures)
                            {
                                if (terrainTex == texture)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
		}
		
		
		//---
		//
		//---
		private void PlayImpactSound()
		{
			//Play the sound once
			int which = UnityEngine.Random.Range(0, currentClip.sounds.Count-1);
			//Debug.Log( "FS sound, state(" + currentState + ")" );
			_audio.pitch 	= UnityEngine.Random.Range(currentClip.minValPitch, currentClip.maxValPitch);
			_audio.volume 	= UnityEngine.Random.Range(currentClip.minValVolume, currentClip.maxValVolume);
			_audio.PlayOneShot( currentClip.sounds[ which ] );
				
				
			clipLength = currentClip.sounds[ which ].length;
		}

        //---
        //
        //---
        private void ApplyImpactDecal(RaycastHit hit)
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            int which = UnityEngine.Random.Range(0, currentClip.decals.Count - 1);

            GameObject decal = currentClip.decals[which];

            GameObject tr = (GameObject)Instantiate(decal, hit.point, rot);
            tr.SendMessage("SurfaceType", hit);
            tr.transform.parent = hit.transform; // parent to hit object so the bullet holes move with object
        }

        //---
        //
        //---
        private void CreateImpactEffect(RaycastHit hit)
        {
            Quaternion rot = Quaternion.identity;
            int which = UnityEngine.Random.Range(0, currentClip.particles.Count - 1);

            ImpactControlEffect effect = currentClip.particles[which];

            if (effect.normalRotate)
            {
                rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }

            GameObject tr = (GameObject)Instantiate(effect.effectObj, hit.point, rot);
            //tr.SendMessage("SurfaceType", hit);
            //tr.transform.parent = hit.transform; // parent to hit object so the bullet holes move with object
        }
	}
}