using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class UserQueryModel : BaseObject
    {
        public UserResponse Data{get;set;}
    }

    public class UserResponse : BaseObject
    {
        public string Email {get;set;}
        public string FirstName {get;set;}

        public string LastName {get;set;}
    }
}