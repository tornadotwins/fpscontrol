//=====
//=====


using UnityEngine; 
using System.Collections;


//---
//
//---
namespace FPSControl
{
	public class IntelliCrosshair : MonoBehaviour 
	{           
		public Transform _camera; 
		//the camera to be used for raycasting, if not set the Main Camera will be used  
		
		public Color defaultColor = Color.white; 
		//the default color of the crosshair
		
		public Color enemyColor = Color.red; 
		//the color needs to be set before testing in the editor, they can't be changed at runntime 
		
		public Color friendlyColor = Color.green;  
		public Color borderColor = Color.black;    
		public string tagForEnemy = "Enemy"; 
		// when a object with this tag is our target, the crosshair will use the enemy Color 
		
		public string tagForFriend = "Friend"; 
		public float targetScanDist = 50.0f; 
		//how far will enemys or friends be detectet 
		
		public bool changeColor = true; 
		//use the color feature   
		
		public int width = 3;
		//the dimensions of the crosshair. width and borderWight are absolute pixel values and wont adjust with resolution. 
		
		public float height = 3.0f; 
		//it is best to use odd nummbers for width and borderWight (1 or 3 work very good) height and offset will adjust to resolution    
		
		public bool border = true; 
		//draw a border (toggle off to save 4-5 drawcalls) 
		
		public int borderWidth = 1;    
		public float offset = 3.0f; 
		//the distance from the four lines to the center in % of the current screenheight   
//        public float scopeOffset = 1.0f;
	    //distance when scoped
		public bool centerPoint = true; 
		//draw a dot in the middle 
		public int centerWidth = 1; 
		//set this to 1 or 3, even numbers can look off 
		public bool showCrosshair = true; 
		//a toggle for the crosshair     
        public bool fadeOutOnScope = true;
        //should crosshair fade out on scope
        public float globalRotation = 0.0f; 
		//you can rotate the crosshair around the center   
		public float alpha = 1.0f; 
		//the overall opacity of the crosshair     
		public bool doAnimate = true;
		public float spreadAmount = 5.0f; 
		//how far will it spread 
		public float spreadSpeed = 20.0f; 
		//how fast will it spread    
		public float shootAmount = 5.0f;  
		public float shootSpeed = 200f; 
		//set this high enough to se the effect    
		public float fadeSpeed = 25.0f;          
		public IntelliCrosshair secondaryCrosshair; 
		//you can create another gameobject, add an IntelliCrosshair script to it and drag it in here 
		//rember that each crosshair will cost you at least 4 drawcalls    
		public bool syncSecondary = true; 
		//this will sync the animation-states 
		[HideInInspector] public bool fade = false; 
		//this will fade off and shrink the crosshair. nice effect when "aiming down sights" 
		[HideInInspector] public bool shoot = false; 
		// toggle this on and off for shooting-effect   
		[HideInInspector] public bool spread = false; 
		//you can trigger this bool  from another script to spread the crosshair   
		
		//------------------------------------------------------------------------------------------------------------------------------------------------------------    
		private Texture2D defaultTexture;  
		private Texture2D enemyTexture;  
		private Texture2D friendlyTexture;  
		private Texture2D borderTexture;    
		private float anim = 0.0f;  
		private RaycastHit ray;  
		private GUIStyle lineStyle; 
		private GUIStyle lineStyleBG;  
		//private Vector2 center;  
		private float _offset = 0.0f;  
		private float _alpha = 1.0f; 
		private float newHeight = 0.0f; 
		private float newOffset = 0.0f;  
		private float newSpreadAmount;  
		private float newShootAmount;  
		private float newSpreadSpeed;  
		private float newShootSpeed;   
		
		Vector2 globalScale = new Vector2 (1, 1); 
		
