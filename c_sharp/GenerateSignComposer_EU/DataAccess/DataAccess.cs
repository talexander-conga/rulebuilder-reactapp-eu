using Conga.Platform.Extensibility.CustomCode.Library;
using Conga.Platform.Extensibility.CustomCode.Library.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GenerateComposerDocV2
{
    public class DataAccess : CodeExtensibility
    {
        private readonly IDataHelper dataHelper;

        /// <summary>
        /// DataAccess
        /// </summary>
        /// <param name="dataHelper"></param>
        public DataAccess(IDataHelper dataHelper)
        {
            this.dataHelper = dataHelper;
        }
        public async Task<List<QueryTermQueryModel>> GetQueryTerm(CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<QueryTermQueryModel> listQueryTermQuery = new List<QueryTermQueryModel>();
            var queryTermQuery = new QueryHelper(dataHelper).GetQueryTermQuery(cancellationToken);
            var queryTermCollection = await dataHelper.QueryAsync<QueryTermQueryModel>(queryTermQuery, false);
            listQueryTermQuery = queryTermCollection?.Result;
            return listQueryTermQuery;
        }

        public async Task<List<QueryTermQueryModel>> GetQueryTermByType(CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<QueryTermQueryModel> listQueryTermQuery = new List<QueryTermQueryModel>();
            var queryTermQuery = new QueryHelper(dataHelper).GetQueryTermQueryByType(cancellationToken);
            var queryTermCollection = await dataHelper.QueryAsync<QueryTermQueryModel>(queryTermQuery, false);
            listQueryTermQuery = queryTermCollection?.Result;
            return listQueryTermQuery;
        }

        public async Task<List<AgreementQueryModel>> GetAgreement(string agreementId,string filter,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<AgreementQueryModel> listAgreementQuery = new List<AgreementQueryModel>();
            var agreementQuery = new QueryHelper(dataHelper).GetAgreementQuery(agreementId,filter,cancellationToken);
            var agreementCollection = await dataHelper.QueryAsync<AgreementQueryModel>(agreementQuery, false);
            listAgreementQuery = agreementCollection?.Result;
            return listAgreementQuery;
        }

        public async Task<List<AgreementQueryModel>> GetAgreementTest(string agreementId,string filter,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<AgreementQueryModel> listAgreementQuery = new List<AgreementQueryModel>();
            var agreementQuery = new QueryHelper(dataHelper).GetAgreementQueryTest(agreementId,filter,cancellationToken);
            var agreementCollection = await dataHelper.QueryAsync<AgreementQueryModel>(agreementQuery, false);
            listAgreementQuery = agreementCollection?.Result;
            return listAgreementQuery;
        }

        public async Task<List<AgreementQueryModel>> GetAgreementById(string agreementId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<AgreementQueryModel> listAgreementQuery = new List<AgreementQueryModel>();
            var agreementQuery = new QueryHelper(dataHelper).GetAgreementByIdQuery(agreementId,cancellationToken);
            var agreementCollection = await dataHelper.QueryAsync<AgreementQueryModel>(agreementQuery, false);
            listAgreementQuery = agreementCollection?.Result;
            return listAgreementQuery;
        }

        public async Task<List<TemplateQueryModel>> GetTemplate(string clauseNames,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<TemplateQueryModel> listTemplateQuery = new List<TemplateQueryModel>();
            var templateQuery = new QueryHelper(dataHelper).GetTemplateQuery(clauseNames,cancellationToken);
            var templateCollection = await dataHelper.QueryAsync<TemplateQueryModel>(templateQuery, false);
            listTemplateQuery = templateCollection?.Result;
            return listTemplateQuery;
        }

        public async Task<List<AccountQueryModel>> GetAccount(string accountId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<AccountQueryModel> listAccountQuery = new List<AccountQueryModel>();
            if (string.IsNullOrEmpty(accountId))
            {
                return listAccountQuery;
            }
            var accountQuery = new QueryHelper(dataHelper).GetAccountQuery(accountId,cancellationToken);
            var accountCollection = await dataHelper.QueryAsync<AccountQueryModel>(accountQuery, false);
            return accountCollection?.Result;
        }

        public async Task<List<ContactQueryModel>> GetContact(string contactId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<ContactQueryModel> listContactQuery = new List<ContactQueryModel>();
            if (string.IsNullOrEmpty(contactId))
            {
                return listContactQuery;
            }
            var contactQuery = new QueryHelper(dataHelper).GetContactQuery(contactId,cancellationToken);
            var contactCollection = await dataHelper.QueryAsync<ContactQueryModel>(contactQuery, false);
            return contactCollection?.Result;
        }

        public async Task<Dictionary<string, object>> UpdateAgreement(Dictionary<string, object> agreement,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var res = await dataHelper.UpdateAsync(Constants.Agreement, agreement);
            return res;
        }
        
    }
}
