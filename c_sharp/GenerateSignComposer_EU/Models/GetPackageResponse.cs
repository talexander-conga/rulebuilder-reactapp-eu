using System.Collections.Generic;
using System.ComponentModel;
using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class GetPackageResponse : BaseObject
    {
        public List<RoleDefination> roles {get; set;}
        public List<DocumentDefination> documents {get;set;}
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
        public string title{get;set;}
    }

    public class DocumentDefination : BaseObject{
        public string id {get;set;}
        public List<ApprovalDefination> approvals {get;set;}
    }
    public class ApprovalDefination : BaseObject{
        public List<FieldDefination> fields{get;set;}

    }

    public class FieldDefination : BaseObject{
        public string binding {get;set;}
        public string name {get;set;}
        public string value {get;set;}
        public string type {get;set;}
    }
}
