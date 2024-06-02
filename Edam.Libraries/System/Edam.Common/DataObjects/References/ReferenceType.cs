
// -----------------------------------------------------------------------------

namespace Edam.DataObjects.References
{

   /// <summary>
   /// Entity, Activity, or Reference type
   /// </summary>
   public enum ReferenceType
   {
      Unknown = 0,
      Entities = 10,
      Application = 11,
      Person = 12,
      Organization = 13,
      Location = 14,
      Activity = 40,
      Rating = 41,
      Group = 50,
      Document = 60,
      Media = 70,
      MediaBlob = 71,
      Note = 80,
      Notification = 81,
      Ledger = 90,
      Request = 100
   }

}
