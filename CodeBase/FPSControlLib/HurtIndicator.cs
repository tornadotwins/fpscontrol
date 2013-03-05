//=====
//=====


using UnityEngine;
using System.Collections;


//---
//
//---
namespace FPSControl
{
	public enum HurtQuadrant
	{
		FRONT,
		BACK,
		LEFT,
		RIGHT
	}
	
	
	public class HurtIndicator : MonoBehaviour
	{
		public float				_fadeRate					= 0.5F;
		public float				_initialAlpha				= 0.0F;
		public float				_maxAlpha					= 1.0F;
		public GameObject			_player;
		public GameObject			_camera;
		public MeshRenderer			_indicatorFront;
		public MeshRenderer			_indicatorBack;
		public MeshRenderer			_indicatorLeft;
		public MeshRenderer			_indicatorRight;
	//	private int					_hurtQuadrant				= 0;
		private Material[]			_indicators;
		
		private static HurtIndicator _singleton;
		
		
		//---
		// store indicators in array and set all alphas to initial state
		//---
		void Awake()
		{
			_singleton = this;
			
			//--- position the indicators in front of the specified camera
			transform.parent = _camera.transform;
			transform.localPosition = Vector3.zero;
			transform.position = transform.position + ( _camera.transform.forward * 7.0F );
			
			//--- cache indicator materials
			_indicators = new Material[4];
			_indicators[ (int)HurtQuadrant.FRONT ] = _indicatorFront.material;
			_indicators[ (int)HurtQuadrant.BACK ] = _indicatorBack.material;
			_indicators[ (int)HurtQuadrant.LEFT ] = _indicatorLeft.material;
			_indicators[ (int)HurtQuadrant.RIGHT ] = _indicatorRight.material;
			
			//--- set initial alpha of indicators, typically 0 to start off hidden
			for( int i=0; i<4; i++ )
			{
				SetAlpha( _indicators[i], _initialAlpha );
			}
			
		}
		
		
		//---
		// Fade any visible hurt indicators
		//---
		void Update()
		{
			for( int i=0; i<4; i++ )
			{
				if( _indicators[i] && _indicators[i].color.a > 0.0F )
				{
					FadeAlpha( _indicators[i], _fadeRate );
				}
			}
		}
		
		
		//---
		//
		//---
	/*	
		private var rotAngle : float = 0;
	private var pivotPoint : Vector2;
	
	function OnGUI () {   
	    pivotPoint = Vector2(Screen.width/2,Screen.height/2);
	    GUIUtility.RotateAroundPivot (rotAngle, pivotPoint); 
	    if(GUI.Button(Rect(Screen.width/2-25, Screen.height/2-25, 50, 50),"Rotate"))
	        rotAngle += 10;
	} 
	*/
		
		
		//---
		// Display indicator based on hurt position in relationship to players body center (TBD)
		//---
		public static void GotHurtPos( float damage, Vector3 hurtPos )
		{
		}
		
		
		//---
		// Display indicator for a specific quadrant
		//---
		public void GotHurtQuadrant( int where )
		{
			HurtQuadrant quad = (HurtQuadrant) where;
			GotHurtQuadrant( 0F, quad );
		}
		public static void GotHurtQuadrant( float damage, HurtQuadrant where )
		{
			_singleton.SetAlpha( _singleton._indicators[ (int) where ], _singleton._maxAlpha );
		}
		
		
		
		//---
		// Fade an indicator by specified rate
		//---
		private void FadeAlpha( Material indicatorMaterial, float fadeRate )
		{
			SetAlpha( indicatorMaterial, indicatorMaterial.color.a - ( fadeRate * Time.deltaTime ) );
		}
		
		
		//---
		// Set indicator alpha
		//---
		private void SetAlpha( Material indicatorMaterial, float alpha )
		{
			Color temp = indicatorMaterial.color;
			temp.a = alpha;
			indicatorMaterial.color  = temp;
		}
	}
}
