namespace Modthara.Lari.Pak;

/// <summary>
/// Represents stream of an uncompressed packaged file.
/// </summary>
public class UncompressedPackagedFileStream : Stream
{
    private readonly Stream _ownerStream;
    private readonly PackagedFile _ownerFile;

    public UncompressedPackagedFileStream(Stream ownerStream, PackagedFile ownerFile)
    {
        _ownerStream = ownerStream;
        _ownerFile = ownerFile;
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_ownerStream.Position < (long)_ownerFile.OffsetInFile
            || _ownerStream.Position > (long)_ownerFile.OffsetInFile + (long)_ownerFile.SizeOnDisk)
        {
            throw new InvalidDataException("Stream at unexpected position while reading packaged file.");
        }

        var readable = (long)_ownerFile.SizeOnDisk - Position;
        var bytesToRead = readable < count ? (int)readable : count;
        return _ownerStream.Read(buffer, offset, bytesToRead);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => (long)_ownerFile.SizeOnDisk;

    public override long Position
    {
        get => _ownerStream.Position - (long)_ownerFile.OffsetInFile;
        set => throw new NotSupportedException();
    }
}
