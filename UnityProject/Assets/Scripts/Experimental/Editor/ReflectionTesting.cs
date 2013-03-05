using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

public class ReflectionTesting : EditorWindow {

	[MenuItem("FPS Control/Test")]
	
	static void Init()
    {
        //Debug.Log("Opening FPSControl.");
        ReflectionTesting window = EditorWindow.GetWindow<ReflectionTesting>();		
//		Assembly assembly = Assembly.GetAssembly(typeof(pb_Geometry_Interface));
//        Type type = assembly.GetType("UnityEditorInternal.LogEntries");
//        MethodInfo method = type.GetMethod("Clear");
//        method.Invoke(new object(), null);
		
		
		
        window.Show();
		
		
		
    }
	
	void OnGUI() {
		if (GUILayout.Button("Test1")) {
			
//			Assembly assembly = Assembly.GetAssembly(typeof(pb_Preferences));
//			
//			PropertyInfo[] properties = typeof(pb_Preferences).GetProperties();
//			foreach(PropertyInfo property in properties)
//			{
//			    PreferenceItem attribute =
//			        PreferenceItem.GetCustomAttribute(property, typeof(PreferenceItem)) as PreferenceItem;
//			
//			    if (attribute != null) // This property has a StoredDataValueAttribute
//			    {
//					Debug.Log(attribute.name);
//					//object obj = Activator.CreateInstance(pb_Preferences);
//			        property.SetValue(assembly, "TEST", null); // null means no indexes
//			    }
//			}
			
			
			//System.Reflection.MemberInfo info = typeof(pb_Preferences);
			
			
//			PreferenceItem[] prefs = (PreferenceItem[])info.GetCustomAttributes(true);
//			Debug.Log(prefs.Length);
//			prefs[0].name = "testing";

			//PropertyDescriptorCollection a = TypeDescriptor.GetProperties(GetType(pb_Preferences));
			//Debug.Log(a.Count);
				
		    //Dim att As CategoryAttribute = DirectCast(prop.Attributes(GetType(CategoryAttribute)), CategoryAttribute)
		    //Dim cat As FieldInfo = att.GetType.GetField("categoryValue", BindingFlags.NonPublic Or BindingFlags.Instance)
		   // cat.SetValue(att, "A better description")
					
			System.Reflection.MemberInfo info = typeof(pb_Preferences).GetMethod("PreferencesGUI");
      		object[] attributes = info.GetCustomAttributes(true);
			Debug.Log(attributes.Length);
			Debug.Log(((PreferenceItem)attributes[0]).name);
			((PreferenceItem)attributes[0]).name= "TEST1";
			Debug.Log(((PreferenceItem)attributes[0]).name);
			
			attributes[0] = new PreferenceItem("TEST2");
			Debug.Log(((PreferenceItem)attributes[0]).name);
			
			//TypeDescriptor.GetAttributes(
			
			//var ca = TypeDescriptor.GetAttributes(typeof(pb_Preferences)).OfType<pb_Preferences>().FirstOrDefault
        	//Console.WriteLine(ca.); // <=== nice
        	//TypeDescriptor.AddAttributes(typeof(Foo),new CategoryAttribute("naughty"));
        	//ca = TypeDescriptor.GetAttributes(typeof(Foo)).OfType<CategoryAttribute>().FirstOrDefault();
        	//Console.WriteLine(ca.Category); // <=== naughty
			
			//PreferenceItem[] prefs = (PreferenceItem[])typeof(pb_Preferences.PreferencesGUI).GetCustomAttributes(typeof(PreferenceItem), false);
			//Debug.Log(prefs.Length);
			//prefs[0].name = "testing";
		}
	}
	
	
	
}
