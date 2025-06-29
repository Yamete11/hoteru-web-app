using System;
using Microsoft.AspNetCore.Identity;
using hoteru_be.Entities;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var hasher = new PasswordHasher<User>();
            Console.WriteLine("Hashed 'zxc':");
            Console.WriteLine(hasher.HashPassword(null, "zxc"));
            Console.WriteLine("Hashed 'qaz':");
            Console.WriteLine(hasher.HashPassword(null, "qaz"));
            Console.WriteLine("Hashed 'jan':");
            Console.WriteLine(hasher.HashPassword(null, "jan"));
        }
    }
}
