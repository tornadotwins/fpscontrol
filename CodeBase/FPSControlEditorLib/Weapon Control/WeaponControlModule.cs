using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    class WeaponControlModule : FPSControlEditorModule
    {
        //Path
        const string guiFolder = "Weapon Control/";

        #region GUI Properties
        Rect[] windowSpaces = new Rect[4]{
            new Rect(258, 216, 330, 184),
            new Rect(582, 216, 330, 184),
            new Rect(258, 401, 330, 184),
            new Rect(582, 401, 330, 184)
        };
        Rect[] buttonTabs = new Rect[2]{
            new Rect(306, 182, 115, 30),
            new Rect(421, 182, 115, 30)
        };
        private int rangeBulletTabIndex = 0;
        private int rangeProjectileTabIndex = 0;
        private int meleeTabIndex = 0;
        private Rect backgroundRect = new Rect(247, 50, 660, 580);
        private Rect noWeaponsBox = new Rect(258, 137, 660, 580);
        private string[] testStrings = new string[] { "Test1", "Test2", "Test3", "Test4", "Test5" };
        #endregion 


        #region GUI
        public override void OnGUI()
        {
            GUIDrawBackground();
            GUIWeaponSelect();
            GUIWeaponTypeSelect();

            if (currentWeapon.isRanged)
            {
                GUIRangeTypeSelect();
                switch (((FPSControlRangedWeapon)currentWeapon.weapon).rangedType)
                {
                    case FPSControlRangedWeaponType.Bullets:
                        GUITab(ref rangeBulletTabIndex, gui_button_weapon_ranged_group1_n, gui_button_weapon_ranged_group1_a, gui_button_weapon_ranged_group2_n, gui_button_weapon_ranged_group2_a);
                        if (rangeBulletTabIndex == 0)
                        {
                            GUIPositionWindow(0);
                            GUIParticalWindow(1);
                            GUIPathWindow(2);
                            GUISoundWindow(3);
                        }
                        else
                        {
                            GUIDamageWindow(0);
                            GUIAmmoReloadingWindow(1);
                            GUIFiringPatternWindow(2);
                            GUIMeshAnimationWindow(3);
                        }
                        break;
                    case FPSControlRangedWeaponType.Projectile:
                        GUITab(ref rangeProjectileTabIndex, gui_button_weapon_projectile_group1_n, gui_button_weapon_projectile_group1_a, gui_button_weapon_projectile_group2_n, gui_button_weapon_projectile_group2_a);
                        if (rangeProjectileTabIndex == 0)
                        {
                            GUIPositionWindow(0);
                            GUIAmmoWindow(1);
                            GUIFiringPatternWindow(2);
                            GUIMeshAnimationWindow(3);
                        }
                        else
                        {
                            GUIPreFirePathWindow(0);
                            GUIParticalWindow(1);
                            GUISoundWindow(2);
                        }
                        break;
                }
            }
            else //Melee Weapon
            {
                GUITab(ref meleeTabIndex, gui_button_weapon_melee_group1_n, gui_button_weapon_melee_group1_a, gui_button_weapon_melee_group2_n, gui_button_weapon_melee_group2_a);
                if (meleeTabIndex == 0)
                {
                    GUIPositionWindow(0);
                    GUIParticalWindow(1);
                    GUIMeleeDamageWindow(2);
                    GUIMeshAnimationWindow(3);
                }
                else
                {
                    GUISoundWindow(0);
                }
            }
            
            if (!weaponsAvailable)
            {
                GUI.DrawTexture(noWeaponsBox, gui_cover);
                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;
                GUI.Label(noWeaponsBox, "No definitions exist for this type. \nPlease add one", gs);
            }   
        
        }


        #region commonUIElements
        private void GUITab(ref int currentTabIndex, Texture tabA_normal, Texture tabA_active, Texture tabB_normal, Texture tabB_active)
        {
            if (GUI.Button(buttonTabs[0], (currentTabIndex == 0 ? tabA_active : tabA_normal), GUIStyle.none))
            {
                currentTabIndex = 0;
            }
            if (GUI.Button(buttonTabs[1], (currentTabIndex == 1 ? tabB_active : tabB_normal), GUIStyle.none))
            {
                currentTabIndex = 1;
            }
        }

        private void GUIRangeTypeSelect()
        {
            Rect rangeTypeRect = new Rect(669, 150, 139, 14);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangedType = (FPSControlRangedWeaponType)EditorGUI.EnumPopup(rangeTypeRect, ((FPSControlRangedWeapon)currentWeapon.weapon).rangedType);
        }

        private void GUIWeaponTypeSelect()
        {
            Rect radioRangeRect = new Rect(366, 149, 11, 11);
            Rect radioMeleeRect = new Rect(438, 149, 11, 11);
            bool newRange = GUI.Toggle(radioRangeRect, currentWeapon.isRanged, "");
            bool newMelee = GUI.Toggle(radioMeleeRect, !currentWeapon.isRanged, "");
            if (currentWeapon.isRanged != newRange || newMelee == currentWeapon.isRanged) //
            {
                if (EditorUtility.DisplayDialog("Caution!", "Do this will discard previous settings \n Are you sure you want to do this?", "Discard", "Cancel"))
                {
                    SwitchRangeType();
                }
            }
        }

        private void GUIWeaponSelect()
        {
            Rect popupRect = new Rect(365, 114, 139, 14);
            Rect deleteRect = new Rect(507, 114, 15, 14);
            Rect newRect = new Rect(527, 114, 76, 14);
            List<string> weaponNames = new List<string>();
            if (!weaponsAvailable) 
            {
                weaponNames.Add("NONE");
            }
            else
            {
                foreach(FPSControlWeapon w in weapons) {
                    weaponNames.Add(w.weaponName);
                }
            }
            int newWeaponIndex = EditorGUI.Popup(popupRect, currentWeaponIndex, weaponNames.ToArray());
            if (weaponsAvailable && newWeaponIndex != currentWeaponIndex)
            {
                Debug.Log("Selecting Weapon");
                currentWeaponIndex = newWeaponIndex;
                SetCurrentWeapon(weapons[currentWeaponIndex]);
                //Selection.activeTransform = currentWeapon.transform;
            }
            GUI.enabled = weaponsAvailable;
            if (GUI.Button(deleteRect, "-"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this?", "Delete", "Cancel"))
                {
                    if (currentWeapon.weapon.transform.childCount == 0 && currentWeapon.weapon.GetComponents<Component>().Length == 2) //No children and the weapon script is the only compon so we will destry the entire object
                    {
                        GameObject.DestroyImmediate(currentWeapon.weapon.gameObject);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(currentWeapon.weapon);
                    }
                    Init();
                    return;
                }
            }
            GUI.enabled = true;
            if (GUI.Button(newRect, "Add New"))
            {
                Prompt("Weapon Name");
            }
        }

        private void GUIDrawBackground()
        {
            if (weaponsAvailable)
            {
                if (currentWeapon.isRanged)
                {
                    GUI.DrawTexture(backgroundRect, gui_background_range);
                }
                else
                {
                    GUI.DrawTexture(backgroundRect, gui_background_non_range);
                } 
            }
            else
            {
                GUI.DrawTexture(backgroundRect, gui_background_range);
            }
        }
        #endregion


        #region CommonWindows
        private void GUIPositionWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_position.width, gui_window_position.height), gui_window_position, GUIStyle.none);

            Knobs.Theme(Knobs.Themes.WHITE, _editor);

            Vector3 newPos = currentWeapon.modelOffset.localPosition;
            newPos.x = Knobs.MinMax(new Vector2(117, 35), newPos.x, -2, 2, 1);
            newPos.y = Knobs.MinMax(new Vector2(187, 35), newPos.y, -2, 2, 2);
            newPos.z = Knobs.MinMax(new Vector2(256, 35), newPos.z, -2, 2, 3);
            currentWeapon.modelOffset.localPosition = newPos;

            Vector3 newRot = currentWeapon.modelOffset.localEulerAngles;
            newRot.x = Knobs.MinMax(new Vector2(117, 116), newRot.x, 0, 360, 4);
            newRot.y = Knobs.MinMax(new Vector2(187, 116), newRot.y, 0, 360, 5);
            newRot.z = Knobs.MinMax(new Vector2(256, 116), newRot.z, 0, 360, 6);
            currentWeapon.modelOffset.localEulerAngles = newRot;            

            GUI.EndGroup();
        }

        private void GUIFiringPatternWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_firing_pattern.width, gui_window_firing_pattern.height), gui_window_firing_pattern, GUIStyle.none);

            GUI.SelectionGrid(new Rect(15, 36, 15, 35), 0, new string[2] { "", "" }, 1, "toggle");
            GUI.Toggle(new Rect(43, 70, 15, 15), true, "");
            
            //FalloffSlider.FiringPatternSlider(((FPSControlRangedWeapon)currentWeapon).

            GUI.EndGroup();
        }

        private void GUIMeshAnimationWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_mesh_animation.width, gui_window_mesh_animation.height), gui_window_mesh_animation, GUIStyle.none);

            Drag.DragArea<Transform>(new Rect(166, 43, 144, 16), "Drag Here");

            EditorGUI.Popup(new Rect(72, 65, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(222, 65, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(72, 87, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(222, 87, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(72, 109, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(222, 109, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(72, 131, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(222, 131, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(72, 152, 81, 14), 0, testStrings);
            EditorGUI.Popup(new Rect(222, 152, 81, 14), 0, testStrings);

            GUI.EndGroup();
        }

        private void GUIParticalWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_particle.width, gui_window_particle.height), gui_window_particle, GUIStyle.none);

            GUIParticalWindow_Sub(new Vector2(13, 61));
            GUIParticalWindow_Sub(new Vector2(13, 87));
            GUIParticalWindow_Sub(new Vector2(13, 113));

            GUI.Toggle(new Rect(20, 152, 15, 15), true, "");

            CheckDragAreaForComponent<Light>(new Rect(39, 152, 66, 18), ref currentWeapon.weapon.weaponParticles.lightBurst);
            
            CheckDragAreaForComponent<Transform>(new Rect(110, 152, 66, 18), ref currentWeapon.weapon.weaponParticles.lightPosition);

            GUI.EndGroup();
        }

        private void GUIParticalWindow_Sub(Vector2 topLeftPos)
        {
            GUI.BeginGroup(new Rect(topLeftPos.x, topLeftPos.y, 300, 22));
            GUI.Toggle(new Rect(7, 2, 15, 15), true, "");
            Drag.DragArea<Transform>(new Rect(26, 2, 66, 18), "Drag");
            Drag.DragArea<Transform>(new Rect(97, 2, 66, 18), "Drag");
            GUI.SelectionGrid(new Rect(181, 2, 31, 15), 0, new string[2] { "", "" }, 2, "toggle");
            GUI.EndGroup();
        }

        private void GUISoundWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_sound.width, gui_window_sound.height), gui_window_sound, GUIStyle.none);

            Drag.DragArea<AudioClip>(new Rect(72, 44, 79, 17), "Drag");
            Drag.DragArea<AudioClip>(new Rect(223, 44, 79, 17), "Drag");
            Drag.DragArea<AudioClip>(new Rect(72, 66, 79, 17), "Drag");
            Drag.DragArea<AudioClip>(new Rect(223, 66, 79, 17), "Drag");
            Drag.DragArea<AudioClip>(new Rect(72, 88, 79, 17), "Drag");
            Drag.DragArea<AudioClip>(new Rect(223, 88, 79, 17), "Drag");

            GUI.EndGroup();
        }

        private void CheckDragAreaForComponent<T>(Rect area, ref T checkInput) where T : UnityEngine.Component
        {
            string displayText = (checkInput == null ? "Drag" : checkInput.name);
            GameObject[] outValues;
            DragResultState dragResult = Drag.DragArea<GameObject>(area, out outValues, displayText);
            if (checkInput != null && dragResult == DragResultState.Click)
            {
                if (ConfirmRemove()) checkInput = default(T);
            }
            else if (dragResult == DragResultState.Drag)
            {
                foreach (GameObject go in outValues)
                {
                    T component = go.GetComponent<T>();
                    if (component != null)
                    {
                        checkInput = component;
                        return;
                    }
                }
            }
        }

        private bool ConfirmRemove()
        {
            return EditorUtility.DisplayDialog("Caution!", "Are you sure you want to remove this?", "Discard", "Cancel");
        }
        #endregion


        #region Range_Bullets
        private void GUIPathWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_path.width, gui_window_path.height), gui_window_path, GUIStyle.none);

            GUI.Toggle(new Rect(15, 36, 15, 15), true, "");

            DragResultState dragResult;
            Material mat;
            dragResult = Drag.DragArea<Material>(new Rect(76, 58, 200, 18), out mat, "Drag Here");
            if (dragResult == DragResultState.Drag)
            {
                
            }
            else if (dragResult == DragResultState.Click)
            {

            }

            Transform t;
            dragResult = Drag.DragArea<Transform>(new Rect(76, 79, 200, 18), out t, "Drag Here");
            if (dragResult == DragResultState.Drag)
            {
                
            }
            else if (dragResult == DragResultState.Click)
            {

            }

            GUI.Toggle(new Rect(15, 100, 15, 15), true, "");

            GUI.EndGroup();
        }

        
        private void GUIAmmoReloadingWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo_reloading.width, gui_window_ammo_reloading.height), gui_window_ammo_reloading, GUIStyle.none);
            GUI.SelectionGrid(new Rect(35, 41, 276, 15), 0, new string[3] { "", "", "" }, 3, "toggle");
            Knobs.MinMax(new Vector2(139, 92), 50, 0, 100, 21);
            GUI.TextField(new Rect(221, 82, 83, 16), "100");
            GUI.TextField(new Rect(236, 123, 68, 16), "0.5");
            GUI.TextField(new Rect(236, 158, 68, 16), "8");
            GUI.SelectionGrid(new Rect(220, 105, 20, 75), 0, new string[2] { "", "" }, 1, "toggle");
            GUI.EndGroup();
        }

        private float damageKnobHelper;
        private void GUIDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_damage.width, gui_window_damage.height), gui_window_damage, GUIStyle.none);
            ((FPSControlRangedWeapon)currentWeapon.weapon).maxDamagePerHit = Knobs.MinMax(new Vector2(21, 35), ((FPSControlRangedWeapon)currentWeapon.weapon).maxDamagePerHit, 0, 100, 31);
            ((FPSControlRangedWeapon)currentWeapon.weapon).disperseRadius = Knobs.MinMax(new Vector2(90, 35), ((FPSControlRangedWeapon)currentWeapon.weapon).disperseRadius, 0, 360, 32);
            ((FPSControlRangedWeapon)currentWeapon.weapon).raycasts = Knobs.Incremental(new Vector2(159, 35), ref damageKnobHelper, ((FPSControlRangedWeapon)currentWeapon.weapon).raycasts, Knobs.Increments.TWENTY, 41);
            FalloffSlider.DamageSlider(currentWeapon.weapon.damageFalloff, new Vector2(13, 134), Repaint);

            GUI.EndGroup();
        }       
        #endregion    
    

        #region Range_Projectile
        private void GUIAmmoWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo.width, gui_window_ammo.height), gui_window_ammo, GUIStyle.none);
            Drag.DragArea<Transform>(new Rect(77, 41, 66, 18), "Drag");
            Drag.DragArea<Transform>(new Rect(216, 41, 66, 18), "Drag");
            Knobs.MinMax(new Vector2(39, 81), 50, 0, 100, 51);
            Knobs.MinMax(new Vector2(114, 81), 50, 0, 100, 51);
            Knobs.MinMax(new Vector2(180, 81), 50, 0, 100, 51);
            Knobs.MinMax(new Vector2(248, 81), 50, 0, 100, 51);
            GUI.EndGroup();
        }

        private void GUIPreFirePathWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_prefire_path.width, gui_window_prefire_path.height), gui_window_prefire_path, GUIStyle.none);
            GUI.Toggle(new Rect(13, 36, 15, 15), true, "");
            Drag.DragArea<Material>(new Rect(56, 58, 66, 18), "Drag");
            Drag.DragArea<Transform>(new Rect(56, 79, 66, 18), "Drag");
            Knobs.MinMax(new Vector2(182, 112), 50, 0, 100, 51);
            Knobs.MinMax(new Vector2(253, 112), 50, 0, 100, 51);
            GUI.EndGroup();
        }
        #endregion


        #region Melee
        private void GUIMeleeDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_melee_damage.width, gui_window_melee_damage.height), gui_window_melee_damage, GUIStyle.none);
            Drag.DragArea<Transform>(new Rect(15, 50, 66, 18), "Drag");
            Knobs.MinMax(new Vector2(110, 35), 50, 0, 100, 11);
            GUI.EndGroup();
        }
        #endregion

        #endregion


        #region logic
        private FPSControlWeapon[] weapons;

        private CurrentWeaponData currentWeapon;
        private bool weaponsAvailable;
        //private bool currentWeaponIsRanged;

        internal int currentWeaponIndex
        {
            get
            {
                return EditorPrefs.GetInt("_FPSControl_CurrentWeaponIndex", 0);
            }
            set
            {
                EditorPrefs.SetInt("_FPSControl_CurrentWeaponIndex", value);
            }
        }
                
        public WeaponControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.WeaponControl;
        }

        public override void Init()
        {
            base.Init();
            LoadAssets();
            LocateWeapons();
        }

        override public void OnFocus(bool rebuild)
        {
            Debug.Log("Focused.");
            LoadAssets();
            LocateWeapons();
            base.OnFocus(rebuild);
        }

        override public void OnLostFocus(bool rebuild)
        {
            Debug.Log("Lost Focus.");
            base.OnLostFocus(rebuild);
        }

        public override void Deinit()
        {
            base.Deinit();
        }

        public override void Update()
        {

        }

        public override void OnPromptInput(string userInput)
        {
            CreateNewWeapon(userInput, true, true);
        }

        private void CreateNewWeapon(string name, bool ranged, bool newObject)
        {
            GameObject go;
            if (newObject)
            {
                go = new GameObject(name);
            }
            else
            {
                go = currentWeapon.transform.gameObject;
            }
            if (ranged)
            {
                FPSControlRangedWeapon weapon = go.AddComponent<FPSControlRangedWeapon>();
                weapon.weaponName = name;
                SetCurrentWeapon(weapon);
            }
            else
            {
                FPSControlMeleeWeapon weapon = go.AddComponent<FPSControlMeleeWeapon>();
                weapon.weaponName = name;
                SetCurrentWeapon(weapon);
            }            
        }

        private void SwitchRangeType()
        {
            GameObject go = currentWeapon.weapon.transform.gameObject;
            string weaponName = currentWeapon.weapon.name;
            FPSControlWeapon oldWeapon = currentWeapon.weapon;
            CreateNewWeapon(weaponName, !currentWeapon.isRanged, false);
            FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
            foreach (FPSControlPlayerWeaponManager manager in managers) //Go through all the managers and make sure we rerefrence the new one
            {
                for (int i = 0; i < manager.weaponActors.Length; i++)
                {
                    if (manager.weaponActors[i] == oldWeapon)
                    {
                        manager.weaponActors[i] = currentWeapon.weapon;
                    }
                }
            }
            GameObject.DestroyImmediate(oldWeapon);            
        }

        private void LocateWeapons()
        {
            weapons = (FPSControlWeapon[])GameObject.FindSceneObjectsOfType(typeof(FPSControlWeapon));
            weaponsAvailable = (weapons.Length > 0);
            if (weaponsAvailable)
            {
                if (currentWeaponIndex >= weapons.Length)
                {
                    currentWeaponIndex = 0;
                }
                SetCurrentWeapon(weapons[currentWeaponIndex]);
            }
        }

        private void SetCurrentWeapon(FPSControlWeapon weapon)
        {
            currentWeapon = new CurrentWeaponData();
            currentWeapon.weapon = weapon;
            currentWeapon.transform = weapon.transform;
            currentWeapon.isRanged = (weapon.GetType() == typeof(FPSControlRangedWeapon));
            CheckCurrentWeapon();
        }

        private void CheckCurrentWeapon()
        {
            if (currentWeapon.modelOffset == null) currentWeapon.modelOffset = GetOrCreateChild(currentWeapon.weapon.transform, currentWeapon.weapon.name + " Model");
            if (currentWeapon.modelControler == null) currentWeapon.modelControler = GetOrCreateChild(currentWeapon.modelOffset, currentWeapon.weapon.name + " Controller");
            if (currentWeapon.weapon.weaponAnimation == null) currentWeapon.weapon.weaponAnimation = GetOrCreateComponent<FPSControlWeaponAnimation>(currentWeapon.modelControler);
            if (currentWeapon.weapon.weaponParticles == null) currentWeapon.weapon.weaponParticles = GetOrCreateComponent<FPSControlWeaponParticles>(currentWeapon.modelControler);
            if (currentWeapon.weapon.weaponSound == null) currentWeapon.weapon.weaponSound = GetOrCreateComponent<FPSControlWeaponSound>(currentWeapon.modelControler);
            if (currentWeapon.isRanged && ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath == null) ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath = GetOrCreateComponent<FPSControlWeaponPath>(currentWeapon.modelControler);
        }

        private Transform GetOrCreateChild(Transform parent, string name)
        {
            if (parent.childCount == 0)
            {
                Transform returnTransform = new GameObject(name).transform;
                returnTransform.parent = parent;
                returnTransform.localEulerAngles = Vector3.zero;
                returnTransform.localPosition = Vector3.zero;
                return returnTransform;
            }
            return parent.GetChild(0);
        }

        private T GetOrCreateComponent<T>(Transform t) where T : Component
        {
            T returnComponent = t.GetComponent<T>();
            if (returnComponent == null) returnComponent = t.gameObject.AddComponent<T>();
            return returnComponent;
        }
        #endregion


        #region GFX
        //Common
        Texture gui_background_range;
        Texture gui_background_non_range;
        Texture gui_cover;
        Texture window_placeholder;

        //Tabs
        Texture gui_button_weapon_ranged_group1_a;
        Texture gui_button_weapon_ranged_group1_n;
        Texture gui_button_weapon_ranged_group2_a;
        Texture gui_button_weapon_ranged_group2_n;
        Texture gui_button_weapon_projectile_group1_a;
        Texture gui_button_weapon_projectile_group1_n;
        Texture gui_button_weapon_projectile_group2_a;
        Texture gui_button_weapon_projectile_group2_n;
        Texture gui_button_weapon_melee_group1_a;
        Texture gui_button_weapon_melee_group1_n;
        Texture gui_button_weapon_melee_group2_a;
        Texture gui_button_weapon_melee_group2_n;

        //Windows
        Texture gui_window_particle;
        Texture gui_window_path;
        Texture gui_window_position;
        Texture gui_window_sound;
        Texture gui_window_mesh_animation;
        Texture gui_window_ammo_reloading;
        Texture gui_window_firing_pattern;
        Texture gui_window_damage;
        Texture gui_window_ammo;
        Texture gui_window_prefire_path;
        Texture gui_window_melee_damage;

        private void LoadAssets()
        {
            //Common
            gui_background_range = LoadPNG(guiFolder, "background_range");
            gui_background_non_range = LoadPNG(guiFolder, "background_non_range");
            gui_cover = LoadPNG(guiFolder, "cover");
            
            //Tabs
            gui_button_weapon_ranged_group1_a = LoadPNG(guiFolder, "button_weapon_ranged_group1_a");
            gui_button_weapon_ranged_group1_n = LoadPNG(guiFolder, "button_weapon_ranged_group1_n");
            gui_button_weapon_ranged_group2_a = LoadPNG(guiFolder, "button_weapon_ranged_group2_a");
            gui_button_weapon_ranged_group2_n = LoadPNG(guiFolder, "button_weapon_ranged_group2_n");
            gui_button_weapon_projectile_group1_a = LoadPNG(guiFolder, "gui_button_weapon_projectile_group1_a");
            gui_button_weapon_projectile_group1_n = LoadPNG(guiFolder, "gui_button_weapon_projectile_group1_n");
            gui_button_weapon_projectile_group2_a = LoadPNG(guiFolder, "gui_button_weapon_projectile_group2_a");
            gui_button_weapon_projectile_group2_n = LoadPNG(guiFolder, "gui_button_weapon_projectile_group2_n");
            gui_button_weapon_melee_group1_a = LoadPNG(guiFolder, "gui_button_weapon_melee_group1_a");
            gui_button_weapon_melee_group1_n = LoadPNG(guiFolder, "gui_button_weapon_melee_group1_n");
            gui_button_weapon_melee_group2_a = LoadPNG(guiFolder, "gui_button_weapon_melee_group2_a");
            gui_button_weapon_melee_group2_n = LoadPNG(guiFolder, "gui_button_weapon_melee_group2_n");

            //Windows
            window_placeholder = LoadPNG(guiFolder, "window_placeholder");
            gui_window_particle = LoadPNG(guiFolder, "window_particle");
            gui_window_path = LoadPNG(guiFolder, "window_path");
            gui_window_position = LoadPNG(guiFolder, "window_position");
            gui_window_sound = LoadPNG(guiFolder, "window_sound");
            gui_window_mesh_animation = LoadPNG(guiFolder, "window_mesh_animation");
            gui_window_ammo_reloading = LoadPNG(guiFolder, "window_ammo_reloading");
            gui_window_firing_pattern = LoadPNG(guiFolder, "window_firing_pattern");
            gui_window_damage = LoadPNG(guiFolder, "window_damage");
            gui_window_ammo = LoadPNG(guiFolder, "window_ammo");
            gui_window_prefire_path = LoadPNG(guiFolder, "window_prefire_path");
            gui_window_melee_damage = LoadPNG(guiFolder, "window_melee_damage");
        }
        #endregion

    }
}
