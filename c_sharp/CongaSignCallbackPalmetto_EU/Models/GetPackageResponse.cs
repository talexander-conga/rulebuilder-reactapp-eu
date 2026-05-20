using System.Collections.Generic;
using Conga.Platform.Common.Models;

namespace CongaSignCallbackPalmetto
{
    public class GetPackageResponse : BaseObject
    {
        public List<DocumentDefination> documents {get;set;}
        public List<RoleDefination> roles {get; set;}
    }

    public class RoleDefination : BaseObject{
        public string name {get;set;}
        public string id {get;set;}

        public List<SignerDefination> signers {get;set;}

    }

    public class SignerDefination : BaseObject{
        public string email {get;set;}
        public string firstName {get;set;}
        public string lastName {get;set;}
        public string SignerName {get;set;}
    }

    public class DocumentDefination: BaseObject{
        public string id {get;set;}
    }
}
