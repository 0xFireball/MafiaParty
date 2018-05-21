using MafiaParty.Logging;
using LiteDB;

namespace MafiaParty {
    public class SContext {
        public SConfiguration configuration;
        public SLogger log;
        public LiteDatabase database;

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