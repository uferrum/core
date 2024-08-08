using System;
using System.Collections.Generic;
using System.Text;

namespace Ferrum.Bytes
{
    /// <summary>
    /// Provides utility methods for reading and writing various data types to a byte buffer.
    /// </summary>
    public static class ByteUtils
    {
        /// <summary>
        /// Writes the specified value to the buffer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void Write(object value, List<byte> buffer)
        {
            if (value is int intValue)
            {
                WriteInt(intValue, buffer);
            }
            else if (value is long longValue)
            {
                WriteLong(longValue, buffer);
            }
            else if (value is float floatValue)
            {
                WriteFloat(floatValue, buffer);
            }
            else if (value is double doubleValue)
            {
                WriteDouble(doubleValue, buffer);
            }
            else if (value is string stringValue)
            {
                WriteString(stringValue, buffer);
            }
            else if (value is bool boolValue)
            {
                WriteBoolean(boolValue, buffer);
            }
            else if (value is byte[] byteArray)
            {
                WriteBytes(byteArray, buffer);
            }
            else
            {
                throw new ArgumentException("Unsupported value type");
            }
        }

        /// <summary>
        /// Writes an integer value to the buffer.
        /// </summary>
        /// <param name="value">The integer value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteInt(int value, List<byte> buffer)
        {
            do
            {
                byte temp = (byte)(value & 0b01111111);
                value >>= 7;
                if (value != 0) temp |= 0b10000000;
                buffer.Add(temp);
            } while (value != 0);
        }

        /// <summary>
        /// Writes a long value to the buffer.
        /// </summary>
        /// <param name="value">The long value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteLong(long value, List<byte> buffer)
        {
            do
            {
                byte temp = (byte)(value & 0b01111111);
                value >>= 7;
                if (value != 0) temp |= 0b10000000;
                buffer.Add(temp);
            } while (value != 0);
        }

        /// <summary>
        /// Writes a byte array to the buffer.
        /// </summary>
        /// <param name="value">The byte array to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteBytes(byte[] value, List<byte> buffer)
        {
            WriteInt(value.Length, buffer);
            buffer.AddRange(value);
        }

        /// <summary>
        /// Writes a string to the buffer.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteString(string value, List<byte> buffer)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            WriteBytes(stringBytes, buffer);
        }

        /// <summary>
        /// Writes a short value to the buffer.
        /// </summary>
        /// <param name="value">The short value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteShort(short value, List<byte> buffer)
        {
            buffer.Add((byte)((value >> 8) & 0xff));
            buffer.Add((byte)(value & 0xff));
        }

        /// <summary>
        /// Writes a float value to the buffer.
        /// </summary>
        /// <param name="value">The float value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteFloat(float value, List<byte> buffer)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            buffer.AddRange(bytes);
        }

        /// <summary>
        /// Writes a double value to the buffer.
        /// </summary>
        /// <param name="value">The double value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteDouble(double value, List<byte> buffer)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            buffer.AddRange(bytes);
        }

        /// <summary>
        /// Writes a boolean value to the buffer.
        /// </summary>
        /// <param name="value">The boolean value to write.</param>
        /// <param name="buffer">The buffer to write to.</param>
        public static void WriteBoolean(bool value, List<byte> buffer)
        {
            buffer.Add(value ? (byte)1 : (byte)0);
        }

        /// <summary>
        /// Reads an integer value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The integer value.</returns>
        public static int ReadInt(List<byte> buffer, ref int position)
        {
            int result = 0;
            int shift = 0;
            byte b;
            do
            {
                b = buffer[position++];
                result |= (b & 0x7f) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return result;
        }

        /// <summary>
        /// Reads a long value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The long value.</returns>
        public static long ReadLong(List<byte> buffer, ref int position)
        {
            long result = 0;
            int shift = 0;
            byte b;
            do
            {
                b = buffer[position++];
                result |= (long)(b & 0x7f) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return result;
        }

        /// <summary>
        /// Reads a byte array of the specified length from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="length">The length of the byte array to read.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ReadBytesByLength(List<byte> buffer, int length, ref int position)
        {
            byte[] bytes = buffer.GetRange(position, length).ToArray();
            position += length;
            return bytes;
        }

        /// <summary>
        /// Reads the remaining bytes from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The remaining byte array.</returns>
        public static byte[] ReadRestBytes(List<byte> buffer, ref int position)
        {
            int length = buffer.Count - position;
            return ReadBytesByLength(buffer, length, ref position);
        }

        /// <summary>
        /// Reads a byte array from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ReadBytes(List<byte> buffer, ref int position)
        {
            int length = ReadInt(buffer, ref position);
            return ReadBytesByLength(buffer, length, ref position);
        }

        /// <summary>
        /// Reads a string from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The string.</returns>
        public static string ReadString(List<byte> buffer, ref int position)
        {
            byte[] bytes = ReadBytes(buffer, ref position);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Reads a short value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The short value.</returns>
        public static short ReadShort(List<byte> buffer, ref int position)
        {
            byte high = buffer[position++];
            byte low = buffer[position++];
            return (short)((high << 8) | low);
        }

        /// <summary>
        /// Reads a float value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The float value.</returns>
        public static float ReadFloat(List<byte> buffer, ref int position)
        {
            byte[] bytes = buffer.GetRange(position, 4).ToArray();
            position += 4;
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads a double value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The double value.</returns>
        public static double ReadDouble(List<byte> buffer, ref int position)
        {
            byte[] bytes = buffer.GetRange(position, 8).ToArray();
            position += 8;
            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Reads a boolean value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The current reading position in the buffer.</param>
        /// <returns>The boolean value.</returns>
        public static bool ReadBoolean(List<byte> buffer, ref int position)
        {
            return buffer[position++] == 1;
        }
    }

}