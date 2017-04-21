// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferManager.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Net
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Defines the properties and methods required to implement a manager for a pre-allocated buffer.
    /// </summary>
    public interface IBufferManager : IDisposable
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets the maximum number of connections to pre-allocate.
        /// </summary>
        int Connections { get; }

        /// <summary>
        /// Gets the size of a buffer for an individual connection.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the number of operations to pre-allocate.
        /// </summary>
        int Operations { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the buffer on the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The arguments for an asynchronous socket event whose buffer should be set.
        /// </param>
        void SetBuffer(SocketAsyncEventArgs args);

        /// <summary>
        /// Clears the buffer on the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The arguments for an asynchronous socket event whose buffer should be cleared.
        /// </param>
        void FreeBuffer(SocketAsyncEventArgs args);

        /// <summary>
        /// Pop an offset from the buffer manager, which can be used to read and write to and from the buffer.
        /// </summary>
        /// <returns>
        /// An offset that can be used to read and write from the buffer.
        /// </returns>
        int Pop();

        /// <summary>
        /// Push the specified <paramref name="offset"/> back into the buffer manager.
        /// </summary>
        /// <param name="offset">
        /// The offset that should be pushed back into the buffer manager.
        /// </param>
        void Push(int offset);
        #endregion
    }
}
