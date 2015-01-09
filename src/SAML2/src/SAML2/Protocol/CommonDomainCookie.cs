using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SAML2.Protocol
{
    /// <summary>
    /// Implements access to the common domain cookie specified in the SAML20 identity provider discovery profile.
    /// </summary>
    public class CommonDomainCookie
    {
        /// <summary>
        /// The name of the common domain cookie.
        /// </summary>
        public const string CommonDomainCookieName = "_samlIdp";

        #region Private variables

        /// <summary>
        /// The cookie collection.
        /// </summary>
        private readonly HttpCookieCollection _cookies;

        /// <summary>
        /// The <c>KnownIdps</c> backing field.
        /// </summary>
        private readonly List<string> _knownIdps;

        /// <summary>
        /// The SAML identity provider.
        /// </summary>
        private readonly string _samlIdp;

        /// <summary>
        /// Indicates if this instance has been loaded.
        /// </summary>
        private bool _isLoaded;

        /// <summary>
        /// Indicates if the cookie is set.
        /// </summary>
        private bool _isSet;

        #endregion

        #region Constructor functions

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDomainCookie"/> class.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        public CommonDomainCookie(HttpCookieCollection cookies)
        {
            _cookies = cookies;
            this._knownIdps = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDomainCookie"/> class.
        /// </summary>
        /// <param name="samlIdp">The SAML identity provider.</param>
        public CommonDomainCookie(string samlIdp)
        {
            this._samlIdp = samlIdp;
            this._knownIdps = new List<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the Common Domain Cookie was set (had valid values).
        /// </summary>
        /// <value><c>true</c> if the Common Domain Cookie is set; otherwise, <c>false</c>.</value>
        public bool IsSet
        {
            get
            {
                Load();

                return _isSet;
            }
        }

        /// <summary>
        /// Gets the list of known IDPs.
        /// </summary>
        /// <value>The known IDPs. Caller should check that values are valid URIs before using them as such.</value>
        public List<string> KnownIdps
        {
            get
            {
                EnsureSet();

                return this._knownIdps;
            }
        }

        /// <summary>
        /// Gets the preferred IDP.
        /// </summary>
        /// <value>The preferred IDP. Caller should check that this value is a valid URI.</value>
        public string PreferredIDP
        {
            get
            {
                EnsureSet();

                return this._knownIdps.Count > 0 ? this._knownIdps[this._knownIdps.Count - 1] : string.Empty;
            }
        }

        #endregion

        #region Private utility functions

        /// <summary>
        /// Ensures the cookie is set.
        /// </summary>
        private void EnsureSet()
        {
            Load();
            if (!_isSet)
            {
                throw new Saml20Exception("The common domain cookie is not set. Please make sure to check the IsSet property before accessing the class' properties.");
            }
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        private void Load()
        {
            if (_cookies != null)
            {
                LoadCookie();
            }

            if (!string.IsNullOrEmpty(this._samlIdp))
            {
                LoadFromString();
            }
        }

        /// <summary>
        /// Loads from string.
        /// </summary>
        private void LoadFromString()
        {
            if (!_isLoaded)
            {
                ParseCookie(this._samlIdp);
                _isSet = true;
                _isLoaded = true;
            }
        }

        /// <summary>
        /// Loads the cookie.
        /// </summary>
        private void LoadCookie()
        {
            if (!_isLoaded)
            {
                var cdc = _cookies[CommonDomainCookieName];
                if (cdc != null)
                {
                    ParseCookie(cdc.Value);
                    _isSet = true;
                }

                _isLoaded = true;
            }
        }

        /// <summary>
        /// Parses the cookie.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        private void ParseCookie(string rawValue)
        {
            var value = HttpUtility.UrlDecode(rawValue);
            var idps = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var base64idp in idps)
            {
                var bytes = Convert.FromBase64String(base64idp);
                var idp = Encoding.ASCII.GetString(bytes);
                _knownIdps.Add(idp);                
            }
        }

        #endregion
    }
}