		void Awake () 
		{      
			defaultTexture = new Texture2D (2, 2);  
			SetTextureColors (defaultTexture, defaultColor);    
			enemyTexture = new Texture2D (2, 2);  
			SetTextureColors (enemyTexture, enemyColor);    
			friendlyTexture = new Texture2D (2, 2);  
			SetTextureColors (friendlyTexture, friendlyColor);    
			borderTexture = new Texture2D (2, 2);  
			SetTextureColors (borderTexture, borderColor);            
			if (transform.camera && _camera == null)   
				_camera = transform; 
			else if (_camera == null)  
				_camera = Camera.main.transform;  
		}    
    
		void Start () 
		{
			lineStyle = new GUIStyle ();  
			lineStyle.normal.background = defaultTexture;  
			lineStyleBG = new GUIStyle ();  
			lineStyleBG.normal.background = borderTexture;  
			FitToResolution (); 
		}   
              
		void OnGUI () 
		{  
			Color thisColor = GUI.color; 
			thisColor.a = alpha * _alpha; 
			GUI.color = thisColor;      
			Vector2 pivotPoint = new Vector2 (Screen.width / 2, Screen.height / 2);  
			if (globalScale != new Vector2 (1, 1)) 
				GUIUtility.ScaleAroundPivot (globalScale, pivotPoint);  
			if (globalRotation != 0)  
				GUIUtility.RotateAroundPivot (globalRotation, pivotPoint);    
			if (showCrosshair) 
			{    
				Vector2 center = new Vector2 ((Screen.width / 2), (Screen.height / 2)); 
				_offset = newOffset + anim;       
				if (border) 
				{  
					float bWidth = width + borderWidth * 2; 
					float cbWidth = centerWidth + borderWidth * 2;
					float borderHeight = newHeight + borderWidth * 2;      //draw the border first    
					GUI.Box (new Rect (center.x - (bWidth / 2), center.y - (((borderHeight - 1) - borderWidth) + _offset), bWidth, borderHeight), GUIContent.none, lineStyleBG);    
					GUI.Box (new Rect (center.x - (bWidth / 2), center.y + ((-borderWidth) + _offset), bWidth, borderHeight), GUIContent.none, lineStyleBG);   
					GUI.Box (new Rect (center.x + ((-borderWidth) + _offset), center.y - (bWidth / 2), borderHeight, bWidth), GUIContent.none, lineStyleBG);   
					GUI.Box (new Rect (center.x - (((borderHeight - 1) - borderWidth) + _offset), center.y - (bWidth / 2), borderHeight, bWidth), GUIContent.none, lineStyleBG);  
					if (centerPoint) 
						GUI.Box (new Rect (center.x - (cbWidth / 2), center.y - (cbWidth / 2), cbWidth, cbWidth), GUIContent.none, lineStyleBG); 
				}     
				//--------------------------------------------------------------------------------------------------------------   
				// now draw the crosshair on top    
				GUI.Box (new Rect (center.x - (width / 2), center.y - ((newHeight - 1) + _offset), width, newHeight), GUIContent.none, lineStyle);    
				GUI.Box (new Rect (center.x - (width / 2), center.y + _offset, width, newHeight), GUIContent.none, lineStyle);    
				GUI.Box (new Rect (center.x + _offset, center.y - (width / 2), newHeight, width), GUIContent.none, lineStyle);    
				GUI.Box (new Rect (center.x - ((newHeight - 1) + _offset), center.y - (width / 2), newHeight, width), GUIContent.none, lineStyle);      
				if (centerPoint)  
					GUI.Box (new Rect (center.x - (centerWidth / 2), center.y - (centerWidth / 2), centerWidth, centerWidth), GUIContent.none, lineStyle);                        
			}   
		}       
		void ChangeColor () 
		{   
			Vector3 fwd = _camera.TransformDirection (Vector3.forward); 
			if (Physics.Raycast (_camera.position, fwd, out ray, targetScanDist)) 
			{
				string colliderTag = ray.collider.transform.tag;
				
				if( colliderTag == "Interact" )
				{
					Transform otherTag;
					
					if( ( otherTag = ray.collider.transform.Find("OtherTag") ) != null )
					{
						colliderTag = otherTag.tag;
					}
				}
				
				if (colliderTag == tagForEnemy)
				{
					lineStyle.normal.background = enemyTexture; 
				}
				else if (colliderTag == tagForFriend)
				{
				 lineStyle.normal.background = friendlyTexture; 
				}
				else
					lineStyle.normal.background = defaultTexture;     
			} 
			else
			{  
				lineStyle.normal.background = defaultTexture; 
			}       
		}   
   
