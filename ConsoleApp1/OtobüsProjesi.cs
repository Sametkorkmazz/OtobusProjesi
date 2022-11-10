using System;

namespace ConsoleApp1
{
    class Yolcu // Yolcu Sınıfı oluşturma 
    {
        public string[] isimListesi; // 40 yolcunun isimlerini ve yakınlıklarını tutucak diziler
        public double[,] DM;
        public double[] koltukUzakliklari = new double[40];
        public int[] koltukYolcuNumaraları = new int[40];

        public Yolcu(string[] liste, double[,] dM)
        {
            this.isimListesi = liste;
            DM = dM;
        }
    }

    class Otobüs // Otobüs Sınıfı
    {
        static Random random = new Random();
        static Yolcu yolcular; // Yolcu sınıfından yolcular nesnesini üretme

        public static int[] ayaktakiYolcular; /* Bu liste zaten oturmuş ve yerleştirilen koltukla bağlantısı olmayan yolcular için uzaklık bakılmaması içindir.
                                               0 dan 39 a kadar sayılar içericektir ve bir yolcu oturduğunda o yolcunun numarası bu listeden çıkarılır */

        public static int bosKoltukSayisi = 40; /* Boş koltuk sayısı ayaktaki yolcuların sayısını vermektedir.
                                                 Uzaklık matrisini gezmek için oluşturulan döngü ayaktaki yolcular kadar tekrar edicektir.*/

        public static double[,] dmK; // Uzaklık matrisinin kopyası. Program boyunca uzaklık matrisinde değişikler yapılacaktır bu yüzden orjinali bu kopyada tutulacak.

        public static int doluKoltukSayisi = 0; // Bu değer, oturan yolcuların ilgili koltuklara yakınlıklarını içeren diziye index görevi görecektir.

        public Otobüs(string[] liste) // Otobüs sınıfı constructor 
        {
            yolcular = new Yolcu(liste, uzakliklariOlustur(random)); /* yolcular nesnesini oluşturma. Bu nesne 40 elemanlı string isim listesi
                                                                      ve 40*40 elemanlı double uzaklık matrisini içericektir. */
            ayaktakiYolcular = new int[40];
            for (int i = 0; i < 40; i++) // ayaktakiYolcular dizisine 0 dan 39 a kadar yolcu numaraları atama
            {
                ayaktakiYolcular[i] = i;
            }

            for (int i = 0; i < 40; i++)
            {
                yolcular.koltukUzakliklari[i] = 0;
            }

            dmK = yolcular.DM;
        }

        static double[,] uzakliklariOlustur(Random random) // 40*40 double uzaklık matrisi DM oluşturan metot
        {
            double[,] DM = new double[40, 40];
            double uzaklik;
            for (int satir = 0; satir < 40; satir++) // En dış döngü matrisin 40 satırı için tekrar eder
            {
                // ikili döngüsü karşılıklı kişilerin ( (5,3) , (3,5) gibi) birbirlerine uzaklıklarını aynı  yapar.
                //Bunun için bulunduğu satırın, satır numarası ile aynı olan sütüna kadarki değerlerini önceki satırlarlardan alır. 
                // 5. satırdaki 5. sütüna kadarki değerleri (5,1) = (1,5) , (4,2) = (2,4) gibi önceden hazırlanan satırlardan alır.
                for (int ikili = 0; ikili < satir; ikili++)
                {
                    DM[satir, ikili] = DM[ikili, satir];
                }

                for (int sutun = satir; sutun < 40; sutun++)
                {
                    while (true)
                    {
                        uzaklik = random.NextDouble() *
                                  9; // 0 ile 10 arasında random double değerleri oluşturma. 0 oluşturulmaması için while döngüsü kullanıldı.
                        if (uzaklik != 0)
                        {
                            break;
                        }
                    }

                    DM[satir, sutun] = uzaklik;
                }

                DM[satir, satir] = 0; // satir ve sütün sayısı aynı olan kutuları 0 yapar. (7,7) = 0 gibi
            }

            return DM;
        }


