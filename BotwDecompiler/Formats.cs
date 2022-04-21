using BotwScripts.Lib;
using BotwScripts.Lib.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotwDecompiler
{
    public static class Formats
    {
        public static Dictionary<string, List<string>> DB = new();
        public static void Load()
        {
            Mtk.UpdateExternal("formats.sjson", $"{Mtk.GetConfig("dynamic")}\\Data", "BotwScripts.Lib/Data");
            byte[] bytes = Yaz0.Decompress(Mtk.GetDynamic("formats.sjson"));
            DB = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(bytes);

            if (DB == null)
            {
                CLI.WriteLine("!error||The file formats could not be loaded!");
                DB = new();
            }
        }

        public static string Get(this string file)
        {
            FileInfo _file = new(file);
            var ext = _file.Extension;

            return "";
        }
    }
}
