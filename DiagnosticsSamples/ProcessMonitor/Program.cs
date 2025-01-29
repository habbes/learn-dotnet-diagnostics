// See https://aka.ms/new-console-template for more information
using ProcessMonitor;

if (args.Length < 1)
{
    PrintHelp();
    return;
}

var command = args[0];

if (command == "gcEvents")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Please specify the process ID of the target process.");
        PrintHelp();
        return;
    }
    int processId = int.Parse(args[1]);
    RuntimeGCEventsPrinter.PrintRuntimeGCEvents(processId);
}
else if (command == "cpuThreshold")
{
    if (args.Length < 4)
    {
        Console.WriteLine("Please specify the process ID, CPU threshold, and dump destination.");
        PrintHelp();
        return;
    }

    int processId = int.Parse(args[1]);
    double threshold = double.Parse(args[2]);
    string dumpDestination = args[3];
    Dumper.Dump(processId, threshold, dumpDestination);
}
else if (command == "trace")
{
    if (args.Length < 4)
    {
        Console.WriteLine("Please specify the process ID, trace duration, and trace destination file.");
        PrintHelp();
        return;
    }

    int processId = int.Parse(args[1]);
    int duration = int.Parse(args[2]);
    string traceDestination = args[3];

    Tracer.TraceProcessForDuration(processId, duration, traceDestination).Wait(); ;
}
else if (command == "ps")
{
    ProcessTracker.PrintProcessStatus();
}
else if (command == "events")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Please specify the process ID of the target process.");
        PrintHelp();
        return;
    }
    int processId = int.Parse(args[1]);
    LiveTracer.PrintEventsLive(processId);
}
else if (command == "profiler")
{
    if (args.Length < 4)
    {
        Console.WriteLine("Please specify the process ID, the profiler GUID and profiler path to attach to the process.");
        PrintHelp();
        return;
    }

    int processId = int.Parse(args[1]);
    Guid profilerGuid = Guid.Parse(args[2]);
    string profilerPath = args[3];
    Profiler.AttachProfiler(processId, profilerGuid, profilerPath);
}
else
{
    Console.WriteLine("Invalid command. Supported commands: gcEvents, dump");

    PrintHelp();
}

void PrintHelp()
{
    Console.WriteLine("Usage: ProcessMonitor <command> <args>");
    Console.WriteLine("Commands:");
    Console.WriteLine("  gcEvents <processId> - Print GC events for the specified process");
    Console.WriteLine("  cpuThreshold <processId> <threshold> <dumpDestination> - Monitor CPU usage and create a dump if it exceeds the specified threshold");
    Console.WriteLine("  trace <processId> <durationInSeconds> <traceDestination> - Trace the specified process for the specified duration and save the trace to the specified file");
    Console.WriteLine("  ps - Print names of processes that published a diagnostics channel");
    Console.WriteLine("  events <processId> - Print live events for the specified process");
    Console.WriteLine("  profiler <processId> <profilerGuid> <profilerPath> - Attach a profiler to the specified process");
}


