using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.DataObjects.Models
{

   public interface IPresentationComponent
   {
      Object ToModelComponent(ModelGroupInfo group, ComponentType type);
      ResultLog ToFileFolder(String fileFolderPath, String fileName = "");
   }

}
