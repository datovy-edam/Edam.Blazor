using Edam.Data.FileSystemModel;

namespace Edam.Web.Models.Application
{

   public class AppSession
   {
      private static readonly string _sessionId = Guid.NewGuid().ToString();

      public static string SessionId
      {
         get { return _sessionId; }
      }

      public static ContainerInfo CurrentContainer { get; set; } = new();
   }

}
