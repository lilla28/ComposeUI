using MorganStanley.ComposeUI.Logging.Entity;

namespace ProjectLogDotNet6
{
    public class ExaD
    {
        private readonly ILogger _logger = LoggerManager.GetCurrentClassLogger<ExaD>();

        internal ExaD() { }

        internal void DoSmtng()
        {
            _logger.LogInformation("Testing from applpication .NET6 ASP.NETCORE vol 1");
            Thread.Sleep(2000);
            _logger.LogWarning("Testing from applpication .NET6 ASP.NETCORE vol 2");
        }
    }
}
