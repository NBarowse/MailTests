using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;

namespace Selenium_Webdriver
{
    [TestClass]
    public class GmailTest
    {
        private IWebDriver driver;
        private string baseUrl;
        private string configValue = ConfigurationManager.AppSettings["browser"];

        [TestInitialize]
        public void SetupTest()
        {
            if ("ff".Equals(this.configValue))
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

            this.baseUrl = "https://www.gmail.com/";

            this.driver.Navigate().GoToUrl(this.baseUrl);
            this.driver.Manage().Window.Maximize();
        }

        [TestMethod]
        public void TestLogin()
        {
            //•	Login to the mail box.
            IsElementVisible(By.Id("identifierId"));
            this.driver.FindElement(By.Id("identifierId")).SendKeys("testuser1.selenium@gmail.com");
            this.driver.FindElement(By.XPath("//*[@id='identifierNext']")).Click();

            IsElementVisible(By.Name("password"));
            this.driver.FindElement(By.Name("password")).SendKeys("testuser123!");
            this.driver.FindElement(By.XPath("//*[@id='passwordNext']")).Click();

            //•	Assert, that the login is successful: Если появляется кнопка Написать, значит вход выполнен успешно
            IsElementVisible(By.XPath("//div[@role='button' and contains(.,'Написать')]")); //button "Написать"
            StringAssert.Contains(this.driver.FindElement(By.XPath("//div[@role='button' and contains(.,'Написать')]")).GetAttribute("innerHTML"), "Написать", "Login failed.");

            //Count how many emails are in 'Sent' folder at start
            this.driver.FindElement(By.XPath("//*[@title='Отправленные']")).Click();
            IsElementVisible(By.XPath("//*[@role='main']//*[@class='Cp']//tr[1]//*[contains(.,'Кому')]"));
            int countOfSentAtStart = this.driver.FindElements(By.XPath("//*[@role='main']//*[@class='Cp']//tr")).Count;

            //Count how many emails are in 'Drafts' folder at start
            int countOfDraftsAtStart = Int32.Parse(this.driver.FindElement(By.XPath("//*[@data-tooltip='Черновики']//*[@class='bsU']")).GetAttribute("innerHTML"));

            //•	Create a new mail(fill addressee, subject and body fields).
            this.driver.FindElement(By.XPath("//div[@role='button' and contains(.,'Написать')]")).Click();
            IsElementVisible(By.XPath("//textarea[@name='to']"));
            this.driver.FindElement(By.XPath("//textarea[@name='to']")).SendKeys("anastasiya.maniak@gmail.com");
            this.driver.FindElement(By.CssSelector("input[name='subjectbox']")).SendKeys("Welcome");
            this.driver.FindElement(By.CssSelector("div[aria-label='Тело письма']")).SendKeys("Hello! How are you?");

            //•	Save the mail as a draft.
            this.driver.FindElement(By.CssSelector("*[alt='Закрыть']")).Click();

            //•	Verify, that the mail presents in ‘Drafts’ folder: Если количество мейлов в папке Drafts стало на 1 больше, значит письмо сохранилось в папке Drafts
            this.driver.SwitchTo().DefaultContent();
            int countOfDraftsAfterMailCreation = Int32.Parse(this.driver.FindElement(By.XPath("//*[@data-tooltip='Черновики']//*[@class='bsU']")).GetAttribute("innerHTML"));
            Assert.AreEqual(countOfDraftsAtStart + 1, countOfDraftsAfterMailCreation, "Draft wasn't saved");

            //•	Verify the draft content(addressee, subject and body – should be the same as in 3     
            this.driver.FindElement(By.XPath("//*[@title='Черновики']")).Click();
            IsElementVisible(By.XPath("//*[@role='main']//*[@class='Cp']//tr[1]//*[contains(.,'Черновик')]"));
            this.driver.FindElement(By.XPath("//*[@role='main']//*[@class='Cp']//tr[1]")).Click(); //click on top draft

            IsElementVisible(By.CssSelector("div[aria-label='Тело письма']"));
            StringAssert.Contains(this.driver.FindElement(By.XPath("//input[@name='to']")).GetAttribute("value"), "anastasiya.maniak@gmail.com", "TO value is not expected");
            StringAssert.Contains(this.driver.FindElement(By.XPath("//*[@class='aYF']")).GetAttribute("innerHTML"), "Welcome", "Subject value is not expected");
            StringAssert.Contains(this.driver.FindElement(By.CssSelector("div[aria-label='Тело письма']")).GetAttribute("innerHTML"), "Hello! How are you?", "Body value is not expected");

            //•	Send the mail.
            this.driver.FindElement(By.XPath("//div[@role='button' and contains(.,'Отправить')]")).Click();

            //•	Verify, that the mail disappeared from ‘Drafts’ folder: Если количество мейлов в папке Drafts стало таким же как и в начале, значит письмо исчезло из папки Drafts
            this.driver.SwitchTo().DefaultContent();
            int countOfDraftsAtEnd = Int32.Parse(this.driver.FindElement(By.XPath("//*[@data-tooltip='Черновики']//*[@class='bsU']")).GetAttribute("innerHTML"));
            Assert.AreEqual(countOfDraftsAtStart, countOfDraftsAtEnd, "Mail didn't disappear from 'Drafts' folder");

            //•	Verify, that the mail is in ‘Sent’ folder: Если количество мейлов в папке Sent стало на 1 больше, значит письмо было отправлено
            this.driver.FindElement(By.XPath("//*[@title='Отправленные']")).Click();
            IsElementVisible(By.XPath("//*[@role='main']//*[@class='Cp']//tr[1]//*[contains(.,'Кому')]"));
            int countOfSentAtEnd = this.driver.FindElements(By.XPath("//*[@role='main']//*[@class='Cp']//tr")).Count;
            Assert.AreEqual(countOfSentAtStart + 1, countOfSentAtEnd, "Mail is not in 'Sent' folder");

            //•	Log off.
            this.driver.FindElement(By.XPath("//*[@class='gb_cb gbii']")).Click();//profile icon
           // IsElementVisible(By.XPath("//a[text()='Выйти']"));
           driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);//try implicit wait
            this.driver.FindElement(By.XPath("//a[text()='Выйти']")).Click();

        }

        [TestCleanup]
        public void CleanUp()
        {
            this.driver.Close();
            this.driver.Quit();
        }

        public void IsElementVisible(By element, int timeoutSecs = 20)
        {
            new WebDriverWait(this.driver, TimeSpan.FromSeconds(timeoutSecs)).Until(ExpectedConditions.ElementIsVisible(element));
        }

    }
}
