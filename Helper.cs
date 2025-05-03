namespace DiaryApp;

public class Helper
{
    public static void ShowColoredMsg(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static void ShowSuccessMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Green);
    } 
    public static void ShowErrorMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Red);
    }
    
    
    public static string? Ask(string question, bool isRequired = false, string validationMsg = "Bu alanı boş bırakamazsın.")
    {
        string? response;
        do
        {
            Console.Write($"{question}");
            response = Console.ReadLine();

            if (isRequired && string.IsNullOrWhiteSpace(response))
            {
                ShowErrorMsg(validationMsg);
            }
            
        } while (isRequired && string.IsNullOrWhiteSpace(response));
        
        return response?.Trim();
    }

    public static int AskNumber(string question, string validationMsg = "Bir sayı girmelisin.")
    {
        while (true)
        {
            var response = Ask(question, true);
            if (int.TryParse(response, out var result))
            {
                return result;
            }
            ShowErrorMsg(validationMsg);
        }
    }

    public static int AskOption(string question, string[] options)
    {
        if (options.Length == 0)
        {
            throw new ArgumentException($"{nameof(options)} içinde seçenekler olmalı.", nameof(options));
        }

        Console.WriteLine(question);
        
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }

        while (true)
        {
            var inputResponse = AskNumber($"Seçiminiz (1-{options.Length}) ");
            if (inputResponse >= 1 && inputResponse <= options.Length)
            {
                return inputResponse;
            }
    
            ShowErrorMsg("Hatalı seçim yaptın.");
        }
    }  
    
    
    
    public static string AskPassword(string question, string validationMsg = "Bu alanı boş bırakamazsın.")
    {
        var password = "";
        do
        {
            Console.Write($"{question}: ");
            password = ReadSecretLine();
            
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMsg(validationMsg);
            }
            
        } while (string.IsNullOrWhiteSpace(password));
        
        return password;
    }
    private static string ReadSecretLine()
    {
        var line = "";
        ConsoleKeyInfo key;
        do
        {
            key =  Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            if (key.Key == ConsoleKey.Backspace && line.Length > 0)
            {
                line = line.Substring(0, line.Length - 1);
                Console.Write("\b \b");
                continue;
            }
            
            if (!IsSecurePassChar(key.KeyChar))
            {
                continue;
            }
            
            line += key.KeyChar;

            Console.Write(key.KeyChar);
            Thread.Sleep(75);
            Console.Write("\b*");

        } while (true);
        /*
        Console.WriteLine("\n");
        return line.Trim();
        */
        Console.WriteLine();
        return line;
    }
    private static bool IsSecurePassChar(char c)
    {
        return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || char.IsWhiteSpace(c);
    }
}