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
    public class GmailTest : BaseGmailTest
    {
        [TestMethod]
        public void TestSendEmail()
        {
            #region Variables
            string mailTo = "anastasiya.maniak@gmail.com";
            string subject = "Subject " + DateTime.Now;
            string text = "Text...";
            #endregion

            #region Locators
            By myDraft = By.XPath("//*[@role='main']//div[@role='link']//span[contains(.,'" + subject + "')]/span");
            #endregion

            //•	Login to the mail box.
            SignIn();

            //•	Assert, that the login is successful: Если появляется кнопка Написать, значит вход выполнен успешно
            WaitForElement(btnWrite);
            StringAssert.Contains(this.driver.FindElement(btnWrite).Text, "Написать", "Login failed.");

            //•	Create a new mail(fill addressee, subject and body fields).
            this.driver.FindElement(btnWrite).Click();
            WaitForElement(nameBox);
            this.driver.FindElement(nameBox).SendKeys(mailTo);
            this.driver.FindElement(subjectBox).SendKeys(subject);
            this.driver.FindElement(bodyBox).SendKeys(text);

            //•	Save the mail as a draft.
            this.driver.FindElement(btnCloseDraft).Click();

            //•	Verify, that the mail presents in ‘Drafts’ folder.
            this.driver.SwitchTo().DefaultContent();
            WaitForElement(drafts);
            this.driver.FindElement(drafts).Click();
            WaitForElement(myDraft);
            Assert.AreEqual(this.driver.FindElement(myDraft).Text, subject, "Draft wasn't saved");

            //•	Verify the draft content(addressee, subject and body – should be the same as in 3  
            this.driver.FindElement(myDraft).Click();

            WaitForElement(bodyBox);
            StringAssert.Contains(this.driver.FindElement(nameBox).GetAttribute("value"), mailTo, "TO value is not expected");
            StringAssert.Contains(this.driver.FindElement(subjectInDraft).Text, subject, "Subject value is not expected");
            StringAssert.Contains(this.driver.FindElement(bodyBox).Text, text, "Body value is not expected");

            //•	Send the mail.
            this.driver.FindElement(btnSend).Click();

            //•	Verify, that the mail disappeared from ‘Drafts’ folder.
            this.driver.SwitchTo().DefaultContent();
            this.driver.Navigate().Refresh();
            WaitForElement(drafts);
            Assert.IsFalse(IsElementPresent(myDraft));

            //•	Verify, that the mail is in ‘Sent’ folder
            this.driver.FindElement(sent).Click();
            WaitForElement(myDraft);
            Assert.AreEqual(this.driver.FindElement(myDraft).Text, subject, "Mail is not in 'Sent' folder");

            //•	Log off.
            LogOut();
        }



    }
}