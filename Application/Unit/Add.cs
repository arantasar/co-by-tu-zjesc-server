using MediatR;
using Persistence.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Unit
{
    public class Add
    {
        public class Command : IRequest<Domain.Unit>
        {
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Command, Domain.Unit>
        {
            public IUnitRepository UnitRepository { get; }
            public Handler(IUnitRepository unitRepository)
            {
                UnitRepository = unitRepository;
            }
            public async Task<Domain.Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await UnitRepository.IfExists(request.Name))
                {
                    throw new Exception("Record exists");
                }

                var unit = new Domain.Unit
                {
                    Name = request.Name
                };

                await UnitRepository.Add(unit);
                UnitRepository.Refresh();

                return unit;
            }
        }
    }
}
