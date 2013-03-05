/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;

namespace FPSControl
{
 
    //! Generic progress bar GUI component.
    /*!
    Provides a graphical representation of the amount of available "health" in a DataController component, in the form of
    an evolving GUI progress bar.
    */
    public class ProgressBar : MonoBehaviour
    {
        /*!
        The DataController component from which the progress bar reads the "health" information to represent graphically.
        */
        public DataController dataController;
        /*!
        The foreground texture of the progress bar.
        */
        public Texture foregroundTexture;
        /*!
        The background texture of the progress bar.
        */
        public Texture backgroundTexture;
        /*!
        Texture describing an increasing amount of damage, in the form of Texture2D asset so that pixels can be read from it on the fly during the
        evolution of the progress bar.
        */
        public Texture2D damageTexture;
        /*!
        Whether the progress bar should be located on the right side of the screen.
        */
        public bool rightSide = false;
     
        /*!
        Draws the progress bar on the screen with the given #backgroundTexture at full width and full height, and the #foregroundTexture
        at a width determined by half of the screen width multiplied by a frame-by-frame reading of the \link DataController#normalizedData normalizedData\endlink
        property of the #dataController object.
        
        If the #rightSide parameter is true, the progress bar is flipped to the right side of the screen.
        
        As the progress bar changes throughout the game, a varying tint color is applied to it to represent the evolution of the "health" value that's
        being read from the #dataController object. This tint is given frame-by-frame by the color of the pixels in the #damageTexture asset at a
        position in normalized (u,v) coordinates within it, where v is always 0.5 and u is also determined by the reading of the \link DataController#normalizedData normalizedData\endlink
        property of the #dataController object.
        */
        void OnGUI ()
        {
         
            // Make rect 10 pixels from side edge and 6 pixels from top. 
            Rect rect = new Rect (10, 6, Screen.width / 2 - 10 - 40, backgroundTexture.height);
         
            // If this is a right side health bar, flip the rect.
            if (rightSide) {
                rect.x = Screen.width - rect.x;
                rect.width = -rect.width;
            }
         
            // Draw the background texture
            GUI.DrawTexture (rect, backgroundTexture);
         
            float data = dataController.normalizedData;
         
            // Multiply width with health before drawing the foreground texture
            rect.width *= data;
         
            // Get color from damage texture
            GUI.color = damageTexture.GetPixelBilinear (data, 0.5f);
         
            // Draw the foreground texture
            GUI.DrawTexture (rect, foregroundTexture);
         
            // Reset GUI color.
            GUI.color = Color.white;
        }
    }
}















