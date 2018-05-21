namespace MafiaParty.Configuration {
    public class DependencyObject {
        public DependencyObject(ISContext context) {
            serverContext = context;
        }

        public ISContext serverContext { get; }
    }
}