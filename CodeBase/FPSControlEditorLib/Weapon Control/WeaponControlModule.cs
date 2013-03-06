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
        private int currentTabIndex = 0;
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

            if (currentWeaponIsRanged)
            {
                GUIRangeTypeSelect();
                switch (((FPSControlRangedWeapon)currentWeapon).rangedType)
                {
                    case FPSControlRangedWeaponType.Bullets:
                        GUITab(gui_button_weapon_ranged_group1_n, gui_button_weapon_ranged_group1_a, gui_button_weapon_ranged_group2_n, gui_button_weapon_ranged_group2_a);
                        if (currentTabIndex == 0)
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
                        GUITab(gui_button_weapon_projectile_group1_n, gui_button_weapon_projectile_group1_a, gui_button_weapon_projectile_group2_n, gui_button_weapon_projectile_group2_a);
                        if (currentTabIndex == 0)
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
                GUITab(gui_button_weapon_melee_group1_n, gui_button_weapon_melee_group1_a, gui_button_weapon_melee_group2_n, gui_button_weapon_melee_group2_a);
                if (currentTabIndex == 0)
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
        private void GUITab(Texture tabA_normal, Texture tabA_active, Texture tabB_normal, Texture tabB_active)
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
            ((FPSControlRangedWeapon)currentWeapon).rangedType = (FPSControlRangedWeaponType)EditorGUI.EnumPopup(rangeTypeRect, ((FPSControlRangedWeapon)currentWeapon).rangedType);
        }

        private void GUIWeaponTypeSelect()
        {
            Rect radioRangeRect = new Rect(366, 149, 11, 11);
            Rect radioMeleeRect = new Rect(438, 149, 11, 11);
            bool newRange = GUI.Toggle(radioRangeRect, currentWeaponIsRanged, "");
            bool newMelee = GUI.Toggle(radioMeleeRect, !currentWeaponIsRanged, "");
            if (currentWeaponIsRanged != newRange || newMelee == currentWeaponIsRanged) //
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
                    if (currentWeapon.transform.childCount == 0 && currentWeapon.GetComponents<Component>().Length == 2) //No children and the weapon script is the only compon so we will destry the entire object
                    {
                        GameObject.DestroyImmediate(currentWeapon.gameObject);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(currentWeapon);
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
                if (currentWeaponIsRanged)
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
            Knobs.MinMax(new Vector2(117, 35), 50, 0, 100, 1);
            Knobs.MinMax(new Vector2(187, 35), 50, 0, 100, 2);
            Knobs.MinMax(new Vector2(256, 35), 50, 0, 100, 3);

            Knobs.MinMax(new Vector2(117, 116), 50, 0, 100, 4);
            Knobs.MinMax(new Vector2(187, 116), 50, 0, 100, 5);
            Knobs.MinMax(new Vector2(256, 116), 50, 0, 100, 6);

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
            
                       //testStrings
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
            Drag.DragArea<Transform>(new Rect(39, 152, 66, 18), "Drag");
            Drag.DragArea<Transform>(new Rect(110, 152, 66, 18), "Drag");

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

            ((FPSControlRangedWeapon)currentWeapon).maxDamagePerHit = Knobs.MinMax(new Vector2(21, 35), ((FPSControlRangedWeapon)currentWeapon).maxDamagePerHit, 0, 100, 31);
            ((FPSControlRangedWeapon)currentWeapon).disperseRadius = Knobs.MinMax(new Vector2(90, 35), ((FPSControlRangedWeapon)currentWeapon).disperseRadius, 0, 360, 32);
            ((FPSControlRangedWeapon)currentWeapon).raycasts = Knobs.Incremental(new Vector2(159, 35), ref damageKnobHelper, ((FPSControlRangedWeapon)currentWeapon).raycasts, Knobs.Increments.TWENTY, 41);
            FalloffSlider.DamageSlider(currentWeapon.damageFalloff, new Vector2(13, 134), Repaint);

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
        private FPSControlWeapon currentWeapon;
        private bool weaponsAvailable;
        private bool currentWeaponIsRanged;

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
            if (Selection.activeTransform == null || Selection.activeTransform.GetComponent<FPSControlWeapon>() != null)
            {
                GameObject go = new GameObject(userInput);
                CreateNewWeapon(go, userInput, true);
            }
            else
            {
                CreateNewWeapon(Selection.activeTransform.gameObject, userInput, true);
            }
        }

        private void CreateNewWeapon(GameObject go, string name, bool ranged)
        {
            if (ranged)
            {
                FPSControlRangedWeapon weapon = go.AddComponent<FPSControlRangedWeapon>();
                weapon.weaponName = name;
                SetCurrentWeapon(weapon);
                UpdateCurrentWeaponInfo();
            }
            else
            {
                FPSControlMeleeWeapon weapon = go.AddComponent<FPSControlMeleeWeapon>();
                weapon.weaponName = name;
                SetCurrentWeapon(weapon);
                UpdateCurrentWeaponInfo();
            }
        }

        private void SwitchRangeType()
        {
            GameObject go = currentWeapon.transform.gameObject;
            string weaponName = currentWeapon.name;
            GameObject.DestroyImmediate(currentWeapon);
            CreateNewWeapon(go, weaponName, !currentWeaponIsRanged);
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
                currentWeaponIndex = currentWeaponIndex;
                UpdateCurrentWeaponInfo();
            }
        }

        private void SetCurrentWeapon(FPSControlWeapon weapon)
        {
            currentWeapon = weapon;
            UpdateCurrentWeaponInfo();
        }

        private void UpdateCurrentWeaponInfo()
        {
            currentWeaponIsRanged = (currentWeapon.GetType() == typeof(FPSControlRangedWeapon));            
        }

        private void SaveChanges()
        {
            
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