        static string[] oturmaPlaniOlustur() // Yolcuları koltuklara yakınlıklarına göre yerleştiren ana metot
        {
            string[] oturmaPlani = new string[40]; // Yolcu isimlerini oturma sıralarına göre tutucak 40 elemanlı dizi
            int yandakiYolcu =
                random.Next(40); // İlk oturan yolcu random seçilir. yandakiYolcu değeri sol koltuktaki yolcuyu tutar.
            int[]
                onKoltukNumaraları =
                    new int[4]; /* Bu dizi, o anda yolcu yerleştirilecek koltuğun önündeki 4 koltuktaki oturan yolcuların numaralarını tutar.
                                                    Otobüsün en önündeki 4 koltuk hariç bu dizi ön, sağ çarpraz , sol çarpraz gibi yakınlık değerlerini bulmakta kullanılacaktır.*/
            onKoltukNumaraları[0] = yandakiYolcu;
            oturmaPlani[0] = yolcular.isimListesi[yandakiYolcu]; // oturma planına ilk yolcu çizilir.
            oturanYolcuyuÇıkart(yandakiYolcu,
                0); /* oturanYolcuyuÇıkart metotu oturan ve o anda bakılan koltuk için bağı olmayan yolcuları uzaklık matrisinde aramamak için yazıldı.
                                                                Bu metot ayaktakiYolcular dizisinden oturan yolcuları çıkartır         */
            for (int i = 1; i < 4; i++) // En ön 4 koltuk yerleşimi
            {
                yandakiYolcu =
                    enOnKoltukYerlesimi(yandakiYolcu); // yandakiYolcu değeri en son oturan yolcu numarasıdır.
                // Önce metota kendisine en yakın yolcunun bulunması için verilir sonra ise bu metot ile bulunan ve o koltuğa oturan yolcu değerini alir.

                oturmaPlani[i] = yolcular.isimListesi[yandakiYolcu];
                onKoltukNumaraları[i] = yandakiYolcu;
            }

            int yerlesilenKoltuk = 4;
            for (int sıra = 0;
                 sıra < 9;
                 sıra++) // Bu döngü en öndeki dörtlü sıra yerleşildikten sonra kalan 9 sıra koltuk için 9 kez tekrar eder.
            {
                onKoltukNumaraları =
                    arkaKoltukYerlesimi(
                        onKoltukNumaraları); // onKoltukNumaraları dizisi yandakiYolcu gibi, sırasıyla yakınlık değerleri aranan ardından dörtlü koltuğa yerleşen yolcuların numaralarını içerir.
                for (int j = 0; j < 4; j++) // 4 koltuğa sırayla yerleşme
                {
                    oturmaPlani[yerlesilenKoltuk] = yolcular.isimListesi[onKoltukNumaraları[j]];
                    yerlesilenKoltuk++;
                }
            }

            return oturmaPlani;
        }

        static int enOnKoltukYerlesimi(int yandakiYolcu) // En öndeki 4 koltuk için yolcu yerleştirme metotu.
        {
            double enKucukSayi = 10;
            int oturtulcakKişi = 0;
            int çıkarılcakKişi = 0;
            for (int i = 0; i < bosKoltukSayisi; i++) // Bu döngü ayaktaki yolcuların sayısı kadar tekrar eder.
            {
                if (enKucukSayi >
                    yolcular.DM
                    [ayaktakiYolcular[i],
                        yandakiYolcu]) // En ön koltuklar olduğu için sadece soldaki yolcuya en yakın yolcu aranır.
                {
                    // Ayaktaki yolcular dizisi ,oturan yolcuların numaralarını içermediği için uzaklık matrisinde bu yolculara bakmaya gerek kalmaz.
                    enKucukSayi = yolcular.DM[ayaktakiYolcular[i], yandakiYolcu];
                    oturtulcakKişi = ayaktakiYolcular[i];
                    çıkarılcakKişi = i;
                }
            }

            oturanYolcuyuÇıkart(çıkarılcakKişi, enKucukSayi); // Oturan yolcu ayaktakiYolcular dizisinden çıkartılır.
            return oturtulcakKişi;
        }

