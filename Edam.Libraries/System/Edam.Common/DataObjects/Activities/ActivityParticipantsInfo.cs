using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityParticipantsInfo : ActivityInfo
   {

      private List<Entities.ParticipantInfo> m_Participants =
         new List<Entities.ParticipantInfo>();
      public List<Entities.ParticipantInfo> Participants
      {
         get { return m_Participants; }
         set
         {
            if (value == null)
            {
               m_Participants.Clear();
               return;
            }
            m_Participants = value;
         }
      }

      public ActivityParticipantsInfo()
      {
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         m_Participants.Clear();
      }

   }

}
