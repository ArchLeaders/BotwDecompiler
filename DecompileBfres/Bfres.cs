using Syroot.BinaryData;
using System.Text;

namespace BfresLibrary
{
    /// <summary>
    /// Copied from KillzXGaming's <a href="https://github.com/KillzXGaming/BfresPlatformConverter/blob/master/Program.cs">BfresPLatformConverter</a>
    /// </summary>
    public class Bfres
    {
        public static bool IsCompressed(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryDataReader(fileStream))
            {
                string magic = reader.ReadString(4, Encoding.ASCII);
                return magic == "Yaz0";
            }
        }

        public static ResFile LoadBfres(string bfresFile)
        {
            if (IsCompressed(bfresFile))
                using (MemoryStream stream = new(Yaz0.Decompress(bfresFile)))
                    return new ResFile(stream);
            else
                using (MemoryStream stream = new(File.ReadAllBytes(bfresFile)))
                    return new ResFile(stream);
        }
    }
}
