using Dgraph;
using Serilog;

namespace M5x.DGraph
{
    internal class DGraphClientBuilder : IDGraphClientBuilder
    {
        private readonly ILogger _logger;

        public DGraphClientBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public IGraphClient BuildClient()
        {
            return new GraphClient(new DgraphClient(), _logger);
        }
    }
}