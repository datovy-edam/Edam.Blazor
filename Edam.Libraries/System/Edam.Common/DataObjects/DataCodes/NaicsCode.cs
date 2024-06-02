using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.DataCodes
{

   public enum NaicsClass
   {
      Unknown = 0,
      AgricultureForestryFishingHunting = 11,
      Mining = 21,
      Utitities = 22,
      Construction = 23,
      Manufacturing = 3133,
      WholesaleTrade = 42,
      RetailTrade = 4445,
      TransportationWarehousing = 4849,
      Information = 51,
      FinanceInsurance = 52,
      RealEstateRentalLeasing = 53,
      ProfessionalScientificTechnicalServices = 54,
      ManagementCompaniesEnterprises = 55,
      AdministrativeSupportRemediationServices = 56,
      EducationalServices = 61,
      HealthCareSocialAssistance = 62,
      ArtsEntertainmentRecreation = 71,
      AccommodationFoodServices = 72,
      OtherServices = 81,
      PublicAdministration = 92
   }

   public enum NaicsCode
   {
      Unknown = 0,
      DataProcessingComputerServices = 518210,
      ComputerSystemsDesignServices = 541512,
      OtherComputerRelatedServices = 541519,
      EducationalServices = 611000,
      ProfessionalManagementDevelopment = 611430
   }

}
