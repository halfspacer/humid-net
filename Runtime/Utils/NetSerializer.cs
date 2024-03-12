using System;
using System.Buffers;
using System.Text;

namespace Utils {
    
    /// <summary>
    /// The NetSerializer class is a utility for serializing and deserializing data types to and from byte arrays.
    /// It uses an ArrayPool to manage buffers, striving to improve performance and reduce memory usage in high-throughput scenarios.
    /// </summary>
    public class NetSerializer : IDisposable {
        private readonly ArrayPool<byte> pool;
        private byte[][] bufferStorage;
        private byte[] currentBuffer;
        private int currentBufferIndex;
        private int bufferStorageIndex;
        private readonly int maxBuffers;
        private int bufferSize;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the NetSerializer class with the specified buffer size and maximum number of buffers.
        /// </summary>
        /// <param name="bufferSize">The size of each buffer in the ArrayPool.</param>
        /// <param name="maxBuffers">The maximum number of buffers that can be managed by the ArrayPool.</param>
        public NetSerializer(int bufferSize = 1024, int maxBuffers = 50) {
            pool = ArrayPool<byte>.Create(bufferSize, maxBuffers);
            this.maxBuffers = maxBuffers;
            this.bufferSize = bufferSize;
            // Initialize the buffer storage array with the maximum number of buffers
            bufferStorage = new byte[maxBuffers][];
            bufferStorageIndex = 0;
            
            // Rent the first buffer from the pool
            currentBuffer = pool.Rent(bufferSize);
            currentBufferIndex = 0;
        }

        /// <summary>
        /// Returns the current buffer as an ArraySegment and resets the current buffer and index.
        /// </summary>
        /// <returns>An ArraySegment representing the current buffer.</returns>
        public ArraySegment<byte> GetBuffer() {
            var data = new ArraySegment<byte>(currentBuffer, 0, currentBufferIndex);
            Reset();
            return data;
        }

        // Resets the current buffer and index, moves the current buffer to the buffers array,
        // and rents a new buffer from the pool for the next serialization/deserialization operation.
        internal void Reset(bool discardBuffer = false) {
            currentBufferIndex = 0;
            
            if (discardBuffer) {
                pool.Return(currentBuffer);
                currentBuffer = pool.Rent(bufferSize);
                return;
            }
            
            bufferStorage[bufferStorageIndex] = currentBuffer;
            bufferStorageIndex = (bufferStorageIndex + 1) % maxBuffers;
            currentBuffer = pool.Rent(currentBuffer.Length);
        }

        // Ensures that the current buffer has enough capacity for the data we want to add.
        // If not, we rent a new buffer with double the required capacity, and copy the current buffer's data to the new buffer,
        // and return the current buffer back to the pool.
        private void EnsureCapacity(int additionalLength) {
            if (currentBufferIndex + additionalLength <= currentBuffer.Length) {
                return;
            }

            bufferSize = (currentBuffer.Length + additionalLength) * 2;
            var newBuffer = pool.Rent(bufferSize);
            Array.Copy(currentBuffer, newBuffer, currentBufferIndex);
            pool.Return(currentBuffer);
            currentBuffer = newBuffer;
        }

        /// <summary>
        /// Returns all buffers back to the ArrayPool and resets the current buffer and index.
        /// Returning the buffers assumes that the caller is done with the serialized data and the buffers can be reused.
        /// You would typically call this method after sending the serialized data over the network.
        /// </summary>
        public void ReturnBuffers() {
            for (var i = 0; i < bufferStorageIndex; i++) {
                if (bufferStorage[i] == null) {
                    continue;
                }

                pool.Return(bufferStorage[i]);
                bufferStorage[i] = null;
            }

            if (currentBuffer != null) {
                pool.Return(currentBuffer);
                currentBuffer = pool.Rent(bufferSize);
            }

            currentBufferIndex = 0;
            bufferStorageIndex = 0;
        }

        // The following Add methods are used to serialize various data types into the current buffer

        public void Add(bool value) {
            EnsureCapacity(sizeof(bool));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(bool);
        }

        public void Add(int value) {
            EnsureCapacity(sizeof(int));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(int);
        }

        public void Add(float value) {
            EnsureCapacity(sizeof(float));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(float);
        }

        public void Add(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                Add(0);
                return;
            }

            var byteCount = Encoding.UTF8.GetByteCount(value);
            EnsureCapacity(sizeof(int) + byteCount);
            Add(byteCount);
            Encoding.UTF8.GetBytes(value, 0, value.Length, currentBuffer, currentBufferIndex);
            currentBufferIndex += byteCount;
        }

        public void Add(uint value) {
            EnsureCapacity(sizeof(uint));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(uint);
        }

        public void Add(ulong value) {
            EnsureCapacity(sizeof(ulong));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(ulong);
        }

        public void Add(ushort value) {
            EnsureCapacity(sizeof(ushort));
            BitConverter.TryWriteBytes(currentBuffer.AsSpan(currentBufferIndex), value);
            currentBufferIndex += sizeof(ushort);
        }

        public void Add(byte value) {
            EnsureCapacity(sizeof(byte));
            currentBuffer[currentBufferIndex] = value;
            currentBufferIndex += sizeof(byte);
        }

        // The following Get methods are used to deserialize various data types from the current buffer

        public string GetString(ref ArraySegment<byte> type) {
            var length = GetInt(ref type);
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            if (length == 0) return "";
            var value = Encoding.UTF8.GetString(type.Array, type.Offset + currentBufferIndex, length);
            currentBufferIndex += length;
            return value;
        }

        public bool GetBool(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = Convert.ToBoolean(type.Array[type.Offset + currentBufferIndex]);
            currentBufferIndex++;
            return value;
        }

        public int GetInt(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToInt32(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 4;
            return value;
        }

        public uint GetUInt(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToUInt32(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 4;
            return value;
        }

        public float GetFloat(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToSingle(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 4;
            return value;
        }

        public long GetLong(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToInt64(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 8;
            return value;
        }

        public ulong GetULong(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToUInt64(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 8;
            return value;
        }

        public short GetShort(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToInt16(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 2;
            return value;
        }

        public ushort GetUShort(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = BitConverter.ToUInt16(type.Array, type.Offset + currentBufferIndex);
            currentBufferIndex += 2;
            return value;
        }

        public byte GetByte(ref ArraySegment<byte> type) {
            if (type.Array == null) throw new ArgumentNullException(nameof(type.Array), NullArrayErrorMessage);
            var value = type.Array[type.Offset + currentBufferIndex];
            currentBufferIndex++;
            return value;
        }

        // Error message for null array
        private const string NullArrayErrorMessage = "Array cannot be null.";
        
        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                // Dispose managed state (managed objects)
                ReturnBuffers();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            currentBuffer = null;
            bufferStorage = null;

            disposed = true;
        }
        
        ~NetSerializer() => Dispose(disposing: false);
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}