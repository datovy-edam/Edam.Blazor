using Edam.Application;
using Edam.Data.AssetUseCases;
using Edam.Data.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Books
{

   public class BookHelper
   {
      public const string BOOK_PROCESSOR_KEY = nameof(IBookItemProcessor);

      /// <summary>
      /// Using dependency injection fetch an instance of a Book Item Processor.
      /// </summary>
      /// <returns>instance of processor is returned</returns>
      public static IBookItemProcessor FetchBookItemProcessor()
      {
         return AppAssembly.FetchInstance(
            BOOK_PROCESSOR_KEY) as IBookItemProcessor;
      }

      /// <summary>
      /// Fetch and initialize Book Item Processor.
      /// </summary>
      /// <param name="useCase">use case map</param>
      /// <param name="jsonDocumentText">JSON document instance</param>
      /// <returns>instance of procesor is returned</returns>
      public static IBookItemProcessor GetBookItemProcessor(
         AssetUseCaseMap useCase, string jsonDocumentText)
      {
         var proc = FetchBookItemProcessor();
         if (proc == null)
         {
            return null;
         }
         proc.Initialize(useCase, jsonDocumentText);
         return proc;
      }

   }

}
