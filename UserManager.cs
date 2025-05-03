namespace DiaryApp;

public static class UserManager
{
     private static string usersFile = "users.txt";
    public static User CurrentUser { get; private set; }
    private static string encryptionKey = "maya123"; // Tüm kullanıcı bilgilerinde kullanılacak şifreleme anahtarı

    public static List<User> LoadUsers()
    {
        var users = new List<User>();
        if (!File.Exists(usersFile)) return users;

        var lines = File.ReadAllLines(usersFile);
        foreach (var line in lines)
        {
            try
            {
                var decrypted = CryptoHelper.Decrypt(line, encryptionKey);
                var parts = decrypted.Split('|');
                users.Add(new User
                {
                    Username = parts[0],
                    FullName = parts[1],
                    Password = parts[2],
                    SecurityQuestion = parts[3],
                    SecurityAnswer = parts[4],
                    CreatedAt = DateTime.Parse(parts[5])
                });
            }
            catch
            {
                
            }
        }
        return users;
    }

    public static void SaveUser(User user)
    {
        var line = $"{user.Username}|{user.FullName}|{user.Password}|{user.SecurityQuestion}|{user.SecurityAnswer}|{user.CreatedAt}";
        var encrypted = CryptoHelper.Encrypt(line, encryptionKey);
        File.AppendAllLines(usersFile, new[] { encrypted });
    }

    public static void LoginOrRegister()
    {
        while (true)
        {
            Console.Clear();
            string[] choices=
            {
            " Giriş Yap",
            " Kayıt Ol",
            " Şifremi Unuttum",
            " Çıkış",
            
            };
            
            Helper.AskOption("", choices);
            
            var secim = Helper.Ask("Seçiminiz");

            switch (secim)
            {
                case "1": if (Login()) return; break;
                case "2": Register(); break;
                case "3": ForgotPassword(); break;
                case "4": Environment.Exit(0); break;
                default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
            }
        }
    }

    private static bool Login()
    {
        var users = LoadUsers();
        string username = Helper.Ask("Kullanıcı adı:");
        string password = Helper.AskPassword("Şifre:");

        foreach (var user in users)
        {
            if (user.Username == username && user.Password == password)
            {
                CurrentUser = user;
                Helper.ShowSuccessMsg($"Hoş geldin {user.FullName}!");
                Thread.Sleep(1000);
                return true;
            }
        }

        Helper.ShowErrorMsg("Kullanıcı adı veya şifre hatalı!");
        Thread.Sleep(1500);
        return false;
    }

    private static void Register()
    {
        Console.Clear();
        string username = Helper.Ask("Yeni kullanıcı adı:");
        string fullname = Helper.Ask("Ad Soyad:");
        string password = Helper.AskPassword("Şifre:");
        string question = Helper.Ask("Güvenlik sorusu belirleyiniz:");
        string answer = Helper.Ask("Cevap:");

        var user = new User
        {
            Username = username,
            FullName = fullname,
            Password = password,
            SecurityQuestion = question,
            SecurityAnswer = answer,
            CreatedAt = DateTime.Now
        };

        SaveUser(user);
        Helper.ShowSuccessMsg("Kayıt başarılı!");
        Thread.Sleep(1000);
    }

    private static void ForgotPassword()
    {
        var users = LoadUsers();
        string username = Helper.Ask("Kullanıcı adınızı girin:");
        var user = users.FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            Helper.ShowErrorMsg("Kullanıcı bulunamadı.");
            Thread.Sleep(1500);
            return;
        }

        Console.WriteLine($"Güvenlik Sorusu: {user.SecurityQuestion}");
        string answer = Helper.Ask("Cevap:");
        if (answer == user.SecurityAnswer)
        {
            string newPass = Helper.AskPassword("Yeni şifre:");
            user.Password = newPass;
            
            var updatedUsers = users.Select(u =>
            {
                if (u.Username == username)
                    return user;
                return u;
            }).ToList();

            File.WriteAllLines(usersFile, updatedUsers.Select(u =>
            {
                var line = $"{u.Username}|{u.FullName}|{u.Password}|{u.SecurityQuestion}|{u.SecurityAnswer}|{u.CreatedAt}";
                return CryptoHelper.Encrypt(line, encryptionKey);
            }));

            Helper.ShowSuccessMsg("Şifreniz başarıyla güncellendi.");
            Thread.Sleep(1500);
        }
        else
        {
            Helper.ShowErrorMsg("Cevap yanlış!");
            Thread.Sleep(1500);
        }
    }
}