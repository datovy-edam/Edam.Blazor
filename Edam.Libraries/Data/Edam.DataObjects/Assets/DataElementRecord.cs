using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Diagnostics;
using Edam.Data;
using Edam.Data.Asset;
using Edam.DataObjects.Assets;
using Edam.Data.AssetSchema;
using Edam.DataObjects.ReferenceData;

namespace Edam.Data.AssetManagement
{

   public class DataElementRecord
   {

      public static ResultsLog<DataReferenceFetchResult> DataRefereneGet(
         DataReferenceFetchRequest request)
      {
         ResultsLog<DataReferenceFetchResult> rlog =
            new ResultsLog<DataReferenceFetchResult>();

         DataReferenceFetchResult results =
            new DataReferenceFetchResult(request);

         DataProvider p = DataProvider.CreateProcedure("Data.DataReferenceGet", 
            ReferenceDataHelper.GetConnectionStringKey());
         try
         {
            p.Params.AddWithValue("@SessionId", request.SessionId, 40);
            p.Params.AddWithValue(
               "@OrganizationId", request.OrganizationID, 20);
            p.Params.AddWithValue("@Root", request.Root, 1024);
            p.Params.AddWithValue("@DomainUri", request.DomainUri, 2048);
            p.Params.AddWithValue("@TermName", request.TermName, 1024);
            p.Params.AddWithValue("@OptionNo", (short)request.Option);

            if (p.OpenReader())
            {
               rlog.ReturnValue = p.GetReturnedValue();
               if (request.Option == 
                  DataReferenceOption.DomainsTermsAndElements)
               {
                  results.Domains = DataReader.GetList<DataDomain>(p.Reader);
                  if (p.Reader.NextResult())
                  {
                     results.Terms =
                        DataReader.GetList<DataTerm>(p.Reader);
                  }
                  if (p.Reader.NextResult())
                  {
                     var terms = DataReader.GetList<AssetDataElement>(p.Reader);
                     results.Elements.AddRange(terms);
                  }
               }
               else if (request.Option == DataReferenceOption.Domains)
               {
                  results.Domains = DataReader.GetList<DataDomain>(p.Reader);
               }
               else if (request.Option == DataReferenceOption.Elements)
               {
                  var elements = DataReader.GetList<AssetDataElement>(p.Reader);
                  results.Elements.AddRange(elements);
               }
               else if (request.Option == DataReferenceOption.Terms)
               {
                  results.Terms = DataReader.GetList<DataTerm>(p.Reader);
               }
               rlog.Data = results;
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

      public static ResultsLog<DataReferenceFetchResult> DomainGet(
         string sessionId, string organizationId, string domainUri)
      {
         DataReferenceFetchRequest request = new DataReferenceFetchRequest();
         request.SessionId = String.IsNullOrWhiteSpace(sessionId) ?
            Session.SessionId : sessionId;
         request.OrganizationID = String.IsNullOrWhiteSpace(organizationId) ?
            Session.OrganizationId : organizationId;
         request.Root = String.Empty;
         request.DomainUri = domainUri;
         request.Option = DataReferenceOption.Domains;
         request.TermName = String.Empty;
         return DataRefereneGet(request);
      }

      public static ResultsLog<DataReferenceFetchResult> ElementGet(
         string sessionId, string organizationId, string domainUri)
      {
         DataReferenceFetchRequest request = new DataReferenceFetchRequest();
         request.SessionId = String.IsNullOrWhiteSpace(sessionId) ?
            Session.SessionId : sessionId;
         request.OrganizationID = String.IsNullOrWhiteSpace(organizationId) ?
            Session.OrganizationId : organizationId;
         request.DomainUri = domainUri;
         request.Option = DataReferenceOption.Elements;
         return DataRefereneGet(request);
      }

      /// <summary>
      /// Domain Insert - Update
      /// </summary>
      /// <param name="domain"></param>
      /// <param name="ns"></param>
      /// <param name="optionNo"></param>
      /// <returns></returns>
      public static IResultsLog DomainInsertUpdate(
         DataDomain domain, short optionNo = 1)
      {
         if (string.IsNullOrWhiteSpace(domain.UpdateSessionId))
         {
            domain.UpdateSessionId = Session.SessionId;
         }

         DataProvider provider = DataProvider.CreateProcedure(
            "Data.DataDomainUpsert",
            ReferenceDataHelper.GetConnectionStringKey());

         ResultsLog<string> results =
            new ResultsLog<string>();

         try
         {
            DataParameters p = provider.Params;

            p.AddWithValue("@SessionId", domain.UpdateSessionId);
            p.AddWithValue("@OrganizationId", domain.OrganizationId.ToUpper());

            p.AddWithValue("@DomainNo", domain.DomainNo);
            p.AddWithValue("@DomainId", domain.DomainId, 20);
            p.AddWithValue("@DataOwnerId", domain.DataOwnerId, 20);
            p.AddWithValue("@DomainUri", domain.DomainUri, 2048);
            p.AddWithValue("@DomainUriPrefix", domain.Prefix, 20);
            p.AddWithValue("@Root", domain.Root);
            p.AddWithValue("@Domain", domain.Domain);
            p.AddWithValue("@DomainName", domain.DomainName, 256);
            p.AddWithValue("@TypeNo", domain.TypeNo);
            p.AddWithValue("@Description", domain.Description, 
               domain.Description.Length);

            p.AddWithValue("@OptionNo", optionNo);
            var r0 = p.AddWithValue("@OutDomainId", domain.DomainId, 20, true);
            var r1 = p.AddWithValue("@OutDomainNo", domain.DomainNo, true);

            if (provider.Exec())
            {
               domain.DomainId = r0.Value.ToString();
               domain.DomainNo = Convert.ToInt32(r1.Value);
               results.Data = domain.DomainId;
               results.Succeeded();
            }
            else
            {
               results.ReturnValue = ReturnCode.ExecProcedureFailed;
               results.Failed(EventCode.StoredProcedureCallFailed);
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (provider != null)
               provider.Dispose();
         }
         return results;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="element"></param>
      /// <param name="ns"></param>
      /// <param name="domainName"></param>
      /// <returns></returns>
      public static IResultsLog ElementInsertUpdate(
         AssetDataElement element, NamespaceInfo ns, string domainName,
         string batchId, int assetTypeNo)
      {
         if (string.IsNullOrWhiteSpace(element.UpdateSessionId))
         {
            element.UpdateSessionId = Session.SessionId;
         }

         DataProvider provider = DataProvider.CreateProcedure(
            "Data.DataElementUpsert", 
            ReferenceDataHelper.GetConnectionStringKey());

         ResultsLog<AssetDataElement> results =
            new ResultsLog<AssetDataElement>();

         // if this entry from the XML Schema, ignore it...
         if (element.Domain == "XMLSchema")
         {
            results.Succeeded();
            results.Data = null;
            return results;
         }

         try
         {
            DataParameters p = provider.Params;

            p.AddWithValue("@SessionId", element.UpdateSessionId);
            p.AddWithValue("@OrganizationId", element.OrganizationId.ToUpper());
            p.AddWithValue("@DataOwnerId", element.DataOwnerId.ToUpper());
            p.AddWithValue("@ReferenceDate", element.ReferenceDate);
            p.AddWithValue("@DomainUri", ns.NamePath.DomainUri);
            p.AddWithValue("@DomainUriPrefix", ns.Prefix);
            p.AddWithValue("@DomainName", domainName);

            if (element.ExpiredDate.HasValue)
            {
               p.AddWithValue("@ExpiredDate", element.ExpiredDate.Value);
            }
            p.AddWithValue("@AssetNo", element.AssetNo);
            p.AddWithValue("@AssetTypeNo", assetTypeNo);
            p.AddWithValue("@Root", element.Root);
            p.AddWithValue("@Domain", element.Domain);
            p.AddWithValue("@Type", element.Type ?? String.Empty);
            p.AddWithValue("@Element", element.Element);
            p.AddWithValue("@ElementNo", element.ElementNo);
            p.AddWithValue("@ParentNo", element.ParentNo);
            p.AddWithValue("@ElementId", element.ElementId);
            p.AddWithValue("@TypeNo", element.TypeNo);
            p.AddWithValue("@ElementURI", element.ElementUri);
            p.AddWithValue("@ElementName", element.ElementName);
            p.AddWithValue("@ElementType", element.ElementType.ToString());
            p.AddWithValue("@ElementDataType", element.ElementDataType);
            p.AddWithValue("@ElementPath", element.ElementPath);
            p.AddWithValue("@Description", element.Description ?? String.Empty);
            p.AddWithValue("@ElementSequenceID", element.ElementSequenceID);
            p.AddWithValue("@ElementTypeNo", element.ElementTypeNo);
            p.AddWithValue("@ElementGroupNo", element.ElementGroupNo);
            p.AddWithValue("@ElementConstraintNo", element.AutoGenerateTypeNo);
            p.AddWithValue("@ElementKeyTypeNo", element.KeyTypeNo);
            p.AddWithValue("@AutoGeneratedTypeNo", element.AutoGenerateTypeNo);
            p.AddWithValue("@StatusNo", element.StatusNo);
            p.AddWithValue("@ValueTypeNo", element.ValueTypeNo);
            p.AddWithValue("@MinLength", element.MinLength);
            p.AddWithValue("@MaxLength", element.MaxLength);
            p.AddWithValue("@MinOccurrence", element.MinOccurrence);
            p.AddWithValue("@MaxOccurrence", element.MaxOccurrence);
            p.AddWithValue("@Nillable", element.Nillable);
            p.AddWithValue("@DefaultValue", element.DefaultValue);
            p.AddWithValue("@FixedValue", element.FixedValue);
            p.AddWithValue("@SampleValue", element.SampleValue);
            p.AddWithValue("@PropertiesBagText", element.PropertiesBagText);
            p.AddWithValue("@ProcessInstructionsBagText", 
               element.ProcessInstructionsBagText);
            p.AddWithValue("@SchemaText", element.SchemaText);
            p.AddWithValue("@OrdinalNo", element.OrdinalNo);
            p.AddWithValue("@SequenceID", element.SequenceId);
            p.AddWithValue("@VersionID", element.VersionId);
            p.AddWithValue("@BatchID", element.BatchId);

            p.AddWithValue("@ConstraintsText", element.ConstraintsText);
            p.AddWithValue("@KindNo", (short)element.Kind);
            p.AddWithValue("@Tags", element.Tags);
            p.AddWithValue("@Guid", element.Guid);

            p.AddWithValue("@OriginalName", element.OriginalName, 128);
            p.AddWithValue("@OriginalDataType", element.OriginalDataType, 128);

            p.AddWithValue("@RealName", element.OriginalName, 128);
            p.AddWithValue("@AlternateName", element.AlternateName, 128);

            var r0 = p.AddWithValue("@OutAssetNo", element.AssetNo, true);
            var r1 = p.AddWithValue("@OutElementNo", element.ElementNo, true);
            var r2 = p.AddWithValue("@OutElementId", 
               element.ElementId, 20, true);

            if (provider.Exec())
            {
               results.Data = element;
               element.AssetNo = Convert.ToInt32(r0.Value);
               element.ElementNo = Convert.ToInt32(r1.Value);
               element.ElementId = r2.Value.ToString();
               results.Succeeded();
            }
            else
            {
               results.ReturnValue = ReturnCode.ExecProcedureFailed;
               results.Failed(EventCode.StoredProcedureCallFailed);
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (provider != null)
               provider.Dispose();
         }
         return results;
      }
   }

}
