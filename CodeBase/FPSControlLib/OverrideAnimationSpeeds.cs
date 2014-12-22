/*
 * Name: OverrideAnimationSpeeds.cs
 * Version: 0.6
 * Author:  Yilmaz Kiymaz (@VoxelBoy)
 * Purpose: Adjust the playback speed of animations through the Inspector
 * License: CC by-sa 3.0 - http://creativecommons.org/licenses/by-sa/3.0/
 * Contact: VoxelBoy on Unity Forums
 */


using UnityEngine;
using System.Collections.Generic;


//---
//
//---
namespace FPSControl
{
	[RequireComponent (typeof(Animation))] //An Animation component is required
	
	
	public class OverrideAnimationSpeeds : MonoBehaviour
	{	
		//All three properties need to be serialized (hence, they're public) but not shown in Inspector (hence, we use HideInInspector)
		[HideInInspector]
		public float animSpeedRange = 5; //+/- range of animation playback speed
		
		[HideInInspector]
		public string[] animsToOverride; //Animation clip names 
		[HideInInspector]
		public float[] animSpeeds; //Animation speeds
		
		public GameObject[] _modelParts;
        Animation animation;
		
		void Awake() {
            animation = GetComponent<Animation>();
            //Overwrite speed of each AnimationState that matches the ones in animsToOverride
			if(animation != null && animsToOverride != null) {
				for(int i=0; i<animsToOverride.Length; i++) {
					AnimationState animState = animation[animsToOverride[i]];
					if(animState == null) continue;
					animState.speed = animSpeeds[i];
				}
			}
		}
		
		public void OverrideAnimationSpeed(string animName, float speed) {
			if(animation == null) return;
			
			for(int i=0; i<animsToOverride.Length; i++) {
				if(animsToOverride[i].Equals(animName)) { //Find the right animation string
					animSpeeds[i] = speed; //Set its new speed
					//Update corresponding AnimationState if it exists
					AnimationState animState = animation[animName];
					if(animState != null)
						animState.speed = speed;
					break; //Break out of the loop since we found the animation string we were looking for
				}
			}
		}
		
		public void CheckArrayConsistency() {
			if(animsToOverride == null) //If arrays haven't been instantiated
				CreateOverrideArrays(); //Create them
			CheckOverrideArrayLengths(); //Make sure the arrays' lengths match the number of clips in the Animation component
		}
		
		void CreateOverrideArrays() {
			if(animation == null) return; //If there's no animation component, early out
			int clipCount = animation.GetClipCount();
			animsToOverride = new string[clipCount];
			animSpeeds = new float[clipCount];
			int animIdx = 0;
			foreach(AnimationState animState in animation) {
				animsToOverride[animIdx] = animState.name;
				animSpeeds[animIdx] = 1f;
				animIdx++;
			}
		}
		
		void CheckOverrideArrayLengths() {
			if(animation == null || animsToOverride == null) return; //If there's no animation component or if the arrays haven't been instantiated, early out
			int clipCount = animation.GetClipCount(); //Get number of clips in the Animation component
			if(animsToOverride.Length != clipCount) { //If a new animation was added to the Animations array
				//Create temporary lists of existing string and float values for the animation overrides
				List<string> tempAnimsToOverride = new List<string>(animsToOverride.Length);
				List<float> tempAnimSpeeds = new List<float>(animsToOverride.Length);
				for(int i=0; i<animsToOverride.Length; i++) { //Populate the lists
					tempAnimsToOverride.Add(animsToOverride[i]);
					tempAnimSpeeds.Add(animSpeeds[i]);
				}
				
				CreateOverrideArrays(); //Re-create the arrays with the proper length
				
				//Copy back the override values to the arrays from the temporary lists
				for(int i=0; i<animsToOverride.Length; i++) {
					if(tempAnimsToOverride.Contains(animsToOverride[i])) {
						int idx = tempAnimsToOverride.IndexOf(animsToOverride[i]);
						animSpeeds[i] = tempAnimSpeeds[idx];
					}
				}
			}
		}
		
		public string GetAnimationName(int idx) {
			if(animsToOverride == null || animsToOverride.Length <= idx) {
				Debug.Log("can't return real animation name at index " + idx.ToString() + ", returning empty string.");
				return "";
			}
			return animsToOverride[idx];
		}
		
		public float GetAnimationSpeed(int idx) {
			if(animSpeeds == null || animSpeeds.Length <= idx) {
				Debug.Log("can't return real animation speed at index " + idx.ToString() + ", returning zero.");
				return 0f;
			}
			return animSpeeds[idx];
		}
		
		
		public void HidePart( int part )
		{
			Debug.Log( "hiding part (" + part + ") of (" + _modelParts.Length + ")" );
			if( part <= _modelParts.Length )
			{
				_modelParts[part].active = false;
			}
		}
		
		
		public void ShowPart( int part )
		{
			if( part <= _modelParts.Length )
			{
				_modelParts[part].active = true;
			}
		}
	}
}