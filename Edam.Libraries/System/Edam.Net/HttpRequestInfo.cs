using Edam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Net
{

   public class HttpRequestInfo
   {
      public string BaseUri { get; set; }
      public string UserID { get; set; }
      public string UserSecret { get; set; }

      public WebApiContentType ContentType { get; set; } = 
         WebApiContentType.ApplicationJson;

      public Dictionary<string, string> Headers { get; set; } = 
         new Dictionary<string, string>();
   }

}
