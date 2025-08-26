using System.Security.Cryptography;
using System.Text;

namespace Venice.Teste.Backend.Application.Helpers.Hash
{
    public class HashGenerator
    {
        public static string GenerateHash(Guid guid1, Guid guid2, Guid guid3)
        {
            var combined = $"{guid1}{guid2}{guid3}";

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(combined);
                var hashBytes = sha256.ComputeHash(bytes);

                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
    }
}