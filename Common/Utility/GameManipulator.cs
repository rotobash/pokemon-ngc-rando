using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.Utility
{
    public class GameManipulator : IDisposable
    {
        private bool disposedValue;

        protected ISO ISO {  get; private set; }
        protected ISOExtractor ISOExtractor { get; private set; }
        public IGameExtractor GameExtractor { get; private set; }

        public GameManipulator(string pathToGameFile)
        {
            if (!File.Exists(pathToGameFile))
            {
                throw new ArgumentException("Cannot open game file with the path provided.");
            }

            ISOExtractor = new ISOExtractor(pathToGameFile);
            ISO = ISOExtractor.ExtractISO();
            GameExtractor = GetGameExtractor(ISO);
        }

        private IGameExtractor GetGameExtractor(ISO iso)
        {

            if (iso.Game == XDCommon.Contracts.Game.XD)
            {
                return new XDExtractor(iso);
            }
            else
            {
                return new ColoExtractor(iso);
            }
        }

        //protected void SaveChanges()
        //{
        //    Console.WriteLine("Packing ISO...");
        //    ISOExtractor.RepackISO(ISO, OutputFile);
        //    Console.WriteLine("Finished!");
        //}

        public void ReleaseGameFile()
        {
            ISOExtractor.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ISO.Dispose();
                    ISOExtractor.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GameManipulator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
