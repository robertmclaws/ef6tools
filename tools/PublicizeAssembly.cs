#:package Mono.Cecil@0.11.6

using Mono.Cecil;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run PublicizeAssembly.cs -- <input.dll> <output.dll>");
    return 1;
}

var inputPath = args[0];
var outputPath = args[1];

// Ensure output directory exists
var outputDir = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

Console.WriteLine($"Reading assembly from: {inputPath}");

var readerParams = new ReaderParameters { ReadSymbols = false, ReadWrite = false };
using var assembly = AssemblyDefinition.ReadAssembly(inputPath, readerParams);

foreach (var module in assembly.Modules)
{
    foreach (var type in module.GetTypes())
    {
        if (type.IsNotPublic) type.IsPublic = true;
        if (type.IsNestedAssembly || type.IsNestedFamilyAndAssembly) type.IsNestedPublic = true;

        foreach (var method in type.Methods)
            if (method.IsAssembly || method.IsFamilyAndAssembly) method.IsPublic = true;

        foreach (var field in type.Fields)
            if (field.IsAssembly || field.IsFamilyAndAssembly) field.IsPublic = true;

        foreach (var prop in type.Properties)
        {
            if (prop.GetMethod?.IsAssembly == true || prop.GetMethod?.IsFamilyAndAssembly == true)
                prop.GetMethod.IsPublic = true;
            if (prop.SetMethod?.IsAssembly == true || prop.SetMethod?.IsFamilyAndAssembly == true)
                prop.SetMethod.IsPublic = true;
        }
    }
}

Console.WriteLine($"Writing publicized assembly to: {outputPath}");
var writerParams = new WriterParameters { WriteSymbols = false };
assembly.Write(outputPath, writerParams);
Console.WriteLine("Publicization complete");

return 0;
