using BfresLibrary;

if (args.Length < 2)
{
    Console.WriteLine("Invalid Arguments | DecompileBfres.exe <\"bfresFile\"> <\"outputFolder\">");
}

string path = args[0];
string outFolder = args[1];

if (!File.Exists(path))
    throw new FileNotFoundException($"Could not find the file '{path}'.");

ResFile res = Bfres.LoadBfres(path);

List<Task> tasks = new();

tasks.Add(Task.Run(() =>
{
    // Colour Anims
    foreach (var resFile in res.ColorAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\ColourAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\ColourAnims\\{resFile.Value.Name}.bfmaa", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
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
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\\\MatVisibilityAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\MatVisibilityAnims\\{resFile.Value.Name}.bfbvi", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    // Models
    foreach (var resFile in res.Models)
    {
        Directory.CreateDirectory($"{outFolder}\\Models");
        resFile.Value.Export($"{outFolder}\\Models\\{resFile.Value.Name}.bfmdl", res);

        foreach (var mat in resFile.Value.Materials)
        {
            Directory.CreateDirectory($"{outFolder}\\Models\\Mat");
            mat.Value.Export($"{outFolder}\\Models\\Mat\\{mat.Value.Name}.bfmat", res);

            Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}/{mat.Value.Name}");
        }

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    // Scene Anims
    foreach (var resFile in res.SceneAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\SceneAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\SceneAnims\\{resFile.Value.Name}.bfscn", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.ShaderParamAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\ShaderParamAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\ShaderParamAnims\\{resFile.Value.Name}.bfmaa", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.ShapeAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\ShapeAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\ShapeAnims\\{resFile.Value.Name}.bfspa", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.SkeletalAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\SkeletalAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\SkeletalAnims\\{resFile.Value.Name}.bfska", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.TexPatternAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\TexPatternAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\TexPatternAnims\\{resFile.Value.Name}.bftxp", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.TexSrtAnims)
    {
        Directory.CreateDirectory($"{outFolder}\\Animations\\SceneAnims");
        resFile.Value.Export($"{outFolder}\\Animations\\SceneAnims\\{resFile.Value.Name}.bfmaa", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

tasks.Add(Task.Run(() =>
{
    foreach (var resFile in res.Textures)
    {
        Directory.CreateDirectory($"{outFolder}\\Textures");
        resFile.Value.Export($"{outFolder}\\Textures\\{resFile.Value.Name}.bftex", res);

        Console.WriteLine($"[BFRES] Exported {res.Name}/{resFile.Value.Name}");
    }
}));

await Task.WhenAll(tasks);