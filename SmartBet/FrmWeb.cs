using System;
using System.Net;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

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


        public FrmWeb()
        {
            InitializeComponent();
        }

        private async void FrmWeb_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            Init();
            webView21.CoreWebView2.Navigate(Url);
        }

        private void CoreWebView2_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = (CoreWebView2)sender;
        }

        private void Init()
        {
            SetCookie();
        }

        private void SetCookie()
        {
            Uri uri = new Uri(Url);
            string cDomain = uri.Host;
            CookieContainer container = CC;
            CookieCollection cc = container.GetCookies(new Uri(Url));
            foreach (System.Net.Cookie c in cc)
            {
                var cookie = webView21.CoreWebView2.CookieManager.CreateCookie(c.Name.ToString(), c.Value.ToString(), cDomain, "/");
                cookie.IsHttpOnly = true;
                //cookie.IsSecure = true;
                webView21.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
            }
        }

        private void toolForward_Click(object sender, EventArgs e)
        {
     
        }

        private void toolBack_Click(object sender, EventArgs e)
        {

        }

        private void toolRefresh_Click(object sender, EventArgs e)
        {

        }
    }


}
