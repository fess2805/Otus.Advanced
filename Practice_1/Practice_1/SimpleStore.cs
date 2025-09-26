namespace Practice_1;

/*Пункт 1. Реализация базового хранилища SimpleStore
   Создайте класс SimpleStore. Внутри он должен использовать Dictionary<string, byte[]> для хранения данных. Реализуйте три публичных метода:
   void Set(string key, byte[] value): добавляет или обновляет значение по ключу.
   byte[] Get(string key): возвращает значение по ключу или null, если ключ не найден.
   void Delete(string key): удаляет ключ и значение.*/
public class SimpleStore
{
    private Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

    public void Set(string key, byte[] value)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (_store.ContainsKey(key)) _store[key] = value;
        else _store.Add(key, value);
    }

    public byte[] Get(string key)
    {
        return string.IsNullOrEmpty(key) ? null
            : _store.TryGetValue(key, out byte[] result) ? result : null;
    }
    
    public void Delete(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (_store.ContainsKey(key)) _store.Remove(key);
    }
}