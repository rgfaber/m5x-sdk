namespace M5x.Camunda.Acl
{
    internal class UserCredentialsInfo
    {
        /// <summary>
        ///     The password of the current authenticated user who changes the password of the user.
        /// </summary>
        public string AuthenticatedUserPassword;

        /// <summary>
        ///     The user's new password.
        /// </summary>
        public string Password;
    }
}