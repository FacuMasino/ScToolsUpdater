using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScToolsUpdater
{
    public partial class Form1 : Form
    {
        readonly ChromeDriver chDrv = new ChromeDriver();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Login();
            Debug.Print(GetToken());
            Debug.Print(GetCookie());
        }

        private void Login()
        {
                chDrv.Url = "https://productores.sancristobal.com.ar/login";
                chDrv.Navigate();
                IWebElement inputUser = chDrv.FindElement(By.Id("nombre_usuario"));
                IWebElement inputPass = chDrv.FindElement(By.Id("password"));
                IWebElement btnLogin = chDrv.FindElement(By.Id("login"));
                WebDriverWait wait = new WebDriverWait(chDrv, TimeSpan.FromSeconds(10));
                wait.Until(d =>
                {
                    return inputUser.Displayed && inputPass.Displayed;
                });

                inputUser.SendKeys("");
                inputPass.SendKeys("");
                btnLogin.Click();
                wait.Until(ExpectedConditions.ElementExists(By.Id("navbar-menu-item-route-inicio")));
        }

        private string GetToken()
        {
            var tk = chDrv.ExecuteScript("return window.localStorage.getItem(\"tk\")");
            return (string)tk;
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
    }
}