using Elasticsearch.Net;
using MediatR;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Unit
{
    public class Delete
    {
        public class Command : MediatR.IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : MediatR.IRequestHandler<Command>
        {
            public Handler(ElasticClient elasticClient)
            {
                ElasticClient = elasticClient;
            }

            public ElasticClient ElasticClient { get; }

            public async Task<MediatR.Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var item = await ElasticClient.DocumentExistsAsync<Domain.Unit>(request.Id, selector => selector.Index("units"));
                if (!item.Exists)
                {
                    throw new Exception("Item not found");
                }

                var result = await ElasticClient.DeleteAsync<Domain.Unit>(request.Id, selector => selector.Index("units"));
                ElasticClient.LowLevel.Indices.Refresh<StringResponse>("units");


                if (!result.IsValid)
                {
                    throw new Exception("Error deleting item");
                }

                return MediatR.Unit.Value;
            }
        }
    }
}
