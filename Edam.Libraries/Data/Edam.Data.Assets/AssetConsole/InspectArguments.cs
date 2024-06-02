using System;
namespace Edam.Data.AssetConsole
{

   /// <summary>
   /// While working with a schema within a process how to inspect elements,
   /// how deep?
   /// </summary>
   public class InspectDepthArguments
   {
      /// <summary>
      /// How deep within the sample should go?
      /// </summary>
      public String MaxThreshold { get; set; }
      /// <summary>
      /// Length of Lists samples.
      /// </summary>
      public String ListLength { get; set; }

      public InspectDepthArguments()
      {
         MaxThreshold = "1";
         ListLength = "1";
      }
   }
}
