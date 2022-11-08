using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public abstract class BaseExtractedFile : IExtractedFile
    {
        protected bool disposedValue;

        public virtual string FileName { get; internal set; }
        public virtual FileTypes FileType { get; internal set; }
        public virtual string Path { get; internal set; }
        public virtual Stream ExtractedFile { get; internal set; }

        public abstract Stream Encode(bool isCompressed);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ExtractedFile.Dispose();
                ExtractedFile = null;

                disposedValue = true;
            }
        }

        ~BaseExtractedFile()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
