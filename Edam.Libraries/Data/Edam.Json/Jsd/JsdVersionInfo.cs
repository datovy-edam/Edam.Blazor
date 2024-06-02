using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Json.Jsd
{

   public class JsdVersionInfo
   {
      private const String HTTP_SCHEMA = "http://json-schema.org/";
      private const String DRAFT04 = "draft-04";
      private const String DRAFT05 = "draft-05";
      private const String DRAFT06 = "draft-06";
      private const String DRAFT07 = "draft-07";
      private const String DRAFT09 = "draft/2019-09";
      private const String DRAFT12 = "draft/2020-12";
      private const String SCHEMA = "/schema#";
      private JsdVersion m_Version = JsdVersion.Draft04;
      public JsdVersionInfo(JsdVersion version)
      {
         m_Version = version;
      }

      public new String ToString()
      {
         return ToString(m_Version);
      }

      public static String ToString(JsdVersion version)
      {
         String txt = HTTP_SCHEMA;
         txt += version switch
         {
            JsdVersion.Draft04 => DRAFT04,
            JsdVersion.Draft05 => DRAFT05,
            JsdVersion.Draft06 => DRAFT06,
            JsdVersion.Draft07 => DRAFT07,
            JsdVersion.Draft09 => DRAFT09,
            JsdVersion.Draft12 => DRAFT12,
            _ => "",
         };
         return txt + SCHEMA;
      }
   }

}
