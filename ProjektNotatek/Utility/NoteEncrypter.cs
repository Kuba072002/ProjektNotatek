using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ProjektNotatek.Utility {
    public class NoteEncrypter {
        private static readonly byte[] key = Encoding.ASCII.GetBytes("Hdu8AFUnxFcfTm8WMyWI56XE12SJFLUm");
        public static string Encrypt(string plain) {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

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

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string cipher) {
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
    }
}
