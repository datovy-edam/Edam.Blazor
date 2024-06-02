using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data;
using Edam.Data.Asset;
using Edam.DataObjects.ReferenceData;

namespace Edam.DataObjects.Assets
{

   public class DataCodeSetRecord
   {

      public static ResultsLog<List<CodeSetInfo>> DataCodeSetGet(
         string sessionId, string organizationId, string codeSetUri,
         int optionNo)
      {
         ResultsLog<List<CodeSetInfo>> rlog =
            new ResultsLog<List<CodeSetInfo>>();

         DataProvider p = DataProvider.CreateProcedure("Data.DataCodeSetGet",
            ReferenceDataHelper.GetConnectionStringKey());
         try
         {
            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId", organizationId, 20);
            p.Params.AddWithValue("@CodeSetUri", codeSetUri, 2048);
            p.Params.AddWithValue("@OptionNo", optionNo);

            if (p.OpenReader())
            {
               rlog.Data = DataReader.GetList<CodeSetInfo>(p.Reader);
               rlog.Succeeded();
            }
         }
         catch (Exception ex)
         {
            rlog.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return rlog;
      }

      public static IResultsLog DataCodeSetUpsert(
         string sessionId, CodeSetInfo codeSet, int optionNo)
      {
         ResultsLog<long> results =
            new ResultsLog<long>();

         DataProvider p = DataProvider.CreateProcedure("Data.DataCodeSetUpsert",
            ReferenceDataHelper.GetConnectionStringKey());
         try
         {
            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId", 
               codeSet.OrganizationId, 20);
            p.Params.AddWithValue("@DomainNo", codeSet.DomainNo);
            p.Params.AddWithValue("@CodeSetUri", codeSet.CodeSetUri, 2048);
            p.Params.AddWithValue("@CodeSetNo", codeSet.CodeSetNo);
            p.Params.AddWithValue("@CodeSetId", codeSet.CodeSetId, 20);
            p.Params.AddWithValue("@CodeSetName", codeSet.CodeSetName, 128);
            p.Params.AddWithValue("@VersionId", codeSet.VersionId, 20);
            p.Params.AddWithValue("@DataOwnerId", codeSet.DataOwnerId, 20);
            p.Params.AddWithValue("@OptionNo", optionNo);
            var outp = p.Params.AddWithValue(
               "@OutCodeSetNo", codeSet.CodeSetNo, true);

            if (p.OpenReader())
            {
               codeSet.CodeSetNo = (long) outp.Value;
               results.Data = codeSet.CodeSetNo;
               results.Succeeded();
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return results;
      }

      public static ResultsLog<List<CodeInfo>> DataCodeGet(
         string sessionId, string organizationId, long codeSetNo,
         int optionNo)
      {
         ResultsLog<List<CodeInfo>> rlog =
            new ResultsLog<List<CodeInfo>>();

         DataProvider p = DataProvider.CreateProcedure("Data.DataCodeGet",
            ReferenceDataHelper.GetConnectionStringKey());
         try
         {
            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId", organizationId, 20);
            p.Params.AddWithValue("@CodeSetNo", codeSetNo);
            p.Params.AddWithValue("@OptionNo", optionNo);

            if (p.OpenReader())
            {
               rlog.Data = DataReader.GetList<CodeInfo>(p.Reader);
               rlog.Succeeded();
            }
         }
         catch (Exception ex)
         {
            rlog.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return rlog;
      }

      public static IResultsLog DataCodeUpsert(
         string sessionId, CodeInfo code, int optionNo)
      {
         ResultsLog<long> results =
            new ResultsLog<long>();

         DataProvider p = DataProvider.CreateProcedure("Data.DataCodeUpsert",
            ReferenceDataHelper.GetConnectionStringKey());
         try
         {
            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId", code.OrganizationId, 20);
            p.Params.AddWithValue("@CodeSetNo", code.CodeSetNo);
            p.Params.AddWithValue("@IdNo", code.IdNo);
            p.Params.AddWithValue("@CodeId", code.CodeId, 40);
            p.Params.AddWithValue("@AlternateId", code.AlternateId, 80);
            p.Params.AddWithValue("@VersionId", code.VersionId, 20);
            p.Params.AddWithValue("@Description", code.Description, 512);
            p.Params.AddWithValue("@CategoryId", code.CategoryId, 20);
            p.Params.AddWithValue("@DataOwnerId", code.DataOwnerId, 20);
            p.Params.AddWithValue("@OptionNo", optionNo);
            var outp = p.Params.AddWithValue("@OutIdNo", code.IdNo, true);

            if (p.OpenReader())
            {
               code.IdNo = (long)outp.Value;
               results.Data = code.IdNo;
               results.Succeeded();
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }
         return results;
      }

   }

}
