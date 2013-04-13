using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using System.Reflection;
#endif
using System.Collections.Generic;
using JsonFx.Json;

namespace FPSControlEditor
{

    public class JSONDeserializer
    {

#if UNITY_EDITOR
	
	static BindingFlags bindingFlags =
			BindingFlags.Instance
			| BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Static
			| BindingFlags.DeclaredOnly
			| BindingFlags.GetField;
		
	
	public static T[] BuildObjects<T>(string json_source) where T : new()
	{
		return BuildObjects<T>(json_source, null);
	}
	
	public static T[] BuildObjects<T>(string json_source, string searchTargetName) where T : new()
	{
		List<T> returnList = new List<T>();
		
		// search through the provided json data string for a specifically named object
		object result = FindInJSON(json_source, searchTargetName);
		
		// if no object was found, return "null"
		if(result==null) return default(T[]);
		
		// try to cast the search result as a usable type
		Dictionary<string, object>[] objDef = result as Dictionary<string, object>[];
		
		// if the try-cast did not fail
		if(objDef!=null)
		{
			foreach(Dictionary<string, object> template in objDef)
			{
				returnList.Add(Build<T>(template));
			}
			
			return returnList.ToArray();
		}
		
		
		return default(T[]);
	}
	
	public static T Build<T>(Dictionary<string, object> template) where T : new()
	{
		return Build<T>(template, bindingFlags);
	}
	
	public static T Build<T>(Dictionary<string, object> template, BindingFlags flags) where T : new()
	{
		FieldInfo[] info = typeof(T).GetFields(flags);
		
		T t = new T();
		
		foreach(string key in template.Keys)
		{
			foreach(FieldInfo i in info)
			{
				
				if(i.Name == key && template[key] != null)
				{
					object val = template[key];
					Type fieldType = i.FieldType;
					
					try
					{
						if(fieldType.IsArray)
						{
							Type destinationType = fieldType.GetElementType();
							Array sourceArray = (Array)val;
							int c = sourceArray.Length;
							Array destinationArray = Array.CreateInstance(destinationType, c);
							
							for(int j = 0; j < c; j++)
							{
								object v = Convert.ChangeType(sourceArray.GetValue(j), destinationType);
								destinationArray.SetValue(v,j);
							}
							
							i.SetValue(t, destinationArray);
						}
						else if(fieldType.IsEnum)
						{
							i.SetValue(t, Enum.Parse(fieldType, (string)val, true) );
						}
						else
						{
							i.SetValue(t, Convert.ChangeType(val, fieldType));
						}
					}
					catch(System.Exception e)
					{
						Debug.LogWarning("Parsing Error - " + e.GetType().Name + "(" + e.Message + ")");
					}
				}
			}
		}
		
		return t;
	}
	
#endif

        public static string Set<T>(T json_obj) { return JsonWriter.Serialize(json_obj); }
        public static T Get<T>(string json_data) { return JsonReader.Deserialize<T>(json_data); }

        public static object FindInJSON(string json_data) { return FindInJSON(json_data, null); }
        public static object FindInJSON(string json_data, string searchTargetName)
        {
            Dictionary<string, object> o_json = JsonReader.Deserialize<Dictionary<string, object>>(json_data);

            if (searchTargetName == string.Empty || searchTargetName == null) return o_json;
            else return Search(o_json, searchTargetName);
        }

        static object Search(Dictionary<string, object> searchObject, string searchTargetName)
        {
            if (searchObject.ContainsKey(searchTargetName))
            {
                return searchObject[searchTargetName];
            }
            else
            {
                foreach (object sub_o in searchObject.Values)
                {
                    // try-cast to appropriate type or type[]
                    Dictionary<string, object> singleResult = sub_o as Dictionary<string, object>;
                    Dictionary<string, object>[] resultList = sub_o as Dictionary<string, object>[];

                    // if the try-cast results in a single result, handle it
                    if (singleResult != null)
                    {
                        object found = Search(singleResult, searchTargetName);
                        if (found != null) return found;
                    }

                    // if the tey-cast results in a multitue of results, handle it
                    else if (resultList != null)
                    {
                        foreach (Dictionary<string, object> result in resultList)
                        {
                            object found = Search(result, searchTargetName);
                            if (found != null) return found;
                        }
                    }
                }
            }

            return null;
        }

        public static string GenerateJSON(object input)
        {
            return JsonWriter.Serialize(input);
        }


    }

}