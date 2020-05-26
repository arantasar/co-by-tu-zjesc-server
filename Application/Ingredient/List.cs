using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Nest;

namespace Application.Ingredient
{
    public class List
    {
        public class Query : MediatR.IRequest<IEnumerable<Domain.Ingredient>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<Domain.Ingredient>>
        {
            public Handler(ElasticClient elasticClient)
            {
                ElasticClient = elasticClient;
            }

            public ElasticClient ElasticClient { get; }

            public async Task<IEnumerable<Domain.Ingredient>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await ElasticClient.SearchAsync<Domain.Ingredient>(item => item.Index("ingredients").MatchAll());

                return new List<Domain.Ingredient>(result.Documents);
            }
        }
    }
}
