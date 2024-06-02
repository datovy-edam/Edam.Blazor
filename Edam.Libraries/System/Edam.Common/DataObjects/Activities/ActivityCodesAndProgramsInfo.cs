using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Activities
{

   public class ActivityCodeDefaultsInfo
   {

      public String DefaultTemplateId { get; set; }

      public ActivityCodeDefaultsInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         DefaultTemplateId = String.Empty;
      }

   }

   public class ActivityCodesAndProgramsInfo
   {

      #region -- 1.00 Properties and Fields

      public List<DataCodes.IDataCode> ActivityTypeGroups { get; set; }
      public List<DataCodes.IDataCode> ObjectStatus { get; set; }
      public List<DataCodes.IDataCode> ObjectScopes { get; set; }
      public List<DataCodes.IDataCode> ActivityTypes { get; set; }
      public List<DataCodes.IDataCode> ProgramTypes { get; set; }
      public List<DataCodes.IDataCode> Templates { get; set; }
      public List<ActivityProgramInfo> Programs { get; set; }
      public ActivityCodeDefaultsInfo Defaults { get; set; }

      #endregion

      public ActivityCodesAndProgramsInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         ActivityTypeGroups = null;
         ObjectStatus = null;
         ObjectScopes = null;
         ActivityTypes = null;
         ProgramTypes = null;
         Templates = null;
         Programs = null;
         Defaults = null;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityCodeDefaultsInfo ReadData(
         ActivityCodeDefaultsInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityCodeDefaultsInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.DefaultTemplateId = DataField.GetString(reader[0]);

         return record;
      }

      public void ReadData(System.Data.Common.DbDataReader reader)
      {
         ReadData(Defaults, reader);
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="defaults">defaults</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of ActivityProgramInfo'es</returns>
      public ActivityCodeDefaultsInfo GetList(
         ActivityCodeDefaultsInfo defaults,
         System.Data.Common.DbDataReader reader)
      {
         if (defaults == null)
            defaults = new ActivityCodeDefaultsInfo();
         while (reader.Read())
         {
            ReadData(defaults, reader);
         }
         return defaults;
      }

#endif

   }

   public class ActivityThreadWrapper
   {
      public List<ActivityContentInfo> Threads { get; set; }
   }

}
