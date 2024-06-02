using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.References
{

   public enum ReferenceBaseType
   {
      TestUser = -1,
      Unknown = 0,
      Personal = 1,
      Administration = 2,
      Institution = 3,
      Corporate = 4,
      Government = 5,
      Member = 6,
      Employee = 7,
      Exchange = 8,
      Promotion = 9,
      Patient = 10,
      BusinessPartner = 11,
      CostCenter = 12,
      Supplier = 13,
      Customber = 14,
      Bank = 15,
      SystemUser = 17,
      Principal = 18,
      Agency = 19,
      Organization = 20,
      ServiceAccount = 21,
      Associate = 22,
      Participant = 23,
      Student = 24,
      Professor = 25,
      Director = 26,
      Signator = 27,
      Instructor = 28,
      Visitor = 29,
      Inspector = 30,
      Observer = 31,
      Sponsor = 32,
      Agent = 33,

      Shipper = 40,
      Carrier = 41,
      Consignee = 42,
      Broker = 43,

      Location = 60,
      Entity = 61,
      ActivityProgram = 62,
      ActivityThread = 63,
      ActivityProgramThreadSession = 64,
      Ledger = 65,
      RegisteredDocument = 66,
      Reference = 67,
      Media = 68,
      ActivityRating = 69,
      Note = 70,
      LedgerEntry = 71,

      Group = 800,
      GroupMember = 801,
      ReferenceObjectId = 999999
   }

}
