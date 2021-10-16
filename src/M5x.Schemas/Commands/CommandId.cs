namespace M5x.Schemas.Commands
{
    public record CommandId : Identity<CommandId>, ICommandId
    {
        public CommandId(string value)
            : base(value)
        {
        }
    }
}