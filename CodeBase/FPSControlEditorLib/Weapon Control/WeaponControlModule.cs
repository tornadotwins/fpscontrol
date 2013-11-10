using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;
using FPSControl.Definitions;

namespace FPSControlEditor
{
    class WeaponControlModule : FPSControlEditorModule
    {
        //Path
        const string guiFolder = "Weapon Control/";

        public override string version
        {
            get
            {
                return "1.1";
            }
        }

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
                switch (((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.rangedType)
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
                            GUIAmmoWindow(1); //Implemented
                            GUIFiringPatternWindow(2); //Implemented
                            GUIAmmoReloadingWindow(3);                            
                        }
                        else
                        {
                            GUIPreFirePathWindow(0); //Implemented
                            GUIParticalWindow(1); //Implemented
                            GUISoundWindow(2); //Implemented
                            GUIMeshAnimationWindow(3); //Implemented
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
            else
            {
                if (GUI.changed)
                {
                    //Debug.Log("Changed");
                }
            }
        }


        #region commonUIElements
        private void SaveMenu()
        {
            if (GUI.Button(new Rect(800, 65, 98, 24), "Save"))
            {
                SaveWeaponCopy(currentWeapon.weapon);
            }
            if (GUI.Button(new Rect(700, 65, 98, 24), "Revert"))
            {
                RevertSavedCopy(currentWeapon.weapon);
            }
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
            GUI.enabled = !Application.isPlaying;
            Rect rangeTypeRect = new Rect(669, 150, 139, 14);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.rangedType = (FPSControlRangedWeaponType)EditorGUI.EnumPopup(rangeTypeRect, ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.rangedType);
            GUI.enabled = true;
        }

        private void GUIWeaponTypeSelect()
        {
            Rect radioRangeRect = new Rect(366, 149, 11, 11);
            Rect radioMeleeRect = new Rect(438, 149, 11, 11);
            
            bool newRange = GUI.Toggle(radioRangeRect, currentWeapon.isRanged, "");
            bool newMelee = GUI.Toggle(radioMeleeRect, !currentWeapon.isRanged, "");
            if (currentWeapon.isRanged != newRange || newMelee == currentWeapon.isRanged)
            {
                if (Application.isPlaying)
                {
                    EditorUtility.DisplayDialog("Error", "Cant do this in play mode", "OK");
                } 
                else if (EditorUtility.DisplayDialog("Caution!", "Do this will discard previous settings \n Are you sure you want to do this?", "Discard", "Cancel"))
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
                    weaponNames.Add(w.definition.weaponName);
                }
            }
            int newWeaponIndex = EditorGUI.Popup(popupRect, currentWeaponIndex, weaponNames.ToArray());
            if (weaponsAvailable && newWeaponIndex != currentWeaponIndex)
            {
                //Debug.Log("Selecting Weapon");
                currentWeaponIndex = newWeaponIndex;
                RevertSavedCopy(currentWeapon.weapon);
                SetCurrentWeapon(weapons[currentWeaponIndex]);                
            }
            GUI.enabled = weaponsAvailable && !Application.isPlaying;
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
            GUI.enabled = !Application.isPlaying;
            if (GUI.Button(newRect, "Add New"))
            {
                Prompt("Weapon Name");
            }
            GUI.enabled = true;
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
        private int pivotSelectHelper = 0;
        private void GUIPositionWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_position.width, gui_window_position.height), gui_window_position, GUIStyle.none);
            Knobs.Theme(Knobs.Themes.WHITE, _editor);
            if (currentWeapon.isRanged)
            {
                pivotSelectHelper = EditorGUI.Popup(new Rect(14, 50, 88, 15), pivotSelectHelper, new string[2] { "View Port", "Scope" });
                if (pivotSelectHelper == 0)
                {
                    if (Application.isPlaying && currentWeapon.weapon.scoped)
                    {
                        currentWeapon.weapon.ExitScope();
                        currentWeapon.weapon.Parent.Player.playerCamera.Scope(false, currentWeapon.weapon.Parent.Player.playerCamera.baseFOV);
                    }
                    GUIPositionWindow_SubA(ref currentWeapon.weapon.definition.pivot, ref currentWeapon.weapon.definition.euler);
                }
                else
                {
                    if (Application.isPlaying && !currentWeapon.weapon.scoped)
                    {
                        currentWeapon.weapon.Scope();
                        currentWeapon.weapon.Parent.Player.playerCamera.Scope(true, currentWeapon.weapon.definition.scopeFOV);
                    }
                    GUIPositionWindow_SubA(ref currentWeapon.weapon.definition.scopePivot, ref currentWeapon.weapon.definition.scopeEuler);
                }
            }
            else
            {
                GUIPositionWindow_SubA(ref currentWeapon.weapon.definition.pivot, ref currentWeapon.weapon.definition.euler);
            }
            GUI.EndGroup();
        }

        private void GUIPositionWindow_SubA(ref Vector3 pivot, ref Vector3 euler)
        {
            pivot.x = Knobs.MinMax(new Vector2(117, 35), pivot.x, -3, 3, 1);
            pivot.y = Knobs.MinMax(new Vector2(187, 35), pivot.y, -3, 3, 2);
            pivot.z = Knobs.MinMax(new Vector2(256, 35), pivot.z, -3, 3, 3);
            euler.x = Knobs.MinMax(new Vector2(117, 116), euler.x, -360, 360, 4);
            euler.y = Knobs.MinMax(new Vector2(187, 116), euler.y, -360, 360, 5);
            euler.z = Knobs.MinMax(new Vector2(256, 116), euler.z, -360, 360, 6); 
        }

        private void GUIFiringPatternWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_firing_pattern.width, gui_window_firing_pattern.height), gui_window_firing_pattern, GUIStyle.none);
            currentWeapon.weapon.weaponAnimation.definition.patternType = (FiringPatternType)GUI.SelectionGrid(new Rect(15, 36, 15, 35), (int)currentWeapon.weapon.weaponAnimation.definition.patternType, new string[2] { "", "" }, 1, "toggle");
            currentWeapon.weapon.weaponAnimation.definition.blend = GUI.Toggle(new Rect(43, 70, 15, 15), currentWeapon.weapon.weaponAnimation.definition.blend, "");
            string fireClipName = ((FPSControlRangedWeapon)currentWeapon.weapon).weaponAnimation.definition.FIRE;
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
                GUI.enabled = !Application.isPlaying;
                FalloffSlider.FiringPatternSlider(((FPSControlRangedWeapon)currentWeapon.weapon).weaponAnimation.firingPattern, fireAnimation.clip, new Vector2(15, 134), Repaint);
                GUI.enabled = true;
            }
            GUI.EndGroup();
            GUI.enabled = true;
        }

        private void GUIMeshAnimationWindow(int windowIndex)
        {
            GUI.enabled = !Application.isPlaying;
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
                foreach (AnimationState animationState in currentWeapon.weapon.weaponAnimation.animation)
                {
                    animationNames.Add(animationState.name);
                }
            }
            GUIMeshAnimationWindow_SubA(new Rect(72, 65, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.ACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 65, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.DEACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 87, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.FIRE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 87, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.EMPTY, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 109, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.RELOAD, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 109, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.WALK, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 131, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.RUN, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 131, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.IDLE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 152, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.SCOPE_IO, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 152, 81, 14), ref currentWeapon.weapon.weaponAnimation.definition.SCOPE_LOOP, animationNames);
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
            ComponetHelper.CopyComponets(currentWeapon.modelControler, go.transform.GetChild(0), CopyComponetStyle.exclusive, typeof(Animation), typeof(LineRenderer));
            ComponetHelper.CopyComponets(go.transform.GetChild(0), currentWeapon.modelControler, CopyComponetStyle.inclusive, false, typeof(Animation), typeof(LineRenderer));
            GameObject.DestroyImmediate(currentWeapon.modelOffset.gameObject);
            CheckCurrentWeapon();
        }

        private void GUIMeshAnimationWindow_RemoveMeshAndAnimations() //handels removing all mesh and animation
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in currentWeapon.modelControler) children.Add(child.gameObject);
            children.ForEach(child => GameObject.DestroyImmediate(child));
            List<string> animationClipNames = new List<string>();
            foreach (AnimationState animationState in currentWeapon.weapon.weaponAnimation.animation) animationClipNames.Add(animationState.name);
            animationClipNames.ForEach(child => { currentWeapon.weapon.weaponAnimation.animation.RemoveClip(child); });
        }

        private bool GUIMeshAnimationWindow_HasAnimations() //handels removing all mesh and animation
        {
            if (currentWeapon.weapon.weaponAnimation.animation.GetClipCount() == 0) return false;
            foreach (AnimationState animationState in currentWeapon.weapon.weaponAnimation.animation)
            {
                if (animationState.clip != null) return true;
            }
            return false;
        }        

        private void GUIParticalWindow(int windowIndex)
        {
            GUI.enabled = !Application.isPlaying;
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_particle.width, gui_window_particle.height), gui_window_particle, GUIStyle.none);
            if (currentWeapon.weapon.weaponParticles.particles == null || currentWeapon.weapon.weaponParticles.particles.Length < 3) //Make sure we have the right amount of particals
            {
                List<FPSControlWeaponParticleData> newParticles;
                int fromIndex = 0;
                if (currentWeapon.weapon.weaponParticles.particles == null)
                {
                    newParticles = new List<FPSControlWeaponParticleData>();
                }
                else
                {
                    newParticles = new List<FPSControlWeaponParticleData>(currentWeapon.weapon.weaponParticles.particles);
                    fromIndex = currentWeapon.weapon.weaponParticles.particles.Length;
                }
                for (int i = fromIndex; i < 3; i++)
                {
                    newParticles.Add(new FPSControlWeaponParticleData());
                }
                currentWeapon.weapon.weaponParticles.particles = newParticles.ToArray();
            }
            GUIParticalWindow_Sub(new Vector2(13, 61), ref currentWeapon.weapon.weaponParticles.particles[0]);
            GUIParticalWindow_Sub(new Vector2(13, 87), ref currentWeapon.weapon.weaponParticles.particles[1]);
            GUIParticalWindow_Sub(new Vector2(13, 113), ref currentWeapon.weapon.weaponParticles.particles[2]);
            currentWeapon.weapon.weaponParticles.lightIsEnabled = GUI.Toggle(new Rect(20, 152, 15, 15), currentWeapon.weapon.weaponParticles.lightIsEnabled, "");
            CheckDragAreaForComponent<Light>(new Rect(39, 152, 66, 18), ref currentWeapon.weapon.weaponParticles.lightBurst);
            CheckDragAreaForComponent<Transform>(new Rect(110, 152, 66, 18), ref currentWeapon.weapon.weaponParticles.lightPosition);
            GUI.EndGroup();
            GUI.enabled = true;
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
            GUI.enabled = !Application.isPlaying;
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_sound.width, gui_window_sound.height), gui_window_sound, GUIStyle.none);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 44, 79, 17), ref currentWeapon.weapon.weaponSound.equipSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 44, 79, 17), ref currentWeapon.weapon.weaponSound.fire1SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 66, 79, 17), ref currentWeapon.weapon.weaponSound.fire2SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 66, 79, 17), ref currentWeapon.weapon.weaponSound.fire3SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 88, 79, 17), ref currentWeapon.weapon.weaponSound.reloadSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 88, 79, 17), ref currentWeapon.weapon.weaponSound.emptySFX);
            GUI.EndGroup();
            GUI.enabled = true;
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
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.isPreFire = false;
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.render = GUI.Toggle(new Rect(13, 36, 15, 15), ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.render, "");
            GUI.enabled = !Application.isPlaying;
            CheckDragAreaForObject<Material>(new Rect(76, 58, 200, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.material);            
            CheckDragAreaForComponent<Transform>(new Rect(76, 79, 200, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.origin);
            GUI.enabled = true;
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.consistentRender = GUI.Toggle(new Rect(15, 100, 15, 15), ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.consistentRender, "");
            impactIndex = EditorGUI.Popup(new Rect(125, 143, 150, 15), impactIndex, impactNames.ToArray());
            currentWeapon.weapon.impactName = impactNames[impactIndex];
            GUI.EndGroup();
        }

        private void GUIAmmoReloadingWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo_reloading.width, gui_window_ammo_reloading.height), gui_window_ammo_reloading, GUIStyle.none);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.reloadType = (ReloadType)GUI.SelectionGrid(new Rect(35, 41, 276, 15), (int)((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.reloadType, new string[3] { "", "", "" }, 3, "toggle");
            GUI.enabled = (((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.reloadType == ReloadType.Clips);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.clipCapacity = Knobs.MinMax(new Vector2(139, 92), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.clipCapacity, 0, 50, 21, true);
            GUI.enabled = (((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.reloadType == ReloadType.Recharge);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.maximumRounds = NumericTextfield(new Rect(221, 82, 83, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.maximumRounds);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.regenerationRate = NumericTextfield(new Rect(236, 123, 68, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.regenerationRate);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.fullRegenerationTime = NumericTextfield(new Rect(236, 158, 68, 16), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.fullRegenerationTime);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.constantRegeneration = (GUI.SelectionGrid(new Rect(220, 105, 20, 75), (((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.constantRegeneration == true ? 0 : 1), new string[2] { "", "" }, 1, "toggle") == 0 ? true : false);
            GUI.enabled = true;
            GUI.EndGroup();
        }

        private void GUIDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_damage.width, gui_window_damage.height), gui_window_damage, GUIStyle.none);
            ((FPSControlRangedWeapon)currentWeapon.weapon).definition.maxDamagePerHit = Knobs.MinMax(new Vector2(21, 35), ((FPSControlRangedWeapon)currentWeapon.weapon).definition.maxDamagePerHit, 0, 100, 31);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.disperseRadius = Knobs.MinMax(new Vector2(90, 35), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.disperseRadius, 0, 1, 32);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.raycasts = Knobs.MinMax(new Vector2(159, 35), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.raycasts, 1, 40, 598, true);
            if (((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff == null) ((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff = new FPSControl.Data.FalloffData();
            GUI.enabled = !Application.isPlaying;
            FalloffSlider.DamageSlider(((FPSControlRangedWeapon)currentWeapon.weapon).damageFalloff, new Vector2(13, 134), Repaint);
            GUI.enabled = true;
            GUI.EndGroup();
        }       
        #endregion    
    

        #region Range_Projectile
        private void GUIAmmoWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo.width, gui_window_ammo.height), gui_window_ammo, GUIStyle.none);
            GUI.enabled = !Application.isPlaying;
            CheckDragAreaForObject<GameObject>(new Rect(77, 41, 66, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).projectileA);
            CheckDragAreaForObject<GameObject>(new Rect(216, 41, 66, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).projectileB);
            GUI.enabled = true;
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirlingSpeed = Knobs.MinMax(new Vector2(39, 81), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirlingSpeed, 0, 360, 111);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.x = Knobs.MinMax(new Vector2(114, 81), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.x, -360, 360, 112);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.y = Knobs.MinMax(new Vector2(180, 81), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.y, -360, 360, 123);
            ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.z = Knobs.MinMax(new Vector2(248, 81), ((FPSControlRangedWeapon)currentWeapon.weapon).rangeDefinition.twirl.z, -360, 360, 114);
            GUI.EndGroup();
        }

        private void GUIPreFirePathWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_prefire_path.width, gui_window_prefire_path.height), gui_window_prefire_path, GUIStyle.none);
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.isPreFire = true;
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.render = GUI.Toggle(new Rect(13, 36, 15, 15), ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.render, "");
            GUI.enabled = !Application.isPlaying;
            CheckDragAreaForObject<Material>(new Rect(56, 58, 66, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.material);
            CheckDragAreaForComponent<Transform>(new Rect(56, 79, 66, 18), ref ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.origin);
            GUI.enabled = true;
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.maxTimeDistance = Knobs.MinMax(new Vector2(182, 112), ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.maxTimeDistance, 0f, 10, 51);
            ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.leavingForce = Knobs.MinMax(new Vector2(253, 112), ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath.definition.leavingForce, 0, 250, 52);
            GUI.EndGroup();
        }
        #endregion


        #region Melee
        private void GUIMeleeDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_melee_damage.width, gui_window_melee_damage.height), gui_window_melee_damage, GUIStyle.none);
            CheckDragAreaForComponent<Collider>(new Rect(15, 50, 66, 18), ref ((FPSControlMeleeWeapon)currentWeapon.weapon).damageTrigger);
            currentWeapon.weapon.definition.maxDamagePerHit = Knobs.MinMax(new Vector2(110, 35), currentWeapon.weapon.definition.maxDamagePerHit, 0, 100, 101);
            GUI.EndGroup();
        }
        #endregion

        #endregion


        #region logic
        private static string TEMP_PREFAB { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TEMPWEAPON.prefab"; } }
        private FPSControlWeapon[] weapons;
        private WeaponData currentWeapon;
        private bool weaponsAvailable;
        private List<string> impactNames = new List<string>();
        private int impactIndex = 0;
        private static string lastWeaponName;


        private const string PREF_SAVED_PREFIX = "FPSCONTROL_WEAPON_SAVED_";

        public override void OnInspectorUpdate()
        {
            
        }

        private void SaveWeaponCopy(FPSControlWeapon weapon)
        {
            Serializer.SaveData<FPSControlWeaponDefinitions>(PREF_SAVED_PREFIX + weapon.definition.weaponName, new FPSControlWeaponDefinitions(currentWeapon.weapon), true, false, false);
            EditorPrefs.SetBool(PREF_SAVED_PREFIX + "NEEDS_SAVING", true);
            if (!Application.isPlaying) SaveIfPrefab(currentWeapon.weapon.gameObject);
        }

        private void SaveIfPrefab(GameObject go)
        {
            var prefabType = PrefabUtility.GetPrefabType(go);
            if (prefabType != PrefabType.DisconnectedPrefabInstance) { //Check if we are a prefab
                GameObject scenePrefab = PrefabUtility.FindPrefabRoot(go); //Get the gameobject that is int he scene
                var prefabObject = PrefabUtility.GetPrefabParent(go);
                PrefabUtility.ReplacePrefab(scenePrefab, prefabObject, ReplacePrefabOptions.ConnectToPrefab);
            }
        }

        private void RevertSavedCopy(FPSControlWeapon weapon)
        {
            if (EditorPrefs.HasKey(PREF_SAVED_PREFIX + weapon.definition.weaponName))
            {
                FPSControlWeaponDefinitions definitions = Serializer.LoadData<FPSControlWeaponDefinitions>(PREF_SAVED_PREFIX + weapon.definition.weaponName, false, false);
                FPSControlWeaponDefinitions.LoadDefintionsIntoWeapon(definitions, ref weapon);
                SaveIfPrefab(weapon.gameObject);
            }
        }

        private void RevertSavedCopys()
        {
            if (EditorPrefs.GetBool(PREF_SAVED_PREFIX + "NEEDS_SAVING", false))
            {
                foreach (FPSControlWeapon w in weapons)
                {
                    if (EditorPrefs.HasKey(PREF_SAVED_PREFIX + w.definition.weaponName))
                    {
                        RevertSavedCopy(w);
                        EditorPrefs.DeleteKey(PREF_SAVED_PREFIX + w.definition.weaponName);
                    }
                }
                EditorPrefs.SetBool(PREF_SAVED_PREFIX + "NEEDS_SAVING", false);
            }
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
            LoadAssets();
            LocateWeapons();
            LoadImpactNames();
            base.Init();
        }

        private bool _rebuild = true;
        override public void OnFocus(bool rebuild)
        {
            _rebuild = rebuild;
            //Debug.Log("Focused - Rebuilt: " + rebuild + " - Wasplaying: " + wasPlaying);            
            Init();
            if (!Application.isPlaying && wasPlaying && !rebuild)
            {
                RevertSavedCopys();
                int i = 0;
                foreach (FPSControlWeapon w in weapons)
                {
                    if (w.definition.weaponName == lastWeaponName)
                    {
                        //Debug.Log("Focusing weapon: " + lastWeaponName);
                        currentWeaponIndex = i;
                        SetCurrentWeapon(w);
                        break;
                    }
                    i++;
                }
            }
            base.OnFocus(rebuild);
        }

        override public void OnLostFocus(bool rebuild)
        {
            //Debug.Log("Lost Focus - Rebuilt: " + rebuild + " - Wasplaying: " + wasPlaying);            
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
                weapon.definition.weaponName = name;
                SetCurrentWeapon(weapon);
            }
            else
            {
                FPSControlMeleeWeapon weapon = go.AddComponent<FPSControlMeleeWeapon>();
                weapon.definition.weaponName = name;
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
            //weapons = (FPSControlWeapon[])Resources.FindObjectsOfTypeAll(typeof(FPSControlWeapon));
            weapons = (FPSControlWeapon[])Object.FindObjectsOfType(typeof(FPSControlWeapon));
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
            //Debug.Log("Setting Weapon");
            currentWeapon = new WeaponData();
            currentWeapon.weapon = weapon;
            currentWeapon.isRanged = (weapon.GetType() == typeof(FPSControlRangedWeapon));
            CheckCurrentWeapon();
            if (Application.isPlaying) lastWeaponName = weapon.definition.weaponName;
            //SetCurrentSave();
        }

        private void CheckCurrentWeapon()
        {
            if (currentWeapon.modelOffset == null) currentWeapon.modelOffset = GetOrCreateChild(currentWeapon.weapon.transform, currentWeapon.weapon.name + " Model");
            if (currentWeapon.modelControler == null) currentWeapon.modelControler = GetOrCreateChild(currentWeapon.modelOffset, currentWeapon.weapon.name + " Controller");
            if (currentWeapon.weapon.weaponAnimation == null) currentWeapon.weapon.weaponAnimation = GetOrCreateComponent<FPSControlWeaponAnimation>(currentWeapon.modelControler);
            if (currentWeapon.weapon.weaponParticles == null) currentWeapon.weapon.weaponParticles = GetOrCreateComponent<FPSControlWeaponParticles>(currentWeapon.modelControler);
            if (currentWeapon.weapon.weaponSound == null) currentWeapon.weapon.weaponSound = GetOrCreateComponent<FPSControlWeaponSound>(currentWeapon.modelControler);
            if (currentWeapon.isRanged && ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath == null) ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath = GetOrCreateComponent<FPSControlWeaponPath>(currentWeapon.modelControler);
            if (currentWeapon.isRanged) ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath = ((FPSControlRangedWeapon)currentWeapon.weapon).weaponPath;
        }

        private void LoadImpactNames()
        {
            impactNames.Clear();
            impactIndex = 0;

            string assetPath = FPSControlMainEditor.RESOURCE_FOLDER + "ImpactControlDefinitions.asset";
            AssetDatabase.ImportAsset(assetPath);

            ImpactControlDefinitions loadedDef = (ImpactControlDefinitions)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ImpactControlDefinitions));

            impactNames.Add("None");
            foreach(ImpactControlDefinition impact in loadedDef.impacts)
            {
                if (!impactNames.Contains(impact.name))
                {
                    impactNames.Add(impact.name);
                }
            }
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
