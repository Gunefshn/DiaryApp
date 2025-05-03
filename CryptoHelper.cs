namespace DiaryApp;
using System.Security.Cryptography;

public static class CryptoHelper
{
    //private const string password = "maya123";
    public static string Encrypt(string plainText, string password)
    {
        // Rastgele salt (16 byte)
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        // Rastgele IV (AES blok boyutu = 16 byte)
        byte[] iv = RandomNumberGenerator.GetBytes(16);

        // Key üretimi
        using var keyGen = new Rfc2898DeriveBytes(password, salt, 100_000);
        byte[] key = keyGen.GetBytes(32); // 256-bit AES key

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var ms = new MemoryStream();

        // Başta salt + iv'yi yaz (ilk 32 byte = 16 salt + 16 iv)
        ms.Write(salt, 0, salt.Length);
        ms.Write(iv, 0, iv.Length);

        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string encryptedText, string password)
    {
        byte[] fullData = Convert.FromBase64String(encryptedText);

        // İlk 16 byte salt, sonraki 16 byte IV
        byte[] salt = new byte[16];
        byte[] iv = new byte[16];
        Array.Copy(fullData, 0, salt, 0, 16);
        Array.Copy(fullData, 16, iv, 0, 16);

        // Geriye kalanlar şifreli veri
        byte[] cipherBytes = new byte[fullData.Length - 32];
        Array.Copy(fullData, 32, cipherBytes, 0, cipherBytes.Length);

        using var keyGen = new Rfc2898DeriveBytes(password, salt, 100_000);
        byte[] key = keyGen.GetBytes(32);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}