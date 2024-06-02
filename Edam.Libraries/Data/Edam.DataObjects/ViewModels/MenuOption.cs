using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.ViewModels
{

   public enum MenuOption
   {
      Unknown = 0,

      Home = 1,
      Dashboard = 10,
      FollowUp = 20,
      EditReferenceData = 21,
      Browse = 22,
      Library = 23,
      Projects = 24,

      People = 30,
      Group = 40,
      Report = 50,
      Viewer = 60,

      ResetApplication = 95,
      Administration = 96,
      PinLogin = 97,
      Login = 98,
      Logout = 99
   }

}
