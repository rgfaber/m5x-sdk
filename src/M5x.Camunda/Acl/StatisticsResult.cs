using System.Collections.Generic;

namespace M5x.Camunda.Acl
{
    public class StatisticsResult
    {
        /// <summary>
        ///     The total number of failed jobs for the running instances.
        /// </summary>
        public int FailedJobs;

        /// <summary>
        ///     The id of the process definition the results are aggregated for.
        /// </summary>
        public string Id;

        public List<IncidentStatisticsResult> Incidents;

        /// <summary>
        ///     The total number of running process instances of this process definition.
        /// </summary>
        public int Instances;

        public override string ToString()
        {
            return Id;
        }
    }
}