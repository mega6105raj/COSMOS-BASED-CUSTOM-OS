using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CosmosKernel1
{
    public static class UserManager
    {
        private static Dictionary<string, string> users = new Dictionary<string, string>
    {
        { "admin", SimpleHash("password123") }
    };

        public static bool Authenticate(string username, string password)
        {
            if (users.ContainsKey(username) && users[username] == SimpleHash(password))
            {
                return true;
            }
            return false;
        }

        private static string SimpleHash(string input)
        {
            int hash = 0;
            foreach (char c in input)
            {
                hash += c;
                hash *= 17;
            }
            return hash.ToString();
        }
    }
}
