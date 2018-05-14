using System;
using System.Collections.Generic;
using System.Text;

namespace Arke.Net.Tests
{
    public static class TestData
    {
        public static byte[] TestBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static string TestString = "Hello World!";
        public static int TestInt = 12345678;
        public static double TestDouble = 1234.5678;
        public static TestObject TestObject = new TestObject { TestBytes = TestBytes, TestDouble = TestDouble, TestInteger = TestInt, TestString = TestString };

    }

    [Serializable]
    public class TestObject
    {
        public string TestString { get; set; }
        public byte[] TestBytes { get; set; }
        public int TestInteger { get; set; }
        public double TestDouble { get; set; }
    }
}
