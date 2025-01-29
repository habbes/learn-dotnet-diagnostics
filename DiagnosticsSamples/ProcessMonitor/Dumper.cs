using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitor;

internal class Dumper
{
    public static void Dump(int processId, double threshold, string dumpDestination)
    {
        List<EventPipeProvider> providers = [
            new EventPipeProvider(
                "System.Runtime",
                EventLevel.Informational,
                (long)ClrTraceEventParser.Keywords.None,
                new Dictionary<string, string> {
                    ["EventCounterIntervalSec"] = "1"
                })];

        var client = new DiagnosticsClient(processId);
        using var session = client.StartEventPipeSession(providers);

        var source = new EventPipeEventSource(session.EventStream);
        source.Dynamic.All += (TraceEvent obj) =>
        {
            if (obj.EventName == "EventCounters")
            {
                var payloadVal = (IDictionary<string, object>)obj.PayloadValue(0);
                var payloadFields = (IDictionary<string, object>)payloadVal["Payload"];
                if (payloadFields["Name"].ToString() == "cpu-usage")
                {
                    var cpuUsage = double.Parse(payloadFields["Mean"].ToString());
                    if (cpuUsage > threshold)
                    {
                        Console.WriteLine($"High CPU usage detected: {cpuUsage}, creating dump at {dumpDestination}");
                        client.WriteDump(DumpType.Normal, dumpDestination);
                    }
                }
            }
        };

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
