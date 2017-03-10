namespace AM.Condo.Net
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Defines the properties and methods required to implement an object pool for <see cref="SocketAsyncEventArgs"/>.
    /// </summary>
    public interface ISocketPool : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets the number of connections supported by the pool.
        /// </summary>
        int Connections { get; }

        /// <summary>
        /// Gets the size of a buffer for a single connection.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the number of operations to pre-allocate.
        /// </summary>
        int Operations { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Pops an instance of a <see cref="SocketAsyncEventArgs"/> from the pool with a pre-allocated buffer.
        /// </summary>
        SocketAsyncEventArgs Pop();

        /// <summary>
        /// Pushes an instance of a <see cref="SocketAsyncEventArgs"/> back into the pool and clears the pre-allocated
        /// buffer.
        /// </summary>
        void Push(SocketAsyncEventArgs args);
        #endregion
    }
}