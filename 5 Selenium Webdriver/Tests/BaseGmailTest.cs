using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.Configuration;
using OpenQA.Selenium.Support.UI;

namespace Selenium_Webdriver
{
    [TestClass]
    public class BaseGmailTest
    {
        protected IWebDriver driver;

        #region Variables
        private string baseUrl = "https://www.gmail.com/";
        private string browser = ConfigurationManager.AppSettings["browser"];
        private string mail = ConfigurationManager.AppSettings["mail"];
        private string pswrd = ConfigurationManager.AppSettings["pswrd"];
        #endregion

        #region Locators
        private By username = By.Id("identifierId");
        private By usernameNext = By.Id("identifierNext");
        private By password = By.Name("password");
        private By passwordNext = By.Id("passwordNext");
        protected By profileIcon = By.XPath("//*[@class='gb_cb gbii']");
        protected By btnExit = By.XPath("//a[text()='Выйти']");
        protected By btnWrite = By.XPath("//div[@role='button' and contains(.,'Написать')]");
        protected By btnCloseDraft = By.CssSelector("*[alt='Закрыть']");
        protected By btnSend = By.XPath("//div[@role='button' and contains(.,'Отправить')]");
        protected By nameBox = By.XPath("//*[@name='to']");
        protected By subjectBox = By.CssSelector("input[name='subjectbox']");
        protected By subjectInDraft = By.XPath("//*[@class='aYF']");
        protected By bodyBox = By.CssSelector("div[aria-label='Тело письма']");
        protected By drafts = By.XPath("//*[@title='Черновики']");
        protected By sent = By.XPath("//*[@title='Отправленные']");
        #endregion

        [TestInitialize]
        public void SetupTest()
        {
            if ("ff".Equals(this.browser))
            {
                var service = FirefoxDriverService.CreateDefaultService();
                this.driver = new FirefoxDriver(service);
            }
            else
            {
                ChromeOptions option = new ChromeOptions();
                option.AddArgument("disable-infobars");
                this.driver = new ChromeDriver(option);
            }

            this.driver.Navigate().GoToUrl(this.baseUrl);
            this.driver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.driver.Close();
            this.driver.Quit();
        }

        protected void SignIn()
        {
            WaitForElement(username);
            this.driver.FindElement(username).SendKeys(mail);
            this.driver.FindElement(usernameNext).Click();

            WaitForElement(password);
            this.driver.FindElement(password).SendKeys(pswrd);
            this.driver.FindElement(passwordNext).Click();
        }

        protected void LogOut()
        {
            this.driver.FindElement(profileIcon).Click();
            WaitForElement(btnExit);
            this.driver.FindElement(btnExit).Click();
        }

        protected void WaitForElement(By locatorKey, int timeoutSecs = 20)
        {
            new WebDriverWait(this.driver, TimeSpan.FromSeconds(timeoutSecs)).Until(ExpectedConditions.ElementIsVisible(locatorKey));
        }

        protected bool IsElementPresent(By locatorKey)
        {
            try
            {
                if (this.driver.FindElement(locatorKey).Displayed) ;
                return true;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
        }


    }
}
