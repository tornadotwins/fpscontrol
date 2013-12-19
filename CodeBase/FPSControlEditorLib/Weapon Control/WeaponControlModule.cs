using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;
using FPSControl.Data;
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
        
        const string PREFAB_PATH = "Prefabs/Weapons";
        const string BUFFER_PREFIX = "$$_tmp_";
        const string BUFFER_PREFAB_FORMAT = "Assets/" + PREFAB_PATH + "/" + BUFFER_PREFIX + "{0}.prefab";
        const string NEW_PREFAB_FORMAT = "Assets/" + PREFAB_PATH + "/{0}.prefab";

        List<GameObject> prefabs = new List<GameObject>();
        int currentIndex = -1;

        GameObject requiredRoot
        {
            get
            {
                FPSControlPlayerWeaponManager[] mgrs = (FPSControlPlayerWeaponManager[]) Resources.FindObjectsOfTypeAll(typeof(FPSControlPlayerWeaponManager));
                if (mgrs.Length == 0) return null;
                //else if (mgrs.Length > 1) Debug.LogWarning("Multiple Weapon Managers found in scene. This may produce unexpected results.");
                return mgrs[0].gameObject;
            }
        }

        FPSControlPlayerWeaponManager WeaponManager { get { return requiredRoot.GetComponent<FPSControlPlayerWeaponManager>(); } }
        WeaponsCatalogue Catalogue
        {
            get
            {
                if (!WeaponManager.weaponPrefabsCatalogue)
                    WeaponManager.weaponPrefabsCatalogue = WeaponsCatalogueInspector.Create();
                return WeaponManager.weaponPrefabsCatalogue;
            }
        }

        int rootChildren { get { return requiredRoot ? requiredRoot.transform.childCount : 0; } }

        public GameObject CurrentPrefab { get { return (prefabs.Count > 0 && currentIndex > -1) ? prefabs[currentIndex] : null; } }
        public FPSControlWeapon PrefabComponent { get { return CurrentPrefab ? CurrentPrefab.GetComponent<FPSControlWeapon>() : null; } }

        public FPSControlWeapon FocusedComponent { get { return EditorApplication.isPlaying ? InstanceComponent : PrefabComponent; } }

        GameObject _prefabInstance;
        public FPSControlWeapon InstanceComponent { get { return _prefabInstance ? _prefabInstance.GetComponent<FPSControlWeapon>() : null; } }
        public T GetInstanceComponent<T>() where T : FPSControlWeapon { return (T) InstanceComponent;}  

        GameObject _tmpPlaymodeBuffer;
        public FPSControlWeapon BufferComponent { get { return _tmpPlaymodeBuffer ? _tmpPlaymodeBuffer.GetComponent<FPSControlWeapon>() : null; } }

        bool dirty = false;

        public List<string> names
        {
            get
            {
                List<string> list = new List<string>();
                for (int i = 0; i < prefabs.Count; i++)
                    list.Add(prefabs[i].name);
                return list;
            }
        }

        override public void OnPlayModeChange(bool playMode)
        {
            base.OnPlayModeChange(playMode);

            if (!playMode)
            {
                // If we had added a prefab during play mode it won't be there when we exit - so we need to add it.
                if (!_prefabInstance) InstantiateCurrent();

                if (!InstanceComponent) return;

                // Alert the user with a prompt if they want to keep or revert changes
                bool prompt = (EditorUtility.DisplayDialog("Save Changes?", "Exiting Play Mode - Would you like to save changes?",
                    "Yes",
                    "No"
                ));
                ClearBuffer(prompt);
                if (prompt) Save();
            }
        }

        void GetPrefabs()
        {
            try
            {
                prefabs = new List<GameObject>();
                List<GameObject> toDelete = new List<GameObject>();
                foreach (string path in Directory.GetFiles(Application.dataPath + "/" + PREFAB_PATH))
                {

                    string shortPath = "Assets" + path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length).Replace('\\', '/');
                    if (shortPath.ToLower().EndsWith(".meta")) continue; // Ignore meta data if version control is being used.
                    //Debug.Log("Loading " + shortPath);
                    if (path.ToLower().EndsWith(".prefab"))
                    {
                        GameObject _tmpPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(shortPath, typeof(GameObject));
                        if (!_tmpPrefab)
                        {
                            Debug.LogError("Couldn't load prefab at path: " + shortPath);
                            continue;
                        }

                        if (_tmpPrefab.name.StartsWith("$$_tmp_"))
                        {
                            toDelete.Add(_tmpPrefab);
                            continue; // Don't include buffer objects.
                        }

                        //Debug.Log("found prefab: " + shortPath + " named: " + _tmpPrefab.name);
                        FPSControlWeapon _tmpComponent = _tmpPrefab.GetComponent<FPSControlWeapon>();
                        if (_tmpComponent && Catalogue.ContainsValue(_tmpPrefab))
                        {
                            //Debug.Log("Found prefab. Adding to collection. " + AssetDatabase.GetAssetPath(_tmpPrefab));
                            prefabs.Add(_tmpPrefab);
                        }
                    }
                }

                // Clean up unused stuff, but only if we aren't playing.
                if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    foreach (GameObject toBeDeleted in toDelete.ToArray())
                        File.Delete(Application.dataPath + "/" + AssetDatabase.GetAssetPath(toBeDeleted).Substring("Assets/".Length));
                }
            }
            catch (System.Exception err) { Debug.LogWarning("Caught Exception: " + err.Message); }
        }

        void CreatePlaymodeBuffer()
        {
            if (!CurrentPrefab) return;
            Debug.Log("Creating buffer object at: " + string.Format(BUFFER_PREFAB_FORMAT, CurrentPrefab.name));
            Object buffer = PrefabUtility.CreateEmptyPrefab(string.Format(BUFFER_PREFAB_FORMAT, CurrentPrefab.name));
            _tmpPlaymodeBuffer = PrefabUtility.ReplacePrefab(_prefabInstance, buffer);
            
            Debug.Log("Successfully created buffer.");
        }

        double _lastWriteTime = 0;
        void WriteToBuffer()
        {
            // We should never be writing to a buffer outside of play mode.
            if (!EditorApplication.isPlaying) return;

            // If the buffer exists, write over it
            if (_tmpPlaymodeBuffer)
            {
                // Write to the buffer only once per second.
                if (EditorApplication.timeSinceStartup - _lastWriteTime > 1D)
                {
                    _tmpPlaymodeBuffer = PrefabUtility.ReplacePrefab(_prefabInstance, _tmpPlaymodeBuffer);
                    dirty = false;
                    _lastWriteTime = EditorApplication.timeSinceStartup;
                }
            }
            else
            {
                Debug.LogError("Cannot write to temporary buffer because it could not be located.");
            }
        }

        void ClearBuffer(bool saveChanges)
        {
            try
            {
                if (saveChanges)
                {
                    if (InstanceComponent)
                    {
                        EditorUtility.CopySerialized(BufferComponent, InstanceComponent);
                        Save();

                    }
                    else
                        Debug.LogError("Could not keep changes because the reference to the Instance component in this scene was broken.");
                }

                // Delete our temp buffer
                string bufferPath = AssetDatabase.GetAssetPath(_tmpPlaymodeBuffer);
                _tmpPlaymodeBuffer = null;
                AssetDatabase.DeleteAsset(bufferPath);
            }
            catch (System.Exception err) { Debug.LogWarning("Caught Exception: " + err.Message); }
        }

        void Delete()
        {
            // Not available in play mode.
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("Cannot Delete Asset", "Deleting Assets is not permitted during Play mode! Stop Play mode to delete.", "OK");
                return;
            }
            
            // Cache refrences to previous objects
            int _prevIndex = currentIndex;
            GameObject _prevPrefabInstance = _prefabInstance;
            FPSControlWeapon _prevInstanceComponent = InstanceComponent;
            GameObject _prevPrefab = CurrentPrefab;
            FPSControlWeapon _prevPrefabComponent = PrefabComponent;

            //FPSControlPlayerWeaponManager mgr = WeaponManager;
            //int indexOf = ArrayUtility.IndexOf<FPSControlWeapon>(mgr.weaponActors, _prevInstanceComponent);
            //ArrayUtility.RemoveAt<FPSControlWeapon>(ref mgr.weaponActors, indexOf); // Remove this from the manager.
            
            // Change the current index if necessary, otherwise we'll let everything move underneathe it
            currentIndex = Mathf.Clamp(currentIndex,0,Mathf.Max(prefabs.Count-1,0));

            Object.DestroyImmediate(_prevPrefabInstance);

            // Destroy Prefab
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_prevPrefab));

            // Re-collect Prefabs
            GetPrefabs();
        }

        void Save()
        {
            Save(_prefabInstance, CurrentPrefab);
        }

        void Save(GameObject prefabInstance, GameObject prefab)
        {
            CheckWeapon(prefabInstance.GetComponent<FPSControlWeapon>());
            PrefabUtility.ReplacePrefab(prefabInstance, prefab);
            //PrefabUtility.RevertPrefabInstance(prefabInstance); // this is to make sure the instance and prefab are fully in-sync again.
            //PrefabUtility.ReplacePrefab(prefabInstance, prefab);
            dirty = false;
            Repaint();
        }

        void Revert()
        {
            Revert(_prefabInstance, PrefabComponent, InstanceComponent);
        }

        void Revert(GameObject prefabInstance, FPSControlWeapon prefabComponent, FPSControlWeapon instanceComponent)
        {
            //FPSControlPlayerWeaponManager mgr = WeaponManager;
            //int indexOf = ArrayUtility.IndexOf<FPSControlWeapon>(mgr.weaponActors, instanceComponent);
            
            if (EditorApplication.isPlaying)
                EditorUtility.CopySerialized(prefabComponent, instanceComponent); // Because the Prefab instance is disconnected during play mode.
            else
                PrefabUtility.RevertPrefabInstance(prefabInstance);

            //mgr.weaponActors[indexOf] = instanceComponent; // In case of a change in range type.

            dirty = false;
            Repaint();
        }

        bool _changedType;
        void ChangeWeaponType<TOld, TNew>() where TOld : FPSControlWeapon where TNew : FPSControlWeapon
        {
            TOld _old = _prefabInstance.GetComponent<TOld>();
            Object.DestroyImmediate(_old);
            TNew _new = _prefabInstance.AddComponent<TNew>();
            Repaint();
        }

        void CreateNew<T>(string n) where T : FPSControlWeapon
        {
            // Create everything we need
            GameObject go = new GameObject(n);
            go.AddComponent<T>();

            // Create the prefab
            GameObject prefab = PrefabUtility.ReplacePrefab(go, PrefabUtility.CreateEmptyPrefab(string.Format(NEW_PREFAB_FORMAT, n)));

            // Destroy the temp 
            Object.DestroyImmediate(go);

            // Add to the catalogue
            Catalogue.Add(n, prefab);

            // Refresh the list
            GetPrefabs();

            // Change the index
            currentIndex = prefabs.IndexOf(prefab);

            // Instantiate the new game object
            InstantiateCurrent();

            // Provide name data to the definition.
            InstanceComponent.definition.weaponName = InstanceComponent.name;

            //FPSControlPlayerWeaponManager mgr = requiredRoot.GetComponent<FPSControlPlayerWeaponManager>();
            //ArrayUtility.Add<FPSControlWeapon>(ref mgr.weaponActors, InstanceComponent); // Add this to the manager.
        }

        void InstantiateCurrent()
        {
            //Debug.Log("Instantiate Current Prefab!");
            if (!CurrentPrefab) return;
            FPSControlPlayerWeaponManager mgr = WeaponManager;
            if (!mgr)
            {
                Debug.LogError("Error! Cannot locate Weapon Manager!");
                return;
            }
            _prefabInstance = (GameObject) PrefabUtility.InstantiatePrefab(CurrentPrefab);
            _prefabInstance.transform.parent = requiredRoot.transform;
            _prefabInstance.transform.localPosition = Vector3.zero;
            _prefabInstance.transform.localRotation = Quaternion.identity;
            
            CheckCurrentWeapon();
        }

        bool IsWeaponRanged { get { return InstanceComponent is FPSControlRangedWeapon; } }

        #region GUI


        public override void OnGUI()
        {
            if (_changedType && Event.current.type == EventType.Repaint)
            {
                _changedType = false;
                return;
            }
            // If we don't have the required root, we shouldn't be able to do anything.
            bool gEnabled = GUI.enabled;
            GUI.enabled = requiredRoot ? gEnabled : false;

            // Do index adjustments if necessary
            if (names.Count == 0) currentIndex = 0;
            else if (currentIndex >= names.Count) currentIndex = names.Count - 1;

            GUIDrawBackground();
            
            SaveMenu();

            // Cache refrences to previous objects for change checks
            int _prevIndex = currentIndex;
            GameObject _prevPrefabInstance = _prefabInstance;
            FPSControlWeapon _prevInstanceComponent = InstanceComponent;
            GameObject _prevPrefab = CurrentPrefab;
            FPSControlWeapon _prevPrefabComponent = PrefabComponent;
           
            GUIWeaponSelect();
            if (InstanceComponent)
            {
                CheckCurrentWeapon();
                
                GUIWeaponTypeSelect();

                if (IsWeaponRanged)
                {
                    GUIRangeTypeSelect();
                    switch (GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.rangedType)
                    {
                        case FPSControlRangedWeaponType.Bullets:
                            GUITab(ref rangeBulletTabIndex,
                                gui_button_weapon_ranged_group2_n,
                                gui_button_weapon_ranged_group2_a,
                                gui_button_weapon_ranged_group1_n,
                                gui_button_weapon_ranged_group1_a
                                );
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
                            GUITab(ref rangeProjectileTabIndex,
                                gui_button_weapon_projectile_group1_n,
                                gui_button_weapon_projectile_group1_a,
                                gui_button_weapon_projectile_group2_n,
                                gui_button_weapon_projectile_group2_a
                            );
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
                else // Melee
                {
                    GUITab(ref meleeTabIndex,
                        gui_button_weapon_melee_group1_n,
                        gui_button_weapon_melee_group1_a,
                        gui_button_weapon_melee_group2_n,
                        gui_button_weapon_melee_group2_a
                    );
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
            }
            else // InstanceComponent doesn't exist.
            {
                GUI.DrawTexture(noWeaponsBox, gui_cover);
                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;
                GUI.Label(noWeaponsBox, "No definitions exist for this type. \nPlease add one", gs);
            }

            if (EditorApplication.isPlaying && currentIndex != -1 && dirty)
                WriteToBuffer();

            #region ##### OLD CODE #####

            /*
            if (weaponsAvailable && currentWeapon.isRanged)
            {
                GUIRangeTypeSelect();
                switch (((FPSControlRangedWeapon)InstanceComponent).rangeDefinition.rangedType)
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
             * */

            #endregion
        }


        #region commonUIElements
        private void SaveMenu()
        {
            if (GUI.Button(new Rect(800, 65, 98, 24), "Save"))
            {
                Save();
                //SaveWeaponCopy(InstanceComponent);
            }
            if (GUI.Button(new Rect(700, 65, 98, 24), "Revert"))
            {
                Revert();
                //RevertSavedCopy(InstanceComponent);
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
            bool gEnabled = GUI.enabled;
            GUI.enabled = Application.isPlaying ? false : gEnabled;
            Rect rangeTypeRect = new Rect(669, 150, 139, 14);
            FPSControlRangedWeapon rangedWeapon = GetInstanceComponent<FPSControlRangedWeapon>();
            rangedWeapon.rangeDefinition.rangedType =
                (FPSControlRangedWeaponType) EditorGUI.EnumPopup(rangeTypeRect, rangedWeapon.rangeDefinition.rangedType);
            //((FPSControlRangedWeapon)InstanceComponent).rangeDefinition.rangedType = 
                //(FPSControlRangedWeaponType)EditorGUI.EnumPopup(rangeTypeRect, ((FPSControlRangedWeapon)InstanceComponent).rangeDefinition.rangedType);
            GUI.enabled = gEnabled;
        }

        private void GUIWeaponTypeSelect()
        {
            bool gEnabled = GUI.enabled;

            Rect radioRangeRect = new Rect(366, 149, 11, 11);
            Rect radioMeleeRect = new Rect(438, 149, 11, 11);

            GUI.enabled = EditorApplication.isPlaying ? false : gEnabled;

            bool newRange = GUI.Toggle(radioRangeRect, IsWeaponRanged, "");
            bool newMelee = GUI.Toggle(radioMeleeRect, !IsWeaponRanged, "");

            GUI.enabled = gEnabled;

            if (IsWeaponRanged != newRange || newMelee == IsWeaponRanged)
            {
                if (Application.isPlaying)
                {
                    EditorUtility.DisplayDialog("Error", "Can't do this in play mode", "OK");
                }
                else if (EditorUtility.DisplayDialog("Caution!", "Do this will discard previous settings \n Are you sure you want to do this?", "Discard", "Cancel"))
                {
                    SwitchWeaponType();
                }
            }

            /*
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
             * */
        }

        private void GUIWeaponSelect()
        {
            bool gEnabled = GUI.enabled;
            Rect popupRect = new Rect(365, 114, 139, 14);
            Rect deleteRect = new Rect(507, 114, 15, 14);
            Rect newRect = new Rect(527, 114, 76, 14);

            int prevIndex = currentIndex;
            GameObject _prevPrefabInstance = _prefabInstance;
            FPSControlWeapon _prevInstanceComponent = InstanceComponent;
            GameObject _prevPrefab = CurrentPrefab;
            FPSControlWeapon _prevPrefabComponent = PrefabComponent;
            if (names.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                currentIndex = EditorGUI.Popup(popupRect, currentIndex, names.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    if (_prevPrefabInstance)
                    {
                        // Alert the user with a prompt if they want to keep or revert changes
                        if (EditorUtility.DisplayDialog("Save Changes?", "Would you like to save your changes?", "Yes", "No"))
                            Save(_prevPrefabInstance, _prevPrefab);
                        else
                            Revert(_prevPrefabInstance, _prevPrefabComponent, _prevInstanceComponent);

                        if (EditorApplication.isPlaying) ClearBuffer(false);
                        else Object.DestroyImmediate(_prevPrefabInstance); // Destroy the instance object
                    }
                    
                    // If we made a change and there isn't an instance of the selected prefab under the root we need to add it
                    Transform _tmp = requiredRoot.transform.Find(names[currentIndex]);
                    if (_tmp)
                    {
                        Debug.Log("Found: " + _tmp.name);
                        // We found one of the same name, but is it linked to the prefab we want?
                        Object prefabParent = PrefabUtility.GetPrefabParent(_tmp.gameObject);
                        if (prefabParent != CurrentPrefab && !EditorApplication.isPlaying) // If they don't match, throw an error and zero out.
                        {

                            Debug.LogError(string.Format("GameObject found in root with name '{0}' does not match the expected prefab '{1}'," +
                            " result was '{2}'",
                                _tmp.name,
                                AssetDatabase.GetAssetPath(CurrentPrefab),
                                AssetDatabase.GetAssetPath(prefabParent))
                            );
                            currentIndex = -1;

                        }
                        else if (EditorApplication.isPlaying && prefabParent != CurrentPrefab)
                        {
                            // Couldn't connect - try and grab it from the existing list of instantiated weapons.
                            bool success = false;
                            foreach (FPSControlWeapon weapon in WeaponManager.WeaponActors)
                            {
                                if (weapon.name == names[currentIndex] && weapon.gameObject.active)
                                {
                                    _prefabInstance = weapon.gameObject;
                                    success = true;
                                }
                            }
                            if(!success)
                            {
                                Debug.LogError("Could not connect to Prefab. Is this the active weapon? Restart and try again.");
                            }
                        }
                        else // It is the one we're looking for...
                        {
                            _prefabInstance = _tmp.gameObject;

                            // If we aren't in play mode
                            if (!EditorApplication.isPlaying)
                            {
                                // If we have a broken prefab reference, we need to reconnect it and resync it.
                                if (PrefabUtility.GetPrefabType(_prefabInstance) == PrefabType.DisconnectedPrefabInstance)
                                {
                                    PrefabUtility.ReconnectToLastPrefab(_prefabInstance);
                                    PrefabUtility.RevertPrefabInstance(_prefabInstance);
                                }
                            }
                            else
                            {
                                CreatePlaymodeBuffer(); // We need to create the Play Mode Buffer.
                                if (!_prefabInstance.active)
                                {
                                    currentIndex = prevIndex;
                                    _prefabInstance = null;
                                    Debug.LogWarning("Cannot focus an inactive weapon during Play Mode!");
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (EditorApplication.isPlaying)
                        {
                            currentIndex = prevIndex;
                            Debug.LogWarning("Cannot focus a missing weapon during Play Mode! Add it outside of Play Mode to edit.");
                            return;
                        }
                        // Can't find the child, instantiate one.
                        InstantiateCurrent();
                    }
                }
            }
            else
            {
                GUI.Label(popupRect, "NONE", EditorStyles.popup);
            } 

            GUI.enabled = Application.isPlaying || names.Count < 1 ? false : gEnabled;

            if (GUI.Button(deleteRect, "-"))
            {
                if(EditorUtility.DisplayDialog("Confirm Delete","Do you want to delete this asset? It can not be undone.","OK","Cancel")) 
                    Delete();
            }

            GUI.enabled = gEnabled;

            if (GUI.Button(newRect, "Add New"))
            {
                Prompt("Weapon Name");
            }

            GUI.enabled = gEnabled;

            #region ######OLD CODE######
            /*
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
                RevertSavedCopy(InstanceComponent);
                SetCurrentWeapon(weapons[currentWeaponIndex]);                
            }
            GUI.enabled = !weaponsAvailable || Application.isPlaying ? false : gEnabled;
            if (GUI.Button(deleteRect, "-"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this?", "Delete", "Cancel"))
                {
                    FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
                    foreach (FPSControlPlayerWeaponManager manager in managers) //Go through all the managers and make sure we rerefrence the new one
                    {
                        List<FPSControlWeapon> actors = new List<FPSControlWeapon>(manager.weaponActors);
                        int index = actors.IndexOf(InstanceComponent);
                        if (index != -1) actors.RemoveAt(index);
                        manager.weaponActors = actors.ToArray();
                    }
                    GameObject.DestroyImmediate(InstanceComponent.transform.gameObject);
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
            */
            #endregion // ######OLD CODE######
        }

        private void GUIDrawBackground()
        {
            if (prefabs.Count > 0)
            {
                if (IsWeaponRanged) GUI.DrawTexture(backgroundRect, gui_background_range);
                else GUI.DrawTexture(backgroundRect, gui_background_non_range);
            }
            else
            {
                GUI.DrawTexture(backgroundRect, gui_background_range);
            }
            
            /*
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
             * */
        }
        #endregion


        #region CommonWindows
        private int pivotSelectHelper = 0;
        private void GUIPositionWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_position.width, gui_window_position.height), gui_window_position, GUIStyle.none);
            EditorGUI.BeginChangeCheck();
            Knobs.Theme(Knobs.Themes.WHITE, _editor);
            if (IsWeaponRanged)
            {
                pivotSelectHelper = EditorGUI.Popup(new Rect(14, 50, 88, 15), pivotSelectHelper, new string[2] { "View Port", "Scope" });
                if (pivotSelectHelper == 0)
                {
                    if (Application.isPlaying && InstanceComponent.scoped)
                    {
                        InstanceComponent.ExitScope();
                        InstanceComponent.Parent.Player.playerCamera.Scope(false, InstanceComponent.Parent.Player.playerCamera.baseFOV);
                    }
                    GUIPositionWindow_SubA(ref InstanceComponent.definition.pivot, ref InstanceComponent.definition.euler);
                }
                else
                {
                    if (Application.isPlaying && !InstanceComponent.scoped)
                    {
                        InstanceComponent.Scope();
                        InstanceComponent.Parent.Player.playerCamera.Scope(true, InstanceComponent.definition.scopeFOV);
                    }
                    GUIPositionWindow_SubA(ref InstanceComponent.definition.scopePivot, ref InstanceComponent.definition.scopeEuler);
                }
            }
            else
            {
                GUIPositionWindow_SubA(ref InstanceComponent.definition.pivot, ref InstanceComponent.definition.euler);
            }
            dirty = EditorGUI.EndChangeCheck() ? true : dirty; // we shouldn't ever clean it except by reverting or saving
            GUI.EndGroup();

            #region #####OLD CODE#####
            /*
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_position.width, gui_window_position.height), gui_window_position, GUIStyle.none);
            Knobs.Theme(Knobs.Themes.WHITE, _editor);
            if (currentWeapon.isRanged)
            {
                pivotSelectHelper = EditorGUI.Popup(new Rect(14, 50, 88, 15), pivotSelectHelper, new string[2] { "View Port", "Scope" });
                if (pivotSelectHelper == 0)
                {
                    if (Application.isPlaying && InstanceComponent.scoped)
                    {
                        InstanceComponent.ExitScope();
                        InstanceComponent.Parent.Player.playerCamera.Scope(false, InstanceComponent.Parent.Player.playerCamera.baseFOV);
                    }
                    GUIPositionWindow_SubA(ref InstanceComponent.definition.pivot, ref InstanceComponent.definition.euler);
                }
                else
                {
                    if (Application.isPlaying && !InstanceComponent.scoped)
                    {
                        InstanceComponent.Scope();
                        InstanceComponent.Parent.Player.playerCamera.Scope(true, InstanceComponent.definition.scopeFOV);
                    }
                    GUIPositionWindow_SubA(ref InstanceComponent.definition.scopePivot, ref InstanceComponent.definition.scopeEuler);
                }
            }
            else
            {
                GUIPositionWindow_SubA(ref InstanceComponent.definition.pivot, ref InstanceComponent.definition.euler);
            }
            GUI.EndGroup()
            */
            #endregion 
        }

        private void GUIPositionWindow_SubA(ref Vector3 pivot, ref Vector3 euler)
        {
            EditorGUI.BeginChangeCheck();
            pivot.x = Knobs.MinMax(new Vector2(117, 35), pivot.x, -3, 3, 1);
            pivot.y = Knobs.MinMax(new Vector2(187, 35), pivot.y, -3, 3, 2);
            pivot.z = Knobs.MinMax(new Vector2(256, 35), pivot.z, -3, 3, 3);
            euler.x = Knobs.MinMax(new Vector2(117, 116), euler.x, -360, 360, 4);
            euler.y = Knobs.MinMax(new Vector2(187, 116), euler.y, -360, 360, 5);
            euler.z = Knobs.MinMax(new Vector2(256, 116), euler.z, -360, 360, 6);
            dirty = EditorGUI.EndChangeCheck() ? true : dirty; // we shouldn't ever clean it except by reverting or saving
        }

        private void GUIFiringPatternWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_firing_pattern.width, gui_window_firing_pattern.height), gui_window_firing_pattern, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            InstanceComponent.weaponAnimation.definition.patternType = 
                (FiringPatternType) GUI.SelectionGrid(new Rect(15, 36, 15, 35), 
                    (int)InstanceComponent.weaponAnimation.definition.patternType, 
                    new string[2] { "", "" }, 1, "toggle");

            InstanceComponent.weaponAnimation.definition.blend = 
                GUI.Toggle(new Rect(43, 70, 15, 15), InstanceComponent.weaponAnimation.definition.blend, "");

            string fireClipName = GetInstanceComponent<FPSControlRangedWeapon>().weaponAnimation.definition.FIRE;
            AnimationState fireAnimation = GetInstanceComponent<FPSControlRangedWeapon>().weaponAnimation.transform.animation[fireClipName];
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
                GUI.enabled = Application.isPlaying ? false : gEnabled;
                FalloffSlider.FiringPatternSlider(GetInstanceComponent<FPSControlRangedWeapon>().weaponAnimation.firingPattern, fireAnimation.clip, new Vector2(15, 134), Repaint);
                GUI.enabled = gEnabled;
            }
            dirty = EditorGUI.EndChangeCheck() ? true : dirty; // we shouldn't ever clean it except by reverting or saving

            GUI.EndGroup();
            GUI.enabled = gEnabled;
        }

        private void GUIMeshAnimationWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;
            GUI.enabled = Application.isPlaying ? false : gEnabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_mesh_animation.width, gui_window_mesh_animation.height), gui_window_mesh_animation, GUIStyle.none);

            EditorGUI.BeginChangeCheck();
            
            GameObject meshPrefab;
            bool hasAnimations = GUIMeshAnimationWindow_HasAnimations();
            string dragText = (!hasAnimations ? "Drag" : "Animation");
            DragResultState dragResult = Drag.DragArea<GameObject>(new Rect(166, 43, 144, 16), out meshPrefab, dragText);
            if (hasAnimations && dragResult == DragResultState.ContextClick)
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
                foreach (AnimationState animationState in InstanceComponent.weaponAnimation.animation)
                {
                    animationNames.Add(animationState.name);
                }
            }
            GUIMeshAnimationWindow_SubA(new Rect(72, 65, 81, 14), ref InstanceComponent.weaponAnimation.definition.ACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 65, 81, 14), ref InstanceComponent.weaponAnimation.definition.DEACTIVATE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 87, 81, 14), ref InstanceComponent.weaponAnimation.definition.FIRE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 87, 81, 14), ref InstanceComponent.weaponAnimation.definition.EMPTY, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 109, 81, 14), ref InstanceComponent.weaponAnimation.definition.RELOAD, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 109, 81, 14), ref InstanceComponent.weaponAnimation.definition.WALK, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 131, 81, 14), ref InstanceComponent.weaponAnimation.definition.RUN, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 131, 81, 14), ref InstanceComponent.weaponAnimation.definition.IDLE, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(72, 152, 81, 14), ref InstanceComponent.weaponAnimation.definition.SCOPE_IO, animationNames);
            GUIMeshAnimationWindow_SubA(new Rect(222, 152, 81, 14), ref InstanceComponent.weaponAnimation.definition.SCOPE_LOOP, animationNames);
            GUI.enabled = gEnabled;

            dirty = EditorGUI.EndChangeCheck() ? true : dirty; // we shouldn't ever clean it except by reverting or saving

            GUI.EndGroup();
        }

        private void GUIMeshAnimationWindow_SubA(Rect rect, ref string currentValue, List<string> possibleValues)
        {
            string _prevValue = currentValue;
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
            GUI.changed = _prevValue != currentValue ? true : GUI.changed;
        }

        private void GUIMeshAnimationWindow_AddMeshAndAnimations(GameObject meshPrefab) //handles adding animation
        {
            GameObject go = GameObject.Instantiate(meshPrefab) as GameObject;
            go.transform.parent = InstanceComponent.transform;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.name = InstanceComponent.name + " Model"; 
            
            ComponetHelper.CopyComponets(InstanceComponent.modelController, 
                go.transform.GetChild(0), 
                CopyComponetStyle.exclusive, 
                typeof(Animation), 
                typeof(LineRenderer));
           
            ComponetHelper.CopyComponets(go.transform.GetChild(0),
                InstanceComponent.modelController, 
                CopyComponetStyle.inclusive, 
                false, 
                typeof(Animation), 
                typeof(LineRenderer));

            GameObject.DestroyImmediate(InstanceComponent.modelOffset.gameObject);
            CheckCurrentWeapon();
        }

        private void GUIMeshAnimationWindow_RemoveMeshAndAnimations() //handles removing all mesh and animation
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in InstanceComponent.modelController) children.Add(child.gameObject);
            children.ForEach(child => GameObject.DestroyImmediate(child));
            List<string> animationClipNames = new List<string>();
            foreach (AnimationState animationState in InstanceComponent.weaponAnimation.animation) animationClipNames.Add(animationState.name);
            animationClipNames.ForEach(child => { InstanceComponent.weaponAnimation.animation.RemoveClip(child); });
        }

        private bool GUIMeshAnimationWindow_HasAnimations() //handles removing all mesh and animation
        {
            if (InstanceComponent.weaponAnimation.animation.GetClipCount() == 0) return false;
            foreach (AnimationState animationState in InstanceComponent.weaponAnimation.animation)
            {
                if (animationState.clip != null) return true;
            }
            return false;
        }        

        private void GUIParticalWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;
            GUI.enabled = Application.isPlaying ? false : gEnabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_particle.width, gui_window_particle.height), gui_window_particle, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            if (InstanceComponent.weaponParticles.particles == null || InstanceComponent.weaponParticles.particles.Length < 3) //Make sure we have the right amount of particles
            {
                List<FPSControlWeaponParticleData> newParticles;
                int fromIndex = 0;
                if (InstanceComponent.weaponParticles.particles == null)
                {
                    newParticles = new List<FPSControlWeaponParticleData>();
                }
                else
                {
                    newParticles = new List<FPSControlWeaponParticleData>(InstanceComponent.weaponParticles.particles);
                    fromIndex = InstanceComponent.weaponParticles.particles.Length;
                }
                for (int i = fromIndex; i < 3; i++)
                {
                    newParticles.Add(new FPSControlWeaponParticleData());
                }
                InstanceComponent.weaponParticles.particles = newParticles.ToArray();
            }

            GUIParticalWindow_Sub(new Vector2(13, 61), ref InstanceComponent.weaponParticles.particles[0]);
            GUIParticalWindow_Sub(new Vector2(13, 87), ref InstanceComponent.weaponParticles.particles[1]);
            GUIParticalWindow_Sub(new Vector2(13, 113), ref InstanceComponent.weaponParticles.particles[2]);
            InstanceComponent.weaponParticles.lightIsEnabled =
                GUI.Toggle(new Rect(20, 152, 15, 15), InstanceComponent.weaponParticles.lightIsEnabled, "");
            CheckDragAreaForComponent<Light>(new Rect(39, 152, 66, 18), ref InstanceComponent.weaponParticles.lightBurst);
            CheckDragAreaForComponent<Transform>(new Rect(110, 152, 66, 18), ref InstanceComponent.weaponParticles.lightPosition);

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;

            GUI.EndGroup();
            GUI.enabled = gEnabled;
        }

        private void GUIParticalWindow_Sub(Vector2 topLeftPos, ref FPSControlWeaponParticleData partical)
        {
            GUI.BeginGroup(new Rect(topLeftPos.x, topLeftPos.y, 300, 22));
            EditorGUI.BeginChangeCheck();
            partical.isEnabled = GUI.Toggle(new Rect(7, 2, 15, 15), partical.isEnabled, "");
            CheckDragAreaForObject<GameObject>(new Rect(26, 2, 66, 18), ref partical.particleSystem);
            CheckDragAreaForComponent<Transform>(new Rect(97, 2, 66, 18), ref partical.position);
            partical.global = (GUI.SelectionGrid(new Rect(181, 2, 31, 15), (partical.global ? 0 : 1), new string[2] { "", "" }, 2, "toggle") == 0);
            dirty = EditorGUI.EndChangeCheck() ? true : dirty;
            
            GUI.EndGroup();
        }

        private void GUISoundWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;

            GUI.enabled = Application.isPlaying ? false : gEnabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_sound.width, gui_window_sound.height), gui_window_sound, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            CheckDragAreaForObject<AudioClip>(new Rect(72, 44, 79, 17), ref InstanceComponent.weaponSound.equipSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 44, 79, 17), ref InstanceComponent.weaponSound.fire1SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 66, 79, 17), ref InstanceComponent.weaponSound.fire2SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 66, 79, 17), ref InstanceComponent.weaponSound.fire3SFX);
            CheckDragAreaForObject<AudioClip>(new Rect(72, 88, 79, 17), ref InstanceComponent.weaponSound.reloadSFX);
            CheckDragAreaForObject<AudioClip>(new Rect(223, 88, 79, 17), ref InstanceComponent.weaponSound.emptySFX);

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;
            

            GUI.EndGroup();

            GUI.enabled = gEnabled;
        }

        private void CheckDragAreaForComponent<T>(Rect area, ref T checkInput) where T : UnityEngine.Component
        {
            string displayText = (checkInput == null ? "Drag" : checkInput.name);
            GameObject[] outValues;
            DragResultState dragResult = Drag.DragArea<GameObject>(area, out outValues, displayText);
            if (checkInput != null && dragResult == DragResultState.ContextClick)
            {
                if (ConfirmRemove()) checkInput = default(T);
            }
            else if (checkInput != null && dragResult == DragResultState.Click)
            {
                EditorGUIUtility.PingObject(checkInput);
            }
            else if (dragResult == DragResultState.Drag)
            {
                foreach (GameObject go in outValues)
                {
                    T component = go.GetComponent<T>();
                    if (component != null)
                    {
                        checkInput = component;
                        GUI.changed = true;
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
            if (checkInput != null && dragResult == DragResultState.ContextClick)
            {
                if (ConfirmRemove()) checkInput = default(T);
            }
            else if (checkInput != null && dragResult == DragResultState.Click)
            {
                EditorGUIUtility.PingObject(checkInput);
            }
            else if (dragResult == DragResultState.Drag)
            {
                checkInput = outValue;
                GUI.changed = true;
            }
        }

        private int NumericTextfield(Rect rect, int currentValue)
        {
            return (int)NumericTextfield(rect, (float)currentValue);
        }

        private float NumericTextfield(Rect rect, float currentValue)
        {
            float _prevValue = currentValue;

            string inputString = GUI.TextField(rect, currentValue.ToString());
            if (inputString.Length > 0)
            {
                inputString = Regex.Replace(inputString, @"[^0-9]", "");
            }
            else
            {
                inputString = "0";
            }
            float result = float.Parse(inputString);
            GUI.changed = _prevValue != result ? true : GUI.changed;

            return result;
        }

        private bool ConfirmRemove()
        {
            bool confirm = EditorUtility.DisplayDialog("Caution!", "Are you sure you want to remove this?", "Discard", "Cancel");
            GUI.changed = confirm ? true : GUI.changed;
            return confirm;
        }
        #endregion


        #region Range_Bullets
        private void GUIPathWindow(int windowIndex)
        {
            if (IsWeaponRanged)
            {
                bool gEnabled = GUI.enabled;

                GUI.BeginGroup(windowSpaces[windowIndex]);
                GUI.Box(new Rect(0, 0, gui_window_path.width, gui_window_path.height), gui_window_path, GUIStyle.none);

                EditorGUI.BeginChangeCheck();

                GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.isPreFire = false;
                GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.render =
                    GUI.Toggle(new Rect(13, 36, 15, 15), GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.render, "");

                GUI.enabled = Application.isPlaying ? false : gEnabled;
                CheckDragAreaForObject<Material>(new Rect(76, 58, 200, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.material);
                CheckDragAreaForComponent<Transform>(new Rect(76, 79, 200, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.origin);
                GUI.enabled = gEnabled;

                GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.consistentRender =
                    GUI.Toggle(new Rect(15, 100, 15, 15), GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.consistentRender, "");
                impactIndex = EditorGUI.Popup(new Rect(125, 143, 150, 15), impactIndex, impactNames.ToArray());
                InstanceComponent.impactName = impactNames[impactIndex];

                dirty = EditorGUI.EndChangeCheck() ? true : dirty;

                GUI.EndGroup();
            }
            else
            {
                
            }
        }

        private void GUIAmmoReloadingWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo_reloading.width, gui_window_ammo_reloading.height), gui_window_ammo_reloading, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.reloadType = 
                (ReloadType)GUI.SelectionGrid(new Rect(35, 41, 276, 15), 
                (int)GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.reloadType,
                new string[3] { "", "", "" }, 
                3, 
                "toggle");

            GUI.enabled = (GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.reloadType == ReloadType.Clips) ? gEnabled : false;
            
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.clipCapacity = 
                Knobs.MinMax(new Vector2(139, 92), 
                GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.clipCapacity, 
                0, 
                50, 
                21, 
                true);
            
            GUI.enabled = (GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.reloadType == ReloadType.Recharge) ? gEnabled : false;
            
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.maximumRounds = 
                NumericTextfield(new Rect(221, 82, 83, 16), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.maximumRounds);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.regenerationRate = 
                NumericTextfield(new Rect(236, 123, 68, 16), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.regenerationRate);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.fullRegenerationTime = 
                NumericTextfield(new Rect(236, 158, 68, 16), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.fullRegenerationTime);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.constantRegeneration = 
                (GUI.SelectionGrid(new Rect(220, 105, 20, 75), 
                (GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.constantRegeneration == true ? 0 : 1), 
                new string[2] { "", "" }, 1, "toggle") == 0 ? true : false);
            
            GUI.enabled = gEnabled;

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;

            GUI.EndGroup();
        }

        private void GUIDamageWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_damage.width, gui_window_damage.height), gui_window_damage, GUIStyle.none);

            EditorGUI.BeginChangeCheck();
            
            GetInstanceComponent<FPSControlRangedWeapon>().definition.maxDamagePerHit = 
                Knobs.MinMax(new Vector2(21, 35), GetInstanceComponent<FPSControlRangedWeapon>().definition.maxDamagePerHit, 0, 100, 31);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.disperseRadius = 
                Knobs.MinMax(new Vector2(90, 35), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.disperseRadius, 0, 1, 32);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.raycasts = 
                Knobs.MinMax(new Vector2(159, 35), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.raycasts, 1, 40, 598, true);
            if (GetInstanceComponent<FPSControlRangedWeapon>().damageFalloff == null) 
                GetInstanceComponent<FPSControlRangedWeapon>().damageFalloff = new FPSControl.Data.FalloffData();
            
            GUI.enabled = Application.isPlaying ? false : gEnabled;
            FalloffSlider.DamageSlider(GetInstanceComponent<FPSControlRangedWeapon>().damageFalloff, new Vector2(13, 134), Repaint);
            
            GUI.enabled = gEnabled;

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;

            GUI.EndGroup();
        }       
        #endregion    
    

        #region Range_Projectile
        private void GUIAmmoWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_ammo.width, gui_window_ammo.height), gui_window_ammo, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            GUI.enabled = Application.isPlaying ? false : gEnabled;
            CheckDragAreaForObject<GameObject>(new Rect(77, 41, 66, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().projectileA);
            CheckDragAreaForObject<GameObject>(new Rect(216, 41, 66, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().projectileB);
            GUI.enabled = gEnabled;
            
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirlingSpeed = 
                Knobs.MinMax(new Vector2(39, 81), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirlingSpeed, 0, 360, 111);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.x = 
                Knobs.MinMax(new Vector2(114, 81), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.x, -360, 360, 112);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.y = 
                Knobs.MinMax(new Vector2(180, 81), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.y, -360, 360, 123);
            GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.z = 
                Knobs.MinMax(new Vector2(248, 81), GetInstanceComponent<FPSControlRangedWeapon>().rangeDefinition.twirl.z, -360, 360, 114);

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;
            GUI.EndGroup();
        }

        private void GUIPreFirePathWindow(int windowIndex)
        {
            bool gEnabled = GUI.enabled;

            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_prefire_path.width, gui_window_prefire_path.height), gui_window_prefire_path, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.isPreFire = true;
            GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.render = 
                GUI.Toggle(new Rect(13, 36, 15, 15), GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.render, "");

            GUI.enabled = Application.isPlaying ? false : gEnabled;
            CheckDragAreaForObject<Material>(new Rect(56, 58, 66, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.material);
            CheckDragAreaForComponent<Transform>(new Rect(56, 79, 66, 18), ref GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.origin);
            GUI.enabled = gEnabled;

            GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.maxTimeDistance = 
                Knobs.MinMax(new Vector2(182, 112), GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.maxTimeDistance, 0f, 10, 51);
            GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.leavingForce = 
                Knobs.MinMax(new Vector2(253, 112), GetInstanceComponent<FPSControlRangedWeapon>().weaponPath.definition.leavingForce, 0, 250, 52);

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;            

            GUI.EndGroup();
        }
        #endregion


        #region Melee
        private void GUIMeleeDamageWindow(int windowIndex)
        {
            GUI.BeginGroup(windowSpaces[windowIndex]);
            GUI.Box(new Rect(0, 0, gui_window_melee_damage.width, gui_window_melee_damage.height), gui_window_melee_damage, GUIStyle.none);

            EditorGUI.BeginChangeCheck();

            CheckDragAreaForComponent<Collider>(new Rect(15, 50, 66, 18), ref ((FPSControlMeleeWeapon)InstanceComponent).damageTrigger);
            InstanceComponent.definition.maxDamagePerHit = 
                Knobs.MinMax(new Vector2(110, 35), InstanceComponent.definition.maxDamagePerHit, 0, 100, 101);

            dirty = EditorGUI.EndChangeCheck() ? true : dirty;
            

            GUI.EndGroup();
        }
        #endregion

        #endregion


        #region logic

        //private static string TEMP_PREFAB { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TEMPWEAPON.prefab"; } }
        //private FPSControlWeapon[] weapons;
        //private WeaponData currentWeapon;
        //private bool weaponsAvailable;
        private List<string> impactNames = new List<string>();
        private int impactIndex = 0;
        //private static string lastWeaponName;


        //private const string PREF_SAVED_PREFIX = "FPSCONTROL_WEAPON_SAVED_";

        #region ##### OLD CODE #####

        /*
        private void SaveWeaponCopy(FPSControlWeapon weapon)
        {
            Serializer.SaveData<FPSControlWeaponDefinitions>(PREF_SAVED_PREFIX + weapon.definition.weaponName, new FPSControlWeaponDefinitions(InstanceComponent), true, false, false);
            EditorPrefs.SetBool(PREF_SAVED_PREFIX + "NEEDS_SAVING", true);
            if (!Application.isPlaying) SaveIfPrefab(InstanceComponent.gameObject);
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
        */

#endregion
        
        public WeaponControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.WeaponControl;
        }

        public override void Init()
        {
            LoadAssets();

            GetPrefabs();

            LoadImpactNames();

            base.Init();
        }

        private bool _rebuild = true;
        override public void OnFocus(bool rebuild)
        {
            _rebuild = rebuild;
            //Debug.Log("Focused - Rebuilt: " + rebuild + " - Wasplaying: " + wasPlaying);            
            Init();

            #region ##### OLD CODE #####

            /*
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
            */

            #endregion

            base.OnFocus(rebuild);
        }

        override public void OnLostFocus(bool rebuild)
        {
            
            //Debug.Log("Lost Focus - Rebuilt: " + rebuild + " - Wasplaying: " + wasPlaying);            
            base.OnLostFocus(rebuild);
        }

        public override void OnDestroy()
        {
            Close();
            base.OnDestroy();
        }

        public override void Deinit()
        {
            Close();
            base.Deinit(); 
        }

        void Close()
        {
            if (_prefabInstance && !EditorApplication.isPlaying)
                Object.DestroyImmediate(_prefabInstance);
        }

        int prevIndex = -1;
        public override void Update()
        {
            /*//if (currentIndex == -1) Debug.Log("index = -1");
            if (currentIndex != prevIndex)
                Debug.Log("index: " + currentIndex);
            prevIndex = currentIndex;
            */
            if (!EditorApplication.isPlaying)
            {
                if (_prefabInstance)
                {
                    //_prefabInstance.transform.localPosition = InstanceComponent.definition.pivot;
                    //_prefabInstance.transform.localRotation = Quaternion.Euler(InstanceComponent.definition.euler);
                }
            }
        }

        public override void OnPromptInput(string userInput)
        {
            if (names.Contains(userInput))
            {
                EditorUtility.DisplayDialog("Cannot Create Weapon", string.Format("There is already a weapon named '{0}' in existance.", userInput), "OK");
                return;
            }
            
            CreateNew<FPSControlRangedWeapon>(userInput);
        }

        private void AttachWeaponToManager()
        {
            //FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
            //if (managers.Length > 0) // Error
            //{
            //    List<FPSControlWeapon> actors = new List<FPSControlWeapon>(managers[0].weaponActors);
            //    actors.Add(InstanceComponent);
            //    managers[0].weaponActors = actors.ToArray();
            //    if (managers.Length > 1)
            //        Debug.LogWarning("More than one Weapon Manager. This may produce unexpected results.");
            //}

                /*
                List<FPSControlWeapon> actors = new List<FPSControlWeapon>(managers[0].weaponActors);
                actors.Add(InstanceComponent);
                managers[0].weaponActors = actors.ToArray();
                InstanceComponent.transform.parent = managers[0].transform;
                InstanceComponent.transform.localPosition = Vector3.zero;
                InstanceComponent.transform.localEulerAngles = Vector3.zero;
                InstanceComponent.transform.localScale = Vector3.one;
                */
            
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
                go = InstanceComponent.transform.gameObject;
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

        private void SwitchWeaponType()
        {
            //FPSControlPlayerWeaponManager mgr = WeaponManager;
            //int indexOf =  ArrayUtility.IndexOf<FPSControlWeapon>(mgr.weaponActors, InstanceComponent);
            //Debug.Log("Manager has instance at index " + indexOf);
            
            if (IsWeaponRanged)
                ChangeWeaponType<FPSControlRangedWeapon, FPSControlMeleeWeapon>();
            else
                ChangeWeaponType<FPSControlMeleeWeapon, FPSControlRangedWeapon>();

            //mgr.weaponActors[indexOf] = InstanceComponent;

            /*
            GameObject go = InstanceComponent.transform.gameObject;
            string weaponName = InstanceComponent.name;
            FPSControlWeapon oldWeapon = InstanceComponent;
            CreateNewWeapon(weaponName, !currentWeapon.isRanged, false);
            FPSControlPlayerWeaponManager[] managers = (FPSControlPlayerWeaponManager[])GameObject.FindSceneObjectsOfType(typeof(FPSControlPlayerWeaponManager));
            foreach (FPSControlPlayerWeaponManager manager in managers) //Go through all the managers and make sure we rerefrence the new one
            {
                for (int i = 0; i < manager.weaponActors.Length; i++)
                {
                    if (manager.weaponActors[i] == oldWeapon)
                    {
                        manager.weaponActors[i] = InstanceComponent;
                    }
                }
            }
            GameObject.DestroyImmediate(oldWeapon);
            */
        }

        // No longer necessary
        private void LocateWeapons()
        {
            /*
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
            */
        }

        // No longer necessary
        private void SetCurrentWeapon(FPSControlWeapon weapon)
        {
            // This function should not be needed anymore.

            /*
            //Debug.Log("Setting Weapon");
            currentWeapon = new WeaponData();
            InstanceComponent = weapon;
            currentWeapon.isRanged = (weapon.GetType() == typeof(FPSControlRangedWeapon));
            CheckCurrentWeapon();
            if (Application.isPlaying) lastWeaponName = weapon.definition.weaponName;

            if (InstanceComponent != null && impactNames.Contains(InstanceComponent.impactName))
            {
                impactIndex = impactNames.IndexOf(InstanceComponent.impactName);
            }
            else
            {
                impactIndex = 0;
            }
            //SetCurrentSave();
             * */
        }

        private void CheckCurrentWeapon()
        {
            CheckWeapon(InstanceComponent);
        }

        private void CheckWeapon(FPSControlWeapon weapon)
        {
            weapon.definition.weaponName = weapon.name;
            if (weapon.modelOffset == null)
                weapon.modelOffset = GetOrCreateChild(weapon.transform, weapon.name + " Model");
            if (weapon.modelController == null)
                weapon.modelController = GetOrCreateChild(InstanceComponent.modelOffset, weapon.name + " Controller");
            if (weapon.weaponAnimation == null)
                weapon.weaponAnimation = GetOrCreateComponent<FPSControlWeaponAnimation>(weapon.modelController);
            if (weapon.weaponParticles == null)
                weapon.weaponParticles = GetOrCreateComponent<FPSControlWeaponParticles>(weapon.modelController);
            if (weapon.weaponSound == null)
                weapon.weaponSound = GetOrCreateComponent<FPSControlWeaponSound>(weapon.modelController);

            if (weapon is FPSControlRangedWeapon && GetInstanceComponent<FPSControlRangedWeapon>().weaponPath == null)
                GetInstanceComponent<FPSControlRangedWeapon>().weaponPath = GetOrCreateComponent<FPSControlWeaponPath>(weapon.modelController);


            //if (currentWeapon.isRanged && ((FPSControlRangedWeapon)InstanceComponent).weaponPath == null) ((FPSControlRangedWeapon)InstanceComponent).weaponPath = GetOrCreateComponent<FPSControlWeaponPath>(currentWeapon.modelController);
            //if (currentWeapon.isRanged) ((FPSControlRangedWeapon)InstanceComponent).weaponPath = ((FPSControlRangedWeapon)InstanceComponent).weaponPath;
        }

        private void LoadImpactNames()
        {
            try
            {
                impactNames = new List<string>();

                impactIndex = 0;

                string assetPath = FPSControlMainEditor.RESOURCE_FOLDER + "ImpactControlDefinitions.asset";
                //AssetDatabase.ImportAsset(assetPath);

                ImpactControlDefinitions loadedDef = (ImpactControlDefinitions)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ImpactControlDefinitions));

                impactNames.Add("None");

                if (loadedDef != null)
                {
                    foreach (ImpactControlDefinition impact in loadedDef.impacts)
                    {
                        if (!impactNames.Contains(impact.name))
                        {
                            impactNames.Add(impact.name);
                        }
                    }
                }

                if (InstanceComponent != null && impactNames.Contains(InstanceComponent.impactName))
                {
                    impactIndex = impactNames.IndexOf(InstanceComponent.impactName);
                }
            }
            catch (System.Exception err) 
            { 
                Debug.LogWarning("Caught Exception: " + err.Message);
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
