using System;
using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class AgreementQueryModel : BaseObject
    {
        public CurrencyField TotalContractValue { get; set; }
        public ContactQueryModel PrimaryContact {get;set;}
        public AccountQueryModel Account {get;set;}
        public DateOnly ContractStartDate{get;set;}
        public DateOnly ContractEndDate{get;set;}
        public string CustomerName_c{get;set;}
        public string CustomerState_c{get;set;}
        public string CustomerStreetAddress_c{get;set;}
        public string CustomerZipCode_c{get;set;}

        public LookupObject RecordOwner{get;set;}
        public Decimal? CNGCU_Min_Earned_Percent_c{get;set;}
        public Decimal? CNGCU_Named_Insured_Count_c{get;set;}
        public string CNGCU_Named_Insured_Multiline_c{get;set;}
        public string Installer_Name_c{get;set;}
        public string Installer_Email_c{get;set;}
          public string CNGCU_Template_Selection_c{get;set;}
        public LookupObject Internal_User_c {get;set;}
        public string CNGCU_Configurations_c{get;set;}
        public string CNGCU_Customer_Email_c{get;set;}
    }
}
