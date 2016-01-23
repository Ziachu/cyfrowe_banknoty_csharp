using System;
using System.Net.Sockets;
using FakeItEasy;
using NUnit.Framework;
using CyfroweBanknoty.Tools;

namespace CyfroweBanknoty.Tests
{
    [TestFixture]
    class TransmitterTests
    {
        public Transmitter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Transmitter();

            Socket socket = A.Fake<Socket>();
            _sut.AddReceiver("Bob", socket);
        }

        [Test]
        public void SendObject_Should_Return_True()
        {
            string str = "Hello world!";

            var result = _sut.sendObject(str, "Bob");

            Assert.That(result, Is.True);
        }
    }

}
