namespace DiaryApp;

public class DiaryHelper
{
   static string fileName = UserManager.CurrentUser.FileName;
    public static void AddNewRecord()
    {
        Console.Clear();
        string password = "maya123"; 
        string todayKey = DateTime.Now.ToString("dd MMMM yyyy");

        bool alreadyExists = false; //Zaten kayıt var mı kontrol etmek için

        if (File.Exists(fileName))
        {
            string[] lines = File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                try
                {
                    string decrypted = CryptoHelper.Decrypt(line, password);
                    if (decrypted.StartsWith(todayKey))
                    {
                        alreadyExists = true;
                        break;
                    }
                }
                catch
                {
                    
                }
            }
        }
//Daha önce bu tarihte bir kayıt eklenmiş ise
        if (alreadyExists)
        {
            string confirm = Helper.Ask("Bugün için kayıt girildi, aynı tarihte yeni bir kayıt eklemek ister misin? (e/h)").ToLower();
            if (confirm != "e")
            {
                Helper.ShowErrorMsg("Kayıt iptal edildi.");
                Thread.Sleep(1500);
                return;
            }
        }
        // Giriş ekranı - siyah yazı, beyaz arka plan
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("Dear Diary...");
        string content = Console.ReadLine();
        Console.ResetColor();

        string date = DateTime.Now.ToString("dd MMMM yyyy"); // Eksik tanımı ekledik

        string encryptedDate = CryptoHelper.Encrypt(date, password);
        string encryptedContent = CryptoHelper.Encrypt(content, password);
        //Dosyaya şirelenmiş metni ekleme 
        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(encryptedDate);
            sw.WriteLine(encryptedContent);
            sw.WriteLine("-------------");
        }

        Helper.ShowSuccessMsg("Kayıt başarılı. Devam etmek için bir tuşa basın...");
        Console.ReadKey();
    }


   public static void ListRecords()
{
    Console.Clear();
    string password = "maya123"; // Şifreleme parolası

    if (!File.Exists(fileName))
    {   //Dosya içerisi boş ise 
        Helper.ShowErrorMsg("Kayıt bulunamadı...");
        Console.WriteLine("Ana menüye dönmek için bir tuşa basın...");
        Console.ReadKey();
        return;
    }

    var allLines = File.ReadAllLines(fileName);
    var records = new List<List<string>>();
    var current = new List<string>();

    foreach (var line in allLines)
    {
        if (line == "-------------")
        {
            if (current.Count == 2)
            {
                try
                {
                    string decryptedDate = CryptoHelper.Decrypt(current[0], password);
                    string decryptedContent = CryptoHelper.Decrypt(current[1], password);
                    records.Add(new List<string> { decryptedDate, decryptedContent });
                }
                catch
                {
                   
                }
            }
            current.Clear();
        }
        else
        {
            current.Add(line);
        }
    }

    if (records.Count == 0)
    {
        Helper.ShowErrorMsg("Kayıt bulunamadı.");
        Thread.Sleep(3000);
        return;
    }

    int index = 0;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("DAILY RECORD:");
        Console.WriteLine("------------");
        Console.WriteLine(records[index][0]); // Tarih
        Console.WriteLine(records[index][1]); // İçerik

        Console.WriteLine("\n1 - Sonraki kayıt | 2 - Düzenle | 3 - Sil | 4 - Ana menü");
        var secim = Helper.Ask("Seçiminiz");

        switch (secim)
        {
            case "1":
                index = (index + 1) % records.Count;
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("Eski metin:");
                Console.WriteLine(records[index][1]);
                var yeniIcerik = Helper.Ask("Yeni metni girin", true);
                records[index][1] = yeniIcerik;
                RewriteAllEncrypted(records, password);
                Helper.ShowSuccessMsg("Güncellenme Başarılı.");
                Thread.Sleep(1000);
                break;

            case "3":
                var inputOk = Helper.Ask("Bu kaydı silmek istediğinize emin misiniz? (e/h)").ToLower();
                if (inputOk == "e")
                {
                    records.RemoveAt(index);
                    if (index >= records.Count)
                        index = 0;
                    RewriteAllEncrypted(records, password);
                    Helper.ShowSuccessMsg("Kayıt silindi.");
                    Console.ReadKey();

                    if (records.Count == 0)
                    {
                        Helper.ShowErrorMsg("Silinecek kayıt yok.");
                        Console.ReadKey();
                        return;
                    }
                }
                break;

            case "4":
                return;

            default:
                Helper.ShowErrorMsg("Geçersiz seçim.");
                break;
        }
    }
}

