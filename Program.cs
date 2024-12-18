using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using FnImportExcel.Configuration;
using FnImportExcel.Utils;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.Json;

namespace FnImportExcel;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        System.Console.WriteLine("\nINIT\n");

        Env _env = new Env();

        // Ruta del archivo Excel
        string sourcePath = _env.SourcePath;
        string fileName= _env.FileName;
        string sheetName= _env.SheetName;

        // Conexión a la base de datos MySQL
        string connectionString = new Env().DBServer;

        SecretManager _secretManager = new SecretManager();

        await _secretManager.InitializeSecretsAsync();

        EncryptionUtil encryptionUtil = new EncryptionUtil(_secretManager);

        #region read excel and write data on database

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                // Abrir conexión a MySQL
                conn.Open();
                
                // Abrir el archivo Excel
                using (var workbook = new XLWorkbook(sourcePath + fileName))
                {
                    // Obtener la primera hoja de trabajo
                    var worksheet = workbook.Worksheet(sheetName);

                    // Obtener los encabezados de la primera fila
                    string query = "UPDATE BudgetDetail SET Salary = @col1 WHERE EmployeeId = @col2";

                    foreach (var row in worksheet.RowsUsed().Skip(3))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            string employeeId = row.Cell(6).GetValue<string>();
                            string salary = row.Cell(15).GetValue<string>();

                            // Encriptar el contenido
                            string? salaryEncrypted = encryptionUtil.Encrypt(salary);
                            cmd.Parameters.AddWithValue($"@col1", salaryEncrypted);
                            cmd.Parameters.AddWithValue($"@col2", employeeId);

                            // Ejecutar la consulta
                            cmd.ExecuteNonQuery();

                            Console.WriteLine($"Fila insertada correctamente: [{employeeId}, {salary}]");
                        }
                    }
                }

                //Desencriptar salarios
                /*
                string query2 = "SELECT EmployeeId, Salary FROM BudgetDetail LIMIT 3";
                using (MySqlCommand cmd = new MySqlCommand(query2, conn))
                {
                    // Ejecutar la consulta y obtener el DataReader
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Leer los resultados
                        while (reader.Read())
                        {
                            // Obtener los datos de las columnas por nombre
                            string id = reader.GetString("EmployeeId");
                            string salary = reader.GetString("Salary");

                            string? salaryDecrypted = encryptionUtil.Decrypt(reader.GetString("Salary"));

                            // Mostrar los resultados antes de desencriptar
                            Console.WriteLine("\nANTES DE DESENCRIPTAR:");
                            Console.WriteLine($"EmployeeId: {id}");
                            Console.WriteLine($"Salary (RSA): {salary}");

                            Console.WriteLine("\nDESPUES DE DESENCRIPTAR:");
                            if (!string.IsNullOrEmpty(salaryDecrypted))
                            {
                                Console.WriteLine($"Resultado Salary: {salaryDecrypted}");
                            }
                            else
                            {
                                Console.WriteLine("Error al desencriptar el contenido.");
                            }
                            Console.WriteLine("--------------------------------------------------------------------------");
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

        System.Console.WriteLine("\nEND\n");
    }
}
