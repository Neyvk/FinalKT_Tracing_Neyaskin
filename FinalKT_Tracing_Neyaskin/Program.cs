using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Diagnostics;

namespace FinalKT_Tracing_Neyaskin
{
    class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(
                    new JsonFormatter(),
                    "logs/app.json",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Tracer.TaskManagerTrace.Switch = new SourceSwitch("SourceSwitch", "All");
            Tracer.TaskManagerTrace.Listeners.Add(new ConsoleTraceListener());
            Tracer.TaskManagerTrace.Listeners.Add(
                new TextWriterTraceListener("taskmanagerTrace.log"));

            try
            {
                Log.Information("TaskManager запущен");

                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Start,
                    0,
                    "Программа запущена");

                Console.WriteLine("Команды: add, remove, list, exit");

                TaskManager manager = new TaskManager();

                while (true)
                {
                    Console.Write("> ");
                    string command = Console.ReadLine()?.Trim().ToLower();

                    switch (command)
                    {
                        case "add":
                            Console.Write("Название: ");
                            manager.AddTask(Console.ReadLine());
                            break;

                        case "remove":
                            Console.Write("Название: ");
                            manager.RemoveTask(Console.ReadLine());
                            break;

                        case "list":
                            manager.ListTasks();
                            break;

                        case "exit":

                            Log.Information("Программа завершается");

                            Tracer.TaskManagerTrace.TraceEvent(
                                TraceEventType.Stop,
                                0,
                                "Конец программы");

                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Необработанная ошибка");
                Console.WriteLine("Произошла ошибка. Подробнее см. логи.");
            }
            finally
            {
                Tracer.TaskManagerTrace.Flush();
                Tracer.TaskManagerTrace.Close();

                Log.CloseAndFlush();
            }
        }
    }
}