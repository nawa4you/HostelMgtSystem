using System;
using System.Collections.Generic;
using BCrypt.Net; // Make sure this using directive is here

namespace PasswordHasher
{
    class class1
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating 20 Random 4-Digit Passwords and their BCrypt Hashes\n");
            Console.WriteLine("-----------------------------------------------------------\n");

            Random random = new Random();
            List<Tuple<string, string>> userCredentials = new List<Tuple<string, string>>();

            for (int i = 1; i <= 20; i++)
            {
                // Generate a random 4-digit number (0000-9999)
                int randomNumber = random.Next(0, 10000);
                string plaintextPassword = randomNumber.ToString("D4"); // Format to 4 digits with leading zeros

                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plaintextPassword, 11); // Cost factor 11

                userCredentials.Add(Tuple.Create(plaintextPassword, hashedPassword));

                Console.WriteLine($"User {i}:");
                Console.WriteLine($"  Plaintext Password: {plaintextPassword}");
                Console.WriteLine($"  Hashed Password:    {hashedPassword}\n");
            }

            Console.WriteLine("-----------------------------------------------------------\n");
            Console.WriteLine("Copy these hashes and update your HomeController.cs _users dictionary.");
            Console.WriteLine("Press any key to close this window.");
            Console.ReadKey();
        }
    }
}
