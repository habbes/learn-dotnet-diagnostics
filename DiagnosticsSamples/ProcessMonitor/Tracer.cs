using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

internal class Tracer
{
    public async static Task TraceProcessForDuration(int processId, int duration, string traceName)
    {
        var cpuProviders = new List<EventPipeProvider>()
        {
            new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.Default),
            new EventPipeProvider("Microsoft-DotNETCore-SampleProfiler", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.None)
        };
        var client = new DiagnosticsClient(processId);
        using var traceSession = client.StartEventPipeSession(cpuProviders);


        Task copyTask = Task.Run(async () =>
        {
            using (FileStream fs = new FileStream(traceName, FileMode.Create, FileAccess.Write))
            {
                await traceSession.EventStream.CopyToAsync(fs);
            }
        });

        await Task.WhenAny(copyTask, Task.Delay(TimeSpan.FromMilliseconds(duration * 1000)));
        traceSession.Stop();
    }
}