using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetManagement
{

   public enum DomainType
   {
      Unknown = 0,
      Asset = 1,
      Instance = 2,
      InformationExchange = 10,
      Form = 20,
      UseCase = 30,
      CodeSet = 40,
      Document = 50,
      Report = 60,
      Table = 70,
      Schema = 80
   }

}
