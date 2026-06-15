using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FinalKT_Tracing_Neyaskin
{
    public class TaskManager
    {
        private readonly List<TaskItem> _tasks = new();

        private int nextId = 1;

        public void AddTask(string title)
        {
            ExceptionHandler.Run("AddTask", () =>
            {
                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Start,
                    0,
                    "Начало AddTask");

                Stopwatch sw = Stopwatch.StartNew();

                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Название задачи не может быть пустым.");

                TaskItem task = new TaskItem(title);

                _tasks.Add(task);

                sw.Stop();

                Log.Information("Добавлена задача {TaskTitle}", task.Title);

                Log.Information("Количество задач после добавления: {Count}", _tasks.Count);

                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Stop,
                    200,
                    $"Завершение AddTask. Добавлена задача \"{task.Title}\". Время: {sw.ElapsedMilliseconds} мс");
            });
        }

        public void RemoveTask(string title)
        {
            ExceptionHandler.Run("RemoveTask", () =>
            {
                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Start,
                    0,
                    "Начало RemoveTask");

                Stopwatch sw = Stopwatch.StartNew();

                var task = _tasks.FirstOrDefault(t =>
                    t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

                if (task == null)
                {
                    sw.Stop();

                    Log.Error("Задача \"{TaskTitle}\" не найдена.", title);

                    Tracer.TaskManagerTrace.TraceEvent(
                        TraceEventType.Error,
                        400,
                        $"Ошибка RemoveTask. Задача \"{title}\" не найдена. Время: {sw.ElapsedMilliseconds} мс");

                    throw new InvalidOperationException($"Задача \"{title}\" не найдена");
                }

                _tasks.Remove(task);

                sw.Stop();

                Log.Information("Задача \"{TaskTitle}\" успешно удалена.", task.Title);

                Log.Information("Количество задач после удаления: {Count}", _tasks.Count);

                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Stop,
                    200,
                    $"Завершение RemoveTask. Удалена задача \"{task.Title}\". Время: {sw.ElapsedMilliseconds} мс");
            });
        }

        public void ListTasks()
        {
            ExceptionHandler.Run("ListTasks", () =>
            {
                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Start,
                    0,
                    "Начало ListTasks");

                Stopwatch sw = Stopwatch.StartNew();

                if (_tasks.Count == 0)
                {
                    sw.Stop();

                    Log.Information("Список задач пуст.");

                    Tracer.TaskManagerTrace.TraceEvent(
                        TraceEventType.Warning,
                        204,
                        $"Список задач пуст. Время: {sw.ElapsedMilliseconds} мс");

                    Console.WriteLine("Список задач пуст.");

                    return;
                }

                Console.WriteLine("\nСписок задач:");

                foreach (var task in _tasks)
                    Console.WriteLine($"- {task.Title}");

                sw.Stop();

                Log.Information("Показан список из {Count} задач", _tasks.Count);

                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Stop,
                    200,
                    $"Завершение ListTasks. Всего задач: {_tasks.Count}. Время: {sw.ElapsedMilliseconds} мс");
            });
        }
    }
}