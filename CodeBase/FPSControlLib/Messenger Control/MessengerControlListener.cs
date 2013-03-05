/*
jmpp: Done!
*/
/*
 * Listener object for MessengerControl class
 */
using UnityEngine;
using System;


namespace FPSControl
{
 
    //! Abstract representation of listener actions triggered by the MessengerControl.
    /*!
    The MessengerControlListener class provides an abstraction layer over the callback actions that the MessengerControl manages for a particular message.
    Through this abstraction, the MessengerControl can easily determine if two listeners being added to or removed from a message are equal, and thus avoid
    unnecessary work.
    */
    public class MessengerControlListener
    {
        /*!
        */
        public Component component;
        /*!
        */
        public string callback;
     
        /*!
        MessengerControlListener constructor. Initializes a new class instance with the name of a callback to abstract and the component containing said callback.
        
        \param newComponent The component holding the callback to be abstracted by this MessengerControlListener instance.
        \param newCallback The callback to be abstracted by this MessengerControlListener instance.        
        */
        public MessengerControlListener (Component newComponent, string newCallback)
        {
            this.component = newComponent;
            this.callback = newCallback;
        }
     
        /*!
        Override of the <a href="http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx">System.Object.Equals()</a> method, providing a local definition of
        equality between two MessengerControlListener instances, the current one, i.e. "this", and the one received by parameter.
        
        The two MessengerControlListener objects are considered to be equal if the name of the callback actions they represent are equal, *and* if the components
        that hold said callbacks are the same. In all other cases, the objects being compared are considered not equal.
        
        \param listenerToCompare The MessengerControlListener instance that "this" should be compared against.
        
        \retval boolean False if listenerToCompare is not an instance of the MessengerControlListener class, if the components holding the callbacks they abstract
        are not the same, or if the callback names are not the same. Otherwise, i.e. if listenerToCompare is an instance of MessengerControlListener, and if the
        callbacks they abstract are the same, and if said callbacks belong to the same component, then the two MessengerControlListener objects are considered
        equal.
        */
        public override bool Equals (System.Object listenerToCompare)
        {
            if (!(listenerToCompare is MessengerControlListener)) {
                return false;
            }
            
            if ((this.component == ((MessengerControlListener)listenerToCompare).component) && (this.callback == ((MessengerControlListener)listenerToCompare).callback)) {
                return true;
            } else {
                return false;
            }
        }
     
        /*!
        Override of the <a href="http://msdn.microsoft.com/en-us/library/system.object.gethashcode.aspx">System.Object.GetHashCode()</a> method, providing a
        local definition of the object hashes used during equality testing of MessengerControlListener instances.
        
        \retval int The hashcode of the MessengerControlListener instance, based on the hashes of the callback name and of the component holding said callback.
        */
        public override int GetHashCode ()
        {
            return component.GetHashCode () ^ callback.GetHashCode ();
        }
    }
}