public static void FindRecordByDate()
{
    Console.Clear();
    string password = "maya123"; // Şifre çözme parolası
    string inputDate = Helper.Ask("Aramak istediğiniz tarihi girin (örn. 01 Mayıs 2025):");

    if (!File.Exists(fileName))
    {
        Helper.ShowErrorMsg("Dosya bulunamadı.");
        Console.ReadKey();
        return;
    }

    var allLines = File.ReadAllLines(fileName);
    var current = new List<string>();
    bool found = false;

    foreach (var line in allLines)
    {
        if (line == "-------------")
        {
            if (current.Count == 2)
            {
                try
                {
                    string decryptedDate = CryptoHelper.Decrypt(current[0], password);
                    string decryptedContent = CryptoHelper.Decrypt(current[1], password);

                    if (decryptedDate == inputDate)
                    {
                        Console.Clear();
                        Console.WriteLine("TARİH: " + decryptedDate);
                        Console.WriteLine("İÇERİK:\n" + decryptedContent);
                        found = true;
                        break;
                    }
                }
                catch
                {
                   
                }
            }
            current.Clear();
        }
        else
        {
            current.Add(line);
        }
    }

    if (!found)
    {
        Helper.ShowErrorMsg("Bu tarihe ait kayıt bulunamadı.");
    }

    Console.WriteLine("\nAna menüye dönmek için bir tuşa basın...");
    Console.ReadKey();
}


    public static void RewriteAllEncrypted(List<List<string>> records, string password)
    {
        using (StreamWriter sw = new StreamWriter(fileName, false))
        {
            foreach (var record in records)
            {
                string encryptedDate = CryptoHelper.Encrypt(record[0], password);
                string encryptedContent = CryptoHelper.Encrypt(record[1], password);
                sw.WriteLine(encryptedDate);
                sw.WriteLine(encryptedContent);
                sw.WriteLine("-------------"); // her metnin altında bu satır var bu satıra göre listeleme yapılıyor
            }
        }
    }

public static void AddFakeRecords()
{
    var fakeDates = new List<DateTime>
    {
        new DateTime(2025, 5, 1),
        new DateTime(2025, 4, 30),
        new DateTime(2025, 4, 29)
    };

    var fakeEntries = new List<string>
    {
        "Dear Diary fkasgflkahfşafşa",
        "Dear Diary sjfksjfgkafa",
        "Dear Diary kfgkagfkaka"
    };

    for (int i = 0; i < fakeDates.Count; i++)
    {
        string formattedDate = fakeDates[i].ToString("dd MMMM yyyy");

        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(formattedDate);  // SADECE gün-ay-yıl
            sw.WriteLine(fakeEntries[i]);
            sw.WriteLine("-------------");
        }
    }
}
public static void DeleteAllRecords()
    {
        Console.Clear();
        Helper.ShowColoredMsg("Tüm kayıtları silmek istediğinize emin misiniz? (e/h):", ConsoleColor.Yellow);
       
        string inputOk = Console.ReadLine().ToLower();

        if (inputOk == "e")
        {
            File.WriteAllText(fileName, string.Empty);
            Helper.ShowSuccessMsg("Tüm kayıtlar silindi");
        }
        else
        {
            Helper.ShowErrorMsg("Silme işlemi iptal edildi");
        }

        Console.WriteLine("Devam etmek için bir tuşa basın...");
        Console.ReadKey();
    }


}