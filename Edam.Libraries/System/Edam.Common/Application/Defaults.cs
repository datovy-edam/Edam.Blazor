using System;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

/// <summary>
/// 
/// </summary>
public class Defaults
{
   private static String defaultDiagnosticsDirectoryPath = "";

   public static Int32    InvalidEntityNo = -9999;
   public static Int32    SessionIdLen = 24;
   public static DateTime NullDate = new DateTime(1800,1,1);
   public static DateTime FutureDate = new DateTime(2100,1,1);
   public static String   StateCode  = "PR";
   public static String   UnknownPostalCodeId = "000000000";
   public static String   DateYyyyMmDdFormat = "yyyy-MM-dd";
   public static String   DefaultApplicationRevisionNo = "ApplicationRevisionNo";

   public struct AppSettingsKeys
   {
      public static readonly String DefaultSupervisorTypeNo = "DefaultSupervisorTypeNo";
      public static readonly String DefaultApplicationId = "DefaultApplicationId";
      public static readonly String DefaultCenterId = "DefaultCenterId";

      public static readonly String DefaultAppDbKey = "DefaultAppDbKey";
      public static readonly String DefaultDbKeyId = "DefaultDatabaseKey";
      public static readonly String DefaultDbCatalog = "DefaultDbCatalog";
      public static readonly String DefaultDbHost = "DefaultDbHost";
   }

   public static String DefaultDiagnosticsDirectoryPath
   {
      get
      {
         if (defaultDiagnosticsDirectoryPath == null)
            defaultDiagnosticsDirectoryPath = String.Empty;
         if (defaultDiagnosticsDirectoryPath.Length == 0)
         {
            // see if there is any configured diagnostics directory path
            //System.Configuration.AppSettingsReader asr =
            //   new System.Configuration.AppSettingsReader();
            //Object val;
            //String errMessage;
            //try
            //{
            //   val = asr.GetValue(
            //      Edam.Application.Resources.
            //         Strings.DefaultDiagnosticsDirectoryPath,
            //         typeof(System.String));
            //}
            //catch (System.Exception ex)
            //{
            //   errMessage = ex.Message;
            //   val = null;
            //}

            //if (val != null)
            //   defaultDiagnosticsDirectoryPath = (String)val;
            //else
               defaultDiagnosticsDirectoryPath = String.Empty;
         }
         return(defaultDiagnosticsDirectoryPath);
      }
   }  // end of DefaultDiagnosticsDirectoryPath property

}  // end of Defaults


}  // end of Edam.Application

