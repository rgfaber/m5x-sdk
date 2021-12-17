namespace M5x.Crypto;

public partial class CryptoEngine
{
    // we'd like symmetric encryption, so we setup our own encryption key and IV
    private static readonly byte[] Key = new byte[8] { 32, 182, 33, 9, 97, 11, 101, 219 };
    private static readonly byte[] Iv = new byte[8] { 36, 64, 32, 151, 14, 13, 73, 43 };
}