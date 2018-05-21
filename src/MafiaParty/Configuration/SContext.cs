using MafiaParty.Logging;
using LiteDB;
using System.Collections.Generic;

namespace MafiaParty {
    public interface ISContext {
        SConfiguration configuration { get; }
        SLogger log { get; }
        LiteDatabase database { get; }
        List<Room> rooms { get; }
    }

    public class SContext : ISContext {
        public SConfiguration configuration { get; }
        public SLogger log { get; }
        public LiteDatabase database { get; private set; }
        public List<Room> rooms { get; }

        public SContext(SConfiguration config) {
            configuration = config;
            log = new SLogger(configuration.logLevel);
            rooms = new List<Room>();
        }

        public void connectDatabase() {
            // Create database
            database = new LiteDatabase(configuration.dbFile);
        }
    }
}