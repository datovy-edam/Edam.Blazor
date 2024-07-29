using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Diagnostics
{

   public class ResultLogBase
   {

      #region    -- 1.11 Success Flag Property Support

      protected Boolean m_Success = true;
      public Boolean Success
      {
         get { return (m_Success); }
      }

      public Verbosity Verbosity { get; set; }

      #endregion
      #region    -- 1.12 Returned Value Property Support

      protected Int32 m_ReturnValue = 0;
      public Int32 ReturnValue
      {
         get { return (m_ReturnValue); }
         set { m_ReturnValue = value; }
      }

      public string ReturnText { get; set; }

      #endregion
      #region    -- 1.13 ResultValueObject and Tag Properties Support

      public Object Tag { get; set; }
      public Object ResultValueObject { get; set; }

      #endregion

   }

}
