using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerGraph.Connector
{

   #region TokenExpirationChanged

   public delegate void TokenExpirationChangedEventHandler(
      object sender, TokenExpirationChangedArgs e);

   public class TokenExpirationChangedArgs : EventArgs
   {
      private readonly string _graph;
      private readonly string _token;
      private readonly long _expiration;
      public TokenExpirationChangedArgs(
         string graph, string token, long expiration)
      {
         _graph = graph;
         _token = token;
         _expiration = expiration;
      }

      public string Graph
      {
         get { return _graph; }
      }
      public string Token
      {
         get { return _token; }
      }
      public long Expiration
      {
         get { return _expiration; }
      }
   }

   #endregion TokenExpirationChanged

}
