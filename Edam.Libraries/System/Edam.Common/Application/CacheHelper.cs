using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;

namespace Edam.Application
{

   // TODO: remove the following DataCodeInfo tables and load the values from DB

   public class CodeSet
   {

      public static List<DataCodeInfo> RecordStatusCode = new List<DataCodeInfo>()
      {
         new DataCodeInfo { CodeId = "A", Description = "Active" },
         new DataCodeInfo { CodeId = "I", Description = "Inactive" },
         new DataCodeInfo { CodeId = "D", Description = "Deleted" },
         new DataCodeInfo { CodeId = "P", Description = "Purged" }
      };

   }

   public class Purpose
   {

      public static List<DataCodeInfo> RequestUserPurposes = new List<DataCodeInfo>()
      {
         new DataCodeInfo { CodeId = "101", Description = "Notify Acceptance" },
         new DataCodeInfo { CodeId = "102", Description = "Acceptance Comment" },
         new DataCodeInfo { CodeId = "103", Description = "Conditional Acceptance" },
         new DataCodeInfo { CodeId = "104", Description = "Request for more Information" },
         new DataCodeInfo { CodeId = "105", Description = "Issues found (please callback)" },
         new DataCodeInfo { CodeId = "106", Description = "Not Approved for specified reasons" },
         new DataCodeInfo { CodeId = "107", Description = "Progress Note" },
         new DataCodeInfo { CodeId = "108", Description = "Attention Required" },
         new DataCodeInfo { CodeId = "109", Description = "Callback Request" },
         new DataCodeInfo { CodeId = "501", Description = "Follow-Up" }
      };

      public static List<DataCodeInfo> PersonNotesPurposes = new List<DataCodeInfo>()
      {
         new DataCodeInfo { CodeId = "104", Description = "Request for more Information" },
         new DataCodeInfo { CodeId = "107", Description = "Progress Note" },
         new DataCodeInfo { CodeId = "108", Description = "Attention Required" },
         new DataCodeInfo { CodeId = "109", Description = "Callback Request" },
         new DataCodeInfo { CodeId = "201", Description = "Payment or Credit Issues Found" },
         new DataCodeInfo { CodeId = "202", Description = "Scheduled Payment Failed" },
         new DataCodeInfo { CodeId = "203", Description = "Service or Account Closing or Cancelation" },
         new DataCodeInfo { CodeId = "20", Description = "Call" },
         new DataCodeInfo { CodeId = "21", Description = "Call Back" },
         new DataCodeInfo { CodeId = "22", Description = "Left Message" },
         new DataCodeInfo { CodeId = "23", Description = "Left Message with Spouse" },
         new DataCodeInfo { CodeId = "24", Description = "Disconnected" },
         new DataCodeInfo { CodeId = "25", Description = "Unavailable" },
         new DataCodeInfo { CodeId = "26", Description = "No Answer" },
         new DataCodeInfo { CodeId = "27", Description = "No Number" },
         new DataCodeInfo { CodeId = "28", Description = "No Response (after 7 days of Message)" },
         new DataCodeInfo { CodeId = "29", Description = "Not In Service" },
         new DataCodeInfo { CodeId = "30", Description = "Wrong Number" },
         new DataCodeInfo { CodeId = "31", Description = "Busy" },
         new DataCodeInfo { CodeId = "32", Description = "Looking into CareerLink (or Other)" },
         new DataCodeInfo { CodeId = "33", Description = "Looking into PathStone (or Other)" },
         new DataCodeInfo { CodeId = "34", Description = "Looking into O.V.R. (or Other)" },
         new DataCodeInfo { CodeId = "35", Description = "N.A. Mailbox Not Set Up " },
         new DataCodeInfo { CodeId = "36", Description = "Forwarded to Voicemail" },
         new DataCodeInfo { CodeId = "37", Description = "No Funds" },
         new DataCodeInfo { CodeId = "38", Description = "Needs Payment Plan" },
         new DataCodeInfo { CodeId = "39", Description = "Not Interested" },
         new DataCodeInfo { CodeId = "40", Description = "Wireless Cus. Not Available" },
         new DataCodeInfo { CodeId = "41", Description = "Interested, Obtaining Requirements" },
         new DataCodeInfo { CodeId = "42", Description = "Not Accepting Calls" },
         new DataCodeInfo { CodeId = "43", Description = "Interested, Will Call Back" },
         new DataCodeInfo { CodeId = "44", Description = "Interested, No Funds" },
         new DataCodeInfo { CodeId = "45", Description = "E-Mailed Information" },
         new DataCodeInfo { CodeId = "501", Description = "Follow-Up" }
      };

