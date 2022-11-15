using System;
using System.IO;
using System.Text;

namespace App.Modules.Netcode.Utils {
    /// <summary>The NetSerializer is responsible for writing and reading basic data types to and from a Byte array.</summary>
    public class NetSerializer {
        public NetSerializer(int bufferSize = 1024) =>  _memoryStream = new MemoryStream(bufferSize);
        private readonly MemoryStream _memoryStream;
        private int _index;
        
        /// <summary>
        /// Get a byte array of bytes added to buffer using the Add methods.
        /// </summary>
        public byte[] GetBuffer() {
            var data = _memoryStream.ToArray();
            Reset();
            return data;
        }
        
        /// <summary>
        /// Resets the buffer. Note that calling GetBuffer() will invoke this method internally,
        /// so there's no need to manually reset before reusing the Serializer object for new Write operations.
        /// </summary>
        public void Reset() {
            _memoryStream.SetLength(0);
            _index = 0;
        }

        /// <summary>
        /// Convert value type to bytes and store it in the memory stream.
        /// Offsets the index by length so that multiple types can be written to the same buffer.
        /// </summary>
        public void Add(bool type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public void Add(int type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public void Add(float type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }
        
        public void Add(string type) {
            if (string.IsNullOrWhiteSpace(type)) {
                var zeroInt = BitConverter.GetBytes(0);
                _memoryStream.Write(zeroInt, 0, zeroInt.Length);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(type);
            var length = BitConverter.GetBytes(bytes.Length);
            _memoryStream.Write(length, 0, length.Length);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length + length.Length;
        }
        
        public void Add(uint type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public void Add(ulong type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public void Add(ushort type) {
            var bytes = BitConverter.GetBytes(type);
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public void Add(byte type) {
            var bytes = new[] {type};
            _memoryStream.Write(bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        /// <summary>
        /// Gets the value from the byte array passed in.
        /// Note that internally it keeps track of the read-index to support reading multiple values back-to-back,
        /// but this means that the serializer must be manually reset before being reused on a new array.
        /// </summary>
        public string GetString(byte[] type) {
            var length = GetInt(type);
            if (length == 0) return "";
            var value = Encoding.UTF8.GetString(type, _index, length);
            _index += length;
            return value;
        }
        
        public bool GetBool(byte[] type) {
            var value = Convert.ToBoolean(type[_index]);
            _index++;
            return value;
        }
        
        public int GetInt(byte[] type) {
            var byteArray = new byte[4];
            Buffer.BlockCopy(type, _index, byteArray, 0, 4);
            var value = BitConverter.ToInt32(byteArray, 0);
            _index += 4;
            return value;
        }
        
        public uint GetUInt(byte[] type) {
            var byteArray = new byte[4];
            Buffer.BlockCopy(type, _index, byteArray, 0, 4);
            var value = BitConverter.ToUInt32(byteArray, 0);
            _index += 4;
            return value;
        }
        
        public float GetFloat(byte[] type) {
            var byteArray = new byte[4];
            Buffer.BlockCopy(type, _index, byteArray, 0, 4);
            var value = BitConverter.ToSingle(byteArray, 0);
            _index += 4;
            return value;
        }
        
        public long GetLong(byte[] type) {
            var byteArray = new byte[8];
            Buffer.BlockCopy(type, _index, byteArray, 0, 8);
            var value = BitConverter.ToInt64(byteArray, 0);
            _index += 8;
            return value;
        }
        
        public ulong GetULong(byte[] type) {
            var byteArray = new byte[8];
            Buffer.BlockCopy(type, _index, byteArray, 0, 8);
            var value = BitConverter.ToUInt64(byteArray, 0);
            _index += 8;
            return value;
        }
        
        public short GetShort(byte[] type) {
            var byteArray = new byte[2];
            Buffer.BlockCopy(type, _index, byteArray, 0, 2);
            var value = BitConverter.ToInt16(byteArray, 0);
            _index += 2;
            return value;
        }
        
        public ushort GetUShort(byte[] type) {
            var byteArray = new byte[2];
            Buffer.BlockCopy(type, _index, byteArray, 0, 2);
            var value = BitConverter.ToUInt16(byteArray, 0);
            _index += 2;
            return value;
        }
        
        public byte GetByte(byte[] type) {
            var value = type[_index];
            _index++;
            return value;
        }
    }
}