using Conga.Platform.Extensibility.CustomCode.Library;
using Conga.Platform.Extensibility.CustomCode.Library.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CongaSignCallbackPalmetto
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

        /// <summary>
        /// Get Account By Type
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountQueryModel>> GetAccountByType(string accountType, CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<AccountQueryModel> listAccountQuery = new List<AccountQueryModel>();
            if (string.IsNullOrEmpty(accountType))
            {
                return listAccountQuery;
            }
            var accountQuery = new QueryHelper(dataHelper).GetAccountQuery(accountType, cancellationToken);
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);

            var accountCollection = await dataHelper.QueryAsync<AccountQueryModel>(accountQuery, false);

            listAccountQuery = accountCollection?.Result;
            //foreach (var item in listAccountQuery)
            //{
            //    ThrowIfCancellationRequested(cancellationToken);
            //    // Perform any additional processing on each item if needed
            //}
            return listAccountQuery;
        }

        public async Task<List<ContractQueryModel>> GetContract(string packageId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            List<ContractQueryModel> listContractQuery = new List<ContractQueryModel>();
            if (string.IsNullOrEmpty(packageId))
            {
                return listContractQuery;
            }
            var contractQuery = new QueryHelper(dataHelper).GetContractQuery(packageId,cancellationToken);
            var contractCollection = await dataHelper.QueryAsync<ContractQueryModel>(contractQuery, false);
            return contractCollection?.Result;
        }
    }
}
