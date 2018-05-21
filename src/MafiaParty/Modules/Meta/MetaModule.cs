using MafiaParty.Utilities;
using Nancy;

namespace MafiaParty.Modules.Meta {
    public class MetaModule : SBaseModule {
        public MetaModule(ISContext serverContext) : base("/meta", serverContext) {
            Get("/", _ => {
                return Response.asJsonNet(new {
                    name = serverContext.configuration.name
                });
            });
        }
    }
}