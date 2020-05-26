using MediatR;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Unit
{
    public class Get
    {
        public class Query : MediatR.IRequest<Domain.Unit>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Domain.Unit>
        {
            public IUnitRepository UnitRepository { get; }

            public Handler(IUnitRepository unitRepository)
            {
                UnitRepository = unitRepository;
            }

            public async Task<Domain.Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                return await UnitRepository.Get(request.Id);
            }
        }


    }
}
