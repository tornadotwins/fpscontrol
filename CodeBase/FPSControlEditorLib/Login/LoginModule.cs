using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;
using System.Net;
using System.Text;
using System.IO;

namespace FPSControlEditor
{
    public class LoginModule : FPSControlEditorModule
    {
        const string BYPASS_LOGIN_USER = "!__=DEV=__!";//bypass for devs ;)
        const string BYPASS_LOGIN_PASS = "==";
        const string LOGIN_URI = "http://gameprefabs.com/login2.php"; //https://gameprefabs.com/login.php http://localhost/~efraim/logintest/index.html
        const string RESULT_SUCCESS_FREEPLAN = "success=1&type=freePlan";
        const string PATH = "Login/";

        #region Events
        public event System.Action<object> OnLoginPress;
        public event System.Action<object> OnLoginSuccess;
        public event System.Action<object> OnLoginFail;
        public event System.Action<object> OnRegisterPress;
        public event System.Action<object> OnOfflineModePress;

        #endregion // Events

        #region GUI Properties

        Texture background;
        Rect backgroundRect = new Rect(343, 197, 220, 203);
        Rect usernameFieldRect = new Rect(426, 254, 113, 16);
        Rect passwordFieldRect = new Rect(426, 285, 113, 16);
        Rect loginButtonRect = new Rect(445, 373, 107, 17);

        Rect registerButtonRect = new Rect(343, 414, 107, 17);
        Rect offlineButtonRect = new Rect(456, 414, 107, 17);

        Rect errorBox = new Rect(343, 175, 220, 20);
        string errorMsg = "";

        #endregion // GUI Properties

        #region Module Properties

        string username = "";
        string password = "";

        bool _fail = false;
        Color failTint = Color.red;
        double time;
        double deltaTime;

        #endregion // Module Properties

        public LoginModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.Login;
        }

