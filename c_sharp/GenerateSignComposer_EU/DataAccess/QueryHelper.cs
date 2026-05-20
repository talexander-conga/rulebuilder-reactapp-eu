using Conga.Platform.Extensibility.CustomCode.Library;
using Conga.Platform.Extensibility.CustomCode.Library.Interfaces;
using Conga.Platform.Extensibility.CustomCode.Library.Models;
using System.Threading;

namespace GenerateComposerDocV2
{
    public class QueryHelper : CodeExtensibility
    {
        private readonly IDataHelper dataHelper;
        public QueryHelper(IDataHelper dataHelper)
        {
            this.dataHelper = dataHelper;
        }

        public IDbQuery GetQueryTermQuery(CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var queryTermQuery = this.dataHelper.CreateQuery(Constants.QueryTerms)
                                        .Select("Id", "Name","CNGCU_Expression_Condition_c","CNGCU_Clauses_c","CNGCU_Type_c","CNGCU_TemplateId_c","Signer_c","Signing_Order_c");
            return queryTermQuery;
        }

        public IDbQuery GetQueryTermQueryByType(CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var queryTermQuery = this.dataHelper.CreateQuery(Constants.QueryTerms)
                                        .Select("Id", "Name","CNGCU_Expression_Condition_c","CNGCU_Clauses_c","CNGCU_Type_c","CNGCU_TemplateId_c")
                                        .Where($"CNGCU_Type_c='Template'")
                                        .OrderBy("Sequence_c", SortDirection.Ascending);
            return queryTermQuery;
        }
        public IDbQuery GetAgreementQuery(string agreementId,string filter,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var agreementQuery = this.dataHelper.CreateQuery(Constants.Agreement)
                                        .Select("Id", "Name","Account","PrimaryContact","TotalContractValue","ContractStartDate","ContractEndDate")
                                        .Where($"Id='{agreementId}' And {filter}");
            return agreementQuery;
        }

        public IDbQuery GetAgreementQueryTest(string agreementId,string filter,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var agreementQuery = this.dataHelper.CreateQuery(Constants.Agreement)
                                        .Select("Id", "Name","Account","PrimaryContact","TotalContractValue","ContractStartDate","ContractEndDate")
                                        .Where($"Id='{agreementId}' AND CNGCU_Class_Code_c IN (41715,41716,41670,10331,10332,46202,43117,45210,15656,62002,62000)");
            return agreementQuery;
        }

        public IDbQuery GetAgreementByIdQuery(string agreementId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var agreementQuery = this.dataHelper.CreateQuery(Constants.Agreement)
                                        .Select("Id", "Name","Account","PrimaryContact","TotalContractValue","ContractStartDate","ContractEndDate","CustomerName_c","RecordOwner","CustomerState_c","CustomerStreetAddress_c","CustomerZipCode_c","CNGCU_Min_Earned_Percent_c","CNGCU_Named_Insured_Count_c","CNGCU_Named_Insured_Multiline_c","Installer_Name_c","Installer_Email_c","CNGCU_Configurations_c", "CNGCU_Template_Selection_c", "Internal_User_c", "CNGCU_Customer_Email_c")
                                        .Where($"Id='{agreementId}'");
            return agreementQuery;
        }

        public IDbQuery GetTemplateQuery(string clauseNames,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var templateQuery = this.dataHelper.CreateQuery(Constants.Template)
                                        .Select("Id", "Name","TextContent")
                                        .Where($"Name in {clauseNames}");
            return templateQuery;
        }

        public IDbQuery GetContactQuery(string contactId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var contractQuery = dataHelper.CreateQuery(Constants.Contact).Select("Id", "Name", "LastName", "FirstName", "Email","MailingStreet","MailingState","MailingCity","MailingCountry","MailingPostalCode").Where($"Id = '{contactId}'")
                               .Limit(1);
            return contractQuery;
        }

        public IDbQuery GetAccountQuery(string accountId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
           var contractQuery = dataHelper.CreateQuery(Constants.Account).Select("Id", "Name", "PrimaryContact.Id","BillingStreet","BillingState","BillingCity","BillingCountry","BillingPostalCode").Where($"Id = '{accountId}'")
                               .Limit(1);
            return contractQuery;
        }
    }
}
