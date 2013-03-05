/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;


namespace FPSControl
{

    //! General purpose frame-by-frame "health" counting component.
    /*!
    Provides "health bar"-like counting functionality for other components that may want to limit their actions based on the
    amount of available "health" at any time throughout the game, such as precisely a "health manager" or even an ammunition-constrained weapon.
    */
    public class DataController : MonoBehaviour
    {
        /*!
        Maximum ammount of "health", with a default value of 100.
        */
        public float max = 100;
        /*!
        Current amount of "health" at any point in the game, defaulting to the same value of 100 as the parameter #max.
        */
        public float current = 100;
        /*!
        Initial ammount of "health", defaulting to the same value of 100 as of 100 as the parameter #max.
        */
        public float initial = 100;
        /*!
        Minimum amount of "health", defaulting to 0.
        */
        public float min = 0;
        /*!
        Speed at which the counter replenishes itself, with a default value of 3.
        */
        public float healingSpeed = 3;
        /*!
        Whether or not the counter replenishes itself, defaulting to false.
        */
        public bool autoHeal = false;
        /*!
        Whether the counter has already been initialized, with an initial value of false. This property is hidden in the Unity inspector.
        */
        [HideInInspector]
        public bool initialized = false;
     
        //   public float normalizedData { get { return min / max; } }
        /*!
        Normalized representation of the current amount of "health" at any point in the game, with respect to its max value.
        */
        public float normalizedData { get { return current / max; } }
     
        /*!
        If #initialized is false, the #current value of "health" in the controller is set to the #initial value.
        
        If the #min parameter is greater than #max, it is bounded down to #max.
        */
        void Start ()
        {        
            if (!initialized)
                current = initial;
            if (min > max)
                min = max;
            initialized = true;
        }
     
        /*!
        Returns early in the first frame of the game, i.e. when <a href="http://docs.unity3d.com/Documentation/ScriptReference/Time-deltaTime.html">Time.deltaTime</a>
        is zero, whenever the game is paused (<a href="http://docs.unity3d.com/Documentation/ScriptReference/Time-timeScale.html">time scale</a> set to zero),
        or when the #autoHeal parameter is false.
        
        If none of these conditions are met, and the #current value of "health" is greater than #min, it is increased every frame at the
        rate dictated by #healingSpeed, until it reaches the \link #max maximum\endlink value, at which point it is clamped.
        */
        void Update ()
        {
            if (Time.deltaTime == 0 || Time.timeScale == 0)
                return;
         
            if (!autoHeal)
                return;
         
            if (current > min)
                current += Time.deltaTime * healingSpeed;
            current = Mathf.Clamp (current, min, max);
        }
    }
}