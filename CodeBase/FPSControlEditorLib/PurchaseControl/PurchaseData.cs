using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FPSControlEditor;
using System.Runtime.CompilerServices;

namespace FPSControlEditor
{
    [Serializable]
    internal class PurchaseModuleData
    {
       public float version;
       public bool purchased;
    }

    [Serializable]
    internal class LoginData
    {
       public bool userExsist;
       public bool passwordMatch;
       public string subscription;
    }

    [Serializable]    
    internal class PurchaseData
    {
        public Dictionary<FPSControlModuleType, PurchaseModuleData> data = new Dictionary<FPSControlModuleType,PurchaseModuleData>();
        public LoginData login = new LoginData();
    }

}
