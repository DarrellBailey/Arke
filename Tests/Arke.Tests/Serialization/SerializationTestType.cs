using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Tests.Serialization
{
    public class SerializationTestType
    {
        public byte Byte { get; set; }

        public sbyte SByte { get; set; }

        public char Char { get; set; }

        public short Short { get; set; }

        public ushort UShort { get; set; }

        public int Int { get; set; }

        public uint UInt { get; set; }

        public long Long { get; set; }

        public ulong ULong { get; set; }

        public float Float { get; set; }

        public double Double { get; set; }

        public bool Bool { get; set; }

        public decimal Decimal { get; set; }

        public DateTime DateTime { get; set; }

        public byte[] ByteArray { get; set; }

        public SerializationTestSubObjectType SubObject { get; set; }
    }

    public class SerializationTestSubObjectType
    {
        public byte Byte { get; set; }

        public sbyte SByte { get; set; }

        public char Char { get; set; }

        public short Short { get; set; }

        public ushort UShort { get; set; }

        public int Int { get; set; }

        public uint UInt { get; set; }

        public long Long { get; set; }

        public ulong ULong { get; set; }

        public float Float { get; set; }

        public double Double { get; set; }

        public bool Bool { get; set; }

        public decimal Decimal { get; set; }

        public DateTime DateTime { get; set; }

        public byte[] ByteArray { get; set; }
    }
}
