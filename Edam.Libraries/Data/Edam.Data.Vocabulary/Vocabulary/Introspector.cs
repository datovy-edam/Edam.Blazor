using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon.Vocabulary
{

   public class Introspector
   {

      private readonly DataSet _dataSet;


      public Introspector(DataSet dataSet)
      {
         _dataSet = dataSet;
      }

      public void IntrospectElements()
      {

         foreach (var element in _dataSet.Elements)
         {
            // find entity
            string[] elementPath = element.Value.FullPath.Split("/");
            string entityFullPath = 
               elementPath[0] + "/" + elementPath[1] + "/" + elementPath[2];

            // make sure that corresponding parent entity exists, if not add it
            var efPath1 = _dataSet.Entities.TryGetValue(
               entityFullPath, out EntityItemInfo? value);
            var entityItem = _dataSet.Entities.Find(entityFullPath);
            if (entityItem == null)
            {
               _dataSet.Entities.Add(element.Value);
            }

            // validate element info and enhance it as needed
            element.Value.Introspect();
         }

      }

   }

}
