using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

// -----------------------------------------------------------------------------
// https://stackify.com/net-core-dependency-injection/

namespace Edam.Application
{

   /// <summary>
   /// Dependency Service
   /// </summary>
   public class DependencyService
   {
      private static IServiceCollection m_Collection = new ServiceCollection();
      private static ServiceProvider m_Provider = null;

      public static IServiceCollection Collection
      {
         get { return m_Collection; }
      }
      public static ServiceProvider Container
      {
         get { return m_Provider; }
      }

      public static void Compile()
      {
         m_Provider = m_Collection.BuildServiceProvider();
      }

      public static T Get<T>()
      {
         return m_Provider.GetService<T>();         
      }
   }

}
