using FnImportExcel.Configuration;
using Google.Cloud.SecretManager.V1;

namespace FnImportExcel.Utils
{
    public class SecretManager
    {
        private readonly SecretManagerServiceClient _client;
        private readonly string? _secretPublicKeyName;
        private readonly string? _secretPrivateKeyName;
        private readonly string? _projectId;
        private Env _env;

        public string? PublicKey { get; private set; }
        public string? PrivateKey { get; private set; }

        public SecretManager()
        {
            _env = new Env();
            _secretPublicKeyName = _env.PublicKey;
            _secretPrivateKeyName = _env.PrivateKey;
            _projectId = _env.ProjectId;

            _client = SecretManagerServiceClient.Create();
        }

        public async Task InitializeSecretsAsync(string versionId = "1")
        {
            try
            {
                SecretVersionName secretPublicKey = new SecretVersionName(_projectId, _secretPublicKeyName, versionId);
                SecretVersionName secretPrivateKey = new SecretVersionName(_projectId, _secretPrivateKeyName, versionId);

                AccessSecretVersionResponse resultPublicKey = await _client.AccessSecretVersionAsync(secretPublicKey);
                AccessSecretVersionResponse resultPrivateKey = await _client.AccessSecretVersionAsync(secretPrivateKey);

                PublicKey = resultPublicKey.Payload.Data.ToStringUtf8();
                PrivateKey = resultPrivateKey.Payload.Data.ToStringUtf8();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
        }
    }
}