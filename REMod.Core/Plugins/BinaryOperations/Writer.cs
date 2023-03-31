using System;
using System.IO;
using System.Text;

namespace REMod.Core.Plugins.BinaryOperations
{
    public class Writer
    {
        public class Write7BitEncodedIntStream : BinaryWriter
        {
            public Write7BitEncodedIntStream(Stream stream)
                : base(stream)
            {
            }

            public new void Write7BitEncodedInt(int i)
            {
                base.Write7BitEncodedInt(i);
            }
        }

        public Endian CurrentEndian;

        public long LastPosition;

        private readonly Stream OpenStream;

        private readonly BinaryWriter WriterB;

        public long Length => OpenStream.Length;

        public long Position
        {
            get
            {
                return OpenStream.Position;
            }
            set
            {
                LastPosition = OpenStream.Position;
                OpenStream.Position = value;
            }
        }

        public Writer(FileStream Package, Endian EndianType = Endian.Little, long Position = 0)
        {
            OpenStream = null;
            WriterB = null;
            CurrentEndian = Endian.Little;
            LastPosition = 0L;
            OpenStream = Package;
            WriterB = new BinaryWriter(OpenStream);
            this.Position = Position;
            CurrentEndian = EndianType;
        }

        public Writer(string Package, Endian EndianType = Endian.Little, long Position = 0)
        {
            OpenStream = null;
            WriterB = null;
            CurrentEndian = Endian.Little;
            LastPosition = 0L;
            OpenStream = new FileStream(Package, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            WriterB = new BinaryWriter(OpenStream);
            this.Position = Position;
            CurrentEndian = EndianType;
        }

        public Writer(byte[] Package, Endian EndianType = Endian.Little, long Position = 0)
        {
            OpenStream = null;
            WriterB = null;
            CurrentEndian = Endian.Little;
            LastPosition = 0L;
            OpenStream = new MemoryStream(Package);
            WriterB = new BinaryWriter(OpenStream);
            this.Position = Position;
            CurrentEndian = EndianType;
        }

        public Writer(MemoryStream Package, Endian EndianType = Endian.Little, long Position = 0)
        {
            OpenStream = null;
            WriterB = null;
            CurrentEndian = Endian.Little;
            LastPosition = 0L;
            OpenStream = Package;
            WriterB = new BinaryWriter(OpenStream);
            this.Position = Position;
            CurrentEndian = EndianType;
        }

        public Writer(Stream Package, Endian EndianType = Endian.Little, long Position = 0)
        {
            OpenStream = null;
            WriterB = null;
            CurrentEndian = Endian.Little;
            LastPosition = 0L;
            OpenStream = Package;
            WriterB = new BinaryWriter(OpenStream);
            this.Position = Position;
            CurrentEndian = EndianType;
        }

        public Writer(string Package, FileMode Mode, Endian EndianType = Endian.Little, long Position = 0)
            : this(new FileStream(Package, Mode, FileAccess.ReadWrite, FileShare.ReadWrite), EndianType, Position)
        {
        }

        public void Close()
        {
            Flush();
            WriterB.Close();
            OpenStream.Close();
        }

        public void Flush()
        {
            WriterB.Flush();
            OpenStream.Flush();
        }

        public void Seek(long Offset) => Seek(Offset, SeekOrigin.Begin);

        public void Seek(long Offset, SeekOrigin SeekOrigin)
        {
            LastPosition = OpenStream.Position;
            WriterB.BaseStream.Seek(Offset, SeekOrigin);
        }

        public void SwapEndian()
        {
            if (CurrentEndian == Endian.Little)
            {
                CurrentEndian = Endian.Big;
            }
            else
            {
                CurrentEndian = Endian.Little;
            }
        }

        public void SetEndian(Endian EndianType) => CurrentEndian = EndianType;

        public void Write(byte[] Buffer) => Write(Buffer, CurrentEndian);

        public void Write(byte[] Buffer, Endian EndianType)
        {
            LastPosition = OpenStream.Position;

            if (EndianType == Endian.Big)
            {
                Functions.SwapSex(Buffer);
            }

            WriterB.Write(Buffer, 0, Buffer.Length);
        }

        public void Write(byte[] Buffer, int Length) => Write(Buffer, Length, CurrentEndian);

        public void Write(byte[] Buffer, int Length, Endian EndianType)
        {
            LastPosition = OpenStream.Position;

            if (EndianType == Endian.Big)
            {
                Functions.SwapSex(Buffer);
            }

            WriterB.Write(Buffer, 0, Length);
        }

        public void Write(byte[] Buffer, int Index, int Length) => Write(Buffer, Index, Length, CurrentEndian);

        public void Write(byte[] Buffer, int Index, int Length, Endian EndianType)
        {
            LastPosition = OpenStream.Position;

            if (EndianType == Endian.Big)
            {
                Functions.SwapSex(Buffer);
            }

            WriterB.Write(Buffer, Index, Length);
        }

        public void WriteDouble(double Value) => WriteDouble(Value, CurrentEndian);

        public void WriteDouble(double Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 8, EndianType);

        public void WriteHexString(string Value)
        {
            byte[] array = Conversions.HexToByteArray(Value);
            Write(array, 0, array.Length);
        }

        public void WriteHexString(string Value, int Length) => WriteHexString(Value, Length, CurrentEndian);

        public void WriteHexString(string Value, int Length, Endian EndianType) => Write(Conversions.HexToByteArray(Value), Length, EndianType);

        public void WriteChar(char Value)
        {
            LastPosition = OpenStream.Position;
            WriterB.Write(Value);
        }

        public void WriteChars(char[] Value)
        {
            for (int i = 0; i < Value.Length; i = checked(i + 1))
            {
                WriteChar(Value[i]);
            }
        }

        public void WriteInt16(short Value) => WriteInt16(Value, CurrentEndian);

        public void WriteInt16(short Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 2, EndianType);

        public void WriteInt24(int Value) => WriteInt24(Value, CurrentEndian);

        public void WriteInt24(int Value, Endian EndianType) => Write(Conversions.Int24ToByteArray(Value), 0, 3, EndianType);

        public void WriteInt32(int Value) => WriteInt32(Value, CurrentEndian);

        public void WriteInt32(int Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 4, EndianType);

        public void WriteInt40(long Value) => WriteInt40(Value, CurrentEndian);

        public void WriteInt40(long Value, Endian EndianType) => Write(Conversions.Int40ToByteArray(Value), 0, 5, EndianType);

        public void WriteInt48(long Value) => WriteInt48(Value, CurrentEndian);

        public void WriteInt48(long Value, Endian EndianType) => Write(Conversions.Int48ToByteArray(Value), 0, 6, EndianType);

        public void WriteInt56(long Value) => WriteInt56(Value, CurrentEndian);

        public void WriteInt56(long Value, Endian EndianType) => Write(Conversions.Int56ToByteArray(Value), 0, 7, EndianType);

        public void WriteInt64(long Value) => WriteInt64(Value, CurrentEndian);

        public void WriteInt64(long Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 8, EndianType);

        public void WriteInt8(sbyte Value) => Write(BitConverter.GetBytes((short)Value), 0, 1);

        public void WriteSingle(float Value) => WriteSingle(Value, CurrentEndian);

        public void WriteSingle(float Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 4, EndianType);

        public void WriteString(string Value) => WriteString(Value, 0, Value.Length);

        public void WriteString(string Value, int Length) => WriteString(Value, 0, Length);

        public void WriteString(string Value, int StartIndex, int Length) => Write(Encoding.ASCII.GetBytes(Value), StartIndex, Length);

        public void WriteUTF8String(string Value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Write(bytes, 0, bytes.Length);
        }

        public void WriteUString(string Value) => WriteUString(Value, 0, Value.Length);

        public void WriteUString(string Value, int Length) => WriteUString(Value, 0, checked(Length * 2));

        public void WriteUString(string Value, int StartIndex, int Length) => Write(Encoding.BigEndianUnicode.GetBytes(Value), StartIndex, checked(Length * 2));

        public void WriteUInt16(ushort Value) => WriteUInt16(Value, CurrentEndian);

        public void WriteUInt16(ushort Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 2, EndianType);

        public void WriteUInt24(uint Value) => WriteUInt24(Value, CurrentEndian);

        public void WriteUInt24(uint Value, Endian EndianType) => Write(Conversions.Int24ToByteArray(checked((int)Value)), 0, 3, EndianType);

        public void WriteUInt32(uint Value) => WriteUInt32(Value, CurrentEndian);

        public void WriteUInt32(uint Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 4, EndianType);

        public void WriteUInt40(ulong Value) => WriteUInt40(Value, CurrentEndian);

        public void WriteUInt40(ulong Value, Endian EndianType) => Write(Conversions.Int40ToByteArray(checked((long)Value)), 0, 5, EndianType);

        public void WriteUInt48(ulong Value) => WriteUInt48(Value, CurrentEndian);

        public void WriteUInt48(ulong Value, Endian EndianType) => Write(Conversions.Int48ToByteArray(checked((long)Value)), 0, 6, EndianType);

        public void WriteUInt56(ulong Value) => WriteUInt56(Value, CurrentEndian);

        public void WriteUInt56(ulong Value, Endian EndianType) => Write(Conversions.Int56ToByteArray(checked((long)Value)), 0, 7, EndianType);

        public void WriteUInt64(ulong Value) => WriteUInt64(Value, CurrentEndian);

        public void WriteUInt64(ulong Value, Endian EndianType) => Write(BitConverter.GetBytes(Value), 0, 8, EndianType);

        public void WriteUnicodeString(string Value)
        {
            char[] chars = Value.ToCharArray();
            if (CurrentEndian == Endian.Big)
            {
                Write(Encoding.BigEndianUnicode.GetBytes(chars), Endian.Little);
            }
            else
            {
                Write(Encoding.Unicode.GetBytes(chars), Endian.Little);
            }
        }

        public void WriteUnicodeString(string Value, int Length) => WriteUnicodeString(Value, 0, Length, CurrentEndian);

        public void WriteUnicodeString(string Value, int StartIndex, int Length) => WriteUnicodeString(Value, StartIndex, Length, CurrentEndian);

        public void WriteUnicodeString(string Value, int StartIndex, int Length, Endian EndianType)
        {
            char[] chars = Value.ToCharArray();
            if (EndianType == Endian.Big)
            {
                Write(Encoding.BigEndianUnicode.GetBytes(chars), StartIndex, Length, Endian.Little);
            }
            else
            {
                Write(Encoding.Unicode.GetBytes(chars), StartIndex, Length, Endian.Little);
            }
        }

        public void Write7BitEncodedInt(int value)
        {
            Write7BitEncodedIntStream write7BitEncodedIntStream = new(OpenStream);
            write7BitEncodedIntStream.Write7BitEncodedInt(value);
            write7BitEncodedIntStream.Close();
        }
    }
}
