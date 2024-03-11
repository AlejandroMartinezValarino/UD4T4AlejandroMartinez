using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UD4T4AlejandroMartinez.MVVM.Models
{
    public class Encrypt
    {
        /// <summary>
        /// Calcula el hash SHA256 de una cadena de texto.
        /// </summary>
        /// <param name="str">La cadena de texto para la cual se calculará el hash.</param>
        /// <returns>El hash SHA256 como una cadena de texto hexadecimal.</returns>
        public static string GetSha256(string str)
        {
            SHA256 sHA = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sHA.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }
    }
}
