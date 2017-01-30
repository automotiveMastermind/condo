namespace PulseBridge.Condo.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Sockets;

    using static System.FormattableString;

    /// <summary>
    /// Represents a pool used to manage a stack of <see cref="SocketAsyncEventArgs"/>.
    /// </summary>
    public class SocketPool : ISocketPool
    {
        #region Private Fields
        /// <summary>
        /// The buffer manager used to pre-allocate a single buffer.
        /// </summary>
        private IBufferManager manager;

        /// <summary>
        /// A value indicating whether or not the buffer manager has already been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The stack of sockets that are managed by the pool.
        /// </summary>
        private ConcurrentStack<SocketAsyncEventArgs> sockets;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketPool"/> class.
        /// </summary>
        public SocketPool()
            : this(new BufferManager(1024, 100, 2))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketPool"/> class.
        /// </summary>
        /// <param name="size">
        /// The size of a buffer for a socket connection.
        /// </param>
        /// <param name="connections">
        /// The number of connections contained within the pool.
        /// </param>
        public SocketPool(int size, int connections)
            : this(new BufferManager(size, connections, 2))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketPool"/> class.
        /// </summary>
        /// <param name="size">
        /// The size of a buffer.
        /// </param>
        /// <param name="connections">
        /// The number of connections contained within the pool.
        /// </param>
        /// <param name="operations">
        /// The number of operations to allocate.
        /// </param>
        public SocketPool(int size, int connections, int operations)
            : this(new BufferManager(size, connections, operations))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketPool"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager used to manage a pre-allocated buffer.
        /// </param>
        public SocketPool(IBufferManager manager)
        {
            // set the manager
            this.manager = manager;

            // initialize the sockets stack
            this.sockets = new ConcurrentStack<SocketAsyncEventArgs>();
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public int Connections => this.manager.Connections;

        /// <inheritdoc/>
        public int Size => this.manager.Size;

        /// <inheritdoc/>
        public int Operations => this.manager.Operations;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public SocketAsyncEventArgs Pop()
        {
            SocketAsyncEventArgs args;

            // get socket event args
            if (this.sockets.TryPop(out args))
            {
                // set the buffer
                this.manager.SetBuffer(args);
            }

            // return the args
            return args;
        }

        /// <inheritdoc/>
        public void Push(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException
                    (nameof(args), Invariant($"The {nameof(args)} parameter cannot be null."));
            }

            // clear the buffer
            this.manager.FreeBuffer(args);

            // push the args back on the stack
            this.sockets.Push(args);
        }

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
                throw new ObjectDisposedException(nameof(SocketPool));
            }

            this.disposed = true;

            if (!disposing)
            {
                return;
            }

            try
            {
                this.manager.Dispose();
                this.manager = null;
            }
            catch
            {
                // swallow exceptions during dispose
            }

            this.sockets.Clear();
            this.sockets = null;
        }
        #endregion
    }
}