		void Update () 
		{  
			FitToResolution ();  
			if (changeColor && showCrosshair)
				ChangeColor ();   
			if (showCrosshair && doAnimate)
				AnimateCrosshair ();    
			if (syncSecondary && secondaryCrosshair != null) 
				SyncSecondary (); 
		}   
  
		void AnimateCrosshair () 
		{    
			if(fade)
			{  
				FadeOutCrosshair (fadeSpeed); 
				return;  
			} 
			else if (_alpha != 1 || _offset < newOffset) 
			{ 
				FadeInCrosshair (fadeSpeed); 
			}   
			if (spread)   
				SpreadCrosshair (newSpreadAmount, newSpreadSpeed);
			else if (!shoot && _offset >= newOffset) 
				ReleaseCrosshair (newSpreadSpeed);   
			if (shoot && !spread && _offset >= newOffset) 
				ShootCrosshair (newShootAmount, newShootSpeed);   
		} 
  
		void SyncSecondary () 
		{ 
			secondaryCrosshair.spread = spread;  
			secondaryCrosshair.shoot = shoot;  
			secondaryCrosshair.fade = fade;
		}    

		void SpreadCrosshair (float amount, float speed)
		{  
			if (anim < amount)  
				anim += Time.deltaTime * speed; 
		}  
      
		void ReleaseCrosshair (float speed) 
		{ 
			if (anim > 0)  anim -= Time.deltaTime * speed;    
			anim = Mathf.Max (anim, 0);  
		}     

		void FadeOutCrosshair (float speed) 
		{
            if (_offset > 0)
            {
                //newOffset = Mathf.Max(newOffset, scopeOffset);
                anim -= Time.deltaTime * (speed * newOffset / 10);
            }
            if (fadeOutOnScope)
            {
                if (_alpha > 0) _alpha -= Time.deltaTime * (speed / 10);
                _alpha = Mathf.Max(_alpha, 0);
            }
		}   
 
		void FadeInCrosshair (float speed) 
		{   
			anim += Time.deltaTime * (speed * (newOffset / 10)); 
			if (_alpha < 1)
				_alpha += Time.deltaTime * speed / 10; 
			_alpha = Mathf.Clamp (_alpha, 0, 1);     
		}    
  
		void ShootCrosshair (float amount, float speed) 
		{   
			if (anim < amount)
				anim += Time.deltaTime * speed; 
			anim = Mathf.Clamp (anim, 0, amount); 
		}  
           
		void FitToResolution ()
		{    
			float screenRatio = Screen.height / 100;  
			newHeight = height * screenRatio; 
			newOffset = offset * screenRatio;  
			newSpreadAmount = spreadAmount * screenRatio; 
			newShootAmount = shootAmount * screenRatio; 
			newSpreadSpeed = spreadSpeed * screenRatio; 
			newShootSpeed = shootSpeed * screenRatio;  
		}   
   
		void SetTextureColors (Texture2D myTexture, Color myColor) 
		{  
			for (int y = 0; y < myTexture.height; ++y) 
			{  
				for (int x = 0; x < myTexture.width; ++x)
				{  
					myTexture.SetPixel (x, y, myColor); 
				}  
			}   
			myTexture.Apply (); 
		}

	}
}