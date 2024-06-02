using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.DataObjects.Dashboards;

namespace Edam.DataObjects.Users
{

   public class UserAlertInfo
   {

      public List<NoteAlertInfo> NoteAlertsList { get; set; }
      public List<RequestAlertInfo> RequestAlertsList { get; set; }

      public UserAlertInfo()
      {
         NoteAlertsList = new List<NoteAlertInfo>();
         RequestAlertsList = new List<RequestAlertInfo>();
         ClearFields();
      }

      public void ClearFields()
      {
         NoteAlertsList.Clear();
         RequestAlertsList.Clear();
      }

   }

}
