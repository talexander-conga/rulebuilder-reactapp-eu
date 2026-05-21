using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class QueryTermQueryModel : BaseObject
    {
        public string CNGCU_Expression_Condition_c{get;set;}
        public string CNGCU_Clauses_c{get;set;}
        public string CNGCU_TemplateId_c{get;set;}
        public string CNGCU_Type_c{get;set;}
        public string Signer_c{get;set;}
        public int? Signing_Order_c{get;set;}
    }
}
