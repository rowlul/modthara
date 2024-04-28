using System.IO.Compression;

namespace Modthara.Essentials;

public interface IArchiver
{
    Task<ZipArchive> OpenZipAsync(string path, ZipArchiveMode mode);
}
