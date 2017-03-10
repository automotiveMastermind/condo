namespace AM.Condo.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Sockets;

    using static System.FormattableString;

    /// <summary>
    /// Represents a manager for allocating a single buffer for use with multiple operations.
    /// </summary>
    public class BufferManager : IBufferManager
    {
        #region Private Fields
        /// <summary>
        /// The size of a pre-allocated buffer for a single connection.
        /// </summary>
        private readonly int size;

        /// <summary>
        /// The maximum number of connections to pre-allocate.
        /// </summary>
        private readonly int connections;

        /// <summary>
        /// The number of operations to pre-allocate.
        /// </summary>
        private readonly int operations;

        /// <summary>
        /// The calculated size of a pre-allocated buffer.
        /// </summary>
        private readonly int calculated;

        /// <summary>
        /// The pre-allocated buffer containing multiple offsets.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// The underlying stack of offsets.
        /// </summary>
        private ConcurrentStack<int> offsets;

        /// <summary>
        /// A value indicating whether or not the buffer manager has already been disposed.
        /// </summary>
        private bool disposed;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferManager"/> class.
        /// </summary>
        public BufferManager()
            : this(1024, 100, 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferManager"/> class.
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
        public BufferManager(int size, int connections, int operations)
        {
            if (size <= 0)
            {
                throw new ArgumentException
                    (Invariant($"The {nameof(size)} parameter must be greater than 0"), nameof(size));
            }

            if (connections <= 1)
            {
                throw new ArgumentException
                    (Invariant($"The {nameof(connections)} parameter must be greater than 1."), nameof(connections));
            }

            if (operations <= 0)
            {
                throw new ArgumentException
                    (Invariant($"The {nameof(operations)} parameter must be greater than 0."), nameof(operations));
            }

            this.size = size;
            this.connections = connections;
            this.operations = operations;
            this.calculated = size * operations;

            // create the offset stack
            this.offsets = new ConcurrentStack<int>();

            // create the initial offset
            int offset = 0;

            // iterate over each connection
            for (int i = 0; i < this.connections; i++)
            {
                // push the offset
                this.offsets.Push(offset);

                // calculate the next offset
                offset += this.size * this.operations;
            }

            // create the buffer
            this.buffer = new byte[offset];
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public int Connections => this.connections;

        /// <inheritdoc/>
        public int Size => this.size;

        /// <inheritdoc/>
        public int Operations => this.operations;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public int Pop()
        {
            int offset;

            while (!this.offsets.TryPop(out offset))
            {
            }

            return offset;
        }

        /// <inheritdoc/>
        public void Push(int offset)
        {
            if (offset % this.calculated != 0)
            {
                throw new ArgumentException(Invariant($"The specified {nameof(offset)} is not valid."), nameof(offset));
            }

            // push the offset
            this.offsets.Push(offset);
        }

        /// <inheritdoc/>
        public void SetBuffer(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get the offset
            var offset = this.Pop();

            // set the buffer
            args.SetBuffer(this.buffer, offset, this.size);
        }

        /// <inheritdoc/>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get the offset
            var offset = args.Offset;

            // clear the args
            args.SetBuffer(null, 0, 0);

            // push the offset
            this.Push(offset);
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
                throw new ObjectDisposedException(nameof(BufferManager));
            }

            this.disposed = true;

            if (!disposing)
            {
                return;
            }

            this.buffer = null;
            this.offsets = null;
        }
        #endregion
    }
}