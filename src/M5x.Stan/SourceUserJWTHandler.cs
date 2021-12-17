using System.IO;
using NATS.Client;

namespace M5x.Stan;

/// <summary>
///     This class is contains the default handlers for the
///     <see cref="Options.UserJWTEventHandler" /> and the
///     <see cref="Options.UserSignatureEventHandler" />.  This class is
///     not normally used directly, but is provided to extend or use for
///     utility methods to read a private seed or user JWT.
/// </summary>
public class SourceUserJWTHandler
{
    /// <summary>
    ///     Creates the default user jwt handler.
    /// </summary>
    /// <param name="jwtString">Full path the to user JWT</param>
    /// <param name="credsString">
    ///     Full path to the user private credentials file.
    ///     May be the same as the jwt file if they are chained.
    /// </param>
    public SourceUserJWTHandler(string jwtString, string credsString)
    {
        JwtString = jwtString;
        CredsString = credsString;
    }

    /// <summary>
    ///     Gets the JWT file.
    /// </summary>
    public string JwtString { get; }

    /// <summary>
    ///     Gets the credentials files.
    /// </summary>
    public string CredsString { get; }

    /// <summary>
    ///     Gets a user JWT from a user JWT or chained credentials file.
    /// </summary>
    /// <param name="source">Full path to the JWT or cred file.</param>
    /// <returns>The encoded JWT</returns>
    public static string LoadUserFromSource(string source)
    {
        string text = null;
        string line = null;
        StringReader reader = null;
        try
        {
            text = source.Trim();
            if (string.IsNullOrEmpty(text)) throw new NATSException("Credentials file is empty");

            reader = new StringReader(text);
            for (line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (line.Contains("-----BEGIN NATS USER JWT-----")) return reader.ReadLine();
                Nkeys.Wipe(line);
            }

            throw new NATSException("Credentials source does not contain a JWT");
        }
        finally
        {
            Nkeys.Wipe(text);
            Nkeys.Wipe(line);
            reader?.Dispose();
        }
    }

    /// <summary>
    ///     Generates a NATS Ed25519 keypair, used to sign server nonces, from a
    ///     private credentials file.
    /// </summary>
    /// <param name="source">The credentials file, could be a "*.nk" or "*.creds" file.</param>
    /// <returns>A NATS Ed25519 KeyPair</returns>
    public static NkeyPair LoadNkeyPairFromSource(string source)
    {
        NkeyPair kp = null;
        string text = null;
        string line = null;
        string seed = null;
        StringReader reader = null;

        try
        {
            text = source.Trim();
            if (string.IsNullOrEmpty(text)) throw new NATSException("Credentials source is empty");

            // if it's a nk file, it only has the nkey
            if (text.StartsWith("SU"))
            {
                kp = Nkeys.FromSeed(text);
                return kp;
            }

            // otherwise assume it's a creds file.
            reader = new StringReader(text);
            for (line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (line.Contains("-----BEGIN USER NKEY SEED-----"))
                {
                    seed = reader.ReadLine();
                    kp = Nkeys.FromSeed(seed);
                    Nkeys.Wipe(seed);
                }

                Nkeys.Wipe(line);
            }

            if (kp == null)
                throw new NATSException("Seed not found in credentials source.");
            else
                return kp;
        }
        finally
        {
            Nkeys.Wipe(line);
            Nkeys.Wipe(text);
            Nkeys.Wipe(seed);
            reader?.Dispose();
        }
    }

    /// <summary>
    ///     The default User JWT Event Handler.
    /// </summary>
    /// <param name="sender">Usually the connection.</param>
    /// <param name="args">Arguments</param>
    public void SourceUserJWTEventHandler(object sender, UserJWTEventArgs args)
    {
        args.JWT = LoadUserFromSource(JwtString);
    }

    /// <summary>
    ///     Utility method to signs the UserSignatureEventArgs server nonce from
    ///     a private credentials file.
    /// </summary>
    /// <param name="source">A file with the private Nkey</param>
    /// <param name="args">Arguments</param>
    private static void SignNonceFromSource(string source, UserSignatureEventArgs args)
    {
        var kp = LoadNkeyPairFromSource(source);
        args.SignedNonce = kp.Sign(args.ServerNonce);
        kp.Wipe();
    }

    /// <summary>
    ///     The default User Signature event handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void SourceUserSignatureHandler(object sender, UserSignatureEventArgs args)
    {
        SignNonceFromSource(CredsString, args);
    }
}