namespace AM.Condo.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using static System.FormattableString;

    /// <summary>
    /// Represents a server used to process NTP connections.
    /// </summary>
    public class NtpServer : IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The port used to to listen for connections.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// A value indicating whether or not the server has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The socket used to listen for connections.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// The provider used to get the current date and time from a trusted authority.
        /// </summary>
        private IClockProvider clock;

        /// <summary>
        /// The pool for managing <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        private ISocketPool pool;

        /// <summary>
        /// A value used to track active connections.
        /// </summary>
        private int connected;

        /// <summary>
        /// The semaphore used to track accepted clients.
        /// </summary>
        private readonly Semaphore accepted;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        public NtpServer()
            : this(123)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port used to listen for requests to the NTP server.
        /// </param>
        public NtpServer(int port)
            : this(port, 100, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port used to listen for requests to the NTP server.
        /// </param>
        /// <param name="clock">
        /// The clock provider used to retrieve the current date and time from a trusted authority.
        /// </param>
        public NtpServer(int port, IClockProvider clock)
            : this(port, 100, clock, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port used to listen for requests to the NTP server.
        /// </param>
        /// <param name="backlog">
        /// The backlog, or number of connections, allowed by the server.
        /// </param>
        public NtpServer(int port, int backlog)
            : this(port, backlog, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port used to listen for requests to the NTP server.
        /// </param>
        /// <param name="backlog">
        /// The backlog, or number of connections, allowed by the server.
        /// </param>
        /// <param name="clock">
        /// The clock provider used to retrieve the current date and time from a trusted authority.
        /// </param>
        public NtpServer(int port, int backlog, IClockProvider clock)
            : this(port, backlog, clock, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port used to listen for requests to the NTP server.
        /// </param>
        /// <param name="backlog">
        /// The backlog, or number of connections, allowed by the server.
        /// </param>
        /// <param name="clock">
        /// The clock provider used to retrieve the current date and time from a trusted authority.
        /// </param>
        /// <param name="pool">
        /// The object pool containing socket arguments used for processing requests.
        /// </param>
        public NtpServer(int port, int backlog, IClockProvider clock, ISocketPool pool)
        {
            // determine if the port is in an acceptable range
            if (port <= 0)
            {
                throw new ArgumentException(Invariant($"{nameof(port)} must be greater than 0."), nameof(port));
            }

            // determine if the backlog is in an acceptable range
            if (backlog <= 0)
            {
                throw new ArgumentException(Invariant($"{nameof(backlog)} must be greater than 0."), nameof(backlog));
            }

            // set the clock
            this.clock = clock ?? new LocalMachineClock();

            // set the socket pool
            this.pool = pool ?? new SocketPool(48, 5, 2);

            // set the port
            this.port = port;

            // get the addresses
            var addresses = System.Net.Dns.GetHostAddressesAsync("localhost").Result;

            // create the endpoint using the first address in the response
            var endpoint = new IPEndPoint(addresses[0], this.port);

            // create the remote address
            var address = endpoint.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any;

            // create the remote endpoint
            var remote = new IPEndPoint(address, 0);

            // initialize the semaphore
            this.accepted = new Semaphore(this.pool.Connections, this.pool.Connections);

            // initialize the socket
            this.socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 30000,
                SendTimeout = 30000
            };

            // iterate over each connection
            for(int i = 0; i < this.pool.Connections; i++)
            {
                // create socket event args
                var args = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = remote
                };

                // set the completed operation handler
                args.Completed += this.Operation;

                // push the args into the pool
                this.pool.Push(args);
            }

            // bind the socket endpoint
            this.socket.Bind(endpoint);

            // accept the next connection
            this.Accept();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the port used for the server.
        /// </summary>
        public int Port => this.port;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not dispose was called manually.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(NtpServer));
            }

            this.disposed = true;

            if (!disposing)
            {
                return;
            }

            try
            {
                // dispose of the socket
                this.socket.Dispose();
            }
            catch
            {
                // swallow exceptions during dispose
            }

            try
            {
                // dispose of the pool
                this.pool.Dispose();
            }
            catch
            {
                // swallow exceptions during dispose
            }
        }

        private void Accept()
        {
            // get arguments for reading from the clients
            var read = this.pool.Pop();

            // increment the connected count
            Interlocked.Increment(ref this.connected);

            // increment the semaphore
            this.accepted.WaitOne();

            // capture the accept socket
            var socket = this.socket;

            // set the socket on the args
            read.UserToken = socket;

            // receive data asynchronously
            if (!socket.ReceiveFromAsync(read))
            {
                // receive data immediately since it is ready
                this.Receive(read);
            }
        }

        private void Operation(object sender, SocketAsyncEventArgs operation)
        {
            // determine if an error has occurred during the operation
            if (operation.SocketError != SocketError.Success)
            {
                // complete the request
                this.Complete(operation);

                // move on immediately
                return;
            }

            switch (operation.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                case SocketAsyncOperation.ReceiveFrom:
                    this.Receive(operation);
                    return;
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    this.Send(operation);
                    return;
            }

            throw new InvalidOperationException
                (Invariant($"The operation ({operation.LastOperation}) cannot be handled."));
        }

        private void Receive(SocketAsyncEventArgs receive)
        {
            // determine if data was received
            if (receive.BytesTransferred == 0 || receive.SocketError != SocketError.Success)
            {
                // complete the connection
                this.Complete(receive);

                // move on immediately
                return;
            }

            // process the request
            this.Process(receive);
        }

        private void Process(SocketAsyncEventArgs process)
        {
            // capture the buffer
            var buffer = process.Buffer;

            // determine if a request for server time was received
            if (process.BytesTransferred != 48 || buffer[0] != 0x1B)
            {
                // complete the connection
                this.Complete(process);

                // move on immediately
                return;
            }

            // get the current ntp timestamp
            var now = this.clock.NtpNow;

            // copy data to the buffer
            Array.Copy(now.Timestamp, 0, buffer, 40, 8);

            // set the buffer and send back the data
            process.SetBuffer(buffer, process.Offset, 48);

            // capture the socket
            var socket = process.UserToken as Socket;

            // asynchronously send the data
            if (!socket.SendToAsync(process))
            {
                // send the data immediately
                this.Send(process);
            }
        }

        private void Send(SocketAsyncEventArgs send)
        {
            // complete the request
            this.Complete(send);
        }

        private void Complete(SocketAsyncEventArgs complete)
        {
            // determine if we have been disposed
            if (!this.disposed)
            {
                // accept the next transmission
                this.Accept();

                // free the buffer and release the args
                this.pool.Push(complete);
            }

            // release the semaphore
            this.accepted.Release();

            // decrement the counter
            Interlocked.Decrement(ref this.connected);
        }
        #endregion
    }
}