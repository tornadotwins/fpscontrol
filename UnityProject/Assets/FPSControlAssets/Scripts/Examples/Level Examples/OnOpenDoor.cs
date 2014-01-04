using UnityEngine;
using System.Collections;
using FPSControl;

public class OnOpenDoor : InteractiveObject
{

    public override void Interact()
    {
        //Debug.Log("I AM THE ONE WHO KNOCKS");
        FPSControlPlayerData.SaveTempWeaponData(); // Save the temp data for changing scenes
        FPSControlPlayerData.SaveWeaponData(0); // Save for new session.

        // We know we're going to be moving to SaveTestB - so we should save there.
        FPSControlPlayerSaveData saveData = new FPSControlPlayerSaveData("SaveTestB",0,100F);
        FPSControlPlayerData.SavePlayerData(saveData);

        Application.LoadLevel("SaveTestB");
    }
}
