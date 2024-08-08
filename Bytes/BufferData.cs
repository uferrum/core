using System;
using System.Collections.Generic;

namespace Ferrum.Bytes
{
    /// <summary>
    /// Represents a buffer of byte data with read/write utilities.
    /// </summary>
    public class BufferData
    {
        /// <summary>
        /// Gets the buffer data.
        /// </summary>
        public List<byte> Data { get; private set; }

        private int _readingPosition;

        /// <summary>
        /// Gets the length of the buffer.
        /// </summary>
        public int BufferLength => Data.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferData"/> class.
        /// </summary>
        /// <param name="data">The initial data for the buffer.</param>
        /// <param name="initialReadingPosition">The initial reading position in the buffer.</param>
        public BufferData(List<byte> data = null, int initialReadingPosition = 0)
        {
            Data = data ?? new List<byte>();
            _readingPosition = initialReadingPosition;
        }

        /// <summary>
        /// Creates a <see cref="BufferData"/> instance from the specified object.
        /// </summary>
        /// <param name="what">The object to convert to <see cref="BufferData"/>.</param>
        /// <returns>A <see cref="BufferData"/> instance.</returns>
        public static BufferData From(object what)
        {
            List<byte> data = new List<byte>();

            if (what is BufferData bufferData)
            {
                data = bufferData.Data;
            }
            else if (what is byte[] byteArray)
            {
                data = new List<byte>(byteArray);
            }
            else if (what is List<byte> byteList)
            {
                data = new List<byte>(byteList);
            }
            else
            {
                return null;
            }

            return new BufferData(data, 0);
        }

        /// <summary>
        /// Concatenates the specified data to the buffer.
        /// </summary>
        /// <param name="other">The data to concatenate.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData Concat(object other)
        {
            if (other is BufferData bufferData)
            {
                other = bufferData.Data;
            }

            if (other is List<byte> byteList)
            {
                Data.AddRange(byteList);
            }
            else if (other is byte[] byteArray)
            {
                Data.AddRange(byteArray);
            }

            return this;
        }

        /// <summary>
        /// Sets the reading position in the buffer.
        /// </summary>
        /// <param name="position">The position to set.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData Seek(int position)
        {
            _readingPosition = position;
            return this;
        }

        /// <summary>
        /// Moves the reading cursor by the specified position.
        /// </summary>
        /// <param name="position">The number of positions to move the cursor.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData MoveCursor(int position)
        {
            _readingPosition += position;
            return this;
        }

        // Write methods

        /// <summary>
        /// Writes the specified value to the buffer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData Write(object value)
        {
            ByteUtils.Write(value, Data);
            return this;
        }

        /// <summary>
        /// Writes an integer value to the buffer.
        /// </summary>
        /// <param name="value">The integer value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteInt(int value)
        {
            ByteUtils.WriteInt(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a long value to the buffer.
        /// </summary>
        /// <param name="value">The long value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteLong(long value)
        {
            ByteUtils.WriteLong(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a byte array to the buffer.
        /// </summary>
        /// <param name="value">The byte array to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteBytes(byte[] value)
        {
            ByteUtils.WriteBytes(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a string to the buffer.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteString(string value)
        {
            ByteUtils.WriteString(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a short value to the buffer.
        /// </summary>
        /// <param name="value">The short value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteShort(short value)
        {
            ByteUtils.WriteShort(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a float value to the buffer.
        /// </summary>
        /// <param name="value">The float value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteFloat(float value)
        {
            ByteUtils.WriteFloat(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a double value to the buffer.
        /// </summary>
        /// <param name="value">The double value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteDouble(double value)
        {
            ByteUtils.WriteDouble(value, Data);
            return this;
        }

        /// <summary>
        /// Writes a boolean value to the buffer.
        /// </summary>
        /// <param name="value">The boolean value to write.</param>
        /// <returns>The current <see cref="BufferData"/> instance.</returns>
        public BufferData WriteBoolean(bool value)
        {
            ByteUtils.WriteBoolean(value, Data);
            return this;
        }

        // Read methods

        /// <summary>
        /// Reads an integer value from the buffer.
        /// </summary>
        /// <returns>The integer value.</returns>
        public int ReadInt()
        {
            return ByteUtils.ReadInt(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a long value from the buffer.
        /// </summary>
        /// <returns>The long value.</returns>
        public long ReadLong()
        {
            return ByteUtils.ReadLong(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a byte array of the specified length from the buffer.
        /// </summary>
        /// <param name="length">The length of the byte array to read.</param>
        /// <returns>The byte array.</returns>
        public byte[] ReadBytesByLength(int length = 1)
        {
            return ByteUtils.ReadBytesByLength(Data, length, ref _readingPosition);
        }

        /// <summary>
        /// Reads the remaining bytes from the buffer.
        /// </summary>
        /// <returns>The remaining byte array.</returns>
        public byte[] ReadRestBytes()
        {
            return ByteUtils.ReadRestBytes(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a byte array from the buffer.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ReadBytes()
        {
            return ByteUtils.ReadBytes(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a string from the buffer.
        /// </summary>
        /// <returns>The string.</returns>
        public string ReadString()
        {
            return ByteUtils.ReadString(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a short value from the buffer.
        /// </summary>
        /// <returns>The short value.</returns>
        public short ReadShort()
        {
            return ByteUtils.ReadShort(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a float value from the buffer.
        /// </summary>
        /// <returns>The float value.</returns>
        public float ReadFloat()
        {
            return ByteUtils.ReadFloat(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a double value from the buffer.
        /// </summary>
        /// <returns>The double value.</returns>
        public double ReadDouble()
        {
            return ByteUtils.ReadDouble(Data, ref _readingPosition);
        }

        /// <summary>
        /// Reads a boolean value from the buffer.
        /// </summary>
        /// <returns>The boolean value.</returns>
        public bool ReadBoolean()
        {
            return ByteUtils.ReadBoolean(Data, ref _readingPosition);
        }

        /// <summary>
        /// Extracts values from the buffer based on the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to use for extraction (e.g., "ssi" for string, string, int).</param>
        /// <returns>An array of extracted values.</returns>
        public object[] Extract(string pattern = "sss")
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return Array.Empty<object>();
            }

            pattern = pattern.ToLower();
            List<object> result = new List<object>();

            foreach (char x in pattern)
            {
                switch (x)
                {
                    case 's':
                        result.Add(ReadString());
                        break;
                    case 'l':
                        result.Add(ReadLong());
                        break;
                    case 'i':
                        result.Add(ReadInt());
                        break;
                    case 'b':
                        result.Add(ReadBoolean());
                        break;
                    case 'd':
                        result.Add(ReadBytes());
                        break;
                    case 'h':
                        result.Add(ReadShort());
                        break;
                    case 'f':
                        result.Add(ReadFloat());
                        break;
                    case 'x':
                        result.Add(ReadDouble());
                        break;
                    default:
                        break;
                }
            }

            return result.ToArray();
        }
    }

}