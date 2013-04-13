using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FPSControlEditor;


namespace FPSControlEditor
{
    
    internal static class PurchaseInfo
    {
        private static PurchaseData data;

        internal static bool CheckForPurchase(FPSControlModuleType module)
        {
            if (module == FPSControlModuleType.BuildControl) return false;
            return true;
        }

        internal static void LoadWebResult(string json)
        {

        }

        private static void CreateTestJSON()
        {
            PurchaseData test = new PurchaseData();

            PurchaseModuleData a = new PurchaseModuleData();
            a.purchased = true;
            a.version = 1.0f;
            test.data.Add(FPSControlModuleType.MusicControl, a);

            PurchaseModuleData b = new PurchaseModuleData();
            b.purchased = true;
            b.version = 1.0f;
            test.data.Add(FPSControlModuleType.WeaponControl, b);

            Debug.Log(JSONDeserializer.GenerateJSON(test));
        }

    }
}
