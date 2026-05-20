using Conga.Platform.Common.Models;

namespace CongaSignCallbackPalmetto
{
    /// <summary>
    /// Account Query Model
    /// </summary>

    public class AccountQueryModel : BaseObject
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }
    }
}
