using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.Controls;

namespace FPSControl
{
    [System.Serializable]
    public abstract class ControlMap
    {
        #region IDs
        public enum ControlID
        {
            NONE, Jump, Crouch, Run, Interact, Defend, Reload, Weapon1, Weapon2, Weapon3, Weapon4, WeaponCycle, Move, Look, Fire, Scope
        }
        /*public const string JUMP = "jump";
        public const string CROUCH = "crouch";
        public const string RUN = "run";
        public const string INTERACT = "interact";
        public const string DEFEND = "defend";
        public const string RELOAD = "reload";

        public const string WEAPON1 = "weapon 1";
        public const string WEAPON2 = "weapon 2";
        public const string WEAPON3 = "weapon 3";
        public const string WEAPON4 = "weapon 4";
        public const string WEAPON5 = "weapon 5";
        public const string WEAPON6 = "weapon 6";
        public const string WEAPON7 = "weapon 7";
        public const string WEAPON8 = "weapon 8";
        public const string WEAPON9 = "weapon 9";
        public const string WEAPON10 = "weapon 10";

        public const string MOVE_LEFT = "move left";
        public const string MOVE_RIGHT = "move right";
        public const string MOVE_FORWARD = "move forward";
        public const string MOVE_BACK = "move back";
        public const string MOVE_X = "move x";
        public const string MOVE_Y = "move y";

        public const string FIRE = "fire";
        public const string SCOPE = "scope";

        public const string LOOK_X = "look x";
        public const string LOOK_Y = "look y";*/
        #endregion // IDs

        public static T Create<T>(string name) where T : ControlMap, new()
        {
            T map = new T();
            map._name = name;
            return map;
        }

        [SerializeField] string _name = "";
        public string name { get { return _name; } private set { _name = value; } }
        public bool invertedLook = false;

        public ControlMap(string n) { name = n; SetIDs(); }

        public abstract void Initialize();

        public abstract bool GetJump();
        public abstract bool GetFire();
        public abstract bool GetScope();

        public abstract bool GetCrouch();
        public abstract bool GetRun();

        public abstract bool GetReload();
        public abstract bool GetDefend();
        public abstract bool GetInteract();

        public abstract bool GetWeapon1();
        public abstract bool GetWeapon2();
        public abstract bool GetWeapon3();
        public abstract bool GetWeapon4();
        public abstract bool GetWeaponToggle();

        //public abstract bool AlsoHasControl(

        public abstract Vector2 GetMovement();
        public abstract Vector2 GetLook();

        protected abstract void SetIDs();
    }

    [System.Serializable]
    public class DesktopControlMap : ControlMap
    {
        //Note: We constrain the time of control here as they are always dealt with at a component level from the same context.
        //Axis
        public DesktopAxis movement = new DesktopAxis();
        public DesktopAxis look = new DesktopAxis();

        //Buttons that are true while down
        public DesktopPersistantButton run = new DesktopPersistantButton();
        public DesktopPersistantButton crouch = new DesktopPersistantButton();
        public DesktopPersistantButton fire = new DesktopPersistantButton();
        public DesktopPersistantButton scope = new DesktopPersistantButton();
        public DesktopPersistantButton defend = new DesktopPersistantButton();

        //Buttons that are true only on the frame they are pressed
        public DesktopButton jump = new DesktopButton();
        public DesktopButton reload = new DesktopButton();
        public DesktopButton interact = new DesktopButton();
        public DesktopButton weaponToggle = new DesktopButton();
        public DesktopButton weapon1 = new DesktopButton();
        public DesktopButton weapon2 = new DesktopButton();
        public DesktopButton weapon3 = new DesktopButton();
        public DesktopButton weapon4 = new DesktopButton();

        public DesktopControlMap() : base("") { }
        public DesktopControlMap(string n) : base(n) { }

        override protected void SetIDs()
        {
            movement.SetID(ControlID.Move);
            look.SetID(ControlID.Look);

            run.SetID(ControlID.Run);
            crouch.SetID(ControlID.Crouch);
            fire.SetID(ControlID.Fire);
            scope.SetID(ControlID.Scope);
            defend.SetID(ControlID.Defend);

            jump.SetID(ControlID.Jump);
            reload.SetID(ControlID.Reload);
            interact.SetID(ControlID.Interact);
            weaponToggle.SetID(ControlID.WeaponCycle);
            weapon1.SetID(ControlID.Weapon1);
            weapon2.SetID(ControlID.Weapon2);
            weapon3.SetID(ControlID.Weapon3);
            weapon4.SetID(ControlID.Weapon4);
        }

