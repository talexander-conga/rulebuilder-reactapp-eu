using Conga.Platform.Extensibility.CustomCode.Library;
using Conga.Platform.Extensibility.CustomCode.Library.Interfaces;
using System.Threading;

namespace CongaSignCallbackPalmetto
{
    public class QueryHelper : CodeExtensibility
    {
        private readonly IDataHelper dataHelper;
        public QueryHelper(IDataHelper dataHelper)
        {
            this.dataHelper = dataHelper;
        }
        /// <summary>
        /// Get Account By Type Query
        /// </summary>
        /// <returns></returns>

        public IDbQuery GetAccountQuery(string accountType, CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var accountQuery = this.dataHelper.CreateQuery(Constants.ObjectName)
                                        .Select("Id", "Name")
                                        .Where($"Type = '{accountType}'")
                                        .Limit(1);
            return accountQuery;
        }
        public IDbQuery GetContractQuery(string packageId,CancellationToken cancellationToken)
        {
            //Add this check in every data transformation logic
            ThrowIfCancellationRequested(cancellationToken);
            var contractQuery = dataHelper.CreateQuery(Constants.ObjectName).Select("Id", "Name","Account.Name").Where($"CNGCU_Package_Id_c = '{packageId}'")
                               .Limit(1);
            return contractQuery;
        }
    }
}
