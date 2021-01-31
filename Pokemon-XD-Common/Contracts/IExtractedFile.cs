using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Contracts
{
    public interface IExtractedFile
    {
        string FileName { get; }
        string Path { get; }
        FileTypes FileType { get; }
        Stream ExtractedFile { get; }
    }
}
