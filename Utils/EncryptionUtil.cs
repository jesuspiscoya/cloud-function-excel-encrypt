using System.Security.Cryptography;
using System.Text;

namespace FnImportExcel.Utils
{
    public class EncryptionUtil
    {
        private static RSA rsa = RSA.Create();

        public EncryptionUtil(SecretManager secretManager)
        {
            // Verificar si el SecretManager tiene la clave pública y privada
            if (string.IsNullOrEmpty(secretManager.PublicKey) || string.IsNullOrEmpty(secretManager.PrivateKey))
            {
                throw new InvalidOperationException("SecretManager debe contener claves pública y privada.");
            }

            // Cargar las claves pública y privada desde el SecretManager
            LoadPublicKey(secretManager.PublicKey);
            LoadPrivateKey(secretManager.PrivateKey);
        }

        // Cargar la clave pública RSA desde el SecretManager
        public void LoadPublicKey(string publicKey)
        {
            try
            {
                string correctedPublicKey = publicKey.Replace("\"\"", "\"");
                rsa.FromXmlString(correctedPublicKey);  // Cargar la clave pública en formato XML
                Console.WriteLine("Clave pública cargada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clave pública: {ex.Message}");
            }
        }

        // Cargar la clave privada RSA desde el SecretManager
        public void LoadPrivateKey(string privateKey)
        {
            try
            {
                string correctedPrivateKey = privateKey.Replace("\"\"", "\"");
                rsa.FromXmlString(correctedPrivateKey);
                Console.WriteLine("Clave privada cargada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clave privada: {ex.Message}");
            }
        }

        // Encriptar el texto usando la clave pública RSA
        public string? Encrypt(string plainText)
        {
            try
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);  // PKCS1 como padding
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al encriptar: {ex.Message}");
                return null;
            }
        }

        // Desencriptar el texto usando la clave privada RSA
        public String? Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText))
                {
                    Console.WriteLine("El texto cifrado es nulo o vacío.");
                    return null;
                }

                byte[] dataToDecrypt = Convert.FromBase64String(cipherText);
                byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);  // PKCS1 como padding

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desencriptar: {ex.Message}");
                return null;
            }
        }

        public int DecryptList(string concatenatedAmounts)
        {
            int totalAmount = 0;
            if (string.IsNullOrEmpty(concatenatedAmounts)) return 0;
            string[] encriptedAmounts = concatenatedAmounts.Split(',');
            foreach (var encriptedAmount in encriptedAmounts)
            {
                totalAmount += int.Parse(Decrypt(encriptedAmount)!);
            }
            return totalAmount;
        }
    }
}