using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Requests;

namespace Edam.DataObjects.SelfHelp
{

   /// <summary>
   /// Registration Request Info.  This information is needed to post a request
   /// using the "Help.HelpRegisterRequestInsertUpdate" stored procedure.
   /// </summary>
   [Serializable]
   public class SelfHelpUserInfo : Entities.UserBaseInfo, Entities.IUserBaseInfo
   {

      #region -- Define Self Help User details

      public SelfHelpRequest RequestType { get; set; }
      public RequestStatus RequestStatus { get; set; }

      public Int16 RequestTypeNo
      {
         get { return (Int16)RequestType; }
         set
         {
            RequestType = (SelfHelpRequest)value;
         }
      }

      public Int16 StatusNo
      {
         get { return (Int16)Status; }
         set
         {
            RequestStatus = (RequestStatus)value;
         }
      }

      public String SessionId { get; set; }
      public String SessionAlternateId { get; set; }
      public String OutSessionId { get; set; }

      public String RequestId { get; set; }
      public Boolean Verified { get; set; }
      public Int16 ValidStatusNo { get; set; }
      public String OutRequestId { get; set; }
      public Int16 OptionNo { get; set; }

      #endregion
      #region -- Define User Details...

      #endregion
      #region -- Initialize Class

      public SelfHelpUserInfo()
      {
         ClearFields();
      }

      public SelfHelpUserInfo(Entities.UserAccountInfo account)
      {
         if (account == null)
            account = new Entities.UserAccountInfo();
         ClearFields();
         Copy(account);
      }

      public new void ClearFields()
      {
         base.ClearFields();
         SessionId = String.Empty;
         SessionAlternateId = String.Empty;
         RequestId = String.Empty;
         RequestTypeNo = 0;
         StatusNo = 0;

         ValidStatusNo = 0;
         OutSessionId = String.Empty;
         OutRequestId = String.Empty;
         OptionNo = 0;
      }

      #endregion

   }

}
