using Edam.Data.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Books
{

   public class BookEventArgs : EventArgs
   {

   }

   public class ProcessCellArgs : EventArgs
   {
      public BookletEventType EventType { get; set; }
      public BookletCellInfo Cell { get; set; }
   }

   delegate void ProcessCellEvent(object sender, ProcessCellArgs args);

}
