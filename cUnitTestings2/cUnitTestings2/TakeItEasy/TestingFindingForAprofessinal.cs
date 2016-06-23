using System;
using FakeItEasy;
using NUnit.Framework;

namespace cUnitTestings2.NewFolder1
{
    [TestFixture]
    internal class TestingFindingForAprofessinal
    {
        [Test]
        public void UserRegistration_CorrectInformtaion_ReturnsTrue()
        {
            var id = 1;
            var name = "veysel";
            var mail = "vysl1com";
            var password = "123456";
            var type = "worker";
            Worker worker = null;
            var users = A.Fake<IUsers>();
            A.CallTo(() => users.controlInformation(name, mail, password, type))
                .Returns(worker = new Worker(name, mail, password, type));
            var database = A.Fake<IUsersInformation>();
            A.CallTo(() => database.Insert(worker)).Returns(worker);
            var registration = new RegistrationService();
            var returnValue = registration.Insert(database, worker);
            Assert.True(returnValue);
            {
            }
        }
    }

    public interface IUsers
    {
        IUsers controlInformation(string name, string mail, string password, string type);
    }

    public class Worker : IUsers
    {
        public Worker(string name, string mail, string password, string type)
        {
        }


        public IUsers controlInformation(string name, string mail, string password, string type)
        {
            Console.Write("deneme");
            if (password.Length > 5 && mail.EndsWith("@%.com"))
            {
                return new Customer(name, mail, password, type);
            }
            throw new NotImplementedException();
        }
    }

    public class Customer : IUsers
    {
        public Customer(string name, string mail, string password, string type)
        {
        }

        public IUsers controlInformation(string name, string mail, string password, string type)
        {
            if (password.Length > 5 && mail.EndsWith("@%.com"))
            {
                return new Customer(name, mail, password, type);
            }
            throw new NotImplementedException();
        }
    }

    public interface IUsersInformation
    {
        IUsers Insert(IUsers u);
        bool Contains(IUsers u);
    }

    public class RegistrationService
    {
        public bool Insert(IUsersInformation database, IUsers u)
        {
          
            if (!database.Contains(u)) return true;
            return true;
        }
    }
}