        public ControlID GetIDBoundToControl(KeyCode keyCode)
        {
            if (ControlMatches(crouch, keyCode)) return crouch.controlID;
            if (ControlMatches(defend, keyCode)) return defend.controlID;
            if (ControlMatches(fire, keyCode)) return fire.controlID;
            if (ControlMatches(jump, keyCode)) return jump.controlID;
            if (ControlMatches(interact, keyCode)) return interact.controlID;
            if (ControlMatches(movement, keyCode)) return movement.controlID;
            if (ControlMatches(look, keyCode)) return look.controlID;
            if (ControlMatches(scope, keyCode)) return scope.controlID;
            if (ControlMatches(run, keyCode)) return run.controlID;
            if (ControlMatches(reload, keyCode)) return reload.controlID;
            if (ControlMatches(weapon1, keyCode)) return weapon1.controlID;
            if (ControlMatches(weapon2, keyCode)) return weapon2.controlID;
            if (ControlMatches(weapon3, keyCode)) return weapon3.controlID;
            if (ControlMatches(weapon4, keyCode)) return weapon4.controlID;
            if (ControlMatches(weaponToggle, keyCode)) return weaponToggle.controlID;
            return ControlID.NONE;
        }

        public ControlID GetIDBoundToMouseButton(int mouseButton)
        {
            if (ControlMatches(crouch, mouseButton)) return crouch.controlID;
            if (ControlMatches(defend, mouseButton)) return defend.controlID;
            if (ControlMatches(fire, mouseButton)) return fire.controlID;
            if (ControlMatches(jump, mouseButton)) return jump.controlID;
            if (ControlMatches(interact, mouseButton)) return interact.controlID;
            if (ControlMatches(scope, mouseButton)) return scope.controlID;
            if (ControlMatches(run, mouseButton)) return run.controlID;
            if (ControlMatches(reload, mouseButton)) return reload.controlID;
            if (ControlMatches(weapon1, mouseButton)) return weapon1.controlID;
            if (ControlMatches(weapon2, mouseButton)) return weapon2.controlID;
            if (ControlMatches(weapon3, mouseButton)) return weapon3.controlID;
            if (ControlMatches(weapon4, mouseButton)) return weapon4.controlID;
            if (ControlMatches(weaponToggle, mouseButton)) return weaponToggle.controlID;
            return ControlID.NONE;
        }

        public ControlID GetIDBoundToMouse()
        {
            if (movement.type == AxisType.Analogue) return movement.controlID;
            if (look.type == AxisType.Analogue) return look.controlID;
            else return ControlID.NONE;
        }

        bool ControlMatches(DesktopButton control, KeyCode key)
        {
            return control.key == key && control.peripheral == Peripheral.Keyboard;
        }

        bool ControlMatches(DesktopButton control, int mouseButton)
        {
            return control.mouseButton == mouseButton && control.peripheral == Peripheral.Mouse;
        }

        bool ControlMatches(DesktopAxis control, KeyCode key)
        {
            return control.type == AxisType.Digital && (control.negativeX == key || control.positiveX == key || control.negativeY == key || control.positiveY == key);
        }

        override public void Initialize() { }

        override public bool GetJump() { return jump.GetValue(); }
        override public bool GetFire() { /*Debug.Log("fire: " + fire.ToString());*/ return fire.GetValue(); }
        override public bool GetScope() { /*Debug.Log("scope: " + scope.ToString());*/ return scope.GetValue(); }

        override public bool GetCrouch() { return crouch.GetValue(); }
        override public bool GetRun() { return run.GetValue(); }

        override public bool GetReload() { return reload.GetValue(); }
        override public bool GetDefend() { return defend.GetValue(); }
        override public bool GetInteract() { return interact.GetValue(); }

        override public bool GetWeapon1() { return weapon1.GetValue(); }
        override public bool GetWeapon2() { return weapon2.GetValue(); }
        override public bool GetWeapon3() { return weapon3.GetValue(); }
        override public bool GetWeapon4() { return weapon4.GetValue(); }
        override public bool GetWeaponToggle() { return weaponToggle.GetValue(); }

        override public Vector2 GetMovement() { return movement.GetValue(); }
        override public Vector2 GetLook() { return look.GetValue(); }
    }

    [System.Serializable]
    public class OuyaControlMap : ControlMap
    {
        //Note: We constrain the time of control here as they are always dealt with at a component level from the same context.
        //Axis
        public DesktopAxis movement;
        public DesktopAxis look;

