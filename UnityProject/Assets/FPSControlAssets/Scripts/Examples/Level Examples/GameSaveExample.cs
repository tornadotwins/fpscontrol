using UnityEngine;
using System.Collections;
using FPSControl;
using FPSControl.Data;

public class GameSaveExample : MonoBehaviour
{
    public static GameSaveExample Instance{ get { return GameObject.Find("[MANAGER]").GetComponent<GameSaveExample>(); } }

    static bool _startup = true;

    void Start()
    {
        
        if (_startup)
        {
            // this is where your data lives
            Debug.Log("Application.persistentDataPath: " + Application.persistentDataPath);
            
            // first run toggle
            _startup = false;

            // load save data
            FPSControlPlayerSaveData saveData = FPSControlPlayerData.LoadPlayerSaveData(0);

            // if we have save data, load level of last save
            if (saveData != null)
            {
                // weapon data will be loaded from the temporary cache on player spawn
                // so knowing this, what we can do is move over our saved data to a temp and let it all work automatically...

                // load the data
                FPSControlPlayerWeaponManagerSaveData weaponSave = FPSControlPlayerData.LoadWeaponData(0);

                if (weaponSave != null) // if we have saved weapons data...
                {
                    FPSControlPlayerData.SaveTempWeaponData(weaponSave); // save it to temp cache
                    Debug.Log("active weapon: " + weaponSave.activeWeaponName);
                }
                // load the last saved level
                Application.LoadLevel(saveData.currentLevelName);
            }
            else
            {
                saveData = new FPSControlPlayerSaveData("SaveTestA", 0, 100F);
                FPSControlPlayerData.SavePlayerData(saveData);
                
                // load start
                Application.LoadLevel("SaveTestA");
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Clear Save Data"))
        {
            PersistentData.DeleteData(PersistentData.NS_PLAYER);
            PersistentData.DeleteData(PersistentData.NS_WEAPONS);
        }

        GUILayout.EndHorizontal();
    }

}
