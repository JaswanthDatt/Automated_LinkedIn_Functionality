using Automated_LinkedIn_Functionality.Models;
using HtmlAgilityPack;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Automated_LinkedIn_Functionality.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Navigate(LoginViewModel model)
        {
            IWebDriver wd = null;
            try
            {
                if (ModelState.IsValid)
                {
                                    
                    string browserName = GetDefaultBrowserName();

                    // Load the corresponding Web Driver for default browser
                    if (browserName.Contains("chrome"))
                    {
                        wd = new ChromeDriver();
                    }
                    if (browserName.Contains("firefox"))
                    {
                        wd = new FirefoxDriver();
                    }

                    // Navigate to LinkedIn url
                    wd.Navigate().GoToUrl("https://www.linkedin.com/");

                    var usrName = wd.FindElement(By.Id("login-email"));
                    var usrPwd = wd.FindElement(By.Id("login-password"));
                    usrName.Clear();
                    usrPwd.Clear();

                    // send username and password
                    usrName.SendKeys(model.UserName);
                    usrPwd.SendKeys(model.Password);

                    // Click the login button on linkedIn
                    wd.FindElement(By.Id("login-submit")).Click();

                    // Validate for Login/Password. Can use LinkedIn Service to verify credentials too.
                    if (wd.PageSource.Contains("that's not the right password. Please try again") 
                        || wd.PageSource.Contains("that's not the right password. Please try again"))
                    {
                        ViewBag.Message = "Oops!!!.Your login credentials seem incorrect. Re-check them.";
                        wd.Dispose();
                    }
                    else
                    {
                 
                        IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(wd, TimeSpan.FromSeconds(15.00));
                        //wait till the web page is loaded
                        wait.Until(driver => ((IJavaScriptExecutor)wd).ExecuteScript("return document.readyState").Equals("complete"));

                        // Naviagte to people URL
                        wd.Navigate().GoToUrl("https://www.linkedin.com/search/results/people/");

                        wait.Until(driver => ((IJavaScriptExecutor)wd).ExecuteScript("return document.readyState").Equals("complete"));


                        wd.Navigate().GoToUrl("https://www.linkedin.com/in/" + Regex.Replace(model.Connection.ToLower(), @"\s+", ""));

                        //wait till the web page is loaded
                        wait.Until(driver => ((IJavaScriptExecutor)wd).ExecuteScript("return document.readyState").Equals("complete"));

                        if (isElementPresent("pv-s-profile-actions__label", wd))
                        {
                            var sendConnection = wd.FindElement(By.ClassName("pv-s-profile-actions__label"));
                            sendConnection.Click();
                        }
                        else if(isElementPresent("pv-s-profile-actions__label", wd))
                        {
                            var msgFrnd = wd.FindElement(By.ClassName("pv-s-profile-actions__label"));
                            msgFrnd.Click();
                        }
                        // Wait till window is launched
                        wait.Until(driver => ((IJavaScriptExecutor)wd).ExecuteScript("return document.readyState").Equals("complete"));

                        IWebElement sendNote = wd.SwitchTo().ActiveElement();
                        sendNote.SendKeys(model.Note);
                        sendNote.SendKeys(Keys.Enter);

                        //IWebDriver sendFrm = wd.SwitchTo().Window(wd.CurrentWindowHandle);
                        //var sndtext = sendFrm.FindElement(By.ClassName("ember-view"));
                        //sndtext.SendKeys(Keys.Enter);
                    }                   
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                wd.Dispose();
            }
            return View(model);
        }

        // Verify if the element in present on web page and handle exception
        // https://stackoverflow.com/questions/27516545/how-to-check-if-element-exists-in-c-sharp-selenium-drivers
        private bool isElementPresent(string className, IWebDriver wd)
        {
            try
            {
                bool isElementDisplayed = wd.FindElement(By.ClassName(className)).Displayed;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // https://stackoverflow.com/questions/13621467/how-to-find-default-web-browser-using-c
        // Get the default browser for the system to load the corresponding Web Driver
        private string GetDefaultBrowserName()
        {
            string browserName = String.Empty;

            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                if (userChoiceKey != null)
                {
                    object progIdValue = userChoiceKey.GetValue("Progid");
                    if (progIdValue != null)
                    {
                        if (progIdValue.ToString().ToLower().Contains("chrome"))
                            browserName = "chrome.exe";
                        else if (progIdValue.ToString().ToLower().Contains("firefox"))
                            browserName = "firefox.exe";
                        else if (progIdValue.ToString().ToLower().Contains("safari"))
                            browserName = "safari.exe";
                        else if (progIdValue.ToString().ToLower().Contains("opera"))
                            browserName = "opera.exe";
                    }
                }
            }

            return browserName;
        }
    }
}