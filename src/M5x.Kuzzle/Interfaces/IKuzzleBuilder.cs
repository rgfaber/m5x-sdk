using KuzzleSdk;

namespace M5x.Kuzzle.Interfaces
{
    public interface IKuzzleBuilder
    {
        IKuzzleApi BuildApi(string url = null);
    }
}