using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Medias
{

   public enum MediaFormat
   {
      Unknown = 0,
      JPEG = 1,
      MPEG = 2,
      TextFile = 3,
      RtfFile = 4,
      MsWordFile = 5,
      PdfFile = 6,
      XmlDocument = 7,
      PNG = 8,
      OfficeWordXml = 9,
      XML = 11,
      XSLT = 14,
      JSON = 16,

      JPEG2000 = 20,
      WSQ = 21,
      Bitmap = 22,
      VectorDatav = 23,
      FaxGroup4Standard = 24
   }

}