        //Buttons that are true while down
        public DesktopPersistantButton run;
        public DesktopPersistantButton crouch;
        public DesktopPersistantButton fire;
        public DesktopPersistantButton scope;
        public DesktopPersistantButton defend;

        //Buttons that are true only on the frame they are pressed
        public DesktopButton jump;
        public DesktopButton reload;
        public DesktopButton interact;
        public DesktopButton weaponToggle;
        public DesktopButton weapon1;
        public DesktopButton weapon2;
        public DesktopButton weapon3;
        public DesktopButton weapon4;
        
        public OuyaControlMap(string n) : base(n) { }

        override public void Initialize() { }

        override protected void SetIDs()
        {

        }

        override public bool GetJump() { return jump.GetValue(); }
        override public bool GetFire() { return fire.GetValue(); }
        override public bool GetScope() { return scope.GetValue(); }

        override public bool GetCrouch() { return crouch.GetValue(); }
        override public bool GetRun() { return run.GetValue(); }

        override public bool GetReload() { return reload.GetValue(); }
        override public bool GetDefend() { return defend.GetValue(); }
        override public bool GetInteract() { return interact.GetValue(); }

        override public bool GetWeapon1() { return weapon1.GetValue(); }
        override public bool GetWeapon2() { return weapon2.GetValue(); }
        override public bool GetWeapon3() { return weapon3.GetValue(); }
        override public bool GetWeapon4() { return weapon4.GetValue(); }
        override public bool GetWeaponToggle() { return weaponToggle.GetValue(); }

        override public Vector2 GetMovement() { return movement.GetValue(); }
        override public Vector2 GetLook() { return look.GetValue(); }
    }

    [System.Serializable]
    public class MobileControlMap : ControlMap
    {
        //Note: We constrain the time of control here as they are always dealt with at a component level from the same context.
        //Axis
        public DesktopAxis movement;
        public DesktopAxis look;

        //Buttons that are true while down
        public DesktopPersistantButton run;
        public DesktopPersistantButton crouch;
        public DesktopPersistantButton fire;
        public DesktopPersistantButton scope;
        public DesktopPersistantButton defend;

        //Buttons that are true only on the frame they are pressed
        public DesktopButton jump;
        public DesktopButton reload;
        public DesktopButton interact;
        public DesktopButton weaponToggle;
        public DesktopButton weapon1;
        public DesktopButton weapon2;
        public DesktopButton weapon3;
        public DesktopButton weapon4;
        
        public MobileControlMap(string n) : base(n) { }

        override public void Initialize() { }

        override protected void SetIDs()
        {

        }

        override public bool GetJump() { return jump.GetValue(); }
        override public bool GetFire() { return fire.GetValue(); }
        override public bool GetScope() { return scope.GetValue(); }

        override public bool GetCrouch() { return crouch.GetValue(); }
        override public bool GetRun() { return run.GetValue(); }

        override public bool GetReload() { return reload.GetValue(); }
        override public bool GetDefend() { return defend.GetValue(); }
        override public bool GetInteract() { return interact.GetValue(); }

        override public bool GetWeapon1() { return weapon1.GetValue(); }
        override public bool GetWeapon2() { return weapon2.GetValue(); }
        override public bool GetWeapon3() { return weapon3.GetValue(); }
        override public bool GetWeapon4() { return weapon4.GetValue(); }
        override public bool GetWeaponToggle() { return weaponToggle.GetValue(); }

        override public Vector2 GetMovement() { return movement.GetValue(); }
        override public Vector2 GetLook() { return look.GetValue(); }
    }

    [System.Serializable]
    public class SteamboxControlMap : ControlMap
    {
        //Note: We constrain the time of control here as they are always dealt with at a component level from the same context.
        //Axis
        public DesktopAxis movement;
        public DesktopAxis look;

        //Buttons that are true while down
        public DesktopPersistantButton run;
        public DesktopPersistantButton crouch;
        public DesktopPersistantButton fire;
        public DesktopPersistantButton scope;
        public DesktopPersistantButton defend;

        //Buttons that are true only on the frame they are pressed
        public DesktopButton jump;
        public DesktopButton reload;
        public DesktopButton interact;
        public DesktopButton weaponToggle;
        public DesktopButton weapon1;
        public DesktopButton weapon2;
        public DesktopButton weapon3;
        public DesktopButton weapon4;
        
        public SteamboxControlMap(string n) : base(n) { }

        override protected void SetIDs()
        {

        }

        override public void Initialize() { }

        override public bool GetJump() { return jump.GetValue(); }
        override public bool GetFire() { return fire.GetValue(); }
        override public bool GetScope() { return scope.GetValue(); }

