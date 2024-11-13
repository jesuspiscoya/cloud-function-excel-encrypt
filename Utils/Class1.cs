using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;  // Para MemoryStream y StreamWriter

public class HelloWorld
{
    private static RSA rsa = RSA.Create();

    // Cargar la clave pública RSA desde formato XML
    public static void LoadPublicKey(string publicKey)
    {
        try
        {
            rsa.FromXmlString(publicKey);  // Cargar la clave pública en formato XML
            Console.WriteLine("Clave publica cargada correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar la clave publica: {ex.Message}");
        }
    }

    // Cargar la clave privada RSA desde formato XML
    public static void LoadPrivateKey(string privateKey)
    {
        try
        {
            rsa.FromXmlString(privateKey);  // Cargar la clave privada en formato XML
            Console.WriteLine("Clave privada cargada correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar la clave privada: {ex.Message}");
        }
    }

    // Encriptar el texto usando la clave pública
    public static string EncryptNew(string plainText)
    {
        try
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);  // Usando PKCS1 como padding
            return Convert.ToBase64String(encryptedData);  // Devolver el resultado como cadena en Base64
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al encriptar: {ex.Message}");
            return null;
        }
    }

    // Desencriptar el texto usando la clave privada
    public static int? DecryptNew(string cipherText)  // Cambiar el tipo de retorno a 'int?' (nullable int)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText))  // Asegurarse de que el texto cifrado no esté vacío o nulo
            {
                Console.WriteLine("El texto cifrado es nulo o vacío.");
                return null;
            }

            byte[] dataToDecrypt = Convert.FromBase64String(cipherText);  // Convertir la cadena Base64 a bytes
            byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);  // Usando PKCS1 como padding
            return Convert.ToInt32(Encoding.UTF8.GetString(decryptedData));  // Convertir los datos desencriptados a texto y luego a int
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al desencriptar: {ex.Message}");
            return null;  // Devolver null en caso de error
        }
    }

    public static int DecryptOld(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return 0;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes("TuClaveSecreta123456".Substring(0, 16));
            aes.IV = Encoding.UTF8.GetBytes("TuIVSecreto123456".Substring(0, 16));

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return Convert.ToInt32(sr.ReadToEnd());
                    }
                }
            }
        }
    }

    public static void Main(string[] args)
    {
        // Llaves en formato XML generadas
        string publicKey = @"<?xml version=""1.0"" encoding=""utf-16""?><RSAKeyValue><Modulus>qGHN32wYy1mAFkjLiPRODvL+KPJqe4u8v9W7D5ZmjCAt0uTnMGRVe0qoGwCq/eOb1fDC6/JJ7a5eUak6fVv0AzSqOKWeVZ2DvXe8z4pQpowzvfDuiiCsebbQS5x8yM5Cq0KUg1dr+/cyDuNIfbwr8Nu4OLKWwFXU7LulHhaNv4k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        string privateKey = @"<?xml version=""1.0"" encoding=""utf-16""?><RSAKeyValue><Modulus>qGHN32wYy1mAFkjLiPRODvL+KPJqe4u8v9W7D5ZmjCAt0uTnMGRVe0qoGwCq/eOb1fDC6/JJ7a5eUak6fVv0AzSqOKWeVZ2DvXe8z4pQpowzvfDuiiCsebbQS5x8yM5Cq0KUg1dr+/cyDuNIfbwr8Nu4OLKWwFXU7LulHhaNv4k=</Modulus><Exponent>AQAB</Exponent><P>3lsRE463pQJf/CoiCR4vcov0WwyKQDkrZqVbTIg0Pur8fArlmbvZsxlbOHB/vybUt7MFyuzfzZZ0rLOtUxibgw==</P><Q>wdwSHp/TkBI3BXE64W1LLxZdo5/d3OHPxLx57ztIg2ynYetH9Lftia4ykVD41T/S+/tIA13p1NmEHd1xylfPAw==</Q><DP>edDqSgymD4B9lWh/vP8Mf3StKjR0iTrIzDNLEWKNPOr/5/UQVGjVm4kIsKLxWYesgfdR9RpQghErWnIVQrD4gQ==</DP><DQ>NKic22i6xSXNgFLzlYxkodPHd8zTLU7Ba0VvMca+ucRn6zdIAnd7tKx22/ZMwVBm9T+anh+ChujW9G3UnQwhFQ==</DQ><InverseQ>GCJ3rsmb+E5os64WhYfP9yMixM3S3aOL+SyqoIklgDWNtsscyTFEm1FMc31t6AVYoFfkuXL0ElgvanhjW33fqA==</InverseQ><D>MbkYC2cndz1JxsreV+YU+vMkvMDBUxRzu46I+9RlGfISthlkb/ThDVSBid/LnsfM6AN9/CEDVc3NCn5XyFcUGjdVOLtrArGOSSRPQkMN/ab6JI9YmiTFnUHdb9WmQl+umvNKkniTWzbFZrQ8G/ifmLfbHeMmz4YwRJ0nU38K+2U=</D></RSAKeyValue>";

        // Cargar claves públicas y privadas
        LoadPublicKey(publicKey);
        LoadPrivateKey(privateKey);

        // Encriptar un mensaje
        string plainText = "10003";
        var resultadoEncryptNew = EncryptNew(plainText);
        Console.WriteLine("Resultado encrypt new (RSA): " + resultadoEncryptNew);

        // Verificar que el texto cifrado no sea nulo antes de intentar desencriptarlo
        if (resultadoEncryptNew != null)
        {
            // Desencriptar el mensaje
            var resultadoDecryptNew = DecryptNew("W8evhMCtX2NVWcQn6ZGjiuIL4PLyy7nybDl8o8o6GGaht1ePB200RrPEG+FVIXlwGgUoTl+61HM4dKufrOAHmKTxNuVBQU0do3lK7WKFrL+48JCut975SON2V442sEhsdwpzWT3SavAsiH/lEMYqBG66GCJhksIobW99CfWSSR4=");
            if (resultadoDecryptNew.HasValue)
            {
                Console.WriteLine("Resultado decrypt new (RSA): " + resultadoDecryptNew.Value);
            }
            else
            {
                Console.WriteLine("Error al desencriptar el mensaje.");
            }
        }
        else
        {
            Console.WriteLine("No se pudo encriptar el mensaje.");
        }
    }
}