using System.IO.Compression;
using System.Text.Json;

var sourceFolder = "appManifest";
var outputFolder = "..\\..\\..\\output";
var settingsPath = "appsettings.json";

var manifestPath = Path.Combine(sourceFolder, "manifest.json");
var colorIconPath = Path.Combine(sourceFolder, "color.png");
var outlineIconPath = Path.Combine(sourceFolder, "outline.png");

if (!File.Exists(manifestPath))
{
    Console.WriteLine("manifest.json not found in appManifest folder");
    return;
}

// Load manifest
var manifest = File.ReadAllText(manifestPath);

// Load settings
var settingsJson = File.ReadAllText(settingsPath);
var settings = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(settingsJson);

if (settings == null || !settings.ContainsKey("Replacements"))
{
    Console.WriteLine("No replacements found in appsettings.json");
    return;
}

var replacements = settings["Replacements"];

string? resolvedVersion = null;
string? resolvedBotName = null;

// Apply replacements
foreach (var kvp in replacements)
{
    var token = "{" + kvp.Key + "}";
    var value = kvp.Value;

    if (value == "{NEW_GUID}")
    {
        value = Guid.NewGuid().ToString();
        Console.WriteLine($"Generated new GUID for {kvp.Key}: {value}");
    }

    if (kvp.Key == "VERSION") 
        resolvedVersion = value;

    if (kvp.Key == "BOT_NAME") 
        resolvedBotName = value;

    manifest = manifest.Replace(token, value);
}

// Validate naming fields
if (resolvedVersion == null) 
{
    Console.WriteLine("ERROR: VERSION token not found in appsettings.json"); 
    return;
} 

if (resolvedBotName == null) 
{
    Console.WriteLine("ERROR: BOT_NAME token not found in appsettings.json"); 
    return; 
}

// Prepare output
Directory.CreateDirectory(outputFolder);
var tempFolder = Path.Combine(outputFolder, "temp");
if (Directory.Exists(tempFolder))
    Directory.Delete(tempFolder, true);

Directory.CreateDirectory(tempFolder);

// Write transformed manifest
var transformedManifestPath = Path.Combine(tempFolder, "manifest.json");
File.WriteAllText(transformedManifestPath, manifest);

// Copy icons
File.Copy(colorIconPath, Path.Combine(tempFolder, "color.png"));
File.Copy(outlineIconPath, Path.Combine(tempFolder, "outline.png"));

// Create ZIP
var zipFileName = $"{resolvedBotName} {resolvedVersion}.zip";
var zipPath = Path.Combine(outputFolder, zipFileName); 

if (File.Exists(zipPath))
    File.Delete(zipPath);

ZipFile.CreateFromDirectory(tempFolder, zipPath);

Directory.Delete(tempFolder, true);

Console.WriteLine("App package successfully created:");
Console.WriteLine(Path.GetFullPath(zipPath));
