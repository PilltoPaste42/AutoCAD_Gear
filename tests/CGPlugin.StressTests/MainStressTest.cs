namespace CGPlugin.StressTests;

using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;

using CGPlugin.Models;
using CGPlugin.Services;

/// <summary>
///     Нагрузочное тестирование основной логики CGPlugin
/// </summary>
public static class MainStressTest
{
    public static void Main(string[] args)
    {
        var fileName = @"MainStressTest_results.csv";
        var filePath = @"C:\!Results";

        var fullFilePath = $"{filePath}\\{fileName}";

        var model = new HelicalGearModel
        {
            Diameter = 200,
            Module = 10,
            TeethAngle = 25,
            TeethCount = 20,
            Width = 50
        };
        var builder = new HelicalGearInventorBuilder();
        builder.FromModel(model);

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        if (File.Exists(fullFilePath))
        {
            File.Delete(fullFilePath);
        }

        var file = File.OpenWrite(filePath + "\\" + fileName);
        if (!file.CanWrite)
        {
            throw new Exception();
        }

        var writer = new StreamWriter(file);
        writer.AutoFlush = true;
        var csv = new StringBuilder();
        var watch = new Stopwatch();

        csv.AppendLine("TotalMemory(KB);" + "UsedMemory(KB);" + "ExecutionTime(S)");
        writer.Write(csv.ToString());
        csv.Clear();

        var q = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
        var mos = new ManagementObjectSearcher(q);

        for (var i = 0; i < 1000; i++)
        {
            watch.Start();
            builder.BuildCitroenGear();
            watch.Stop();

            var executeTime = Math.Round(watch.Elapsed.TotalSeconds, 2);
            ulong totalMemory = 0;
            ulong usedMemory = 0;
            foreach (var obj in mos.Get())
            {
                totalMemory = (ulong)obj["TotalVisibleMemorySize"];
                usedMemory = (ulong)obj["TotalVisibleMemorySize"] - (ulong)obj["FreePhysicalMemory"];
            }

            csv.AppendLine($"{totalMemory};{usedMemory};{executeTime}");
            writer.Write(csv.ToString());

            csv.Clear();
            watch.Reset();

            Console.WriteLine($"{i}: UsedRAM={usedMemory}KB Time={executeTime}S");
        }

        Console.WriteLine("Success!");
        writer.Close();
    }
}