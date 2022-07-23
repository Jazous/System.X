using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace System.X.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = System.IO.File.ReadAllBytes("E:\\3.jpg");

            //byte[] b1 = Fn.Image.ThumbW(bytes, 100, 80);
            //System.IO.File.WriteAllBytes("E:\\3_x1000_100.jpg", b1);

            //byte[] b2 = Fn.Image.ThumbH(bytes, 100, 80);
            //System.IO.File.WriteAllBytes("E:\\3_x1000_80.jpg", b2);

            //byte[] b3 = Fn.Image.Compress(bytes, 80);
            //System.IO.File.WriteAllBytes("E:\\3_ori_90.jpg", b3);

            //byte[] b4 = Fn.Image.Thumb(bytes, 100, 200, 90);
            //System.IO.File.WriteAllBytes("E:\\3_1.jpg", b4);

            //byte[] b5 = Fn.Image.Cut(bytes, 1200, 1200, 500, 500,  90);
            //System.IO.File.WriteAllBytes("E:\\3_2.jpg", b5);

            Diagnostics.Stopwatch watch=new Diagnostics.Stopwatch();
            watch.Start();
            byte[] b6 = Fn.Image.RevColor(bytes);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            System.IO.File.WriteAllBytes("E:\\3_006.jpg", b6);

            //byte[] b7 = Fn.Image.FilterBlue(bytes);
            //System.IO.File.WriteAllBytes("E:\\3_002.jpg", b7);

            //byte[] b8 = Fn.Image.FlipH(bytes);
            //System.IO.File.WriteAllBytes("E:\\3_003.jpg", b8);

            //byte[] b9 = Fn.Image.LD(bytes, 50);
            //System.IO.File.WriteAllBytes("E:\\3_004.jpg", b9);

            //byte[] bx = Fn.Image.DrawRect(bytes, 100, 100, 900, 900, SkiaSharp.SKColors.Red, 4);
            //System.IO.File.WriteAllBytes("E:\\3_005.jpg", bx);



            //var di = new System.IO.DirectoryInfo(@"D:\Downloads\dji_thermal_sdk_v1.1_20211029");
            //di.CopyTo(@"D:\generic");
            //watch.Start();
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedTicks);
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
        public DateTime CreateDate { get; set; }

        public Department Department { get; set; }

        public List<Role> Roles { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DepartmentDuty Duty { get; set; }

    }
    public class DepartmentDuty
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}