        override public bool GetCrouch() { return crouch.GetValue(); }
        override public bool GetRun() { return run.GetValue(); }

        override public bool GetReload() { return reload.GetValue(); }
        override public bool GetDefend() { return defend.GetValue(); }
        override public bool GetInteract() { return interact.GetValue(); }

        override public bool GetWeapon1() { return weapon1.GetValue(); }
        override public bool GetWeapon2() { return weapon2.GetValue(); }
        override public bool GetWeapon3() { return weapon3.GetValue(); }
        override public bool GetWeapon4() { return weapon4.GetValue(); }
        override public bool GetWeaponToggle() { return weaponToggle.GetValue(); }

        override public Vector2 GetMovement() { return movement.GetValue(); }
        override public Vector2 GetLook() { return look.GetValue(); }
    }

    namespace Controls
    {
        public enum Peripheral { Mouse = 0, Keyboard = 1 }
        public enum AxisType { Analogue = 0, Digital = 1 }

        [System.Serializable]
        public abstract class InputControl
        {
            public ControlMap.ControlID controlID { get { return _controlID; } }
            [SerializeField] ControlMap.ControlID _controlID = ControlMap.ControlID.NONE;
            public void SetID(ControlMap.ControlID id)
            {
                if (_controlID != ControlMap.ControlID.NONE) throw new System.Exception("Error: IDs can only ever be set once.");
                _controlID = id;
            }
        }
        
        [System.Serializable]
        public class DesktopAxis : InputControl, IEnumerable
        {
            public const string MOUSE_X = "Mouse X";
            public const string MOUSE_Y = "Mouse Y";

            public AxisType type = AxisType.Analogue;

            public KeyCode positiveX = KeyCode.None;
            public KeyCode negativeX = KeyCode.None;
            public KeyCode positiveY = KeyCode.None;
            public KeyCode negativeY = KeyCode.None;

            public IEnumerator GetEnumerator()
            {
                yield return negativeX;
                yield return positiveX;
                yield return negativeY;
                yield return positiveY;
            }

            public override string ToString()
            {
                string s = "";

                if(type == AxisType.Analogue) s = "Mouse Axis: " + MOUSE_X + ", " + MOUSE_Y;
                else if (type == AxisType.Digital) s = "Keyboard Axis: X("+negativeX+","+positiveX+") , Y("+negativeY+","+positiveY+")";

                return s;
            }

            public Vector2 GetValue()
            {
                if (type == AxisType.Analogue)
                {
                    float x = Input.GetAxisRaw(MOUSE_X);
                    float y = Input.GetAxisRaw(MOUSE_Y);

                    return new Vector2(x, y);
                }
                else if (type == AxisType.Digital)
                {
                    float x = 0;
                    float y = 0;

                    bool nX = Input.GetKey(negativeX);
                    bool pX = Input.GetKey(positiveX);
                    bool nY = Input.GetKey(negativeY);
                    bool pY = Input.GetKey(positiveY);

                    if (nX && pX) x = 0;
                    else if (nX) x = -1;
                    else if (pX) x = 1;

                    if (nY && pY) y = 0;
                    else if (nY) y = -1;
                    else if (pY) y = 1;

                    return new Vector2(x, y);
                }

                return new Vector2();
            }
        }

        [System.Serializable]
        public class DesktopButton : InputControl
        {
            public Peripheral peripheral = Peripheral.Keyboard;
            public KeyCode key = KeyCode.None;
            public int mouseButton = 0;

            public DesktopButton() : base() { }

            public override string ToString()
            {
                string s = "";

                if (peripheral == Peripheral.Mouse) s = "Mouse Button: " + mouseButton;
                else if (peripheral == Peripheral.Keyboard) s = "Keyboard Button: "+key;

                return s;
            }

            public virtual bool GetValue()
            {
                //Debug.Log("Button");
                if (peripheral == Peripheral.Keyboard)
                    return Input.GetKeyDown(key);
                else if (peripheral == Peripheral.Mouse)
                    return Input.GetMouseButtonDown(mouseButton);
                else
                    return false;
            }
        }

        [System.Serializable]
        public class DesktopPersistantButton : DesktopButton
        {
            public DesktopPersistantButton() : base() { }
            
            override public bool GetValue()
            {
                //Debug.Log("Persistant Button");
                if (peripheral == Peripheral.Keyboard)
                    return Input.GetKey(key);
                else if (peripheral == Peripheral.Mouse)
                {
                    return Input.GetMouseButton(mouseButton);
                }
                else
                    return false;
            }
        }
    }
}
