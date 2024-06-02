using System;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.ViewModels
{

   public interface IControlView
   {
      IMenu ParentMenu { get; set; }

      // TODO: Implement this as a Delegate
      void Back(Object sender, EventArgs e);
   }

}
