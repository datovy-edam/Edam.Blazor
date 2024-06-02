using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole.Services;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Data.AssetProject;
using Edam.Diagnostics;
using Edam.InOut;

namespace Edam.Data.Asset.Services
{

   public class ProjectConsole
   {

      static ProjectConsole()
      {
         AssetServiceHelper.PrepareProceduresRegistry();
      }

      public static void Initialize()
      {

      }

      /// <summary>
      /// Get Arguments for the Process...
      /// </summary>
      /// <param name="projectItem">project item</param>
      /// <returns>instance of results log will be returned and if successfull
      /// it will contain the conctext</returns>
      public static ResultsLog<ProcessArgumentsContext> GetArgsContext(
         object projectItem)
      {
         ResultsLog<ProcessArgumentsContext> results = 
            new ResultsLog<ProcessArgumentsContext>();
         ItemBaseInfo item = projectItem as ItemBaseInfo;
         if (item == null)
         {
            results.Failed(EventCode.Failed);
            return results;
         }

         string currDirectory = System.IO.Directory.GetCurrentDirectory();
         try
         {
            // move to the project arguments folder...
            System.IO.Directory.SetCurrentDirectory(
               item.Path.Replace("\\Arguments", String.Empty));

            // perform requested service...
            var context = AssetServiceHelper.GetArgsContext(item.Full);

            // prepare results...
            results.ResultValueObject = context;
            results.Data = context;

            results.Copy(context.Results);
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            System.IO.Directory.SetCurrentDirectory(currDirectory);
         }

         return results;
      }

      /// <summary>
      /// Get Arguments for the Process...
      /// </summary>
      /// <param name="projectItem">project item</param>
      /// <returns>instance of results log will be returned and if successfull
      /// it will contain the conctext</returns>
      public static ResultsLog<object> Execute(object projectItem,
         AssetConsoleArgumentsInfo arguments, string argumentsFilePath)
      {
         ResultsLog<object> results = new ResultsLog<object>();
         ItemBaseInfo item = projectItem as ItemBaseInfo;
         if (item == null)
         {
            results.Failed(EventCode.Failed);
            return results;
         }

         string currDirectory = System.IO.Directory.GetCurrentDirectory();
         try
         {
            // move to the project arguments folder...
            System.IO.Directory.SetCurrentDirectory(
               item.Path.Replace("\\Arguments", String.Empty));

            // perform requested service...
            AssetServiceHelper.Execute(arguments, argumentsFilePath);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            System.IO.Directory.SetCurrentDirectory(currDirectory);
         }

         return results;
      }

      /// <summary>
      /// Process Item using given ItemBaseInfo...
      /// </summary>
      /// <param name="projectItem">object that contains the ItemBaseInfo
      /// </param>
      /// <param name="procedure">(optional) procedure to execute</param>
      /// <returns>ResultsLog instance is returned</returns>
      public static ResultsLog<object> ProcessItem(object projectItem,
         AssetConsoleProcedure procedure = AssetConsoleProcedure.Unknown)
      {
         ResultsLog<object> results = new ResultsLog<object>();
         ItemBaseInfo item = projectItem as ItemBaseInfo;
         if (item == null)
         {
            results.Failed(EventCode.Failed);
            return results;
         }

         string currDirectory = System.IO.Directory.GetCurrentDirectory();
         try
         {
            // move to the project arguments folder...
            System.IO.Directory.SetCurrentDirectory(
               item.Path.Replace("\\Arguments", String.Empty));

            // perform requested service...
            var context = AssetServiceHelper.Execute(item.Full, procedure);

            // prepare results...
            AssetConsoleArgumentsInfo args = 
               context.Results.ResultValueObject as AssetConsoleArgumentsInfo;
            results.ResultValueObject = args;

            results.Copy(context.Results);
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            System.IO.Directory.SetCurrentDirectory(currDirectory);
         }

         return results;
      }

   }

}
