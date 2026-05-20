using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
  
    public class TransactionResponse : BaseObject
    {
        public ReponseModelDefination preSignedCreateResponseModel {get;set;}
        public bool isSuccessful{get;set;}
    }

    public class ReponseModelDefination : BaseObject{
        public List<fileInfoDetailsListDefination> fileInfoDetailsList {get;set;}
    }

    public class fileInfoDetailsListDefination : BaseObject{
        public string transactionId {get;set;}
    }
}