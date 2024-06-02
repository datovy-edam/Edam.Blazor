using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;

// -----------------------------------------------------------------------------

namespace Edam.Net.Web
{

   public class WebHelper
   {

#if WEBCLIENT_SUPPORT_

      static public IpGeoInfo GetComputerPublicIp()
      {
         string url = "http://freegeoip.net/xml/";
         WebClient wc = new WebClient();
         wc.Proxy = null;
         byte[] buff = wc.DownloadData(url);
         wc.Dispose();
         String xmlText = System.Text.Encoding.ASCII.GetString(buff);
         IpGeoInfo ip = Kif.Serialization.XmlSerializer<IpGeoInfo>.
            FromXmlString(xmlText);
         return ip;
      }

#endif

   }

}
