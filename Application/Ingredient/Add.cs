using Domain;
using MediatR;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Ingredient
{
    class Add
    {
        public class Command : MediatR.IRequest<Domain.Ingredient>
        {
            [Required]
            public string Name { get; set; }
            [Url]
            public string Photo { get; set; } = "";
        }

        public class Handler : IRequestHandler<Command, Domain.Ingredient>
        {
            public Handler(ElasticClient elasticClient)
            {
                ElasticClient = elasticClient;
            }

            public ElasticClient ElasticClient { get; }

            public Task<Domain.Ingredient> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
