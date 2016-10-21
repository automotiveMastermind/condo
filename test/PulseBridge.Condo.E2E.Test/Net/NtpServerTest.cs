namespace PulseBridge.Condo.Net
{
    using System.Net;
    using System.Net.Sockets;

    using Xunit;
    using Moq;

    [Class(nameof(NtpServer))]
    public class NtpServerTest
    {
        [Fact]
        public void ValidRequest()
        {
            // arrange
            var expected = NtpTimestamp.UtcNow;
            var clock = Mock.Of<IClockProvider>(mock => mock.NtpNow == expected);

            var port = 8080;
            var addresses = Dns.GetHostAddressesAsync("localhost").Result;
            var endpoint = new IPEndPoint(addresses[0], port);

            var data = new byte[48];
            data[0] = 0x1B;

            // act
            using (var server = new NtpServer(port, clock))
            {
                using (var socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect(endpoint);

                    socket.SendTimeout = 30000;
                    socket.ReceiveTimeout = 30000;

                    socket.Send(data);

                    socket.Receive(data);
                }
            }

            // assert
            var actual = new NtpTimestamp(data, 40);
            Assert.Equal(expected, actual);
        }
    }
}