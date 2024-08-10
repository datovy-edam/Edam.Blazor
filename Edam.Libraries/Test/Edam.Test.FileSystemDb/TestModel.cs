using Edam.Data.FileSystemDb;
using Edam.Data.FileSystemModel;
using Edam.InOut;

namespace Edam.Test.FileSystemDb
{

   [TestClass]
   public class TestModel
   {
      private FileSystemInstance GetInstance()
      {
         // initialize repository
         FileSystemInstance instance = new FileSystemInstance();
         instance.SetContainer("", Guid.NewGuid().ToString());
         return instance;
      }

      [TestMethod]
      public void TestContextInitialization()
      {
         // initialize repository
         FileSystemInstance instance = GetInstance();

         TestPathParsing();

         instance.Dispose();
      }

      [TestMethod]
      public void TestPathParsing()
      {
         FileItemInfo fitem = new FileItemInfo();
         fitem.FullPath = "C:/Users/esobr/Documents/Edam.Studio";
         CatalogPathItem item = new CatalogPathItem(fitem);

         fitem.FullPath = "C:/Users/esobr/Documents/Edam.Studio/coco.json";
         item = new CatalogPathItem(fitem);

      }

      [TestMethod]
      public void TestTreeBuilder()
      {
         FileSystemInstance instance = GetInstance();
         CatalogInfo catalog =
            new CatalogInfo(instance, Guid.NewGuid().ToString());
         catalog.InitializeCatalog("", true);
      }

      [TestMethod]
      public void TestPathTreeBuilder()
      {
         FileSystemInstance instance = GetInstance();
         CatalogInfo catalog = 
            new CatalogInfo(instance, Guid.NewGuid().ToString());
         catalog.InitializeCatalog("");

         CatalogTreeBuilder builder = new CatalogTreeBuilder(instance, catalog);
         var pitem = builder.GetItem("C:/Users/esobr/Documents/Edam.Studio");
         pitem = builder.GetItem("C:/Users/esobr/Documents/Edam.Studio/coco.json");
      }
   }

}
