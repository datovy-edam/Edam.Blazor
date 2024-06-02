using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.
// Upra Framework prepared by Eduardo Sobrino, PR.Net

namespace Edam.Data
{

   /// <summary>
   /// Data Provider Factory...
   /// </summary>
   public class DataProviderFactory
   {
      public static readonly String DefaultInvariantName =
         "System.Data.SqlClient";

      /*
      private static System.Data.Common.DbProviderFactory m_Factory;
      public static System.Data.Common.DbProviderFactory Factory
      {
         get { return m_Factory; }
      }

      /// <summary>
      /// Provider Invariant Name.
      /// </summary>
      /// <param name="invariantName">(optional) provider invariant name
      /// </param>
      static DataProviderFactory()
      {
         m_Factory = DbProviderFactories.
            GetFactory(DefaultInvariantName);
      }
       */

      public static DbParameter CreateParameter()
      {
         return new SqlParameter();
      }

      public static DbConnection CreateConnection()
      {
         return new SqlConnection();
      }

      public static DbCommand CreateCommand()
      {
         return new SqlCommand();
      }

   }  // end of DataProviderFactory

}
