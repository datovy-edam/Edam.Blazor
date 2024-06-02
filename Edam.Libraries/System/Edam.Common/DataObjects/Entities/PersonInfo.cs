using System;
using System.Collections.Generic;

#if DATA_SUPPORT_
using System.Data.Common;
#endif

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.DataObjects.Locations;
using Edam.DataObjects.Notes;

namespace Edam.DataObjects.Entities
{
   
   public interface IPersonRecord : IPersonBaseIdentificationInfo
   {
      List<LocationAddressInfo> Locations { get; set; }
      List<NoteInfo> Notes { get; set; }
   }

   public class PersonInfo : PersonIdentificationInfo, IPersonRecord
   {
      public readonly static String FIELD_DELIMITER = "~~~";

      public List<LocationAddressInfo> Locations { get; set; }
      public List<NoteInfo> Notes { get; set; }

      public DateTime? NoteLastReferenceDate { get; set; }

      public Int32 NoteDaysSinceLast
      {
         get
         {
            if (!NoteLastReferenceDate.HasValue)
               NoteLastReferenceDate = new DateTime(2100, 1, 1);
            TimeSpan s = DateTime.Now - NoteLastReferenceDate.Value;
            return s.Days;
         }
      }

      public PersonInfo() : base()
      {
         Locations = new List<LocationAddressInfo>();
         Notes = new List<NoteInfo>();
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         Locations.Clear();
         Notes.Clear();
         NoteLastReferenceDate = new DateTime(2100, 1, 1);
      }

      public Boolean Validate()
      {
         Boolean v = !String.IsNullOrWhiteSpace(Name.GivenName) &&
            !String.IsNullOrWhiteSpace(Name.FatherSurname) &&
            !String.IsNullOrWhiteSpace(Email) &&
            !String.IsNullOrWhiteSpace(Phone.PhoneNumber);
         return v;
      }

      public Boolean ValidateFind()
      {
         Boolean v =
            !String.IsNullOrWhiteSpace(Email) ||
            !String.IsNullOrWhiteSpace(Phone.PhoneNumber);
         return v;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>Instance of PersonInfo is returned</returns>
      public static PersonInfo ReadData(DbDataReader reader)
      {
         PersonInfo p = new PersonInfo();
         PersonIdentificationInfo.ReadData(reader, p);
         Int32 i = PersonIdentificationInfo.INDEX_TILL_NOTE;
         if (reader.FieldCount > i)
         {
            var l = DataField.GetString(reader[i]);
            var t = DataField.GetString(reader[i + 1]);
            var a = LocationAddressInfo.GetDelimitedAddress(l, FIELD_DELIMITER);
            var n = NoteInfo.GetDelimitedNote(t, FIELD_DELIMITER);
            if (a != null)
               p.Locations.Add(a);
            if (n != null)
               p.Notes.Add(n);
         }
         return p;
      }

      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public new static List<PersonInfo> GetList(DbDataReader reader)
      {
         List<PersonInfo> l =
            new List<PersonInfo>();
         PersonInfo o;
         while (reader.Read())
         {
            o = ReadData(reader);
            l.Add(o);
         }
         return l;
      }
      
#endif

   }
}
