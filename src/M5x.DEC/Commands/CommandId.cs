using M5x.DEC.Schema;

namespace M5x.DEC.Commands
{
    public record CommandId : Identity<CommandId>, ICommandId
    {
        public CommandId(string value)
            : base(value)
        {
        }
    }
}