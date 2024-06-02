using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.ViewModels
{

   public class GotoEventArgs : EventArgs
   {
      public Boolean ShowMainMenu { get; set; }
      public MenuOption MenuOption { get; set; }
      public Object State { get; set; }

      public GotoEventArgs(Object state = null) : base()
      {
         MenuOption = MenuOption.Unknown;
         ShowMainMenu = false;
         State = state;
      }
   }

   public interface IMenuItemParent
   {
      void Goto(Object sender, GotoEventArgs e);
   }

}
