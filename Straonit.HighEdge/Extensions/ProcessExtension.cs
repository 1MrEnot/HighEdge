namespace Straonit.HighEdge.Extensions;

using System.Diagnostics;

public static class ProcessExtension
{
    public static async Task<string> ExecBashCommand(this Process process, string command, string arguments = "")
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = command,
            Arguments = arguments
        };
        process.StartInfo = startInfo;
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return output;
    }
}