using System;
using System.Configuration;
using SAML2.Identity;

namespace SAML2.Config
{
    /// <summary>
    /// Persistent Pseudonym configuration element.
    /// </summary>
    public class PersistentPseudonym
    {
        /// <summary>
        /// Persistent Pseudonym mapper instance.
        /// </summary>
        private IPersistentPseudonymMapper _mapper;

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        public string Mapper { get; set; }

        /// <summary>
        /// Returns the runtime-class configured pseudonym mapper (if any is present) for a given IdP.
        /// </summary>
        /// <returns>The <see cref="IPersistentPseudonymMapper"/> implementation.</returns>
        public IPersistentPseudonymMapper GetMapper()
        {
            if (!string.IsNullOrEmpty(Mapper))
            {
                _mapper = (IPersistentPseudonymMapper)Activator.CreateInstance(Type.GetType(Mapper), true);
            }
            else
            {
                _mapper = null;
            }

            return _mapper;
        }
    }
}
