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
        public string version;
        public bool purchased;
        public string url;
        public string purl;
    }

    [Serializable]
    internal class LoginData
    {
        public bool userExsist = true;
        public bool passwordMatch = false;
    }

    [Serializable]    
    internal class ResponseData
    {
        public Dictionary<string, PurchaseModuleData> purchaseData = new Dictionary<string, PurchaseModuleData>();
        public LoginData login = new LoginData();
    }

}
