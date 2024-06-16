using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.data
{
    public class Hash
    {
        public static string Hashpassword(string password)
        {
            string ps = string.Empty;
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            ps = builder.ToString();
            return ps;
        }
    }
}