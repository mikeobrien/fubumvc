using System.IO;
using FubuCore;
using ICSharpCode.SharpZipLib.Zip;

namespace Tests
{
    public static class Extensions
    {
         public static string UnZipTextFile(this Stream stream, string filename)
         {
             using (var file = new ZipFile(stream))
             using (var fileStream = file.GetInputStream(file.GetEntry(filename)))
                return fileStream.ReadAllText();
         }
    }
}