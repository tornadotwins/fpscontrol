//=====
//=====


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//---
//
//---
namespace FPSControl
{
	public class PickUpLogic : MonoBehaviour
	{
		public String		weaponInUseName;
	//	public GUIText		pickupGUIText;
	//	public string		pickupText;
	//	public Color		pickupTextColor;
	//	public Color		highlightColor;
	//	public Color		nonhighlightColor;
	//	public Renderer[]	highlightParts;
		
		private bool		isCollected = false;
		
	
		public void Collected ()
		{
			if( !isCollected )
			{
	//			NonHighlighted();
				gameObject.SetActiveRecursively( false );
				Destroy (gameObject, 0.5F);
			}
		}
		
	/*	
		public void Highlighted()
		{
			pickupGUIText.text = pickupText;
			pickupGUIText.font.material.color = pickupTextColor;
			
			foreach( Renderer rend in highlightParts )
			{
				rend.material.SetColor( "_OutlineColor", highlightColor );
			}
		}
		
		
		public void NonHighlighted()
		{
			pickupGUIText.text = "";
			pickupGUIText.font.material.color = pickupTextColor;
			
			foreach( Renderer rend in highlightParts )
			{
				rend.material.SetColor( "_OutlineColor", nonhighlightColor );
			}
		}
		*/
	}
}