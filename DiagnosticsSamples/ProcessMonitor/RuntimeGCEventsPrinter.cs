﻿using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

namespace ProcessMonitor;

internal class RuntimeGCEventsPrinter
{
    public static void PrintRuntimeGCEvents(int processId)
    {
        var providers = new List<EventPipeProvider>()
        {
            new EventPipeProvider("Microsoft-Windows-DotNETRuntime",
            EventLevel.Informational,
            (long)ClrTraceEventParser.Keywords.GC)
        };

        var client = new DiagnosticsClient(processId);
        using (EventPipeSession session = client.StartEventPipeSession(providers, false))
        {
            var source = new EventPipeEventSource(session.EventStream);

            source.Clr.All += (TraceEvent obj) => Console.WriteLine(obj.ToString());

            try
            {
                source.Process();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while processing events: {ex}");
            }
        }
    }
}
