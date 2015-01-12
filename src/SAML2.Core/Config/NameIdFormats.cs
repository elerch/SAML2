using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    public class NameIdFormats : List<NameIdFormat>
    {
        public NameIdFormats() : base() { }
        public NameIdFormats(IEnumerable<NameIdFormat> collection) : base(collection) { }
        /// <summary>
        /// Gets a value indicating whether to allow creation of new NameIdFormats.
        /// </summary>
 
        public bool AllowCreate { get; } // What is the point of this???
    }
}
