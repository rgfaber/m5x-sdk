using M5x.DEC.Core.VersionedTypes;

namespace M5x.DEC.Jobs
{
    public interface IJobDefinitionService : IVersionedTypeDefinitionService<JobVersionAttribute, JobDefinition>
    {
    }
}