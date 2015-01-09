using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SAML2.Protocol.Pages
{
    /// <summary>
    /// A base class for asp pages
    /// </summary>
    public class BasePage : Page
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePage"/> class.
        /// </summary>
        public BasePage()
        {
            InitControls();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        /// <value>The title text.</value>
        public string TitleText { get; set; }

        /// <summary>
        /// Gets the main panel.
        /// </summary>
        /// <value>The main panel.</value>
        protected Panel MainPanel { get; private set; }

        /// <summary>
        /// Gets the body panel.
        /// </summary>
        /// <value>The body panel.</value>
        protected Panel BodyPanel { get; private set; }

        /// <summary>
        /// Gets the header panel.
        /// </summary>
        /// <value>The header panel.</value>
        protected Panel HeaderPanel { get; private set; }

        /// <summary>
        /// Sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        protected string HeaderText
        {
            set
            {
                HeaderPanel.Controls.Clear();
                HeaderPanel.Controls.Add(new LiteralControl(value));
            }
        }

        /// <summary>
        /// Gets the footer panel.
        /// </summary>
        /// <value>The footer panel.</value>
        protected Panel FooterPanel { get; private set; }

        /// <summary>
        /// Sets the footer text.
        /// </summary>
        /// <value>The footer text.</value>
        protected string FooterText
        {
            set
            {
                FooterPanel.Controls.Clear();
                FooterPanel.Controls.Add(new LiteralControl(value));
            }
        }

        /// <summary>
        /// Gets the html head element.
        /// </summary>
        /// <value>The head.</value>
        protected HtmlHead Head { get; private set; }

        #endregion

        #region Private utility functions

        /// <summary>
        /// Gets the encoding meta tag.
        /// </summary>
        /// <returns>The encoding meta tag.</returns>
        private static HtmlMeta GetEncodingMetaTag()
        {
            return new HtmlMeta { HttpEquiv = "Content-Type", Content = "text/html; charset=utf-8" };
        }

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        private void InitControls()
        {
            MainPanel = new Panel { ID = "mainPanel" };

            Controls.Add(new LiteralControl("<!DOCTYPE html>" + Environment.NewLine + "<html>" + Environment.NewLine));

            Head = new HtmlHead { Title = TitleText };
            Head.Controls.Add(GetEncodingMetaTag());

            Controls.Add(Head);
            Controls.Add(new LiteralControl("<body>"));

            HeaderPanel = new Panel { ID = "headerPanel" };
            BodyPanel = new Panel { ID = "bodyPanel" };
            FooterPanel = new Panel { ID = "footerPanel" };

            MainPanel.Controls.Add(HeaderPanel);
            MainPanel.Controls.Add(BodyPanel);
            MainPanel.Controls.Add(FooterPanel);

            Controls.Add(MainPanel);

            Controls.Add(new LiteralControl(Environment.NewLine + "</body>" + Environment.NewLine + "</html>"));
        }

        #endregion
    }
}