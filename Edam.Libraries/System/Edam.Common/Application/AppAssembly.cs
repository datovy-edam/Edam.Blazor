using System;
using System.Collections.Generic;
using System.Reflection;
namespace Edam.Application
{

   public enum RegistryType
   {
      Unknown = 0,
      Registered = 1,
      Added = 2
   }

   public enum RegistryObjectType
   {
      Unknown = 0,
      Type = 1,
      Instance = 2
   }

   public class RegistryObjectInfo
   {
      public String KeyId { get; set; }
      public RegistryObjectType ObjectType { get; set; }
      public String Alias { get; set; }
      public Object Instance { get; set; }
   }

   /// <summary>
   /// Simple class to support the ability to register types by name and get
   /// an instance of the class at run time.
   /// </summary>
   /// <remarks>
   /// In your app if you can find a better way to provide IoC / Dependency
   /// Injection be my guess, and/or replace this class with needed code to
   /// manage those.
   /// </remarks>
   public class AppAssembly
   {
      private static readonly Dictionary<string, RegistryObjectInfo>
         typeRegistry = new Dictionary<string, RegistryObjectInfo>();

      public AppAssembly()
      {
      }

      /// <summary>
      /// Register Type with given name.
      /// </summary>
      /// <param name="registryName">type name</param>
      /// <param name="type">type to register</param>
      /// <returns>true is returned if name was added</returns>
      public static RegistryType RegisterType(
         string registryName, Type type, string alias = null)
      {
         if (typeRegistry.ContainsKey(registryName))
            return RegistryType.Registered;

         var o = new RegistryObjectInfo
         {
            KeyId = registryName,
            ObjectType = RegistryObjectType.Type,
            Instance = type,
            Alias = String.IsNullOrWhiteSpace(alias) ? registryName : alias
         };

         var i = typeRegistry.TryAdd(registryName, o);
         return i ? RegistryType.Added :
            RegistryType.Unknown;
      }

      /// <summary>
      /// Register Object with given name.
      /// </summary>
      /// <param name="registryName">type name</param>
      /// <param name="obj">object to register</param>
      /// <returns>true is returned if name was added</returns>
      public static RegistryType RegisterInstance(
         string registryName, Object obj, string alias = null)
      {
         if (typeRegistry.ContainsKey(registryName))
            return RegistryType.Registered;

         var o = new RegistryObjectInfo
         {
            KeyId = registryName,
            ObjectType = RegistryObjectType.Instance,
            Instance = obj,
            Alias = String.IsNullOrWhiteSpace(alias) ? registryName : alias
         };

         var i = typeRegistry.TryAdd(registryName, o);
         return i ? RegistryType.Added :
            RegistryType.Unknown;
      }

      /// <summary>
      /// Create Instance by registered type name.
      /// </summary>
      /// <param name="regitryName">register name</param>
      /// <returns>instance is returned if instantiated, else null</returns>
      public static Object FetchInstance(string regitryName)
      {
         if (!typeRegistry.ContainsKey(regitryName))
            return null;

         if (!typeRegistry.TryGetValue(
            regitryName, out RegistryObjectInfo obj))
            return null;
         if (obj.ObjectType == RegistryObjectType.Type)
            return CreateInstance((Type)obj.Instance);
         return obj.Instance;
      }

      /// <summary>
      /// Given an Alias, get corresponding Key ID.
      /// </summary>
      /// <param name="alias">alias to search</param>
      /// <returns>if found the key-id is returned, else null</returns>
      public static string GetKeyId(string alias)
      {
         foreach (var o in typeRegistry.Values)
         {
            if (o.Alias == alias)
            {
               return o.KeyId;
            }
         }
         return null;
      }

      /// <summary>
      /// Given a Key Id, get corresponding Alias.
      /// </summary>
      /// <param name="keyId">key ID</param>
      /// <returns>if found the alias is returned, else null</returns>
      public static string GetAlias(string keyId)
      {
         foreach (var o in typeRegistry.Values)
         {
            if (o.KeyId == keyId)
            {
               return o.Alias;
            }
         }
         return null;
      }

      /// <summary>
      /// Create Instance by registered type name.
      /// </summary>
      /// <param name="regitryName">register name, optionally specify the
      /// alias instead of the registry name</param>
      /// <returns>instance is returned if instantiated, else null</returns>
      public static T FetchInstance<T>(string registryName)
      {
         string rname = registryName;

         // try first by key then by alias
         if (!typeRegistry.ContainsKey(registryName))
         {
            rname = GetKeyId(registryName);
            if (String.IsNullOrEmpty(rname))
            {
               return default;
            }
         }

         if (!typeRegistry.TryGetValue(
            rname, out RegistryObjectInfo obj))
            return default;

         if (obj.ObjectType == RegistryObjectType.Type)
         {
            return CreateInstance<T>((Type)obj.Instance); ;
         }
         return (T)obj.Instance;
      }

      /// <summary>
      /// Create Instance of given type.
      /// </summary>
      /// <param name="type">type</param>
      /// <returns>instance is returned if instantiated, else null</returns>
      public static Object CreateInstance(Type type)
      {
         return Activator.CreateInstance(type);
      }

      /// <summary>
      /// Create Instance of given type.
      /// </summary>
      /// <param name="type">type</param>
      /// <returns>instance is returned if instantiated, else null</returns>
      public static T CreateInstance<T>(Type type)
      {
         return (T) Activator.CreateInstance(type);
      }

   }

}
