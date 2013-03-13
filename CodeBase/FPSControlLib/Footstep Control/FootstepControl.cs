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
		
		private FPSControlPlayer				player;	
		private FootstepControlDefinitions		def;
		private List<System.Object>				soundLib;
		private FootstepControlDefinition		currentClip			= null;
		private float							currentTime			= 0f;
		private PlayerState						currentState		= null;
		private bool							isStandingStill	= true;
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
				
				if( player.currentState != currentState )
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
			
			if (Physics.Raycast (transform.position, -transform.up, out hit)) 
			{
				foreach( FootstepControlDefinition obj in soundLib )
				{
					//See if there's a texture to check on
					if( hit.collider.tag.ToString().Equals(obj.tag.ToString()) )
					{
						currentClip = obj;
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
								break;
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
			if( currentTime <= 0 && !isStandingStill && currentClip != null ) // && (currentState != MoveState.None) )
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