        public override void Init()
        {
            time = EditorApplication.timeSinceStartup;

            background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "login_bg.png");
            username = EditorPrefs.GetString(FPSControlUserObject.KEY_HANDLE, "");
        }

        public override void OnGUI()
        {
            deltaTime = EditorApplication.timeSinceStartup - time;
            Color gc = GUI.color;
            
            if (_fail)
            {
                failTint = Color.Lerp(failTint, gc, (float) deltaTime * .0001F);
                GUI.color = failTint;

                if (failTint.r == gc.r && failTint.g == gc.b && failTint.b == gc.b && failTint.a == gc.a)
                {
                    failTint = Color.red;
                    _fail = false;
                }
                _editor.Repaint();
            }
            
            GUI.Box(backgroundRect, background, GUIStyle.none);
            
            username = GUI.TextField(usernameFieldRect, username);
            password = GUI.PasswordField(passwordFieldRect, password, "*"[0]);

            GUI.color = gc;

            if (GUI.Button(loginButtonRect, "sign in"))
            {
                OnLoginPress(null);
                DoLogin(true);
            }

            if (GUI.Button(registerButtonRect, "register"))
            {
                errorMsg = "";
                _fail = false;
                failTint = Color.red;
                OnRegisterPress(null);
            }
            GUI.enabled = false;
            if (GUI.Button(offlineButtonRect, "offline mode"))
            {
                errorMsg = "";
                _fail = false;
                failTint = Color.red;
                OnOfflineModePress(null);
            }
            GUI.enabled = true;

            GUIStyle gs = new GUIStyle();
            gs.normal.textColor = Color.red;
            gs.alignment = TextAnchor.MiddleCenter;
            if (errorMsg != "") GUI.Box(errorBox, errorMsg, gs);

            
        }

        #region Server Code

        void DoLogin(bool b)
        {
            errorMsg = "";
			
			string result = Login(username, password, true);

            RespounceHandler.LoadWebResult(result);

            if (RespounceHandler.validPassword)
            {
                OnLoginSuccess(new FPSControlUserObject(username, System.DateTime.Now));
                errorMsg = "";
            }
            else
            {
                if (!RespounceHandler.validUser)
                {
                    errorMsg = "User not found";
                }
                else
                {
                    errorMsg = "Incorrect Password";
                }
                OnLoginFail(null);
            }


            Debug.Log(result);

            /*
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://equals-equals.com/test.php?username="+username+"&password="+password); //https://gameprefabs.com/login.php?username=test&password=123");
            //req.Proxy = new System.Net.WebProxy("", true); //true means no proxy
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string result = sr.ReadToEnd().Trim();
            */
            //Debug.Log(result);

            //b = (result == RESULT_SUCCESS_FREEPLAN);

            //Debug.Log(result + " == success=1&type=freePlan " + b); 



            //if(b)
            //{
            //    OnLoginSuccess(new FPSControlUserObject(username,System.DateTime.Now));
            //    errorMsg = "";
            //}
           // else
           // {
            //    _fail = true;
			//	if(result=="error=IncorrectLogin2")
			//	{
			//		errorMsg = "Incorrect Login";
			//	}
			//	else
			//	{
			//		errorMsg = "Error: " + result;
			//	}
                //errorMsg = "Error: "+result;
				//Debug.Log("Login Failure: "+result);
                //Debug.Log("Login Failure");
            //    OnLoginFail(null);
            //}
        }

        internal string Login(string un, string pwd, bool t)
        {

            //a quick check for the super-secret bypass
            if (un == BYPASS_LOGIN_USER && pwd == BYPASS_LOGIN_PASS) return RESULT_SUCCESS_FREEPLAN;

            // this is what we are sending
            string post_data = "username="+un+"&password="+pwd;

            // this is where we will send it
            string uri = LOGIN_URI; 

            try
            {
                _fail = false;
                failTint = Color.red;

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                // create a request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri); 
                request.KeepAlive = true;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";

                // turn our request string into a byte stream
                byte[] postBytes = Encoding.ASCII.GetBytes(post_data);

                // this is important - make sure you specify type this way
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postBytes.Length;
				request.Timeout = 15000;
				request.AllowAutoRedirect = false;
                Stream requestStream = request.GetRequestStream();

                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                //requestStream.Close();

                // grab te response and print it out to the console along with the status code
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				//Debug.Log ("Web Responded: " + response.GetResponseStream ());
				
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
				string myResult = new StreamReader(response.GetResponseStream()).ReadToEnd();
				
				//Debug.Log ("Stream Responded: " + myResult );
				
                return myResult; //sr.ReadToEnd();
            }
            catch(System.Exception ex)
            {
                _fail = true;
                failTint = Color.red;
                //Debug.Log("Exception caught: " + ex.Message);
                errorMsg = "Cannot connect to server.";
				return errorMsg;
            }

            return "Returned empty handed";
        }

        bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
		/*
        internal void Login(string email, string pwd)
        {

			// Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            System.Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            System.Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
        }  
		 */
        #endregion // Server Code
    }

    public class FPSControlUserObject : object
    {
        internal const string KEY_TIMESTAMP = "_FPSControl_UserLoginTimestamp";
        internal const string KEY_HANDLE = "_FPSControl_UserLoginHandle";
        internal const int MAX_SESSION = 60;
        
        public string name
        {
            get
            {
                return EditorPrefs.GetString(KEY_HANDLE, "");
            }
            set
            {
                EditorPrefs.SetString(KEY_HANDLE, value);
            }
        }
        public System.DateTime loginTime
        {
            get
            {
                return GetTimestamp();
            }
            set
            {
                EditorPrefs.SetString(KEY_TIMESTAMP, value.ToString());
            }
        }

        public bool loggedIn
        {
            get
            {
                System.DateTime now = System.DateTime.Now;
                return (now.Subtract(loginTime).TotalMinutes < MAX_SESSION);
            }
        }

        internal static void LogOut()
        {
            EditorPrefs.SetString(KEY_TIMESTAMP, System.DateTime.MinValue.ToString());
        }

        internal static FPSControlUserObject current
        {
            get
            {
                if (GetHandle() != string.Empty && GetTimestamp() != System.DateTime.MinValue)
                {
                    return new FPSControlUserObject(GetHandle(), GetTimestamp());
                }

                return null;
            }
        }

        public FPSControlUserObject(string n, System.DateTime t)
        {
            name = n;
            loginTime = t;
			
			
            //Debug.Log("Creating User Object: " + n + ", " + t.ToString());
        }

        internal static System.DateTime GetTimestamp()
        {
            System.DateTime ts;
            if (!System.DateTime.TryParse(EditorPrefs.GetString(KEY_TIMESTAMP, System.DateTime.MinValue.ToString()), out ts))
            {
                return System.DateTime.MinValue;
            }
            return ts;
        }

        internal static string GetHandle()
        {
            return EditorPrefs.GetString(KEY_HANDLE, string.Empty);
        }
    }
}
