using MafiaParty.Logging;

namespace MafiaParty {
    public class SConfiguration {
        public string name = "MafiaParty";
        public SLogger.LogLevel logLevel = SLogger.LogLevel.Information;
        public string dbFile = "mfp.lidb";
        public string[] corsOrigins = new string[0];
    }
}