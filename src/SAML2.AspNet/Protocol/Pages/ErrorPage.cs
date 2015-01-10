using System;
using System.Web.Configuration;
using System.Web.UI;

namespace SAML2.Protocol.Pages
{
    /// <summary>
    /// A page for displaying error messages
    /// </summary>
    public class ErrorPage : BasePage
    {
        #region Constructor functions

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPage"/> class.
        /// </summary>
        public ErrorPage()
        {
            OverrideConfig = false;
            ErrorText = string.Empty;
            TitleText = ErrorMessages.GenericError;
            HeaderText = ErrorMessages.GenericError;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>The error text.</value>
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to override config.
        /// </summary>
        /// <value><c>true</c> if config is overridden; otherwise, <c>false</c>.</value>
        public bool OverrideConfig { get; set; }

        #endregion

        #region overridden page functions

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            var err = ErrorMessages.GenericError;
            
            var conf = WebConfigurationManager.OpenWebConfiguration(Context.Request.Path);
            var ces = (CustomErrorsSection)conf.GetSection("system.web/customErrors");
            
            if (ces != null && !OverrideConfig)
            {
                switch (ces.Mode)
                {
                    case CustomErrorsMode.Off:
                        err = ErrorText;
                        break;
                    case CustomErrorsMode.On:
                        // Display generic error
                        break;
                    case CustomErrorsMode.RemoteOnly:
                        if (Context.Request.IsLocal)
                        {
                            err = ErrorText;
                        }

                        break;
                }
            }
            else
            {
                // OverrideConfig: Display detailed error message
                err = ErrorText;
            }
            
            BodyPanel.Controls.Add(new LiteralControl(err));
        }

        #endregion
    }
}
