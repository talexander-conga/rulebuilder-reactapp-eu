using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class ContactQueryModel : BaseObject
        {
            public string Email {get;set;}
            public string FirstName {get;set;}

            public string LastName {get;set;}
            public string MailingStreet {get;set;}
            public string MailingState {get;set;}
            public string MailingCity {get;set;}
            public string MailingCountry {get;set;}
            public string MailingPostalCode {get;set;}

        }
}
