using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAML2.Config;
using SAML2.Utils;

namespace SAML2.Protocol.Pages
{
    /// <summary>
    /// Page that handles selecting an IdP when more than one is configured
    /// </summary>
    public class SelectSaml20IDP : BasePage
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            TitleText = Resources.PageIdentityProviderSelectTitle;
            HeaderText = Resources.PageIdentityProviderSelectTitle;
            
            BodyPanel.Controls.Add(new LiteralControl(Resources.PageIdentityProviderSelectDescription));
            BodyPanel.Controls.Add(new LiteralControl("<br/><br/>"));

            var config = Saml2Config.GetConfig();

            foreach (var endPoint in config.IdentityProviders)
            {
                if (endPoint.Metadata != null)
                {
                    var link = new HyperLink
                                   {
                                       Text =
                                           string.IsNullOrEmpty(endPoint.Name)
                                               ? endPoint.Metadata.EntityId
                                               : endPoint.Name,
                                       NavigateUrl = IdpSelectionUtil.GetIdpLoginUrl(endPoint.Id)
                                   };

                    // Link text. If a name has been specified in web.config, use it. Otherwise, use id from metadata.
                    BodyPanel.Controls.Add(link);
                    BodyPanel.Controls.Add(new LiteralControl("<br/>"));
                }
                else
                {
                    var label = new Label { Text = endPoint.Name };
                    label.Style.Add(HtmlTextWriterStyle.TextDecoration, "line-through");
                    BodyPanel.Controls.Add(label);

                    label = new Label { Text = " (Metadata not found)" };
                    label.Style.Add(HtmlTextWriterStyle.FontSize, "x-small");
                    BodyPanel.Controls.Add(label);

                    BodyPanel.Controls.Add(new LiteralControl("<br/>"));
                }
            }
        }
    }
}
