using System;
namespace Edam.Data.AssetConsole
{

   public enum AssetConsoleProcedure
   {

      Unknown = 0,
      CreateProject = 1,

      // Asset DB (context) to DDL (folder/file)
      DdlFromDb = 10,
      JsdFromDb = 11,
      XsdFromDb = 12,

      // Schema to Output (folder/file)
      ToFile = 30,
      DdlToFile = 31,
      JsdToFile = 32,
      XsdToFile = 33,
      DbToFile = 34,
      DbDdlToFile = 35,
      EdiToFile = 36,
      EdiToXsd = 37,
      ToJsonLd = 38,
      ToXsd = 39,
      UseCaseToFile = 40,

      // (folder/file) Schema to Asset DB (context)
      ToDb = 50,
      DdlToDb = 51,
      JsdToDb = 52,
      XsdToDb = 53,
      EdiToDb = 54,
      AssetsToDb = 60,
      DbDdlToDb = 61,

      // schema to Assets
      DdlImportToAssets = 70,
      DdlToAssets = 71,
      JsdToAssets = 72,
      XsdToAssets = 73,
      DbToAssets = 74,
      DbDdlToAssets = 75,
      DbDdlToGsql = 76,
      EdiToAssets = 77,

      // from Assets
      AssetsToDdl = 90,
      AssetsToDataTemplate = 91,
      AssetsToGql = 92,
      AssetsToXsd = 93,
      AssetsToJsd = 94,
      AssetsToJld = 95,
      AssetsToOpenXml = 96,
      AssetsToLexiconWorkbook = 97,
      AssetsToLexiconDatabase = 98,

      // json
      JsonToAssets = 105,

      // other
      FileToCodeSet = 120,

      // To Dictionary
      XsdToAssetsReport = 800

   }

}
