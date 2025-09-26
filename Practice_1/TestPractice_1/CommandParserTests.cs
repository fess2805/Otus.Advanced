using Practice_1;

namespace TestPractice_1;
/*   Пункт 5. Написание Unit-тестов для парсера
   Создайте проект с юнит-тестами на xUnit. Напишите тесты для CommandParser, которые проверяют следующие сценарии:
   Корректный разбор команды SET с тремя аргументами.
   Корректный разбор команды GET с двумя аргументами.
   Обработка некорректной команды (например, без ключа).
   Обработка команды с лишними пробелами между аргументами.
 */
public class CommandParserTests
{
    [Fact]
    public void CommandParserSetCommandTest()
    {
        var resultParse = CommandParser.Parse("SET user:2 test".AsSpan());
        Assert.False(resultParse.Command.IsEmpty);
        Assert.False(resultParse.Key.IsEmpty);
        Assert.False(resultParse.Value.IsEmpty);
    }
    
    [Fact]
    public void CommandParserGetCommandTest()
    {
        var resultParse = CommandParser.Parse("GET user:5".AsSpan());
        Assert.False(resultParse.Command.IsEmpty);
        Assert.False(resultParse.Key.IsEmpty);
        Assert.True(resultParse.Value.IsEmpty);
    }
    
    [Fact]
    public void CommandParserManyBlanksTest()
    {
        var resultParse = CommandParser.Parse("   SET   user:2    test   ".AsSpan());
        Assert.False(resultParse.Command.IsEmpty);
        Assert.False(resultParse.Key.IsEmpty);
        Assert.False(resultParse.Value.IsEmpty);
    }
    
    [Fact]
    public void CommandParserWithoutKeyTest()
    {
        var resultParse = CommandParser.Parse("GET test_without_key".AsSpan());
        Assert.True(resultParse.Command.IsEmpty);
        Assert.True(resultParse.Key.IsEmpty);
        Assert.True(resultParse.Value.IsEmpty);
    }
}