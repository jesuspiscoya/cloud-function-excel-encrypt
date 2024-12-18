namespace FnImportExcel.Configuration
{
    public class Env
    {
        public Env()
        {
            string value = null;

            value = Environment.GetEnvironmentVariable("PATH_SOURCE") ?? @"";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("PATH_SOURCE", "PATH_SOURCE no está configurado.");
            }
            SourcePath = value;

            value = Environment.GetEnvironmentVariable("FILE_NAME") ?? "";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("FILE_NAME", "FILE_NAME no está configurado.");
            }
            FileName = value;
            
            value = Environment.GetEnvironmentVariable("SHEET_NAME") ?? "";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("SHEET_NAME", "SHEET_NAME no está configurado.");
            }
            SheetName = value;

            value = Environment.GetEnvironmentVariable("DB_SERVER") ?? "";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("DB_SERVER", "DB_SERVER no está configurado.");
            }
            DBServer = value;

            value = Environment.GetEnvironmentVariable("PUBLIC_KEY_NAME") ?? "PUBLIC_KEY";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("PUBLIC_KEY_NAME", "PUBLIC_KEY_NAME no está configurado.");
            }
            PublicKey = value;

            value = Environment.GetEnvironmentVariable("PRIVATE_KEY_NAME") ?? "PRIVATE_KEY";
                if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("PRIVATE_KEY_NAME", "PRIVATE_KEY_NAME no está configurado.");
            }
            PrivateKey = value;

            value = Environment.GetEnvironmentVariable("PROJECT_ID") ?? "head-count-uat";
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("PROJECT_ID", "PROJECT_ID no está configurado.");
            }
            ProjectId = value;
        }

        public string SourcePath { get; set; }
        public string FileName { get; set; }
        public string SheetName { get; set; }
        public string DBServer { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string ProjectId { get; set; }
    }
}
