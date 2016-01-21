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
            Socket socket = A.Fake<Socket>();
            _sut = new Transmitter(socket);
        }

        [Test]
        public void SendObject_Should_Return_True()
        {
            string str = "Hello world!";

            var result = _sut.sendObject(str);

            Assert.That(result, Is.True);
        }
    }

}
