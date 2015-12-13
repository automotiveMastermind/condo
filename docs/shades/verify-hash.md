---
layout: docs
title: verify-hash
group: shades
---

use namespace = 'System.Diagnostics'
use namespace = 'System.IO'

default working_path = '${ Directory.GetCurrentDirectory() }'

default verify_assembly = ''
default verify_hash = ''

@{
    Build.Log.Header("verify-hash");
    Build.Log.Argument("assembly", verify_assembly);
    Build.Log.Argument("hash", verify_hash);

    if (string.IsNullOrEmpty(verify_assembly))
    {
        Log.Warn("verify-hash: no assembly was specified.");

        return;
    }

    Log.Info(string.Format("verify-hash: verifying authenticode signature of {0}", verify_assembly));

    var signPaths = new[]
        {
            @"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool.exe",
            @"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe",
            @"C:\Program Files\Windows Kits\10\bin\x86\signtool.exe",
            @"C:\Program Files\Windows Kits\8.1\bin\x86\signtool.exe"
        };

    var signtool = signPaths.FirstOrDefault(File.Exists);

    if (string.IsNullOrEmpty(signtool))
    {
        Log.Warn("signtool.exe was not located on the machine. Please install Visual Studio 2013 or greater. If you are using Visual Studio 2015, you must install the ClickOnce Publishing Tools in order to make signtool.exe available.");
    }
    else
    {
        var start = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = working_path,
                FileName = signtool,
                RedirectStandardOutput = true,
                Arguments = "verify /pa /v \"" + verify_assembly + "\""
            };

        var process = Process.Start(start);
        var output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            File.Delete(verify_assembly);

            throw new Exception(string.Format("The signature verification for {0} failed:{1}{2}", verify_assembly, Environment.NewLine, output));
        }

        if (!string.IsNullOrEmpty(verify_hash))
        {
            var lines = output.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            var hash_line = lines[3];
            var actual = hash_line.Substring(hash_line.IndexOf(":") + 1).Trim();

            if (!string.Equals(verify_hash, actual, StringComparison.Ordinal))
            {
                File.Delete(verify_assembly);

                throw new Exception(string.Format("The hash comparison for {0} failed: expected '{1}', actual '{2}'", verify_assembly, verify_hash, actual));
            }
        }

        Log.Info("verify-hash: authenticode signature successfully verified.");
    }

    Build.Log.Header();
}