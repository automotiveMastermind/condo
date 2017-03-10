namespace AM.Condo.Net
{
    using System;
    using System.Net.Sockets;

    using Xunit;

    [Class(nameof(SocketPool))]
    public class SocketPoolTest
    {
        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenDefault_CreatesPool()
        {
            // arrange
            var size = 1024;
            var connections = 100;
            var operations = 2;

            // act
            var actual = new SocketPool();

            // assert
            Assert.Equal(size, actual.Size);
            Assert.Equal(connections, actual.Connections);
            Assert.Equal(operations, actual.Operations);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenSizeDefined_CreatesPool()
        {
            // arrange
            var size = 512;
            var connections = 100;
            var operations = 2;

            // act
            var actual = new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(size), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenConnectionsDefined_CreatesPool()
        {
            // arrange
            var size = 1024;
            var connections = 50;
            var operations = 2;

            // act
            var actual = new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(connections), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Ctor_WhenOperationsDefined_CreatesPool()
        {
            // arrange
            var size = 1024;
            var connections = 100;
            var operations = 1;

            // act
            var actual = new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

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
            Action act = () => new SocketPool(size, connections, operations);

            // assert
            Assert.Throws<ArgumentException>(nameof(operations), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Push_WhenArgsNull_Throws()
        {
            // arrange
            var args = default(SocketAsyncEventArgs);

            // act
            var actual = new SocketPool();
            Action act = () => actual.Push(args);

            // arrange
            Assert.Throws<ArgumentNullException>(nameof(args), act);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Push_WhenArgsDefined_Succeeds()
        {
            // arrange
            var args = new SocketAsyncEventArgs();

            // act
            var target = new SocketPool();
            target.Push(args);

            // assert
            // assertion provided by no exception
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Pop_WhenArgsNotAvailable_ReturnsNull()
        {
            // arrange
            var args = new SocketAsyncEventArgs();

            // act
            var target = new SocketPool();
            var actual = target.Pop();

            // assert
            Assert.Null(actual);
        }
    }
}