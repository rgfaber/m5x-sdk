using System.IO;
using System.IO.Compression;

namespace M5x.Utils
{
    public class ZipUtils
    {
        /// <summary>
        ///     Compresses file
        /// </summary>
        /// <param name="fi">uncompressed file</param>
        /// <returns>compressed file, same location as original compressed file, but .gz added to extension</returns>
        public static FileInfo Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (var inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and already compressed files.
                if (!(((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden) &
                      (fi.Extension != ".gz"))) return null;
                var compressedFilePath = $"{fi.FullName}.gz";

                // Create the compressed file.
                using (var outFile = File.Create(compressedFilePath))
                {
                    using (var compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        // Copy the source file into the compression stream.
                        inFile.CopyTo(compress);
                    }
                }

                return new FileInfo(compressedFilePath);
            }
        }

        /// <summary>
        ///     Uncompresses file
        /// </summary>
        /// <param name="fi">compressed file</param>
        /// <returns>uncompressed file, same location as original compressed file, but minus the extension</returns>
        public static FileInfo Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (var inFile = fi.OpenRead())
            {
                // Get original file name
                var compressedFilePath = fi.FullName;
                var decompressedFilePath = compressedFilePath.Remove(compressedFilePath.Length - fi.Extension.Length);

                // Create the decompressed file.
                using (var outFile = File.Create(decompressedFilePath))
                {
                    using (var decompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        // Copy the decompression stream into the output file.
                        decompress.CopyTo(outFile);
                    }
                }

                return new FileInfo(decompressedFilePath);
            }
        }

        /// <summary>
        ///     Uncompresses file
        /// </summary>
        /// <param name="ms">compressed file</param>
        /// <param name="fileName">Name of the compressed file.</param>
        /// <returns>
        ///     uncompressed file, stored in the TransactionRouter temporary path with same name, but minus the extension (.gz)
        /// </returns>
        public static FileInfo Decompress(MemoryStream ms, string fileName)
        {
            // Get original file name
            var decompressedFilePath = Path.Combine(PathUtils.InternetCacheDir(), fileName.Remove(fileName.Length - 3));

            // Create the decompressed file.
            using (var outFile = File.Create(decompressedFilePath))
            {
                using (var decompress = new GZipStream(ms, CompressionMode.Decompress))
                {
                    // Copy the decompression stream into the output file.
                    decompress.CopyTo(outFile);
                }
            }

            return new FileInfo(decompressedFilePath);
        }

        /// <summary>
        ///     Uncompresses file
        /// </summary>
        /// <param name="ms">compressed file</param>
        /// <param name="fileName">Name of the compressed file.</param>
        /// <param name="temporaryPath">The temporary path.</param>
        /// <returns>
        ///     uncompressed file, stored in the TransactionRouter temporary path with same name, but minus the extension (.gz)
        /// </returns>
        public static FileInfo Decompress(MemoryStream ms, string fileName, string temporaryPath)
        {
            // Get original file name
            var decompressedFilePath = Path.Combine(temporaryPath, fileName.Remove(fileName.Length - 3));

            // Create the decompressed file.
            using (var outFile = File.Create(decompressedFilePath))
            {
                using (var decompress = new GZipStream(ms, CompressionMode.Decompress))
                {
                    // Copy the decompression stream into the output file.
                    decompress.CopyTo(outFile);
                }
            }

            return new FileInfo(decompressedFilePath);
        }

        // public static StreamResponse CreateZip(IEnumerable<StreamResponse> docs)
        // {
        //     var result = new StreamResponse(GuidUtils.NullGuid);
        //     try
        //     {
        //         using var s = new MemoryStream();
        //         using (var arch = new ZipArchive(s, ZipArchiveMode.Create, true))
        //         {
        //             if (docs != null)
        //                 foreach (var doc in docs)
        //                 {
        //                     var entry = arch.CreateEntry(doc.Name, CompressionLevel.Optimal);
        //                     if (entry == null) continue;
        //                     using var entryStream = entry.Open();
        //                     entryStream.WriteAsync(doc.Content, 0, doc.Content.Length);
        //                 }
        //         }
        //
        //         s.Seek(0, 0);
        //         result.IsEmpty = s.Length == 0;
        //         result.Content = s.ToByteArray();
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e.Message);
        //         result.Errors.Add("ZipUtils.CreateZip.Error", e.ToXeption());
        //     }
        //     finally
        //     {
        //         Log.CloseAndFlush();
        //     }
        //
        //     return result;
        // }
    }
}