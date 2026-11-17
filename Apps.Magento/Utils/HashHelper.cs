using System.Security.Cryptography;
using System.Text;

namespace Apps.Magento.Utils;

public static class HashHelper
{
    public static string Sha256(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        return Convert.ToHexString(SHA256.HashData(contentBytes));
    }
}