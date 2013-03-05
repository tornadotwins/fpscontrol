/*
jmpp: Done!
*/
/*
 * MessengerControl class for allowing game objects to communicate without being aware of
 * one another.
 *
 * Register a listener using: 
 * MessengerControl.AddListener(message:String, component:Component, callback:String)
 * 
 * Remove a listener using:
 * MessengerControl.RemoveListener(message:String, component:Component, callback:String)
 *
 * Broadcast a message using:
 * MessengerControl.Broadcast(message:String)
 * or
 * MessengerControl.Broadcast(message:String, parameter)
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace FPSControl
{
    //! Centralized message registering, broadcasting, & listener-action triggering service.
    /*!
    The MessengerControl compononet provides a centralized facility for the broadcasting of messages that have been registered with the controller, and triggering the
    "listener" actions that were registered for each broadcasted message.
    
    MessengerControl proxies the addition & removal of each message's listeners through corresponding AddListener() & RemoveListener() static methods,
    each accepting the message to be managed, the listener to be added or removed, and the component in which the listener action lives.
    */
    public class MessengerControl : MonoBehaviour
    {
        private static Dictionary<string,List<MessengerControlListener> > messages = new Dictionary<string,List<MessengerControlListener> > ();
     
        /*!
        Accepts as arguments a message to which a new "listener" is to be added, the new listener callback function that should
        be triggered upon broadcasts of the message, and the component that holds this callback action.
        
        If any of these arguments is null, AddListener emits a suitable warning and returns immediately.
        
        If the message to which the listener is being added is not already registered with the controller, a new empty list of listeners is created
        for it and the passed-in action is added as its first callback. If the message already exists, the new listener is added to the end of its
        existing callback list.
        
        \param message The message to which a listener action is being added.
        \param component The component that holds the listener action being added to the message.
        \param callback The name of the callback action to add as listener of the message.
        */
        public static void AddListener (string message, Component component, string callback)
        {
            if ((message == null) || (component == null) || (callback == null)) {
                Debug.Log ("Attempt to add a Listener with null arguments!");
                return;
            }
         
            if (!messages.ContainsKey (message)) {
                messages [message] = new List<MessengerControlListener> ();
            }
         
            MessengerControlListener listener = new MessengerControlListener (component, callback);
            messages [message].Add (listener);
        }


        /*!
        Accepts as arguments the message from which a "listener" is being removed, the old listener callback function that
        should no longer be triggered upon broadcasts of the message, and the component that holds this callback.
        
        If the passed-in listener is not registered for the requested message, or if the requested message is not currently being managed by the
        controller, a suitable warning is emitted and no further actions are taken. If, on the other hand, the list of callbacks for the requested 
        message becomes empty upon successful listener removal, the message itself is also removed from the list of messages managed & broadcasted
        by the controller.
        
        \param message The message from which a listener action is being removed.
        \param component The component that holds the listener action being removed from the message.
        \param callback The name of the callback action to remove from the list of listeners of the message.
        */
        public static void RemoveListener (string message, Component component, string callback)
        {
            MessengerControlListener listener = new MessengerControlListener (component, callback);
         
            if (messages.ContainsKey (message)) {
                List<MessengerControlListener> listeners = messages [message];
                bool listenerRemoved = false;
             
                // Use a for loop instead of foreach to do the removal in one pass without
                // breaking iterators
                for (int i = 0; i < listeners.Count; i++) {
                    if (listeners [i].Equals (listener)) {
                        messages [message].Remove (listeners [i]);
                        listenerRemoved = true;
                        i--;
                    }
                }
             
                if (!listenerRemoved) {
                    Debug.Log ("Message does not contain a matching Listener");
                } else {
                    if (messages [message].Count <= 0) {
                        messages.Remove (message);
                    }
                }
            } else {
                Debug.Log ("Messenger does not contain any Listeners for this message");
            }
        }
     
        public static void Broadcast (string message)
        {
            MessengerControl.Broadcast (message, null);
        }
 
        /*!
        Accepts a string representing the message to be broadcasted and an object of parameters that should be passed to each of its listeners, null by default.
        
        If the requested message is not currently being managed by the controller, a suitable warning is printed and no further actions are taken.
        Otherwise, each callback registered through AddListener() for the requested message is executed on a first-registered/first-called basis,
        with the object of parameters as their sole argument.
        
        \param message Name of the message to be broadcasted.
        \param parameter The sole argument that listener actions will be passed upon invocation due to message broadcasts.
        */
        public static void Broadcast (string message, object parameter)
        {
            if (messages.ContainsKey (message)) {
                // Clone the listener list here prior to iterating through it. We do this because it's possible
                // that delivering the message will cause the receiver to add or remove a listener, which
                // could interfere with our looping.
                List<MessengerControlListener> listeners = new List<MessengerControlListener> (messages [message]);
             
                foreach (MessengerControlListener oneListener in listeners) {
                    if (parameter == null) {
                        oneListener.component.gameObject.SendMessage (oneListener.callback);
                    } else {
                        oneListener.component.gameObject.SendMessage (oneListener.callback, parameter);
                    }
                }
            } else {
                Debug.Log ("Messenger does not contain any Listeners for this message");
            }
        }
    }
}
