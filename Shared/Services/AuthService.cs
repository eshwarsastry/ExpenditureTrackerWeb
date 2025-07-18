﻿using System.Security.Cryptography;
using System.Text;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface IAuthService
    {
        public string ComputeHash(string password);
    }

    public class AuthService: IAuthService
    {
        public string ComputeHash(string password)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }


}
