namespace M5x.Camunda.Acl
{
    public class IdentityVerifiedUser
    {
        /// <summary>
        ///     Verification successful or not
        /// </summary>
        public bool Authenticated;

        /// <summary>
        ///     The id of the user
        /// </summary>
        public string AuthenticatedUser;
    }
}