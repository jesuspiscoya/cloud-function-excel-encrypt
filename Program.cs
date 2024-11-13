using ClosedXML.Excel;
using FnImportExcel.Configuration;
using FnImportExcel.Utils;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace FnImportExcel;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        System.Console.WriteLine("\nINIT\n");

        // Ruta del archivo Excel
        string filePath = @"C:\Users\jpiscoya\Desktop\Libro1.xlsx";

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
                using (var workbook = new XLWorkbook(filePath))
                {
                    // Obtener la primera hoja de trabajo
                    var worksheet = workbook.Worksheet(1);

                    // Obtener los encabezados de la primera fila
                    List<String> columns = worksheet.Row(1).CellsUsed().Select(cell => cell.Value.ToString()).ToList();
                    List<List<String>> rows = worksheet.RowsUsed().Skip(1).Select(row => row.Cells().Select(cell => cell.Value.ToString()).ToList()).ToList();
                    List<string> stmnCols = Enumerable.Range(0, columns.Count).Select(i => $"@col{i}").ToList();
                    string query = $"INSERT INTO cobertura ({string.Join(", ", columns)}) VALUES({string.Join(", ", stmnCols)})";

                    foreach (var row in rows)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            for (int i = 0; i < row.Count; i++)
                            {
                                // Encriptar el contenido
                                string? valueEncrypted = encryptionUtil.Encrypt(row[i]);
                                cmd.Parameters.AddWithValue($"@col{i}", valueEncrypted);
                            }

                            // Ejecutar la consulta
                            cmd.ExecuteNonQuery();

                            Console.WriteLine($"Fila insertada correctamente: {JsonSerializer.Serialize(row)}");
                        }
                    }
                }

                string query2 = "SELECT EmployeeId, Salary FROM BudgetDetail";
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
                            string budget = reader.GetString("BudgetDetail");

                            string? idDecrypted = encryptionUtil.Decrypt(reader.GetString("EmployeeId"));
                            string? budgetDecrypted = encryptionUtil.Decrypt(reader.GetString("BudgetDetail"));

                            // Mostrar los resultados antes de desencriptar
                            Console.WriteLine("\nANTES DE DESENCRIPTAR:");
                            Console.WriteLine($"Distribuidora (RSA): {id}");
                            Console.WriteLine("--------------------------------------------------------------------------");
                            Console.WriteLine($"IDGrupo (RSA): {budget}");
                            Console.WriteLine("--------------------------------------------------------------------------");

                            Console.WriteLine("\nDESPUES DE DESENCRIPTAR:");
                            if (!string.IsNullOrEmpty(idDecrypted))
                            {
                                Console.WriteLine($"Resultado Distribuidora: {idDecrypted}");
                            }
                            if (!string.IsNullOrEmpty(budgetDecrypted))
                            {
                                Console.WriteLine($"Resultado IDGrupo: {budgetDecrypted}");
                            }
                            else
                            {
                                Console.WriteLine("Error al desencriptar el contenido.");
                            }
                        }
                    }
                }
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
