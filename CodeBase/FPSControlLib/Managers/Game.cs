/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;


namespace FPSControl
{

    //! Game manager singleton component.
    /*!
    The Game manager singleton handles key game-wide tasks, such the activation of intro scenes when apropriate, and general purpose GUI left & right progress
    bars. It has to be added to a single gameObject in a dedicated initial "loading" scene, so as to ensure
    it's readily available to all callers througout the entire game; otherwise, callers loading in parallel to the Game manager and trying to acquire
    references to it in their Awake or Start methods will likely trigger a null reference exception.
    
    Adding the Game manager to a second gameObject will cause it (the secondary gameObject) to be destroyed.
    */
    public class Game : MonoBehaviour
    {
    
        private static Game manager = null;

        /*!
        Static reference to the single Game manager instance throughout the entire game. In order to acquire it, callers simply have to point a reference
        of type Game to it.
        */
        public static Game Manager { get { return manager; } }
     
        /*!
        Game-wide right progress bar.
        */
        public ProgressBar rightProgressBar;
        /*!
        Game-wide left progress bar.
        */
        public ProgressBar leftProgressBar;
        
        /*!
        Game-wide gameObject used for the display of miscellaneous helper tooltips.
        */
        public GameObject helpText;
        
        private bool showMenu = false;
     
        /*!
        Saves a reference to the single, game-wide instance of the game manager component in the static #Manager
        property, and marks the containing gameObject as persistent throughout the entire game through Unity's <a href="http://docs.unity3d.com/Documentation/ScriptReference/Object.DontDestroyOnLoad.html">DontDestroyOnLoad</a> method;
        i.e. the gameObject containing the manager will survive all scene loading & unloading operations.
        
        If the static #Manager property is found to be not null, i.e. there's already a game manager in the scene, the new instance destroys
        itself & its containing gameObject to preserve the singleton characteristic of the manager.
        */
        private void Awake ()
        {
            if (manager == null) {
                manager = this;
            } else {
                DestroyImmediate (gameObject);
                return;
            }
            DontDestroyOnLoad (gameObject);
         
        }
     
        /*!
        Broadcasts the "FadeIn" and "Intro" messages to the \link MessengerControl messenger controller\endlink.
        */
        private void Start ()
        {
            MessengerControl.Broadcast ("FadeIn", "Intro");
        }
     
     
        /*!
        Assigns it's sole argument to the data controller property of the \link #rightProgressBar right progress bar\endlink.
        
        \param dc Source data controller.
        */
        public void SetRighthandDataController (DataController dc)
        {
            rightProgressBar.dataController = dc;
        }
     
     
        /*!
        Performs a frame-by-frame monitoring of specific user requests.
        
        If the 'H' key is pressed at any time during the game, the manager activates the \link #helpText help text\endlink gameObject in order to display its contents
        to the user.
        
        If the Escape key is pressed at any time, the Game manager toggles the state of the game menu and the cursor's visibility & lock mode:
        if hidden, the cursor is shown & unlocked; if shown, it is hidden & locked.
        
        If they 'Y' key is pressed while the game menu is active (defined in the OnGUI() method), the manager accepts this input as confirmation of the Quit button and quits the game.
        */
        public void Update ()
        {
            //--- toggle help text
            if (UnityEngine.Input.GetKeyUp (KeyCode.H)) {
                if (helpText != null)
                    helpText.active = !helpText.active;
            }
         
            //--- display Quit menu
            if (UnityEngine.Input.GetKeyUp (KeyCode.Escape)) {
                showMenu = !showMenu;
                Cursor.visible = showMenu;
                Cursor.lockState = showMenu ? CursorLockMode.Locked : CursorLockMode.None;
            }
         
            //--- accept "Y" keypress as quit confirmation
            if (showMenu && UnityEngine.Input.GetKeyUp (KeyCode.Y)) {
                Application.Quit ();
            }
        }
     

        /*!
        Brings up the game menu when the Escape key is pressed, as monitored by the Update() method, and presents the
        user with a clickable "Quit" confirmation dialog.
        */
        public void OnGUI ()
        {
            if (showMenu) {
                int boxWidth = Screen.width / 4;
                int boxHeight = Screen.height / 10;
                int leftOffset = 16;
             
                GUI.BeginGroup (new Rect (Screen.width - boxWidth - leftOffset, boxHeight, boxWidth, 100));
                GUI.Box (new Rect (0, 0, boxWidth, 120), "Quit ShellShock?");
                if (GUI.Button (new Rect (boxWidth / 3, 32, 50, 50), "Yes")) {
                    Application.Quit ();
                }
                GUI.EndGroup ();
            }
        }
    }
}