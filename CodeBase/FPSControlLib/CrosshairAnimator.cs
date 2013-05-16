//===
//===


using UnityEngine;
using System.Collections;


//---
//
//---
namespace FPSControl
{
	public class CrosshairAnimator : MonoBehaviour
	{
	
	//a basic example of how you could controll the crosshair from another script
	
	
		private static CrosshairAnimator _singleton;
		
		public IntelliCrosshair crosshair; //a reference to the crosshair we want to controll
        //public FPSControlPlayerMovement playerMovement; 
	
		//---
		//
		//---
		void  Awake ()
		{
			_singleton = this; 
				
			// if crosshair is not set....get the one from this gameobject
//			if(crosshair == null)
//			crosshair=transform.GetComponent<IntelliCrosshair>();
		}
	
	
		//---
		//
		//---
		void  Update ()
		{	
			//here we toggle the booleans of the IntelliCrosshair script	
//			crosshair.shoot=Input.GetButtonDown("Fire1");		//you may want to controll this inside your Fire function of your weapon
//			crosshair.fade=Input.GetKey(KeyCode.Q);			//if you  are aim down sights fade it off...if not fade it in
//            if (playerMovement.Player.moving) ToggleSpread(true);
//            else ToggleSpread(false);
		}

        public void SetCrossHair(IntelliCrosshair crosshair)
        {
            if (this.crosshair) this.crosshair.enabled = false;
            this.crosshair = crosshair;
            this.crosshair.enabled = true;
        }

        public void Fire(bool fire)
        {
            crosshair.shoot = fire;
        }

        public void StartZoom()
        {
            if(crosshair) crosshair.fade = true;
        }

        public void EndZoom()
        {
            if (crosshair) crosshair.fade = false;
        }

        public void Move()
        {
            if (crosshair) ToggleSpread(true);
        }

        public void BeStill()
        {
            if (crosshair) ToggleSpread(false);
        }
	
	
		//---
		//
		//---
		void  OnGUI ()
		{
			//$$	GUI.Label( new Rect(10,40,400,100),"hold E to spread  *** LMB to simulate shooting *** Q for fade out\nESC for options");
		}
		
		
		//---
		// set spread state of the IntelliCrosshair component
		//---
		public static void ToggleSpread( bool state )
		{
            //Debug.Log("toggle: " + state);
            _singleton.crosshair.spread = state;
		}
		
	}
}