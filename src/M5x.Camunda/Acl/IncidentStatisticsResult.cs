namespace M5x.Camunda.Acl
{
    public class IncidentStatisticsResult
    {
        /// <summary>
        ///     The total number of incidents for the corresponding incident type.
        /// </summary>
        public int IncidentCount;

        /// <summary>
        ///     The type of the incident the number of incidents is aggregated for.
        /// </summary>
        public string IncidentType;

        public override string ToString()
        {
            return $"{IncidentType} ({IncidentCount})";
        }
    }
}