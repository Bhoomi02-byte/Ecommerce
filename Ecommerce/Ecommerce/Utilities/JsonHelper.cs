using System.Text.Json;

public static class JsonHelper
{
    private static Dictionary<int, string> _messages;
    static JsonHelper()
    {
        LoadMessages();
    }
    private static void LoadMessages()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "responses.json");
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _messages = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
        }
        else
        {
            _messages = new Dictionary<int, string>();
        }
    }

    public static string GetMessage(int code, params object[] args)
    {
        if (_messages.TryGetValue(code, out var message))
        {
            return string.Format(message, args);
        }

        return "";
    }
}
