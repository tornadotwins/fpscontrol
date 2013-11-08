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
	
	public class FootstepControl : MonoBehaviour
	{	
		[SerializeField] private float stepsPerSecondWalking		= 1.0f;
		[SerializeField] private float stepsPerSecondRunning		= 0.5f;
		[SerializeField] private float stepsPerSecondCrouching	= 1.5f;		
		[SerializeField] private float stepsPerSecondBackwards	= 1.0f;
        [SerializeField] private float updateSurfaceTimer = 0.5f;	
		
		private FPSControlPlayer				player;	
		private FootstepControlDefinitions		def;
		private List<System.Object>				soundLib;
		private FootstepControlDefinition		currentClip			= null;
		private float							currentTime			= 0f;
        private float                           surfaceTime         = 0f;
		private PlayerState						currentState		= null;
		private bool							isStandingStill	    = true;
        private bool                            didJumpCheck        = false;
		private float							clipLength			= 1F;
		private AudioSource						_audio;
		
		//---
		//
		//---
		public void Start()
		{
			soundLib = new List<System.Object>();
			//Get Footstep Definitions
			def = (FootstepControlDefinitions)Resources.Load("FootstepControlDefinitions");
			foreach( FootstepControlDefinition obj in def.footsteps)
			{
	    		if( obj.type.ToString().Equals(gameObject.tag.ToString()))
				{
	    			soundLib.Add( obj );
				}
			}
			
			//--- create an audiosource just for footsteps
			_audio = gameObject.AddComponent<AudioSource>();
	    	
			//Get the Player controls
			player = GetComponent<FPSControlPlayer>();
			//TODO Add check to see if we are a NPC
			
			//--- check initial standing surface
			CheckStandingSurface();
		}
		
		//---
		//
		//---
		public void Update () 
		{
			if( player != null )
			{
                //Walking, Running, Backwards, Crouching, Jumped, None
				//TODO Add jump logic
				//if( player.state != MoveState.None ) Debug.Log( "FS state: " + player.state );

                bool reset = false; // for any checks that require updating the current state

                // double checking the jump state
                if (didJumpCheck && currentState != player["Jump"] && !player.jumping)
                {
                    reset = true;
                }

                if (player.currentState != currentState || reset)
				{
					//Debug.Log( "FS switch state " + player.state + " -- " + currentState );
					currentState = player.currentState;
					
					//if( currentState != MoveState.None ) isStandingStill = false;
                    isStandingStill = (currentState == player["Idle"]) || (currentState == player["Jump"]);
					
					CheckTimeToNewSound();
//$$					SetNewTimer();
					
					if( isStandingStill )
					{
						//Debug.Log( "FS should stop sound. clip (" + audio + ")" );
						if(_audio.isPlaying )
						{
							_audio.Stop();
							currentTime = 0F;
							clipLength = 0F;
						}
					}
				}
				
				currentTime -= Time.deltaTime;

                if (surfaceTime > 0)
                    surfaceTime -= Time.deltaTime;
				
				CheckTimeToNewSound();
			}
		}
		
		
		//---
		// Only check on new collision if we need to play a new sound
		//---
		public void OnCollisionEnter( Collision collision )
        {
			CheckStandingSurface();
		}
		
		
		//---
		// check the surface the player is currently on to see if it's charastics are in footstep defs
		//---
		private void CheckStandingSurface()
		{
			RaycastHit hit;
            bool found = false;

            surfaceTime = updateSurfaceTimer;
			
			if (Physics.Raycast (transform.position, -transform.up, out hit)) 
			{
				foreach( FootstepControlDefinition obj in soundLib )
				{
					//See if there's a texture to check on
					if( hit.collider.tag.ToString().Equals(obj.tag.ToString()) )
					{
						currentClip = obj;
                        found = true;
		        		break;
					}
					//Otherwise check on material. This check is a little expensive, please use tags.
					else if( ( hit.collider.gameObject.renderer != null ) && ( hit.collider.gameObject.renderer.material.mainTexture != null ) )
					{
						foreach( Texture texture in obj.textures )
						{
							if( hit.collider.gameObject.renderer.material.mainTexture == texture )
							{
								currentClip = obj;
                                found = true;
								break;
							}
						}
					}
				}

                //If nothing was found check to see if we're on a terrain
                // this is more expensive, so only check if the flag for it is set
                if (!found && def.terrainCheck)
                {
                    Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        // try to determine best texture at this point
                        Vector3 pos = terrain.transform.InverseTransformPoint(hit.point);

                        // calculate which splat map cell the worldPos falls within (ignoring y)
                        int mapX = (int)(((pos.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth);
                        int mapZ = (int)(((pos.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight);

                        float[, ,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

                        float maxMix = 0;
                        int maxIndex = 0;

                        int numTex = terrain.terrainData.splatPrototypes.Length;

                        // loop through each mix value and find the maximum
                        for (int i = 0; i < numTex; ++i)
                        {
                            if (splatmapData[0, 0, i] > maxMix)
                            {
                                maxIndex = i;
                                maxMix = splatmapData[0, 0, i];
                            }
                        }

                        Texture terrainTex = terrain.terrainData.splatPrototypes[maxIndex].texture;

                        foreach (FootstepControlDefinition obj in soundLib)
                        {
                            foreach (Texture texture in obj.textures)
                            {
                                if (terrainTex == texture)
                                {
                                    currentClip = obj;
                                    break;
                                }
                            }
                        }
                    }
                }
			}
		}
		
		//---
		//
		//---
		private void CheckTimeToNewSound()
		{
            if (isStandingStill || currentClip == null)
            {
                return;
            }

            if (surfaceTime <= 0 && updateSurfaceTimer > 0)
            {
                CheckStandingSurface();
            }

			if( currentTime <= 0 ) // && (currentState != MoveState.None) )
			{
				//Play the sound once
				int which = UnityEngine.Random.Range(0, currentClip.sounds.Count-1);
				//Debug.Log( "FS sound, state(" + currentState + ")" );
				_audio.pitch 	= UnityEngine.Random.Range(currentClip.minValPitch, currentClip.maxValPitch);
				_audio.volume 	= UnityEngine.Random.Range(currentClip.minValVolume, currentClip.maxValVolume);
				_audio.PlayOneShot( currentClip.sounds[ which ] );
				
				
				clipLength = currentClip.sounds[ which ].length;
				
				//reset time
				SetNewTimer();
			}
		}
		
		
		//---
		//
		//---
		private void SetNewTimer()
		{
			//Reset isStandingStill if we started moving
			isStandingStill = false;
			
			//Debug.Log( "FS new state (" + currentState + "), pstate (" + player.state + ")"  );
            if (currentState == player["Idle"])
            {
                isStandingStill = true;
            }
            else if (currentState == player["Jump"] || player.jumping)
            {
                didJumpCheck = true; // we need to double check the jump state later on
                isStandingStill = true;
            }
            else if (currentState == player["Run"])
            {
                currentTime = stepsPerSecondRunning;
            }
            else if (currentState == player["Walk"])
            {
                if (player.crouching)
                {
                    currentTime = stepsPerSecondCrouching;
                }
                else
                {
                    if(player.reversing)
                        currentTime = stepsPerSecondBackwards; 
                    else 
                        currentTime = stepsPerSecondWalking;
                }
            }
            /*
			switch(currentState)
			{
				case MoveState.Walking:
					currentTime = stepsPerSecondWalking;
					break;
				case MoveState.Running:
					currentTime = stepsPerSecondRunning;
					break;
				case MoveState.Backwards:
					currentTime = stepsPerSecondBackwards;
					break;
				case MoveState.Crouching:
					currentTime = stepsPerSecondCrouching;
					break;
				case MoveState.Jumped:
					//currentTime = stepsPerSecondWalking;
					//--- not really still, but most effective at stopping sounds
					isStandingStill = true;
					break;
				case MoveState.None:
					isStandingStill = true;
					break;
			}*/
		}
	}
}