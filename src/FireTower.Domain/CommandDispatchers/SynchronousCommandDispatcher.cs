using System.Collections.Generic;
using System.Linq;
using BlingBag;
using FireTower.Domain.Exceptions;

namespace FireTower.Domain.CommandDispatchers
{
    public class SynchronousCommandDispatcher : ICommandDispatcher
    {
        readonly IBlingInitializer<DomainEvent> _blingInitializer;
        readonly IEnumerable<ICommandHandler> _handlers;

        public SynchronousCommandDispatcher(IBlingInitializer<DomainEvent> blingInitializer, IEnumerable<ICommandHandler> handlers)
        {
            _blingInitializer = blingInitializer;
            _handlers = handlers;
        }

        public void Dispatch(object command)
        {
            var commandHandlers = _handlers.Where(commandHandler => commandHandler.CommandType == command.GetType()).ToList();
            if (!commandHandlers.Any()) throw new NoAvailableHandlerException(command.GetType());

            foreach (var commandHandler in commandHandlers)
            {
                _blingInitializer.Initialize(commandHandler);
                commandHandler.Handle(command);
            }
        }
    }
}