        static int[] arkaKoltukYerlesimi(int[] onKoltukYolcuları)
        // En ön 4 koltuktan sonra 9 sıra koltuk için yerleşimi yapan metot.
        {
            int ÇıkarılcakYolcu = 0;
            double enKucukDeger = 150;
            double toplam;
            int[] YerleşilenSıraYolcuları = new int[4]; // Dörtlü koltuğa sırasıyla soldan sağa  oturan yolcuların numaralarını tututak dizi.
            for (int KoltukNumarası = 0; KoltukNumarası < bosKoltukSayisi; KoltukNumarası++) // onKoltukYolcuları dizisi sırasıyla soldan sağa oturanları içerir. 0. index en soldaki 3. index en sağdaki yolcudur.
            {
                toplam = yolcular.DM[ayaktakiYolcular[KoltukNumarası], onKoltukYolcuları[0]] + yolcular.DM[ayaktakiYolcular[KoltukNumarası], onKoltukYolcuları[1]]; // En sola yolcu yerleştirken sadece üst ve sağ çarprazına bakılır.
                if (enKucukDeger > toplam)
                {
                    enKucukDeger = toplam;
                    YerleşilenSıraYolcuları[0] = ayaktakiYolcular[KoltukNumarası];
                    ÇıkarılcakYolcu = KoltukNumarası;
                }
            }

            oturanYolcuyuÇıkart(ÇıkarılcakYolcu, enKucukDeger);
            int yandakiYolcu = YerleşilenSıraYolcuları[0];
            for (int KoltukNumarası = 1; KoltukNumarası < 3; KoltukNumarası++) // Dörtlü sırada ,soldan 2. ve 3. koltukların yerleşim hesapları benzediği için bu döngu iki koltuk için sırasıyla hesap yapar.
            {
                enKucukDeger = 150; // Hesaplamada , ayaktaki yolcuların ön dörtlükte oturan belli yolculara uzaklıkları toplamı karşılaştırılır
                for (int YolcuNumarası = 0; YolcuNumarası < bosKoltukSayisi; YolcuNumarası++) // Bu hesaplamada soldan 2. ve 3. koltuk için, onKoltukYolcuları dizisinden ön , sol çarpraz ve sağ çarprazda oturan yolcuların numaraları alınır.
                {
                    toplam = yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası - 1]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası + 1]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası], yandakiYolcu]; // Soldaki yolcu da hesaplamaya katılır
                    if (enKucukDeger > toplam)
                    {
                        enKucukDeger = toplam;
                        YerleşilenSıraYolcuları[KoltukNumarası] = ayaktakiYolcular[YolcuNumarası];
                        ÇıkarılcakYolcu = YolcuNumarası;
                    }
                }

                yandakiYolcu = YerleşilenSıraYolcuları[KoltukNumarası];
                oturanYolcuyuÇıkart(ÇıkarılcakYolcu, enKucukDeger);
            }

            enKucukDeger = 150;
            for (int KoltukNumarası = 0; KoltukNumarası < bosKoltukSayisi; KoltukNumarası++) // Bu döngü dörtlü sırada en sağta oturtulcak kişi içindir. Bu kişi için sağ çarpraz bakılmaz.
            {
                toplam = yolcular.DM[ayaktakiYolcular[KoltukNumarası], yandakiYolcu] +
                         yolcular.DM[ayaktakiYolcular[KoltukNumarası], onKoltukYolcuları[2]] +
                         yolcular.DM[ayaktakiYolcular[KoltukNumarası], onKoltukYolcuları[3]];
                if (enKucukDeger > toplam)
                {
                    enKucukDeger = toplam;
                    YerleşilenSıraYolcuları[3] = ayaktakiYolcular[KoltukNumarası];
                    ÇıkarılcakYolcu = KoltukNumarası;
                }
            }

            oturanYolcuyuÇıkart(ÇıkarılcakYolcu, enKucukDeger);
            return YerleşilenSıraYolcuları; // Dörtlü sıraya oturan yolcu listesini döndürür.
        }

        static void oturanYolcuyuÇıkart(int çıkarılanYolcu, double enKucukDeger)
        // Bu method önceden oturmuş ve bakılan koltuk için ilişkisi olmayan yolcuları uzaklık matrisinde aramamak içindir.
        {
            yolcular.koltukYolcuNumaraları[doluKoltukSayisi] = ayaktakiYolcular[çıkarılanYolcu]; // Bakılan koltuğa oturan yolcunun numarasını kaydetme
            if (çıkarılanYolcu != 39) // ayaktakiYolcular dizisinde 0 dan 39 a kadar yolcu numaraları vardır.
                                      // Oturan yolcuyu çıkartırken, oturan yolcunun numarasının bulunduğu dizi elemanına bir sağındaki elemanın değerini atar. Bu işlem sağdaki elemanlar için de yapılır. 
            {
                for (int i = çıkarılanYolcu; i < bosKoltukSayisi - 1; i++)
                {
                    ayaktakiYolcular[i] = ayaktakiYolcular[i + 1];
                }
            }


            yolcular.koltukUzakliklari[doluKoltukSayisi] = enKucukDeger; // Oturtulan yolcunun ilgili koltuklara uzaklığını içeren dizi.
            doluKoltukSayisi++;
            bosKoltukSayisi--;
        }

        public void yolculariYerlestir()
        {
            // 40 yolcu oturmaPlaniOlustur metotu ile oturmaPlani dizisine atılır
            string[] oturmaPlani = oturmaPlaniOlustur();
            for (int i = 0; i < 40; i += 4) // Koltukta oturan yolcunun ismi ve numarası sırayla yazılır. Burda uzaklıklar yazılmaz

            {
                for (int j = i; j < i + 4; j++) // string.format metotları ile veriler sağdan sola alt alta okunaklı yazılır. Double sayilar virgülden sonra 2 basamak olucak şekilde yazılır.
                {
                    Console.Write(string.Format("{0,15}", yolcular.koltukYolcuNumaraları[j]));
                    if (j == i + 1)
                    {
                        Console.Write("\t\t");
                    }
                }

                Console.WriteLine();
                for (int j = i; j < i + 4; j++)
                {
                    Console.Write(string.Format("{0,15}", oturmaPlani[j]));
                    if (j == i + 1)
                    {
                        Console.Write("\t\t");
                    }
                }

                Console.Write("\n\n");
            }

            Console.Write("Uzakliklari ile: \n\n\n");
            for (int i = 0; i < 40; i += 4)
            // Burda yolcu numaraları ,isimleri ve  uzaklıkları da yazılır.

            {
                for (int j = i; j < i + 4; j++)
                {
                    Console.Write(string.Format("{0,15}", yolcular.koltukYolcuNumaraları[j]));
                    if (j == i + 1)
                    {
                        Console.Write("\t\t");
                    }
                }

                Console.Write("\n\n");
                for (int j = i; j < i + 4; j++)
                {
                    Console.Write(string.Format("{0,15:0.##}", yolcular.koltukUzakliklari[j]));
                    if (j == i + 1)
                    {
                        Console.Write("\t\t");
                    }
                }

                Console.WriteLine();
                for (int j = i; j < i + 4; j++)
                {
                    Console.Write(string.Format("{0,15}", oturmaPlani[j]));
                    if (j == i + 1)
                    {
                        Console.Write("\t\t");
                    }
                }

                Console.Write("\n\n");
            }

            double toplamUzaklik = 0;
            for (int i = 0; i < 40; i++)
            {
                toplamUzaklik += yolcular.koltukUzakliklari[i];
            }

            Console.WriteLine(string.Format("Tüm Yolcuların Uzaklıklar Toplamı: {0:0.##}", toplamUzaklik));
        }
    }

    internal class OtobüsProjesi
    {
        static void Main(string[] args)
        {
            string liste = "İSMAİL SAMET YAHYA GAMZEGÜL MEVLÜT DERYA KAAN BÜŞRA HANDE İLKER NURGÜL MÜMÜN ŞEVKET MERVE YAĞMUR HALİL NAİL AHMET DİDEM YENER SEDA AYŞE FATMA ALİ SEÇKİN EKREM DEFNE RIDVAN DUHAN ABDULMELİK YÜCEL REŞAT ERDAL SERHAN FERHAN SAMİME AHMET GÜLŞAH ERTUNÇ FARUK";
            string[] isimListesi = liste.Split(' ');
            // Yukardaki büyük stringde her boşluk gördüğünde boşluktan önceki kelimeyi bu diziye atar.

            Otobüs şoför = new Otobüs(isimListesi); // Otobüs sınıfından şoför nesnesi oluşturma.
            şoför.yolculariYerlestir(); // Otobüs sınıfının ana metotu çağırma
        }
    }
}