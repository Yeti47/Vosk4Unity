using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZipFileExtractor
{

#if NET_STANDARD_2_0

    private class NetStandardZipFileExtractor : ZipFileExtractor
    {
        public override void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }
    }

#else

    private class NetFrameworkZipFileExtractor : ZipFileExtractor
    {
        public override void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        {
            Yetibyte.Compression.ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }
    }

#endif

    public abstract void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName);

    public static ZipFileExtractor Create()
    {
#if NET_STANDARD_2_0
        return new NetStandardZipFileExtractor();
#else    
        UnityEngine.Debug.Log("Using library Yetibyte.Compression to extract archive for .NET Framework 4.x compatility.");
        return new NetFrameworkZipFileExtractor();
#endif
    }

}
