using System;
using System.Collections.Generic;
using System.Reflection;

// -----------------------------------------------------------------------------

namespace Edam.Services
{

   public class TypeActivator
   {

      /// <summary>
      /// Activate type defined in given assembly using its name.
      /// </summary>
      /// <param name="assemblyName">assembly name</param>
      /// <param name="typeName">type name</param>      
      /// <returns>object instance is returned, null if issues found</returns>
      public static Object Activate(String assemblyName, String typeName)
      {
         AssemblyName n = new AssemblyName(assemblyName);
         Assembly asm = Assembly.Load(n);
         Type type = asm.GetType(typeName);
         Object obj = Activator.CreateInstance(type);
         return obj;
      }

      /// <summary>
      /// Activate type defined in given assembly using its name.
      /// </summary>
      /// <param name="assemblyName">assembly name</param>
      /// <param name="typeName">type name</param>      
      /// <param name="parameter">parameter to pass</param>  
      /// <returns>object instance is returned, null if issues found</returns>
      public static Object Activate(
         String assemblyName, String typeName, String parameter)
      {
         AssemblyName n = new AssemblyName(assemblyName);
         Assembly asm = Assembly.Load(n);
         Type type = asm.GetType(typeName);
         Object obj = Activator.CreateInstance(type, parameter);
         return obj;
      }

      /// <summary>
      /// Activate type using given type info.
      /// </summary>
      /// <param name="type">type info</param>
      /// <returns>object instance is returned, null if issues found</returns>
      public static Object Activate(TypeInfo type)
      {
         if (type.Assembly == null)
         {
            AssemblyName n = new AssemblyName(type.AssemblyName);
            type.Assembly = Assembly.Load(n);
         }
         Type typ = type.Assembly.GetType(type.TypeName);
         Object obj = Activator.CreateInstance(typ);
         return obj;
      }

      /// <summary>
      /// Activate type related to given key as configured in the Services 
      /// config file.
      /// </summary>
      /// <param name="key"></param>
      /// <returns>object instance is returned, null if issues found</returns>
      //public static Object Activate(String key)
      //{
      //   TypeInfo type = ServicesSession.Configurations.FindType(key);
      //   if (type == null)
      //      return null;
      //   return Activate(type);
      //}

   }


}
