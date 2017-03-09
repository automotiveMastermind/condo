namespace AM.Condo.Net
{
    using System;
    using System.Net.Sockets;

    using Xunit;

    [Class(nameof(BufferManager))]
    public class BufferManagerTest
    {
        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenDefault_CreatesBuffer()
        {
            // arrange
            var size = 1024;
            var connections = 100;
            var operations = 2;

            // act
            var actual = new BufferManager();

            // assert
            Assert.Equal(size, actual.Size);
            Assert.Equal(connections, actual.Connections);
            Assert.Equal(operations, actual.Operations);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenSizeDefined_CreatesBuffer()
        {
            // arrange
            var size = 512;
            var connections = 100;
            var operations = 2;

            // act
            var actual = new BufferManager(size, connections, operations);

            // assert
            Assert.Equal(size, actual.Size);
            Assert.Equal(connections, actual.Connections);
            Assert.Equal(operations, actual.Operations);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenSizeBelowZero_Throws()
        {
            // arrange
            var size = -1;
            var connections = 100;
            var operations = 2;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(size), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenSizeZero_Throws()
        {
            // arrange
            var size = 0;
            var connections = 100;
            var operations = 2;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(size), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenConnectionsDefined_CreatesBuffer()
        {
            // arrange
            var size = 1024;
            var connections = 50;
            var operations = 2;

            // act
            var actual = new BufferManager(size, connections, operations);

            // assert
            Assert.Equal(size, actual.Size);
            Assert.Equal(connections, actual.Connections);
            Assert.Equal(operations, actual.Operations);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenConnectionsBelowOne_Throws()
        {
            // arrange
            var size = 1024;
            var connections = 0;
            var operations = 2;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(connections), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenConnectionsOne_Throws()
        {
            // arrange
            var size = 1024;
            var connections = 1;
            var operations = 2;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(connections), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenOperationsDefined_CreatesBuffer()
        {
            // arrange
            var size = 1024;
            var connections = 100;
            var operations = 1;

            // act
            var actual = new BufferManager(size, connections, operations);

            // assert
            Assert.Equal(size, actual.Size);
            Assert.Equal(connections, actual.Connections);
            Assert.Equal(operations, actual.Operations);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenOperationsBelowZero_Throws()
        {
            // arrange
            var size = 1024;
            var connections = 2;
            var operations = -1;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(operations), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenOperationsZero_Throws()
        {
            // arrange
            var size = 1024;
            var connections = 2;
            var operations = 0;

            // act
            Action act = () => new BufferManager(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(operations), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Push_WhenOffsetInvalid_Throws()
        {
            // arrange
            var size = 1024;
            var connections = 2;
            var operations = 2;

            var offset = size * operations + 1;

            // act
            var actual = new BufferManager(size, connections, operations);

            Action act = () => actual.Push(offset);

            // assert
            Assert.Throws<ArgumentException>(nameof(offset), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Push_WhenOffsetValid_Succeeds()
        {
            // arrange
            var size = 1024;
            var connections = 2;
            var operations = 2;

            var offset = size * operations;

            // act
            var actual = new BufferManager(size, connections, operations);
            actual.Push(offset);

            // assert
            // assertion provided by no exception
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Pop_Succeeds()
        {
            // arrange
            // act
            var actual = new BufferManager();
            var offset = actual.Pop();

            // assert
            Assert.InRange(offset, 0, int.MaxValue);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void SetBuffer_WhenArgsNull_Throws()
        {
            // arrange
            var args = default(SocketAsyncEventArgs);

            // act
            var actual = new BufferManager();
            Action act = () => actual.SetBuffer(args);

            // assert
            Assert.Throws<ArgumentNullException>(nameof(args), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void SetBuffer_WhenArgsValid_Succeeds()
        {
            // arrange
            var size = 1024;
            var connections = 2;
            var operations = 2;

            var args = new SocketAsyncEventArgs();

            // act
            var actual = new BufferManager(size, connections, operations);
            actual.SetBuffer(args);

            // assert
            Assert.NotNull(args.Buffer);
            Assert.Equal(size, args.Count);
            Assert.True(args.Offset > 0);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void ClearBuffer_WhenArgsNull_Throws()
        {
            // arrange
            var args = default(SocketAsyncEventArgs);

            // act
            var actual = new BufferManager();
            Action act = () => actual.SetBuffer(args);

            // assert
            Assert.Throws<ArgumentNullException>(nameof(args), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void ClearBuffer_WhenArgsValid_Succeeds()
        {
            // arrange
            var buffer = new byte[10];
            var offset = 0;
            var count = 10;

            var args = new SocketAsyncEventArgs();
            args.SetBuffer(buffer, offset, count);

            // act
            var actual = new BufferManager();
            actual.FreeBuffer(args);

            // assert
            Assert.Null(args.Buffer);
            Assert.Equal(0, args.Offset);
            Assert.Equal(0, args.Count);
        }
    }
}