using System;

// -----------------------------------------------------------------------------

namespace Edam.Application.Resources
{

public class Strings
{

   public const String Unknown = "Unknown";

   /// <summary>
   /// The DefaultDiagnosticsDirectoryPath configuration entry should be made
   /// in the App.config file.
   /// </summary>
   public const String DefaultDiagnosticsDirectoryPath =
      "DefaultDiagnosticsDirectoryPath";

   public const String True = "true";
   public const String False = "false";

   public const String DefaultOrganization = "DefaultOrganization";
   public const String DefaultUrlSource = "DefaultUrlSource";
   public const String DefaultApplicationLogPath = "DefaultApplicationLogPath";
   public const String DefaultLoggerDateFormat = "yy-MM-dd HH:mm:ss";
   public const String DefaultDateTimeShortFormat = "MM/dd/yyyy hh:mm";
   public const String DefaultDateTimeFormat = "yyyy-MM-dd HH:mm";
   public const String DefaultDateYyyyMmDdFormat = "yyyy-MM-dd";
   public const String DefaultDateYyyyMmDdMonthDateFormat =
      "yyyy-MM-dd MMMM dddd";
   public const String DefaultDateYyyyMonthddDateFormat = "yyyy-MMMM-dd dddd";
   public const String DefaultTimeShortFormat = "hh:mmt";
   public const String DefaultMilitaryTimeShortFormat = "HH:mm";
   public const String DefaultYearMonthLabelFormat = "yyyy MMMM";

   public const String EnvironmentKey = "Environment";
   public const String Production = "production";
   public const String Testing = "testing";
   public const String DefaultCountry = "US";

   public const String UseDatabase = "UseDatabase";
   public const String DefaultVerbosityLevel = "DefaultVerbosityLevel";
   public const String DefaultApplicationIdKey = "DefaultApplicationId";
   public const String DefaultMerchantKey = "DefaultMerchantKey";
   public const String DefaultMerchantProductionKey = "DefaultMerchantProductionKey";
   public const String DefaultMerchantTestKey = "DefaultMerchantTestKey";
   public const String DefaultKey = "Default";
   public const String MerchantPayPalKey = "PayPal";

   public const String DefaultDummyId = "DUMMY";
   public const String DefaultAgencyId = "COMMONS";
   public const String DefaultAgencyName = "Commons";
   public const String DefaultPublicId = "Public";
   public const String DefaultPersonId = "Person";
   public const String Undefined = "undefined";
   public const String Success = "Success";
   public const String Failed = "Failed";
   public const String ContentImagesPathKey = "ContentImagesPath";
   public const String WebAppUrlKey = "WebAppUrl";

      public struct CacheKeys
      {
         public const String EntityAgenciesGetKey = "EntityAgenciesGetKey";
         public const String OffenseRegistryListKey = "OffenseRegistryListKey";
         public const String OffenseListKey = "OffenseListKey";
         public const String OffenseListTypesKey = "OffenseListTypesKey";
         public const String TownCodesKey = "TownCodes";
      }

      public struct StoreProcedure
      {
         public const String FailedWithReturnValue = "Failed with Return value of (";
         public const String FailedExecution = "Stored Procedure Execution Failed";
      }

      public struct HttpContentTypes
      {
         public const String ApplicationOctetStream = "application/octet-stream";
         public const String ApplicationJson = "application/json";
      }

      public struct Html
      {
         public const String TagBreak = "<br/>";
      }

   }  // end of Strings

}  // end of Edam.Application.Resources

