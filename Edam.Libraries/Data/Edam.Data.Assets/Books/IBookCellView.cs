using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Books
{

   public interface IBookCellView
   {
      BookletCellInfo GetCell();
      void SetCell(BookletCellInfo cell);

      string GetInputText();
      string GetOutputText();

      void SetInputText(string text);
      void SetOutputText(string text);

      object Instance { get; }
   }

}
