using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.Serialization;
using Edam.TextParse;
using Edam.Data.Asset;
using Newtonsoft.Json;

namespace Edam.Data.Books
{

   /// <summary>
   /// Booklet Cell details.
   /// </summary>
   public class BookletCellInfo
   {
      public string BookletId { get; set; }
      public string CellId { get; set; } = Guid.NewGuid().ToString();
      public string ReferenceId { get; set; }

      /// <summary>
      /// Cell Type that could be: Text or Code.
      /// </summary>
      public BookletCellType CellType { get; set; } = BookletCellType.Unknown;

      /// <summary>
      /// Cell content type such as: Text, Html, MD, SQL, JSONATA or other
      /// </summary>
      public BookletTextType TextType { get; set; } = BookletTextType.Text;

      public string Text { get; set; }

      /// <summary>
      /// an instance of a UI control that is used to hold text or code.
      /// </summary>
      [JsonIgnore]
      public IBookCellView Instance { get; set; }

      /// <summary>
      /// Set output text...
      /// </summary>
      /// <param name="outputText">text to output</param>
      public void SetOutputText(string outputText)
      {
         Instance.SetOutputText(outputText);
      }
   }

}
