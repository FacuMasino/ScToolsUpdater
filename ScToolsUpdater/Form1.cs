using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScToolsUpdater
{
    public partial class Form1 : Form
    {
        public ChromeDriver chDrv;
        
        public Form1()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            DotNetEnv.Env.TraversePath().Load();
        }

        private void Login()
        {
            chDrv.Url = "https://productores.sancristobal.com.ar/login";
            chDrv.Navigate();
            IWebElement inputUser = chDrv.FindElement(By.Id("nombre_usuario"));
            IWebElement inputPass = chDrv.FindElement(By.Id("password"));
            IWebElement btnLogin = chDrv.FindElement(By.Id("login"));
            WebDriverWait wait = new WebDriverWait(chDrv, TimeSpan.FromSeconds(15));
            wait.Until(d =>
            {
                return inputUser.Displayed && inputPass.Displayed;
            });

            inputUser.SendKeys(System.Environment.GetEnvironmentVariable("USER"));
            inputPass.SendKeys(System.Environment.GetEnvironmentVariable("PWD"));
            btnLogin.Click();
            wait.Until(ExpectedConditions.ElementExists(By.Id("navbar-menu-item-route-inicio")));
        }

        private string GetToken()
        {
            var tk = chDrv.ExecuteScript("return window.localStorage.getItem(\"tk\")");
            return "Bearer " + (string)tk;
        }

        private string GetCookie()
        {
            chDrv.Url = "https://productores.sancristobal.com.ar/legado?branchIndex=1";
            chDrv.Navigate();
            WebDriverWait wait = new WebDriverWait(chDrv, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.ClassName("iframeBody")));
            wait.Until(ExpectedConditions.ElementExists(By.Id("cuit")));
            chDrv.Url = "https://portalpas.sancristobal.com.ar/EmissionAutomovil/";
            chDrv.Navigate();
            wait.Until(ExpectedConditions.ElementExists(By.Id("account")));
            string cookie = chDrv.Manage().Cookies.GetCookieNamed(".ASPXAUTH").Value;
            return cookie;
        }

        public async static Task<bool> UpdateToken(string tk)
        {
            try
            {
                string baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");
                HttpClient req = new HttpClient();
                var content = await req.GetAsync(baseUrl+ "/setBearerTk?tk=" + tk);
                if (content.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async static Task<bool> UpdateCookie(string ck)
        {
            try
            {
                string baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");
                HttpClient req = new HttpClient();
                var content = await req.GetAsync(baseUrl + "/setAuthKey?key=" + ck);
                if (content.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            label1.Text = "Actualizando credenciales...";
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            var options = new ChromeOptions();
            options.AddArgument("headless");
            chDrv = new ChromeDriver(service, options);
            Login();
            string token = GetToken();
            string cookie = GetCookie();
            bool tkResult = await UpdateToken(token);
            bool ckResult = await UpdateCookie(cookie);
            Debug.Print("TK: " + (tkResult ? "ok" : "error"));
            Debug.Print("CK: " + (ckResult ? "ok" : "error"));
            label1.Text = "Finalizado.";
            chDrv.Dispose();
            Application.Exit();
        }
    }
}