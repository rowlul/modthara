using System.IO.Abstractions;
using System.IO.Compression;

namespace Modthara.Essentials;

public class Archiver : IArchiver
{
    private readonly IFileSystem _fileSystem;

    public Archiver(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<ZipArchive> OpenZipAsync(string path, ZipArchiveMode mode)
    {
        FileMode fileMode;
        FileAccess fileAccess;
        FileShare fileShare;

        switch (mode)
        {
            case ZipArchiveMode.Read:
                fileMode = FileMode.Open;
                fileAccess = FileAccess.Read;
                fileShare = FileShare.Read;
                break;

            case ZipArchiveMode.Create:
                fileMode = FileMode.CreateNew;
                fileAccess = FileAccess.Write;
                fileShare = FileShare.None;
                break;

            case ZipArchiveMode.Update:
                fileMode = FileMode.OpenOrCreate;
                fileAccess = FileAccess.ReadWrite;
                fileShare = FileShare.None;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }

        var zipFileStream =
            _fileSystem.FileStream.New(path, fileMode, fileAccess, fileShare, StreamBufferSize, useAsync: true);

        var zipArchive = await Task.Run(() => new ZipArchive(zipFileStream, ZipArchiveMode.Read)).ConfigureAwait(false);
        return zipArchive;
    }

    private const int StreamBufferSize = 0x1000;
}