      public static List<DataCodeInfo> RatingNotesPurposes = new List<DataCodeInfo>()
      {
         new DataCodeInfo { CodeId = "104", Description = "Request for more Information" },
         new DataCodeInfo { CodeId = "107", Description = "Progress Note" },
         new DataCodeInfo { CodeId = "108", Description = "Attention Required" },
         new DataCodeInfo { CodeId = "109", Description = "Callback Request" },
         new DataCodeInfo { CodeId = "401", Description = "Check Clalifications" },
         new DataCodeInfo { CodeId = "402", Description = "Participation Note" },
         new DataCodeInfo { CodeId = "20", Description = "Call" },
         new DataCodeInfo { CodeId = "21", Description = "Call Back" },
         new DataCodeInfo { CodeId = "22", Description = "Left Message" },
         new DataCodeInfo { CodeId = "23", Description = "Left Message with Spouse" },
         new DataCodeInfo { CodeId = "24", Description = "Disconnected" },
         new DataCodeInfo { CodeId = "25", Description = "Unavailable" },
         new DataCodeInfo { CodeId = "26", Description = "No Answer" },
         new DataCodeInfo { CodeId = "27", Description = "No Number" },
         new DataCodeInfo { CodeId = "28", Description = "No Response (after 7 days of Message)" },
         new DataCodeInfo { CodeId = "29", Description = "Not In Service" },
         new DataCodeInfo { CodeId = "30", Description = "Wrong Number" },
         new DataCodeInfo { CodeId = "31", Description = "Busy" },
         new DataCodeInfo { CodeId = "32", Description = "Looking into CareerLink (or Other)" },
         new DataCodeInfo { CodeId = "33", Description = "Looking into PathStone (or Other)" },
         new DataCodeInfo { CodeId = "34", Description = "Looking into O.V.R. (or Other)" },
         new DataCodeInfo { CodeId = "35", Description = "N.A. Mailbox Not Set Up " },
         new DataCodeInfo { CodeId = "36", Description = "Forwarded to Voicemail" },
         new DataCodeInfo { CodeId = "37", Description = "No Funds" },
         new DataCodeInfo { CodeId = "38", Description = "Needs Payment Plan" },
         new DataCodeInfo { CodeId = "39", Description = "Not Interested" },
         new DataCodeInfo { CodeId = "40", Description = "Wireless Cus. Not Available" },
         new DataCodeInfo { CodeId = "41", Description = "Interested, Obtaining Requirements" },
         new DataCodeInfo { CodeId = "42", Description = "Not Accepting Calls" },
         new DataCodeInfo { CodeId = "43", Description = "Interested, Will Call Back" },
         new DataCodeInfo { CodeId = "44", Description = "Interested, No Funds" },
         new DataCodeInfo { CodeId = "45", Description = "E-Mailed Information" },
         new DataCodeInfo { CodeId = "501", Description = "Follow-Up" }
      };

   }

   public class CacheHelper
   {

      // TODO: get types from the database and remove static lists above...
      public static List<DataCodeInfo> GetNoteTypes(Int16 groupNo)
      {
         return Purpose.PersonNotesPurposes;
      }

      public static List<DataCodeInfo> GetRecordStatusCodes()
      {
         return CodeSet.RecordStatusCode;
      }

   }

}
