using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using JSON = Edam.Json.Jsd.JsdSchema;

namespace Edam.Json.JsonSample
{

   public class JsonInstance
   {

      private StringBuilder m_Builder = new StringBuilder();
      public string Indent = String.Empty;

      public string JsonText
      {
         get { return m_Builder.ToString(); }
      }

      public JsonInstance()
      {
         OpenInstance();
      }

      public void OpenInstance()
      {
         m_Builder.AppendLine(Indent + JSON.BLOCK_OPEN);
      }

      public void CloseInstance()
      {
         m_Builder.AppendLine(Indent + JSON.BLOCK_CLOSE);
      }

      #region -- 4.00 - Get Properties, Arrays, and Blocks

      public string GetProperty(
         string property, string value, bool continues)
      {
         return Indent + "\"" + property + "\"" + JSON.SEPARATOR 
            + " \"" + value + "\"" + (continues ? "," : String.Empty);
      }

      public string GetArrayStart(string property)
      {
         return Indent + "\"" + property + "\"" 
            + JSON.SEPARATOR + " " + JSON.ARRAY_OPEN;
      }

      public string GetArrayEnd(bool continues)
      {
         return Indent + JSON.ARRAY_CLOSE + (continues ? "," : String.Empty);
      }

      public string GetBlockStart(string property)
      {
         return Indent + "\"" + property + "\""
            + JSON.SEPARATOR + " " + JSON.BLOCK_OPEN;
      }

      public string GetBlockStart()
      {
         return Indent + JSON.BLOCK_OPEN;
      }

      public string GetBlockEnd(bool continues)
      {
         return Indent + JSON.BLOCK_CLOSE + (continues ? "," : String.Empty);
      }

      #endregion
      #region -- 4.00 - Add JSON Properties, Arrays and Blocks

      public void AddProperty(string property, string value, bool continues)
      {
         m_Builder.AppendLine(GetProperty(property, value, continues));
      }

      public void AddArrayStart(string property)
      {
         m_Builder.AppendLine(GetArrayStart(property));
      }

      public void AddArrayEnd(bool continues)
      {
         m_Builder.AppendLine(GetArrayEnd(continues));
      }

      public void AddBlockStart(string property)
      {
         m_Builder.AppendLine(GetBlockStart(property));
      }

      public void AddBlockStart()
      {
         m_Builder.AppendLine(GetBlockStart());
      }

      public void AddBlockEnd(bool continues = false)
      {
         m_Builder.AppendLine(GetBlockEnd(continues));
      }

      public void AddEmptyLine()
      {
         m_Builder.AppendLine(String.Empty);
      }

      #endregion

   }

}
