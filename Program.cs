namespace DiaryApp;

class Program
{
    static void Main(string[] args)
    {
        UserManager.LoginOrRegister(); //Kullanıcı giriş ekranı
        Menu();
    }

    static void Menu()
    {
        while (true)
        {
            Console.Clear();
            string[] options = {
                "Yeni Kayıt Ekle",
                "Kayıtları Listele",
                "Tarihe Göre Kayıt Ara",
                "Tüm Kayıtları Sil",
                "Çıkış"
            };
            int inputOption = Helper.AskOption("MENU", options);
            switch (inputOption)
            {
                case 1:
                    DiaryHelper.AddNewRecord();
                    break;
                case 2:
                    DiaryHelper.ListRecords();
                    break;
                case 3:
                    DiaryHelper.FindRecordByDate();
                    break;
                case 4:
                    DiaryHelper.DeleteAllRecords();
                    break;
                case 5:
                    Console.WriteLine("Çıkılıyor...");
                    return;
            }
        }
    }
}