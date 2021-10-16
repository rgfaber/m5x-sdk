namespace M5x.Camunda.Acl
{
    public class CaseInstanceInfo
    {
        /// <summary>
        ///     A flag indicating whether the case instance is active or not.
        /// </summary>
        public bool Active;

        /// <summary>
        ///     The business key of the case instance.
        /// </summary>
        public string BusinessKey;

        /// <summary>
        ///     The id of the case definition that this case instance belongs to.
        /// </summary>
        public string CaseDefinitionId;

        /// <summary>
        ///     A flag indicating whether the case instance is completed or not.
        /// </summary>
        public bool Completed;

        /// <summary>
        ///     The id of the case instance.
        /// </summary>
        public string Id;

        /// <summary>
        ///     The tenant id of the case instance.
        /// </summary>
        public string TenantId;

        public override string ToString()
        {
            return Id;
        }
    }
}