using MediatR;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Unit
{
    public class List
    {
        public class Query : MediatR.IRequest<IEnumerable<Domain.Unit>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<Domain.Unit>>
        {
            public Handler(ElasticClient elasticClient)
            {
                ElasticClient = elasticClient;
            }

            public ElasticClient ElasticClient { get; }

            public async Task<IEnumerable<Domain.Unit>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await ElasticClient.SearchAsync<Domain.Unit>(item => item
                    .Index("units")
                    .MatchAll()
                    .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
                    );

                return new List<Domain.Unit>(result.Documents);
            }
        }
    }
}
