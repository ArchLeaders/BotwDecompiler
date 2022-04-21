using BotwScripts.Lib;

var outputDir = ".\\";

if (args.Length < 1)
    if (!CLI.Option("Not output directory was found. Use the working directory? "))
        return;

else outputDir = args[0];

Directory.CreateDirectory(outputDir);

var nx = "";
bool isNx = Mtk.GetConfig("game_dir").Value.GetBoolean();

if (isNx) nx = "_nx";

string? basegame = Mtk.GetConfig($"game_dir{nx}").Value.GetString();
string? update = Mtk.GetConfig("update_dir").Value.GetString();
string? dlc = Mtk.GetConfig($"dlc_dir{nx}").Value.GetString();

if (basegame == null)
{
    CLI.WriteLine("!error||The base game dump could not be found. Confirm BCML is installed and setup.");
    return;
}

if (update == null && isNx != true)
{
    CLI.WriteLine("!error||The base game dump could not be found. Confirm BCML is installed and setup.");
    return;
}

foreach (var file in Directory.EnumerateFiles(basegame, "*.*", SearchOption.AllDirectories))
    Decompile(file);

foreach (var file in Directory.EnumerateFiles(basegame, "*.*", SearchOption.AllDirectories))
    Decompile(file);

foreach (var file in Directory.EnumerateFiles(basegame, "*.*", SearchOption.AllDirectories))
    Decompile(file);

async Task Decompile(string file)
{

}