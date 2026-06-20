
namespace Tebex_RCON.Tebex
{
    /// <summary>
    /// TebexPlatform is a container class for the current plugin version and telemetry information about the current runtime.
    /// </summary>
    public class TebexPlatform
    {
        private String _pluginVersion;
        private TebexTelemetry _telemetry;
        public TebexPlatform(String pluginVersion, TebexTelemetry telemetry)
        {
            this._pluginVersion = pluginVersion;
            this._telemetry = telemetry;

        }
    
        public TebexTelemetry GetTelemetry()
        {
            return _telemetry;
        }

        public string GetPluginVersion()
        {
            return _pluginVersion;
        }
    }    
}
