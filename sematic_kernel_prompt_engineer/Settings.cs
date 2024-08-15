

using Microsoft.Extensions.Configuration;

namespace sematic_kernel_prompt_engineer
{
    public class Settings
    {
        public static (string apiKey, string? orgId) LoadFromSecrets()
        {
            IConfiguration config = new ConfigurationBuilder()
                       .AddUserSecrets<Settings>()
                       .Build();

            try
            {

                string? apiKey = string.IsNullOrEmpty(config.GetValue<string>("apiKey")) ? null : config.GetValue<string>("apiKey");
                string? orgId = config.GetValue<string>("orgId") ?? null;
                return (apiKey, orgId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
                return ("", "");
            }
        }
    }
}
