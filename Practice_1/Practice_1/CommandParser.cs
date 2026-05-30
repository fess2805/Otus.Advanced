using System.Runtime.InteropServices;

namespace Practice_1;

/*
 Пункт 2. Проектирование парсера команд
   Создайте статический класс CommandParser. 
   В нем создайте публичный статический метод Parse, который принимает ReadOnlySpan 
   и возвращает кортеж из трех элементов: (ReadOnlySpan Command, ReadOnlySpan Key, ReadOnlySpan Value). 
   Этот метод будет отвечать за разбор команды.
   
   Пункт 3. Реализация логики парсинга
   Реализуйте логику метода Parse. Он должен разбирать входящую последовательность байт, 
   представляющую собой строку вида "COMMAND KEY VALUE" (например, "SET user:1 data"). 
   Команда, ключ и значение разделены пробелами. Необходимо использовать методы IndexOf и Slice для Span, 
   чтобы извлечь три компоненты без создания новых строк или массивов.
   
   Пункт 4. Обработка команд с разным числом аргументов
   Модифицируйте парсер так, чтобы он корректно обрабатывал команды с разным количеством аргументов. 
   Например, команда GET user:1 содержит только команду и ключ. В этом случае Value в возвращаемом кортеже 
   должен быть пустым (.IsEmpty). Если команда некорректна (например, отсутствует ключ), 
   метод должен возвращать пустой кортеж по умолчанию.
   
   Пункт 5. Написание Unit-тестов для парсера
   Создайте проект с юнит-тестами на xUnit. Напишите тесты для CommandParser, которые проверяют следующие сценарии:
   Корректный разбор команды SET с тремя аргументами.
   Корректный разбор команды GET с двумя аргументами.
   Обработка некорректной команды (например, без ключа).
   Обработка команды с лишними пробелами между аргументами.
 */

public static class CommandParser
{

    public static CommandParserResult<char> Parse(ReadOnlySpan<byte> input)
    {
        var charSpan = MemoryMarshal.Cast<byte, char>(input);
        return Parse(charSpan);
    }

    public static CommandParserResult<char> Parse(ReadOnlySpan<char> input)
    {
        if (input.Length < 3) return CommandParserResult<char>.Default;
        var result = CommandParserResult<char>.Default;
        while (input.Length > 3)
        {
            var blankPosition = input.IndexOf(' ');
            if (blankPosition < 1 && !result.Key.IsEmpty)
            {
                result.Value = input;
                break;
            }
            if (blankPosition == -1 || blankPosition > 2)
            {
                if (result.Command.IsEmpty) result.Command = blankPosition > 0 
                    ? input.Slice(0, blankPosition) : input.Slice(0);
                else
                {
                    var checkingValue = blankPosition > 0 
                        ? input.Slice(0, blankPosition) : input.Slice(0);
                    if (CheckKey(checkingValue)) result.Key = checkingValue;
                    else result.Value = checkingValue;
                }
            }
            input = blankPosition != -1 ? input.Slice(blankPosition + 1) : input.Slice(input.Length);
        }
        return result.Key.IsEmpty ? CommandParserResult<char>.Default : result;
    }

    internal static bool CheckKey (ReadOnlySpan<char> chekingValue)
    {
        return chekingValue.IndexOf(':') > 0;
    }
}

public ref struct CommandParserResult<T>
{
    
    
    public CommandParserResult()
    {
        Command = default;
        Key = default;
        
    }

    public static CommandParserResult<T> Default => new CommandParserResult<T>()
    {
        Command = ReadOnlySpan<T>.Empty, Value = ReadOnlySpan<T>.Empty, Key = ReadOnlySpan<T>.Empty
    };
    public ReadOnlySpan<T> Command { get; set; }
    public ReadOnlySpan<T> Key { get; set; }
    public ReadOnlySpan<T> Value { get; set; }
}