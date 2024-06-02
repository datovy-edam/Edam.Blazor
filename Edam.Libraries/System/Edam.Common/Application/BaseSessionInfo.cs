using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Entities = Edam.DataObjects.Entities;
using Edam.DataObjects.Properties;

namespace Edam.Application
{

   public class BaseSessionInfo
   {

      public String Key { get; set; }
      public Edam.LocaleLanguage Language { get; set; }
      public RuntimeEnvironment RuntimeEnvironment { get; set; }
      public String  ApplicationId { get; set; }
      public String  SessionId { get; set; }
      public String  DomainId { get; set; }
      public String  UserId { get; set; }
      public Boolean UserIsActive { get; set; }
      public Boolean UserCanUseApplication { get; set; }
      public Boolean UseDatabase { get; set; }
      public String  ApplicationName { get; set; }
      public String  EntityId { get; set; }

      public String OrganizationId { get; set; }
      public String AgentId { get; set; }
      public String AgentName { get; set; }
      public String EmailUrlId { get; set; }
      public String Email { get; set; }

      protected static Dictionary<string, IProperty> m_Properties = 
         new Dictionary<string, IProperty>();
      public Dictionary<string,IProperty> Properties
      {
         get { return m_Properties; }
         set { m_Properties = value; }
      }

      public List<Entities.EntityEmailInfo> EntityEmails { get; set; }
      public List<Edam.DataObjects.Application.
         ApplicationEntityGroupInfo> EntityGroups { get; set; }
      public List<BasePolicyInfo> Policies { get; set; }

      public BaseSessionInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         Key = String.Empty;
         RuntimeEnvironment = GetRuntimeEnvironment();
         Language = LocaleLanguage.Unknown;
         ApplicationId = String.Empty;
         SessionId = String.Empty;
         DomainId = String.Empty;
         UserId = String.Empty;
         UserIsActive = false;
         UserCanUseApplication = false;
         ApplicationName = String.Empty;
         Policies = null;
         EntityId = String.Empty;
         OrganizationId = String.Empty;
         AgentId = String.Empty;
         AgentName = String.Empty;
         UseDatabase = true;
      }

      /// <summary>
      /// Get the Runtime Environment... (Production or Testing)
      /// </summary>
      /// <remarks>Testing is the default in the case that the app was not
      /// specifically configured or set</remarks>
      /// <returns>the environment is returned</returns >
      public RuntimeEnvironment GetRuntimeEnvironment()
      {
         if (this.RuntimeEnvironment == RuntimeEnvironment.Unknown)
         {
            String e = AppSettings.GetString(
               Resources.Strings.EnvironmentKey);
            if (String.IsNullOrEmpty(e))
               this.RuntimeEnvironment = RuntimeEnvironment.Test;
            else
            {
               e = e.ToLower();
               if (e == Resources.Strings.Production)
                  this.RuntimeEnvironment = RuntimeEnvironment.Production;
               else
                  this.RuntimeEnvironment = RuntimeEnvironment.Test;
            }
         }
         return this.RuntimeEnvironment;
      }

   }

}

