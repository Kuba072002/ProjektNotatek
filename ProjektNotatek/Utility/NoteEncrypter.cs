using ProjektNotatek.Data;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ProjektNotatek.Utility {
    public class NoteEncrypter {
        //private static readonly byte[] key = Encoding.ASCII.GetBytes("Hdu8AFUnxFcfTm8WMyWI56XE12SJFLUm");
        private static CustomPasswordHasher passwordHasher = new();
        public static void Encrypt(string content,string password,out string encodedcontent,out string encodedpassword) {
            byte[] hashedPassword = passwordHasher.HashMethod(password, 10000);
            encodedpassword = Convert.ToBase64String(hashedPassword);
            byte[] key = new byte[32];
            Buffer.BlockCopy(hashedPassword, 8, key, 0, 32);
            // byte[] key2 = Encoding.ASCII.GetBytes("Hdu8AFUnxFcfTm8WMyWI56XE12SJFLUm");

            byte[] plainBytes = Encoding.UTF8.GetBytes(content);

            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                                     ? stackalloc byte[encryptedDataLength]
                                     : new byte[encryptedDataLength].AsSpan();

            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            RandomNumberGenerator.Fill(nonce);

            using var aes = new AesGcm(key);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            encodedcontent = Convert.ToBase64String(encryptedData);
            //return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string cipher, string password) {
            //byte[] key = Encoding.ASCII.GetBytes(password);
            byte[] hashedPassword = Convert.FromBase64String(password);
            byte[] key = new byte[32];
            Buffer.BlockCopy(hashedPassword, 8, key, 0, 32);

            Span<byte> encryptedData = Convert.FromBase64String(cipher).AsSpan();

            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            Span<byte> plainBytes = cipherSize < 1024
                                  ? stackalloc byte[cipherSize]
                                  : new byte[cipherSize];
            using var aes = new AesGcm(key);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public static string CheckPasswordQuality(string password)
        {
            bool hasLowerLetter = false;
            bool hasUpperLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            int poolSize = 0;
            if (password == null || password.Length == 0)
                return "Haslo nie moze byc puste";
            foreach (char c in password)
            {
                if (char.IsLower(c)) 
                    hasLowerLetter = true;
                else if (char.IsUpper(c))
                    hasUpperLetter = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else
                    hasSpecialChar = true;
            }

            if (hasLowerLetter)
                poolSize += 26;
            if (hasUpperLetter)
                poolSize += 26;
            if (hasDigit)
                poolSize += 10;
            if (hasSpecialChar)
                poolSize += 32;
            
            double entropy = password.Length * Math.Log2(poolSize);
            if (entropy < 25)
                return("Bardzo słabe hasło" + entropy.ToString("0.##"));
            if (entropy < 40)
                return("Hasło nie jest wystarczajaco mocne " + entropy.ToString("0.##"));
            else
                return "ok";
        }
    }
}
