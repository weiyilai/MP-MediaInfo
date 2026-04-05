#region Copyright (C) 2017-2026 Yaroslav Tatarenko

// Copyright (C) 2017-2026 Yaroslav Tatarenko
// This product uses MediaInfo library, Copyright (c) 2002-2026 MediaArea.net SARL. 
// https://mediaarea.net

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MediaInfo.TestFilesGenerator;

internal static class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("MKA Test File Generator");
    Console.WriteLine("Usage: generator [outputDir] [seed] [count] [ffmpegPath] [parallelism]");
    Console.WriteLine("  outputDir   — output folder          (default: ./TestAudio)");
    Console.WriteLine("  seed        — RNG seed               (default: 42)");
    Console.WriteLine("  count       — number of files        (default: 3000)");
    Console.WriteLine("  ffmpegPath  — path to ffmpeg binary  (default: ffmpeg)");
    Console.WriteLine("  parallelism — parallel workers        (default: CPU count)");
    Console.WriteLine();

    var outputDir = GetArg(args, 0, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAudio"));
    var seed = int.Parse(GetArg(args, 1, "42"));
    var count = int.Parse(GetArg(args, 2, "3000"));
    var ffmpegPath = GetArg(args, 3, "ffmpeg");
    var parallelism = int.Parse(GetArg(args, 4, Environment.ProcessorCount.ToString()));

    Console.WriteLine($"  Output      : {outputDir}");
    Console.WriteLine($"  Seed        : {seed}");
    Console.WriteLine($"  Count       : {count}");
    Console.WriteLine($"  FFmpeg      : {ffmpegPath}");
    Console.WriteLine($"  Parallelism : {parallelism}");
    Console.WriteLine();

    if (!CheckFfmpeg(ffmpegPath))
    {
      Console.Error.WriteLine("ERROR: FFmpeg not found or not executable.");
      Console.Error.WriteLine("       Install FFmpeg and add it to PATH, or pass the full path as argument 4.");
      Environment.Exit(1);
    }

    await new FileGenerator(outputDir, ffmpegPath, seed, parallelism)
      .Generate(count);
  }

  #region Helpers

  private static bool CheckFfmpeg(string ffmpegPath)
  {
    try
    {
      var psi = new ProcessStartInfo
      {
        FileName = ffmpegPath,
        Arguments = "-version",
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardError  = true
      };
      
      using var p = Process.Start(psi);
      p?.WaitForExit(5_000);
      return p?.ExitCode == 0;
    }
    catch
    {
      return false;
    }
  }

  private static string GetArg(string[] args, int index, string defaultValue) =>
    args.Length > index ? args[index] : defaultValue;

  #endregion
}
