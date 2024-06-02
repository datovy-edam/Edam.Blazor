using System;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data
{

   public interface ILDbObject
   {
      string TableName { get; }
   }

   public interface ILDbIntIdObject : ILDbObject
   {
      int IdNo { get; set; }
   }

   public interface ILDbIdObject : ILDbObject
   {
      string Id { get; set; }
   }

}
