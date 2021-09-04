using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;

namespace System.X.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
           // byte[] buffer = System.IO.File.ReadAllBytes(@"D:\dataset\精细\DJI_0001.JPG");
            //byte[] res= Fn.Image.Thumbnail(buffer, 400, 300);
            //byte[] res2 = Fn.Image.Thumbnail(buffer, 0.5f);
            //System.IO.File.WriteAllBytes(@"D:\dataset\精细\DJI_0001_0001.JPG", res2);
            // string tt = Base32Encoding.ToString(Fn.UTF8.GetBytes("20210902204100000"));
            //string cc=  Fn.UTF8.GetString(Base32Encoding.ToBytes(tt));
            return;
            //0、1、1 - 1、2批次、10、10批次
            //List<string> dataList = new List<string>() { "c", "0", "1", "e", "1-1", "10", "2", "2批次", "1批次", "1 c", "a", "1-大", "11", "11 批次", "11-01", "24", "10批次", "d", "a2", " " };
            //List<string> dataList = new List<string>() { "1", "1-1", "2", "2批次", "1批次", "1 c", " " };
            var watch = new Diagnostics.Stopwatch();
            watch.Start();
            watch.Stop();
            //foreach (string data in dataList)
            //{
            //    Console.WriteLine(txt);
            //}
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
    public class Base32Encoding
    {
        public static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('='); //remove padding characters
            int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            int mask = 0, arrayIndex = 0;

            foreach (char c in input)
            {
                int cValue = CharToValue(c);

                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        public static string ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException("input");
            }

            int charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
            char[] returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            int arrayIndex = 0;

            foreach (byte b in input)
            {
                nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ValueToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            //if we didn't end with a full char
            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ValueToChar(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '='; //padding
            }

            return new string(returnArray);
        }

        private static int CharToValue(char c)
        {
            int value = (int)c;

            //65-90 == uppercase letters
            if (value < 91 && value > 64)
            {
                return value - 65;
            }
            //50-55 == numbers 2-7
            if (value < 56 && value > 49)
            {
                return value - 24;
            }
            //97-122 == lowercase letters
            if (value < 123 && value > 96)
            {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        private static char ValueToChar(byte b)
        {
            if (b < 26)
            {
                return (char)(b + 65);
            }

            if (b < 32)
            {
                return (char)(b + 24);
            }

            throw new ArgumentException("Byte is not a value Base32 value.", "b");
        }

    }

    public class UserEntity
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }
}