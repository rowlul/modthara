namespace Modthara.Lari.Pak;

/// <summary>
/// Represents stream of an uncompressed packaged file.
/// </summary>
/// <param name="ownerStream">Owner package stream of the packaged file.</param>
/// <param name="ownerFile">Packaged file to read.</param>
public class UncompressedPackagedFileStream(Stream ownerStream, PackagedFile ownerFile) : Stream
{
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (ownerStream.Position < (long)ownerFile.OffsetInFile
            || ownerStream.Position > (long)ownerFile.OffsetInFile + (long)ownerFile.SizeOnDisk)
        {
            throw new InvalidDataException("Stream at unexpected position while reading packaged file.");
        }

        var readable = (long)ownerFile.SizeOnDisk - Position;
        var bytesToRead = readable < count ? (int)readable : count;
        return ownerStream.Read(buffer, offset, bytesToRead);
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
    public override long Length => (long)ownerFile.SizeOnDisk;

    public override long Position
    {
        get => ownerStream.Position - (long)ownerFile.OffsetInFile;
        set => throw new NotSupportedException();
    }
}
