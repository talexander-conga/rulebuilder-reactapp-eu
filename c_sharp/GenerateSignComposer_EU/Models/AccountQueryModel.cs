using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class AccountQueryModel : BaseObject
    {
        public ContactQueryModel PrimaryContact { get; set; }
        public string BillingStreet { get; set; }
        public string BillingState { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPostalCode { get; set; }
    }
}
