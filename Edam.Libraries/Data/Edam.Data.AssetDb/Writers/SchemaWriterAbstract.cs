using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.AssetManagement.Writers
{
   public abstract class SchemaWriterAbstract<T>
   {
      protected T m_Context;
      public SchemaWriterAbstract(T context)
      {

      }
   }
}
