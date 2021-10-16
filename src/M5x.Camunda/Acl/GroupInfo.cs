﻿namespace M5x.Camunda.Acl
{
    public class GroupInfo
    {
        /// <summary>
        ///     The id of the group
        /// </summary>
        public string Id;

        /// <summary>
        ///     The name of the group.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The type of the group.
        /// </summary>
        public string Type;

        public override string ToString()
        {
            return Id;
        }
    }
}