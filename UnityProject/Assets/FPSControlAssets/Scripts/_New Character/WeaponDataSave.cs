using UnityEngine;
using System.Collections;
using FPSControl;
using FPSControl.PersistentData;

public class WeaponDataSave : MonoBehaviour
{

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Save Weapon Data"))
        {
            FPSControlPlayerData.SaveWeaponData();
        }

        GUILayout.EndHorizontal();
    }
        
}
