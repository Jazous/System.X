using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.X.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //IEnumerable<string> dd = new List<string>() { "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "b", "c", "1", "2", "3", "a", "b", "c", "1", "2", "3", "a", "b", "c", "Z" };

            List<UserEntity> userList = new List<UserEntity>();
            userList.Add(new UserEntity() { Id = 1, Name = "a", CreateDate = DateTime.Now, Roles = new List<Role>() { new Role() { Id = 1, Name = "超级管理员" } }, Department = new Department() { Id = 1, Name = "研发部", Duty = new DepartmentDuty() { Id = 1 } } });
            userList.Add(new UserEntity() { Id = 2, Name = "b", CreateDate = DateTime.Now, Roles = new List<Role>() { new Role() { Id = 2, Name = "管理员" } }, Department = new Department() { Id = 1, Name = "研发部", Duty = new DepartmentDuty() { Id = 1 } } });
            userList.Add(new UserEntity() { Id = 3, Name = "c", CreateDate = DateTime.Now, Roles = new List<Role>() { new Role() { Id = 2, Name = "管理员" } }, Department = new Department() { Id = 2, Name = "人事部", Duty = new DepartmentDuty() { Id = 2 } } });
            userList.Add(new UserEntity() { Id = 4, Name = "d", CreateDate = DateTime.Now, Roles = new List<Role>() { new Role() { Id = 2, Name = "管理员" } }, Department = new Department() { Id = 2, Name = "人事部", Duty = new DepartmentDuty() { Id = 3 } } });
            userList.Add(new UserEntity() { Id = 5, Name = "e", CreateDate = DateTime.Now, Roles = new List<Role>() { new Role() { Id = 2, Name = "管理员" } }, Department = new Department() { Id = 3, Name = "财务部", Duty = new DepartmentDuty() { Id = 3 } } });
            var query = userList.AsQueryable();

            var predicate = Fn.Expression.Equal<UserEntity>("Department.Duty.Id", "3");


            var data = query.Where(predicate).ToList();

            //dd.Contains("z", true);
            //var watch = new Diagnostics.Stopwatch();
            //watch.Start();
            //dd.Contains("z", true);
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