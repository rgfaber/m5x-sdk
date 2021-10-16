using System;
using System.Collections.Generic;

namespace M5x.DEC.Core.VersionedTypes
{
    public interface IVersionedTypeDefinitionService<TAttribute, TDefinition>
    {
        void Load(IReadOnlyCollection<Type> types);
        IEnumerable<TDefinition> GetDefinitions(string name);
        bool TryGetDefinition(string name, int version, out TDefinition definition);
        IEnumerable<TDefinition> GetAllDefinitions();
        TDefinition GetDefinition(string name, int version);
        TDefinition GetDefinition(Type type);
        IReadOnlyCollection<TDefinition> GetDefinitions(Type type);
        bool TryGetDefinition(Type type, out TDefinition definition);
        bool TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions);
        void Load(params Type[] types);
    }
}