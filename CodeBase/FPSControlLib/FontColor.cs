/*
jmpp: Done!
*/
using UnityEngine;


namespace FPSControl
{
    //! Programatic interface for the modification of a material's tint color.
    /*!
    The FontColor component provides a programatic interface to change the tint color of the materials in the <a href="http://docs.unity3d.com/Documentation/ScriptReference/GUIText.html">guiText</a>
    or <a href="http://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">renderer</a> components that may be attached to the gameObject this component is also attached to.
    */      
    public class FontColor : MonoBehaviour
    {
        /*!
        Color tint that should be applied to the <a href="http://docs.unity3d.com/Documentation/ScriptReference/GUIText-material.html">material</a> of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/GUIText.html">guiText</a>
        component attached to this gameObject, defaulting to <a href="http://docs.unity3d.com/Documentation/ScriptReference/Color-black.html">Color.black</a>.
        */      
        public Color textColor = Color.black;
     
        /*!
        Calls the SetColor() method with the #textColor argument.
        */      
        public void Awake ()
        {
            SetColor (textColor);
        }
     
  
        /*!
        Sets the #textColor instance variable to the passed-in _color parameter. Additionally, if the gameObject this component is attached to possesses either a <a href="http://docs.unity3d.com/Documentation/ScriptReference/GUIText.html">guiText</a>
        or <a href="http://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">renderer</a> component, the tint color properties of their corresponding
        materials are also set to the value of _color.
        
        \param _color The tint <a href="http://docs.unity3d.com/Documentation/ScriptReference/Color.html">color</a> to set the #textColor variable to and to apply to the available materials.
        */      
        public void SetColor (Color _color)
        {
            textColor = _color;
         
            if (guiText != null) {
                guiText.material.color = _color;
            }
         
            if (renderer != null) {
                renderer.material.color = _color;
            }
        }
    }
}