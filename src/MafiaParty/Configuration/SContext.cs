using MafiaParty.Logging;
using LiteDB;

namespace MafiaParty {
    public interface ISContext {
        SConfiguration configuration { get; }
        SLogger log { get; }
        LiteDatabase database { get; }
    }

    public class SContext : ISContext {
        public SConfiguration configuration { get; }
        public SLogger log { get; }
        public LiteDatabase database { get; private set; }

        public SContext(SConfiguration config) {
            configuration = config;
            log = new SLogger(configuration.logLevel);
        }

        public void connectDatabase() {
            // Create database
            database = new LiteDatabase(configuration.dbFile);
        }
    }
}