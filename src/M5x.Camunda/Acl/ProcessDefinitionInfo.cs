﻿namespace M5x.Camunda.Acl
{
    public class ProcessDefinitionInfo
    {
        /// <summary>
        ///     The category of the process definition.
        /// </summary>
        public string Category;

        /// <summary>
        ///     The deployment id of the process definition.
        /// </summary>
        public string DeploymentId;

        /// <summary>
        ///     The description of the process definition.
        /// </summary>
        public string Description;

        /// <summary>
        ///     The file name of the process definition diagram, if it exists.
        /// </summary>
        public string Diagram;

        /// <summary>
        ///     The id of the process definition.
        /// </summary>
        public string Id;

        /// <summary>
        ///     The key of the process definition, i.e. the id of the BPMN 2.0 XML process definition.
        /// </summary>
        public string Key;

        /// <summary>
        ///     The name of the process definition.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The file name of the process definition.
        /// </summary>
        public string Resource;

        /// <summary>
        ///     A flag indicating whether the definition is suspended or not.
        /// </summary>
        public bool Suspended;

        /// <summary>
        ///     The tenant id of the process definition.
        /// </summary>
        public string TenantId;

        /// <summary>
        ///     The version of the process definition that the engine assigned to it.
        /// </summary>
        public int Version;

        /// <summary>
        ///     The version tag of the process definition.
        /// </summary>
        public string VersionTag;

        public override string ToString()
        {
            return Id;
        }
    }
}