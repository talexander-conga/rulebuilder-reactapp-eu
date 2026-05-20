using Conga.Platform.Common.Models;

namespace CongaSignCallbackPalmetto
{
    public class ContractQueryModel : BaseObject
    {
        public string CNGCU_Package_Id_c {get; set;}
        public AccountQueryModel Account {get;set;}
    }
}
