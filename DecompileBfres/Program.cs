using BfresLibrary;
using BfresLibrary.WiiU;
using DecompileBfres;
using System.Drawing;
using System.Text.Json;

if (args.Length < 2)
{
    Console.WriteLine("Invalid Arguments | DecompileBfres.exe <\"bfresFile\"> <\"outputFolder\">");
    return;
}

string path = args[0];
string outFolder = args[1];

if (!File.Exists(path))
    throw new FileNotFoundException($"Could not find the file '{path}'.");

Directory.CreateDirectory(outFolder);

ResFile res = Bfres.LoadBfres(path);
SortedDictionary<string, dynamic> bfresJson = new();
List<Task> tasks = new();

Console.WriteLine($"[BFRES] {res.Name}: Set meta data");

bfresJson.Add("Alignment", res.Alignment);
bfresJson.Add("ByteOrder", $"{res.ByteOrder}");
bfresJson.Add("Switch", res.IsPlatformSwitch);
bfresJson.Add("Name", $"{res.Name}");
bfresJson.Add("Textures", new List<string>());
bfresJson.Add("Models", new List<string>());
bfresJson.Add("Animations", new Dictionary<string, dynamic>()
{
    { "ColourAnim", null },
    { "MatVisibilityAnims", null },
    { "SceneAnims", null },
    { "ShaderParamAnims", null },
    { "ShapeAnims", null },
    { "SkeletalAnims", null },
    { "TexPatternAnims", null },
    { "TexSrtAnims", null },
});
bfresJson.Add("Version", $"{res.VersioFull}");

tasks.Add(Task.Run(() =>
{
    // Colour Anims
    foreach (var resFile in res.ColorAnims)
        AddAnim("ColourAnim", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    // External Files
    foreach (var resFile in res.ExternalFiles)
    {
        Directory.CreateDirectory($"{outFolder}\\External");
        File.WriteAllBytes($"{outFolder}\\External\\{resFile.Key}", resFile.Value.Data);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Key}");
    }
}));

tasks.Add(Task.Run(() =>
{
    // Material Visibility Anims
    foreach (var resFile in res.MatVisibilityAnims)
        AddAnim("MatVisibilityAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    // Models
    foreach (var resFile in res.Models)
    {
        AddGeneric("Models", resFile.Value.Name);

        foreach (var mat in resFile.Value.Materials)
        {
            Directory.CreateDirectory($"{outFolder}\\Materials\\{resFile.Value.Name}");
            mat.Value.Export($"{outFolder}\\Materials\\{resFile.Value.Name}\\{mat.Value.Name}.bfmat", res);

            Console.WriteLine($"[BFRES] [MATERIALS] Exported {res.Name}/{resFile.Value.Name}/{mat.Value.Name}");
        }
    }
}));

tasks.Add(Task.Run(() =>
{
    // Scene Anims
    foreach (var resFile in res.SceneAnims)
        AddAnim("SceneAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.ShaderParamAnims)
        AddAnim("ShaderParamAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.ShapeAnims)
        AddAnim("ShapeAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.SkeletalAnims)
        AddAnim("SkeletalAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.TexPatternAnims)
        AddAnim("TexPatternAnims", resFile.Value.Name);
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.TexSrtAnims)
        AddAnim("TexSrtAnims", resFile.Value.Name);
}));

foreach (var resFile in res.Textures)
{
    tasks.Add(Task.Run(() =>
    {
        // Export PNG
        try
        {
            Texture tex = (Texture)resFile.Value;
            Directory.CreateDirectory($"{outFolder}\\Textures");
        
            AddGeneric("Textures", resFile.Value.Name);

            byte[] data = new byte[0];

            DirectXTexLibrary.TextureDecoder.Decode(tex.Format.GetDXGI(), tex.GetDeswizzledData(0, 0), (int)tex.Width, (int)tex.Height, out data);
            Bitmap btm = Ftex.GetBitmap(Ftex.ConvertBgraToRgba(data), (int)tex.Width, (int)tex.Height);

            btm.Save($"{outFolder}\\Textures\\{resFile.Value.Name}.jpg");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[BFRES] [TEXTURES] Could not decode {resFile.Value.Name}");
            Console.ResetColor();
        }

    }));
}

await Task.WhenAll(tasks);

bool isNull = true;
foreach (var type in bfresJson["Animations"])
{
    if (type.Value != null)
        isNull = false;
}

if (isNull) bfresJson["Animations"] = null;

if (bfresJson["Models"].Count == 0)
    bfresJson["Models"] = null;

if (bfresJson["Textures"].Count == 0)
    bfresJson["Textures"] = null;

using (var stream = File.OpenWrite($"{outFolder}\\{res.Name}.json"))
    await JsonSerializer.SerializeAsync(stream, bfresJson, new JsonSerializerOptions()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    });

void AddAnim(string type, string name)
{
    if (bfresJson["Animations"][type] == null)
        bfresJson["Animations"][type] = new List<string>();

    bfresJson["Animations"][type].Add(name);
    bfresJson["Animations"][type].Sort();

    Console.WriteLine($"[BFRES] [ANIMATIONS] Add {name}::<{type}>");
}

void AddGeneric(string key, string value)
{
    bfresJson[key].Add(value);
    bfresJson[key].Sort();

    Console.WriteLine($"[BFRES] Add {value}::<{key}>");
}