using Edam.DataObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.ReferenceData
{

   public interface IReferenceDataTemplateReader
   {
      List<ReferenceDataTemplateInfo> FromFolder(string folderPath);
   }

}
