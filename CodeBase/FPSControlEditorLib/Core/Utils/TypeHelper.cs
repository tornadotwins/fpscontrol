using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

namespace FPSControlEditor
{
    class TypeHelper
    {
        public static Type GetType(string TypeName)
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {

                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;

                // Ask that assembly to return the proper Type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;

            }

            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in assemblies)
            {
                if (A != null)
                {
                    // See if that assembly defines the named type
                    type = A.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            // The type just couldn't be found...
            return null;
        }

    }
}
