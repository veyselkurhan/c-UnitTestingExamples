using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy;
namespace cUnitTestings2.LoginTests
{
    [TestFixture]
    public class LoginServiceTests
    {
        private IAccountRepository accountRepository;
        private IAccount account;
        private LoginService service;

        [SetUp]
        public void SetUp()
        {
            this.account = A.Fake<IAccount>();
            this.accountRepository = A.Fake<IAccountRepository>();
            A.CallTo(() => accountRepository.Find(A<string>.Ignored)).Returns(this.account);
            this.service = new LoginService(accountRepository);

        }

        [Test]
        public void SecondLogin_AlreadyLogin_NotPermitAgainLogin()
        {
            //Arrange
            A.CallTo(() => this. account.PasswordMatches(A<string>.Ignored)).Returns(true);

            //act
            service.Login("username","password");
            service.Login("username","password");
            //Assert
            A.CallTo(()=>this.account.SetLoggedIn(true)).MustHaveHappened(Repeated.Exactly.Once);

        }

        [Test]
        public void NotRevokeTest_twofailLoginAttempts_NotRevokeUser()
        {
            //Arrange
            A.CallTo(() => this.account.PasswordMatches(A<string>.Ignored)).Returns(false);
            
            //act
            this.service.Login("username","wrong password");
            this.service.Login("username","wrong password");
            this.service.Login("vk","wrong password");

            A.CallTo(()=>this.account.SetRevoked(true)).MustNotHaveHappened();
        }
        [Test]
        public void LoginTest_paswordMatches_loggedIn()
        {
            //Arrange
            A.CallTo(() =>this. account.PasswordMatches(A<string>.Ignored)).Returns(true);
            //Act
           this. service.Login("username", "password");
            //Assert
            A.CallTo(() =>this. account.SetLoggedIn(true)).MustHaveHappened();

        }

        [Test]
        public void RevokeAccount_3FailLoginAttempts_RevokeAccount()
        {
            //Arrange
            A.CallTo(() =>this. account.PasswordMatches(A<string>.Ignored)).Returns(false);
            //act
           this. service.Login("username", "wrong password");
            this.service.Login("username", "wrong password");
            this.service.Login("username", "wrong password");
            //Assert
            A.CallTo(() => this.account.SetRevoked(true)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void FailLogin_passwordWrong_NotLoggedIn()
        {
            //Arrange
            A.CallTo(() =>this. account.PasswordMatches(A<string>.Ignored)).Returns(false);
            //act
            this.service.Login("username", "wrong password");
            A.CallTo(() => this.account.SetLoggedIn(true)).MustNotHaveHappened();
        }
    }

    public interface IAccount
    {
        
        bool PasswordMatches(string password);
        void SetLoggedIn(bool b);
        void SetRevoked(bool b);

       
    }

    public interface IAccountRepository
    {
        IAccount Find(string name);
    }

    public class LoginService
    {
        private int loginCount;
        private int numberOfFailedAttempts = 0;
        private IAccountRepository accountRepository;
        private string previousLoginUsername;
        public LoginService(IAccountRepository accountRepository)
        {
            this. loginCount = 0;
            this.accountRepository = accountRepository;
            this.previousLoginUsername=string.Empty;
        }
        public void Login(string username, string password)
        {

            var account = this.accountRepository.Find(username);

         if (account.PasswordMatches(password)&&loginCount==0)
         {
             loginCount = 1;
            account.SetLoggedIn(true);
            }

            if (!account.PasswordMatches(password) && this.previousLoginUsername.Equals(username))
            {
                this.numberOfFailedAttempts++;
            }
            else
            {
                this.numberOfFailedAttempts = 1;
                this.previousLoginUsername = username;
            }
            if (this.numberOfFailedAttempts == 3)
            {
                account.SetRevoked(true);
            }
        }
    }
}
