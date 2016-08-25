namespace PulseBridge.Condo.Build.Tasks
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that gets the current time from a Network Time Protocol (NTP) compliant
    /// server.
    /// </summary>
    /// <remarks>
    /// The default implementation will use the time server provided by the National Institute of Standards and
    /// Technology (NIST) within the United States.
    /// </remarks>
    public class GetServerTime : Task
    {
        /// <summary>
        /// Gets an accurate server time from NIST represented in UTC.
        /// </summary>
        [Output]
        public string DateTimeUtc { get; private set; }

        /// <summary>
        /// Gets or sets the URI of the time server used to get the server time.
        /// </summary>
        public string Uri { get; set; } = "time.nist.gov";

        /// <summary>
        /// Gets or sets the port of the time server used to get the server time.
        /// </summary>
        /// <returns></returns>
        public int Port { get; set; } = 123;

        /// <summary>
        /// Executes the <see cref="GetServerTime"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                // get the current address of the time server from DNS
                var addresses = Dns.GetHostEntryAsync(this.Uri).Result.AddressList;

                // create the endpoint using the first address in the response
                var endpoint = new IPEndPoint(addresses[0], this.Port);

                // create a byte array to retain the request/response from the socket
                var data = new byte[48];
                data[0] = 0x1B;

                // create a new socket to connect to the time server
                using (var socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp))
                {
                    // connect to the endpoint
                    socket.Connect(endpoint);

                    // wait no more than 5 seconds for the time to be received
                    socket.ReceiveTimeout = 5000;

                    // send the request
                    socket.Send(data);

                    // get the response
                    socket.Receive(data);
                }

                // create the seconds and second fraction bits from the time server
                ulong seconds = (ulong)data[40] << 24 | (ulong)data[41] << 16 | (ulong)data[42] << 8 | (ulong)data[43];
                ulong fraction = (ulong)data[44] << 24 | (ulong)data[45] << 16 | (ulong)data[46] << 8 | (ulong)data[47];

                // calculate the total millisconds since 1900
                var ms = (seconds * 1000) + ((fraction * 1000) / 0x100000000L);

                // generate the utc time stamp from the response received via NTP
                var date = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ms);

                // set the date and time as received from the server
                this.DateTimeUtc = date.ToString("o", CultureInfo.InvariantCulture);
            }
            catch (Exception netEx)
            {
                // log a warning
                this.Log.LogWarning($"Unable to retrieve time from the time server {this.Uri} on port {this.Port}, reverting to local time.");
                this.Log.LogWarningFromException(netEx);

                // use local time as a fallback
                this.DateTimeUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            }

            // return true
            return true;
        }
    }
}