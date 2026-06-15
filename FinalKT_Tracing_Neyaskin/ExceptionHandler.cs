using Serilog;

public static class ExceptionHandler
{
    public static void Run(string operationDescription, Action action)
    {
        try
        {
            Log.Verbose("Начало операции {Operation}", operationDescription);

            action();

            Log.Verbose("Конец операции {Operation}", operationDescription);
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Ошибка во время операции {Operation}. Текст ошибки: {ErrorMessage}",
                operationDescription,
                ex.Message);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Произошла ошибка. Подробнее см. логи.");
            Console.ResetColor();
        }
    }
}