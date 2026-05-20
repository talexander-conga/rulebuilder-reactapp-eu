using Conga.Platform.Common.Models;

namespace GenerateComposerDocV2
{
    public class ErrorResponse : BaseObject
    {
        public string message {get; set;}
        public string messageKey {get; set;}
        public string name {get; set;}
        public string technical {get; set;}
    }
}
