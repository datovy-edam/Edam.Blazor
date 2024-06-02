using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;
namespace Edam.DataObjects.Models
{

   public class ApplicationElementInfo : ElementInfo
   {

      private const string JSON_EXT = "json";
      private const string DATA_TEMPLATES_FOLDER = "DataTemplates";

      public static ElementNodeInfo GetElementNode(
         string fileName, string name, string description)
      {
         string filePath = Session.GetApplicationFullPath(
            DATA_TEMPLATES_FOLDER, fileName, JSON_EXT);
         ElementNodeInfo info = ElementNodeInfo.FromJsonFile(filePath, name,
            description);
         return info;
      }

   }

}
