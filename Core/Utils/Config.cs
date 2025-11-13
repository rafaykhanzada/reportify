using Microsoft.Extensions.Configuration;

namespace Core.Utils
{
    public static class Config
    {
        public static string env { get; set; }
        public static IConfiguration config { get; set; }
    }
}
