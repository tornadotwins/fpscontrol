using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            SaveMenu();
            GUIWeaponSelect();
            if (weaponsAvailable) GUIWeaponTypeSelect();

            if (weaponsAvailable && currentWeapon.isRanged)
            {
                GUIRangeTypeSelect();
                switch (((FPSControlRangedWeapon)currentWeapon.weapon).rangedType)
                {
                    case FPSControlRangedWeaponType.Bullets:
                        GUITab(ref rangeBulletTabIndex, gui_button_weapon_ranged_group2_n, gui_button_weapon_ranged_group2_a, gui_button_weapon_ranged_group1_n, gui_button_weapon_ranged_group1_a);
                        if (rangeBulletTabIndex == 0)
                        {
                            GUIPositionWindow(0); //Implemented
                            GUIParticalWindow(1); //Implemented
                            GUIPathWindow(2); //Implemented
                            GUISoundWindow(3); //Implemented
                        }
                        else
                        {
                            GUIDamageWindow(0); //Implemented
                            GUIAmmoReloadingWindow(1); //Implemented
                            GUIFiringPatternWindow(2); //Implemented
                            GUIMeshAnimationWindow(3); //Implemented
                        }
                        break;
                    case FPSControlRangedWeaponType.Projectile:
                        GUITab(ref rangeProjectileTabIndex, gui_button_weapon_projectile_group1_n, gui_button_weapon_projectile_group1_a, gui_button_weapon_projectile_group2_n, gui_button_weapon_projectile_group2_a);
                        if (rangeProjectileTabIndex == 0)
                        {
                            GUIPositionWindow(0); //Implemented
                            GUIAmmoWindow(1);
                            GUIFiringPatternWindow(2); //Implemented
                            GUIMeshAnimationWindow(3); //Implemented
                        }
                        else
                        {
                            GUIPreFirePathWindow(0); //Implemented
                            GUIParticalWindow(1); //Implemented
                            GUISoundWindow(2); //Implemented
                        }
                        break;
                }
            }
            else if (weaponsAvailable) //Melee Weapon
            {
                GUITab(ref meleeTabIndex, gui_button_weapon_melee_group1_n, gui_button_weapon_melee_group1_a, gui_button_weapon_melee_group2_n, gui_button_weapon_melee_group2_a);
                if (meleeTabIndex == 0)
                {
                    GUIPositionWindow(0); //Implemented
                    GUIParticalWindow(1); //Implemented
                    GUIMeleeDamageWindow(2); //Implemented
                    GUIMeshAnimationWindow(3); //Implemented
                }
                else
                {
                    GUISoundWindow(0); //Implemented
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
        private void SaveMenu()
        {
            if (GUI.Button(new Rect(800, 65, 98, 24), "Save"))
            {
                SaveWeaponCopy();
            }
            if (GUI.Button(new Rect(700, 65, 98, 24), "Revert"))
            {
                RevertToSaved();
            }
            //if (GUI.Button(new Rect(600, 65, 98, 24), "ReloadWorkingCopy"))
            //{
            //    ReloadWorkingCopy();
            //}
        }


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
            }
            GUI.enabled = weaponsAvailable;
            if (GUI.Button(deleteRect, "-"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this?", "Delete", "Cancel"))
                {
                    FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
                    foreach (FPSControlPlayerWeaponManager manager in managers) //Go through all the managers and make sure we rerefrence the new one
                    {
                        List<FPSControlWeapon> actors = new List<FPSControlWeapon>(manager.weaponActors);
                        int index = actors.IndexOf(currentWeapon.weapon);
                        if (index != -1) actors.RemoveAt(index);
                        manager.weaponActors = actors.ToArray();
                    }
                    GameObject.DestroyImmediate(currentWeapon.weapon.transform.gameObject);
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
            currentWeapon.weapon.pivot.x = Knobs.MinMax(new Vector2(117, 35), currentWeapon.weapon.pivot.x, -2, 2, 1);
            currentWeapon.weapon.pivot.y = Knobs.MinMax(new Vector2(187, 35), currentWeapon.weapon.pivot.y, -2, 2, 2);
            currentWeapon.weapon.pivot.z = Knobs.MinMax(new Vector2(256, 35), currentWeapon.weapon.pivot.z, -2, 2, 3);
            currentWeapon.weapon.euler.x = Knobs.MinMax(new Vector2(117, 116), currentWeapon.weapon.euler.x, 0, 360, 4);
            currentWeapon.weapon.euler.y = Knobs.MinMax(new Vector2(187, 116), currentWeapon.weapon.euler.y, 0, 360, 5);
            currentWeapon.weapon.euler.z = Knobs.MinMax(new Vector2(256, 116), currentWeapon.weapon.euler.z, 0, 360, 6);   
            GUI.EndGroup();
        }

        private void GUIFiringPatternWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_firing_pattern.width, gui_window_firing_pattern.height), gui_window_firing_pattern, GUIStyle.none);
            currentWeapon.weaponAnimation.patternType = (FiringPatternType)GUI.SelectionGrid(new Rect(15, 36, 15, 35), (int)currentWeapon.weaponAnimation.patternType, new string[2] { "", "" }, 1, "toggle");
            currentWeapon.weaponAnimation.blend = GUI.Toggle(new Rect(43, 70, 15, 15), currentWeapon.weaponAnimation.blend, "");
            string fireClipName = ((FPSControlRangedWeapon)currentWeapon.weapon).weaponAnimation.FIRE;
            AnimationState fireAnimation = ((FPSControlRangedWeapon)currentWeapon.weapon).weaponAnimation.transform.animation[fireClipName];
            if (fireAnimation == null)
            {
                Rect rect = new Rect(15, 134, 290, 19);
                Color c = GUI.backgroundColor;
                GUI.backgroundColor = Color.black;
                GUI.Box(rect, "No fire animation set");
                GUI.backgroundColor = c;
            }
            else
            {
                FalloffSlider.FiringPatternSlider(((FPSControlRangedWeapon)currentWeapon.weapon).weaponAnimation.firingPattern, fireAnimation.clip, new Vector2(15, 134), Repaint);
            }
            GUI.EndGroup();
        }

        private void GUIMeshAnimationWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_mesh_animation.width, gui_window_mesh_animation.height), gui_window_mesh_animation, GUIStyle.none);

            GameObject meshPrefab;
            bool hasAnimations = GUIMeshAnimationWindow_HasAnimations();
            string dragText = (!hasAnimations ? "Drag" : "Animation");
            DragResultState dragResult = Drag.DragArea<GameObject>(new Rect(166, 43, 144, 16), out meshPrefab, dragText);
            if (hasAnimations && dragResult == DragResultState.Click)
            {
                if (EditorUtility.DisplayDialog("Caution!", "Are you sure you want to remove this?", "Discard", "Cancel"))
                {
                    GUIMeshAnimationWindow_RemoveMeshAndAnimations();
                }
            }
            else if (dragResult == DragResultState.Drag)
            {
                bool addNewMeshAndAnimation = true;
                if (hasAnimations)
                {
                    if (EditorUtility.DisplayDialog("Caution!", "Are you sure you want to overwrite your old mesh?", "Overwrite", "Cancel"))
                    {
                        GUIMeshAnimationWindow_RemoveMeshAndAnimations();
                    }
                    else
                    {
                        addNewMeshAndAnimation = false;
                    }
                }
                if (addNewMeshAndAnimation)
                {
                    GUIMeshAnimationWindow_AddMeshAndAnimations(meshPrefab);                    
                }
            }
            List<string> animationNames = new List<string>();
            if (!hasAnimations)
            {
                GUI.enabled = false;
                animationNames = new List<string>();
                animationNames.Add("NONE");
            } 
            else 
            {
                animationNames.Add(" ");
                foreach (AnimationState animationState in currentWeapon.weaponAnimation.animation)
                {
                    animationNames.Add(animationState.name);
                }
            }
            GUIMeshAnimationWindow_SubA(new Rect(72, 65, 81, 14), ref currentWeapon.weaponAnimation.ACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 65, 81, 14), ref currentWeapon.weaponAnimation.DEACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 87, 81, 14), ref currentWeapon.weaponAnimation.FIRE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 87, 81, 14), ref currentWeapon.weaponAnimation.EMPTY, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 109, 81, 14), ref currentWeapon.weaponAnimation.RELOAD, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 109, 81, 14), ref currentWeapon.weaponAnimation.WALK, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 131, 81, 14), ref currentWeapon.weaponAnimation.RUN, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 131, 81, 14), ref currentWeapon.weaponAnimation.IDLE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 152, 81, 14), ref currentWeapon.weaponAnimation.SCOPE_IO, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 152, 81, 14), ref currentWeapon.weaponAnimation.SCOPE_LOOP, animationNames);
            GUI.enabled = true;
            GUI.EndGroup();
        }

        private void GUIMeshAnimationWindow_SubA(Rect rect, ref string currentValue, List<string> possibleValues)
        {
            int currentIndex = possibleValues.IndexOf(currentValue);
            if (currentIndex == -1) currentIndex = 0;
            currentIndex = EditorGUI.Popup(rect, currentIndex, possibleValues.ToArray());
            if (currentIndex == 0)
            {
                currentValue = null;
            } 
            else 
            {
                currentValue = possibleValues[currentIndex];
            }            
        }

        private void GUIMeshAnimationWindow_AddMeshAndAnimations(GameObject meshPrefab) //handels adding animation
        {
            GameObject go = GameObject.Instantiate(meshPrefab) as GameObject;
            go.transform.parent = currentWeapon.weapon.transform;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.name = currentWeapon.weapon.name + " Model";
            ComponetHelper.CopyComponets(currentWeapon.modelControler, go.transform.GetChild(0), CopyComponetStyle.exclusive, typeof(Animation));
            ComponetHelper.CopyComponets(go.transform.GetChild(0), currentWeapon.modelControler, CopyComponetStyle.inclusive, false, typeof(Animation));
            GameObject.DestroyImmediate(currentWeapon.modelOffset.gameObject);
            CheckCurrentWeapon();
        }

        private void GUIMeshAnimationWindow_RemoveMeshAndAnimations() //handels removing all mesh and animation
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in currentWeapon.modelControler) children.Add(child.gameObject);
            children.ForEach(child => GameObject.DestroyImmediate(child));
            List<string> animationClipNames = new List<string>();
            foreach (AnimationState animationState in currentWeapon.weaponAnimation.animation) animationClipNames.Add(animationState.name);
            animationClipNames.ForEach(child => { currentWeapon.weaponAnimation.animation.RemoveClip(child); });
        }

        private bool GUIMeshAnimationWindow_HasAnimations() //handels removing all mesh and animation
        {
            if (currentWeapon.weaponAnimation.animation.GetClipCount() == 0) return false;
            foreach (AnimationState animationState in currentWeapon.weaponAnimation.animation)
            {
                if (animationState.clip != null) return true;
            }
            return false;
        }        

        private void GUIParticalWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_particle.width, gui_window_particle.height), gui_window_particle, GUIStyle.none);
            if (currentWeapon.weaponParticles.particles == null || currentWeapon.weaponParticles.particles.Length < 3) //Make sure we have the right amount of particals
            {
                List<FPSControlWeaponParticleData> newParticles;
                int fromIndex = 0;
                if (currentWeapon.weaponParticles.particles == null)
                {
                    newParticles = new List<FPSControlWeaponParticleData>();
                }
                else
                {
                    newParticles = new List<FPSControlWeaponParticleData>(currentWeapon.weaponParticles.particles);
                    fromIndex = currentWeapon.weaponParticles.particles.Length;
                }
                for (int i = fromIndex; i < 3; i++)
                {
                    newParticles.Add(new FPSControlWeaponParticleData());
                }
                currentWeapon.weaponParticles.particles = newParticles.ToArray();
            }
            GUIParticalWindow_Sub(new Vector2(13, 61), ref currentWeapon.weaponParticles.particles[0]);
            GUIParticalWindow_Sub(new Vector2(13, 87), ref currentWeapon.weaponParticles.particles[1]);
            GUIParticalWindow_Sub(new Vector2(13, 113), ref currentWeapon.weaponParticles.particles[2]);
            currentWeapon.weaponParticles.lightIsEnabled = GUI.Toggle(new Rect(20, 152, 15, 15), currentWeapon.weaponParticles.lightIsEnabled, "");
            CheckDragAreaForComponent<Light>(new Rect(39, 152, 66, 18), ref currentWeapon.weaponParticles.lightBurst);            
            CheckDragAreaForComponent<Transform>(new Rect(110, 152, 66, 18), ref currentWeapon.weaponParticles.lightPosition);
            GUI.EndGroup();
        }

        private void GUIParticalWindow_Sub(Vector2 topLeftPos, ref FPSControlWeaponParticleData partical)
        {
            GUI.BeginGroup(new Rect(topLeftPos.x, topLeftPos.y, 300, 22));
            partical.isEnabled = GUI.Toggle(new Rect(7, 2, 15, 15), partical.isEnabled, "");
            CheckDragAreaForObject<GameObject>(new Rect(26, 2, 66, 18), ref partical.particleSystem);
            CheckDragAreaForComponent<Transform>(new Rect(97, 2, 66, 18), ref partical.position);
            partical.global = (GUI.SelectionGrid(new Rect(181, 2, 31, 15), (partical.global ? 0 : 1), new string[2] { "", "" }, 2, "toggle") == 0 ? false : true);
            GUI.EndGroup();
        }

        private void GUISoundWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_sound.width, gui_window_sound.height), gui_window_sound, GUIStyle.none);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 44, 79, 17), ref currentWeapon.weaponSound.equipSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 44, 79, 17), ref currentWeapon.weaponSound.fire1SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 66, 79, 17), ref currentWeapon.weaponSound.fire2SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 66, 79, 17), ref currentWeapon.weaponSound.fire3SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 88, 79, 17), ref currentWeapon.weaponSound.reloadSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 88, 79, 17), ref currentWeapon.weaponSound.emptySFX);
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

        private void CheckDragAreaForObject<T>(Rect area, ref T checkInput) where T : UnityEngine.Object
        {
            string displayText = (checkInput == null ? "Drag" : checkInput.name);
            T outValue;
            DragResultState dragResult = Drag.DragArea<T>(area, out outValue, displayText);
            if (checkInput != null && dragResult == DragResultState.Click)
            {
                if (ConfirmRemove()) checkInput = default(T);
            }
            else if (dragResult == DragResultState.Drag)
            {
                checkInput = outValue;
            }
        }

        private int NumericTextfield(Rect rect, int currentValue)
        {
            return (int)NumericTextfield(rect, (float)currentValue);
        }

        private float NumericTextfield(Rect rect, float currentValue)
        {
            string inputString = GUI.TextField(rect, currentValue.ToString());
            if (inputString.Length > 0)
            {
                inputString = Regex.Replace(inputString, @"[^0-9]", "");
            }
            else
            {
                inputString = "0";
            }
            
            return float.Parse(inputString);
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
            currentWeapon.weaponPath.isPreFire = false;
            currentWeapon.weaponPath.render = GUI.Toggle(new Rect(13, 36, 15, 15), currentWeapon.weaponPath.render, "");
            CheckDragAreaForObject<Material>(new Rect(76, 58, 200, 18), ref currentWeapon.weaponPath.material);
            CheckDragAreaForComponent<Transform>(new Rect(76, 79, 200, 18), ref currentWeapon.weaponPath.origin);
            currentWeapon.weaponPath.consistentRender = GUI.Toggle(new Rect(15, 100, 15, 15), currentWeapon.weaponPath.consistentRender, "");
            GUI.EndGroup();
        }


        private float ammoKnobHelper;
        private void GUIAmmoReloadingWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo_reloading.width, gui_window_ammo_reloading.height), gui_window_ammo_reloading, GUIStyle.none);
            ((FPSControlRangedWeapon)currentWeapon.weapon).reloadType = (ReloadType)GUI.SelectionGrid(new Rect(35, 41, 276, 15), (int)((FPSControlRangedWeapon)currentWeapon.weapon).reloadType, new string[3] { "", "", "" }, 3, "toggle");
            GUI.enabled = (((FPSControlRangedWeapon)currentWeapon.weapon).reloadType == ReloadType.Clips);
            ((FPSControlRangedWeapon)currentWeapon.weapon).clipCapacity = Knobs.Incremental(new Vector2(139, 92), ref ammoKnobHelper, ((FPSControlRangedWeapon)currentWeapon.weapon).clipCapacity, Knobs.Increments.FIFTY, 21);
            GUI.enabled = (((FPSControlRangedWeapon)currentWeapon.weapon).reloadType == ReloadType.Recharge);
            ((FPSControlRangedWeapon)currentWeapon.weapon).maximumRounds = NumericTextfield(new Rect(221, 82, 83, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).maximumRounds);
            ((FPSControlRangedWeapon)currentWeapon.weapon).regenerationRate = NumericTextfield(new Rect(236, 123, 68, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).regenerationRate);
            ((FPSControlRangedWeapon)currentWeapon.weapon).fullRegenerationTime = NumericTextfield(new Rect(236, 158, 68, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).fullRegenerationTime);
            ((FPSControlRangedWeapon)currentWeapon.weapon).constantRegeneration = (GUI.SelectionGrid(new Rect(220, 105, 20, 75), (((FPSControlRangedWeapon)currentWeapon.weapon).constantRegeneration == true ? 0 : 1), new string[2] { "", "" }, 1, "toggle") == 0 ? true : false);
            GUI.enabled = true;
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
            if (((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff == null) ((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff = new FPSControl.Data.FalloffData();
            FalloffSlider.DamageSlider(((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff, new Vector2(13, 134), Repaint);
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
            Knobs.MinMax(new Vector2(39, 81), 50, 0, 100, 111);
            Knobs.MinMax(new Vector2(114, 81), 50, 0, 100, 112);
            Knobs.MinMax(new Vector2(180, 81), 50, 0, 100, 123);
            Knobs.MinMax(new Vector2(248, 81), 50, 0, 100, 114);
            GUI.EndGroup();
        }

        private void GUIPreFirePathWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_prefire_path.width, gui_window_prefire_path.height), gui_window_prefire_path, GUIStyle.none);
            currentWeapon.weaponPath.isPreFire = true;
            currentWeapon.weaponPath.render = GUI.Toggle(new Rect(13, 36, 15, 15), currentWeapon.weaponPath.render, "");
            CheckDragAreaForObject<Material>(new Rect(56, 58, 66, 18), ref currentWeapon.weaponPath.material);
            CheckDragAreaForComponent<Transform>(new Rect(56, 79, 66, 18), ref currentWeapon.weaponPath.origin);
            currentWeapon.weaponPath.maxDistance = Knobs.MinMax(new Vector2(182, 112), currentWeapon.weaponPath.maxDistance, 0, 500, 51);
            currentWeapon.weaponPath.leavingForce = Knobs.MinMax(new Vector2(253, 112), currentWeapon.weaponPath.leavingForce, 0, 100, 52);
            GUI.EndGroup();
        }
        #endregion


        #region Melee
        private void GUIMeleeDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_melee_damage.width, gui_window_melee_damage.height), gui_window_melee_damage, GUIStyle.none);
            CheckDragAreaForComponent<Collider>(new Rect(15, 50, 66, 18), ref ((FPSControlMeleeWeapon)currentWeapon.weapon).damageTrigger);
            currentWeapon.weapon.maxDamagePerHit = Knobs.MinMax(new Vector2(110, 35), currentWeapon.weapon.maxDamagePerHit, 0, 100, 101);
            GUI.EndGroup();
        }
        #endregion

        #endregion


        #region logic
        private static string TEMP_PREFAB { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TEMPWEAPON.prefab"; } }
        private FPSControlWeapon[] weapons;
        private WeaponData currentWeapon;
        private bool weaponsAvailable;        

        private void SetCurrentSave()
        {
            Undo.SetSnapshotTarget(currentWeapon.weapon, "weapon");
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget();

            //Undo.SetSnapshotTarget(currentWeapon.weaponParticles, "weaponParticles");
            //Undo.CreateSnapshot();
            //Undo.ClearSnapshotTarget();
        }
        
        private void SaveWeaponCopy()
        {
            Undo.SetSnapshotTarget(currentWeapon.weapon, "weapon");
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget();

            //Undo.SetSnapshotTarget(currentWeapon.weaponParticles, "weaponParticles");
            //Undo.CreateSnapshot();
            //Undo.ClearSnapshotTarget();
        }

        private void RevertToSaved()
        {
            Undo.SetSnapshotTarget(currentWeapon.weapon, "weapon");
            Undo.RestoreSnapshot();

            //Undo.SetSnapshotTarget(currentWeapon.weaponParticles, "weaponParticles");
            //Undo.RestoreSnapshot();
        }

        private void ReloadWorkingCopy()
        {
            
        }

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
            Init();
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
            AttackWeaponToManager();
        }

        private void AttackWeaponToManager()
        {
            FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
            if (managers.Length > 0)
            {
                List<FPSControlWeapon> actors = new List<FPSControlWeapon>(managers[0].weaponActors);
                actors.Add(currentWeapon.weapon);
                managers[0].weaponActors = actors.ToArray();
                currentWeapon.weapon.transform.parent = managers[0].transform;
                currentWeapon.weapon.transform.localPosition = Vector3.zero;
                currentWeapon.weapon.transform.localEulerAngles = Vector3.zero;
                currentWeapon.weapon.transform.localScale = Vector3.one;
            }
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
                go = currentWeapon.weapon.transform.gameObject;
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
            weapons = (FPSControlWeapon[])Resources.FindObjectsOfTypeAll(typeof(FPSControlWeapon));
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
            Debug.Log("Setting Weapon");
            currentWeapon = new WeaponData();
            currentWeapon.weapon = weapon;
            currentWeapon.isRanged = (weapon.GetType() == typeof(FPSControlRangedWeapon));
            CheckCurrentWeapon();
            SetCurrentSave();
        }

        private void CheckCurrentWeapon()
        {
            if (currentWeapon.modelOffset == null) currentWeapon.modelOffset = GetOrCreateChild(currentWeapon.weapon.transform, currentWeapon.weapon.name + " Model");
            if (currentWeapon.modelControler == null) currentWeapon.modelControler = GetOrCreateChild(currentWeapon.modelOffset, currentWeapon.weapon.name + " Controller");
            if (currentWeapon.weapon.weaponAnimation == null) currentWeapon.weapon.weaponAnimation = GetOrCreateComponent<FPSControlWeaponAnimation>(currentWeapon.modelControler);
            currentWeapon.weaponAnimation = currentWeapon.weapon.weaponAnimation;
            if (currentWeapon.weapon.weaponParticles == null) currentWeapon.weapon.weaponParticles = GetOrCreateComponent<FPSControlWeaponParticles>(currentWeapon.modelControler);
            currentWeapon.weaponParticles = currentWeapon.weapon.weaponParticles;
            if (currentWeapon.weapon.weaponSound == null) currentWeapon.weapon.weaponSound = GetOrCreateComponent<FPSControlWeaponSound>(currentWeapon.modelControler);
            currentWeapon.weaponSound = currentWeapon.weapon.weaponSound;
            if (currentWeapon.isRanged && ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath == null) ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath = GetOrCreateComponent<FPSControlWeaponPath>(currentWeapon.modelControler);
            if (currentWeapon.isRanged) currentWeapon.weaponPath = ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath;
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
