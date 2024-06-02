using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   public enum QueryStringTag
   {
      Unknown = 0,
      SessionId = 1,
      OrganizationId = 2,
      DeviceId = 3,
      Email = 4,
      UserEmail = 5,
      Password = 6,
      ReferenceId = 7,
      RequestId = 8,
      ApiApplicationId = 9,
      ApiReferenceId = 10,
      ApiKeyValue = 11,
      Request = 12,
      Language = 13,
      Option = 14,
      EntityId = 15,
      GivenName = 16,
      MiddleName = 17,
      FatherLastName = 18,
      MotherLastName = 19,
      FullName = 20,
      SocialSecurityId = 21,
      DriverLicenseId = 22,
      PhoneNumber = 23,
      BirthDate = 24,
      Alias = 25,
      Description = 26,
      ReferenceDate = 27,
      ReferenceType = 28,
      NoteId = 29,
      Status = 30,
      GroupNo = 31,
      OptionNo = 32,
      TemplateNo = 33
   }

   public class QueryStringParameters
   {

      private Int16 m_Count = 0;
      protected System.Text.StringBuilder m_Builder =
         new System.Text.StringBuilder();

      public void Add(String key, String value)
      {
         if (String.IsNullOrWhiteSpace(key))
            return;
         m_Builder.Append(((m_Count >= 1) ? "&" : "?") + key + "=" +
            (String.IsNullOrWhiteSpace(value) ? String.Empty : value));
         m_Count++;
      }

      public void Add(String key, DateTime? value)
      {
         Add(key, (value.HasValue ? String.Empty : value.Value.ToString(
            Edam.Application.Resources.Strings.DefaultDateTimeFormat)));
         m_Count++;
      }

      public override String ToString()
      {
         return m_Builder.ToString();
      }

   }

   public class QueryStringBuilder : QueryStringParameters
   {

      public readonly static String[] Tags =
      {
         "unknown",
         "sessionId",
         "organizationId",
         "deviceId",
         "email",
         "userEmail",
         "password",
         "referenceId",
         "requestId",
         "apiApplicationId",
         "apiReferenceId",
         "apiKeyValue",
         "request",
         "language",
         "option",
         "entityId",
         "givenName",
         "middleName",
         "fatherLastName",
         "motherLastName",
         "fullName",
         "socialSecurityId",
         "driverLicenseId",
         "phoneNumber",
         "birthDate",
         "alias",
         "description",
         "referenceDate",
         "referenceType",
         "noteId",
         "status",
         "groupNo",
         "optionNo",
         "templateNo"
      }  ;

      public void Add(QueryStringTag tag, String value)
      {
         String key = Tags[(Int32)tag];
         Add(key, value);
      }

      public void Add(QueryStringTag tag, DateTime? value)
      {
         String key = Tags[(Int32)tag];
         Add(key, (value.HasValue ? value.Value.ToString(
            Edam.Application.Resources.Strings.DefaultDateTimeFormat) :
            String.Empty));
      }

      public void Add(QueryStringTag tag, int? value)
      {
         String key = Tags[(int)tag];
         Add(key, (value.HasValue ? value.Value.ToString() :
            String.Empty));
      }

      //public void Add(QueryStringTag tag, int? value)
      //{
      //   String key = Tags[(int)tag];
      //   Add(key, value.ToString());
      //}

      public void Add(QueryStringTag tag, short? value)
      {
         String key = Tags[(Int32)tag];
         Add(key, (value.HasValue ? value.Value.ToString() :
            String.Empty));
      }

   }

}
