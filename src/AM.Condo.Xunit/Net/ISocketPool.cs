// <copyright file="ISocketPool.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Net
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Defines the properties and methods required to implement an object pool for <see cref="SocketAsyncEventArgs"/>.
    /// </summary>
    public interface ISocketPool : IDisposable
    {
        #region Properties and Indexers
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
        /// <returns>
        /// The <see cref="SocketAsyncEventArgs"/> to use for the next operation.
        /// </returns>
        SocketAsyncEventArgs Pop();

        /// <summary>
        /// Pushes an instance of a <see cref="SocketAsyncEventArgs"/> back into the pool and clears the pre-allocated
        /// buffer.
        /// </summary>
        /// <param name="args">
        /// The <see cref="SocketAsyncEventArgs"/> to return to the socket pool.
        /// </param>
        void Push(SocketAsyncEventArgs args);
        #endregion
    }
}
