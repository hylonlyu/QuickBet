using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace EatZD
{
        public partial class FrmWeb : Form
    {
        public string Url
        {
            get;
            set;
        }
        public CookieContainer CC
        {
            get;
            set;
        }

        ChromiumWebBrowser chromeBrowser;
        public FrmWeb()
        {
            InitializeComponent();
        }

        private void FrmWeb_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            CefSettings setting = new CefSettings();
            Cef.Initialize(setting);
            SetCookie();
            chromeBrowser = new ChromiumWebBrowser(Url);
            this.panel1.Controls.Add(this.chromeBrowser);
            chromeBrowser.LifeSpanHandler = new OpenPageSelf();
            chromeBrowser.Dock = DockStyle.Fill;
        }

        private void SetCookie()
        {
            Uri uri = new Uri(Url);
            string cDomain = uri.Host;
            CookieContainer container = CC;
            CookieCollection cc = container.GetCookies(new Uri(Url));
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
            foreach (System.Net.Cookie c in cc)
            {
                //    InternetSetCookie("http://" + cDomain, c.Name.ToString(), c.Value.ToString());

                cookieManager.SetCookie("http://" + cDomain, new CefSharp.Cookie()
                {
                    Domain = cDomain,
                    Name = c.Name.ToString(),
                    Value = c.Value.ToString(),
                    Expires = DateTime.MinValue
                });
            }

          

        }
    }

    /// <summary>
    /// 在自己窗口打开链接
    /// </summary>
    internal class OpenPageSelf : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;
            chromiumWebBrowser.Load(targetUrl);
            return true; //Return true to cancel the popup creation copyright by codebye.com.
        }
    }
}
