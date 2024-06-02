using System;
namespace Edam.Data.Asset
{

   /// <summary>
   /// While attempting to map a type-system into a relational table/data set.
   /// A type within a type-system can get mapped into an FK data-elements
   /// or is mapped based on instancing of all type inherited elements of
   /// the type a child of the child and so on...
   /// </summary>
   /// <remarks>valid values are instanceKeys (for FK's) or
   /// instanceChildren (for getting inherited elements of the type)
   /// </remarks>
   public enum ElementTransform
   {
      Unknown = 0,
      InstanceKeys = 1,
      InstanceChildren = 2
   }

}
