using System;

namespace ConsoleApp1
{
    class Yolcu
    {
        public string[] isimListesi;
        public double[,] DM;
        public double[] koltukUzakliklari = new double[40];
        public int[] koltukYolcuNumaraları = new int[40];

        public Yolcu(string[] liste, double[,] dM)
        {
            this.isimListesi = liste;
            DM = dM;
        }
    }

    class Otobüs
    {
        static Random random = new Random();
        static Yolcu yolcular;

        public static int[]
            ayaktakiYolcular;

        public static int bosKoltukSayisi = 40;

        public static double[,] dmK;

        public static int doluKoltukSayisi = 0;

        public Otobüs(string[] liste)
        {
            yolcular = new Yolcu(liste, uzakliklariOlustur(random));
            ayaktakiYolcular = new int[40];
            for (int i = 0; i < 40; i++)
            {
                ayaktakiYolcular[i] = i;
            }

            for (int i = 0; i < 40; i++)
            {
                yolcular.koltukUzakliklari[i] = 0;
            }

            dmK = yolcular.DM;
        }

        static double[,] uzakliklariOlustur(Random random)
        {
            double[,] DM = new double[40, 40];
            double uzaklik;
            for (int satir = 0; satir < 40; satir++)
            {
                for (int ikili = 0; ikili < satir; ikili++)
                {
                    DM[satir, ikili] = DM[ikili, satir];
                }

                for (int sutun = satir; sutun < 40; sutun++)
                {
                    while (true)
                    {
                        uzaklik = random.NextDouble() *
                                  9;
                        if (uzaklik != 0)
                        {
                            break;
                        }
                    }

                    DM[satir, sutun] = uzaklik;
                }

                DM[satir, satir] = 0;
            }


            return DM;
        }

        public void yolculariYerlestir()
        {
            string[] oturmaPlani = oturmaPlaniOlustur();
            for (int i = 0; i < 40; i += 4)
            {
                for (int j = i; j < i + 4; j++)
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
            Console.WriteLine();
        }

        static string[] oturmaPlaniOlustur()
        {
            string[] oturmaPlani = new string[40];
            int yandakiYolcu =
                random.Next(40);
            int[]
                onKoltukNumaraları =
                    new int[4];
            onKoltukNumaraları[0] = yandakiYolcu;
            oturmaPlani[0] = yolcular.isimListesi[yandakiYolcu];
            oturanYolcuyuÇıkart(yandakiYolcu,
                0);
            for (int i = 1; i < 4; i++)
            {
                yandakiYolcu =
                    enOnKoltukYerlesimi(yandakiYolcu);

                oturmaPlani[i] = yolcular.isimListesi[yandakiYolcu];
                onKoltukNumaraları[i] = yandakiYolcu;
            }

            int yerlesilenKoltuk = 4;
            for (int sıra = 0;
                 sıra < 9;
                 sıra++)
            {
                onKoltukNumaraları =
                    arkaKoltukYerlesimi(
                        onKoltukNumaraları);
                for (int j = 0; j < 4; j++)
                {
                    oturmaPlani[yerlesilenKoltuk] = yolcular.isimListesi[onKoltukNumaraları[j]];
                    yerlesilenKoltuk++;
                }
            }

            return oturmaPlani;
        }

        static int enOnKoltukYerlesimi(int yandakiYolcu)
        {
            double enKucukSayi = 10;
            int oturtulcakKişi = 0;
            int çıkarılcakKişi = 0;
            for (int i = 0; i < bosKoltukSayisi; i++)
            {
                if (enKucukSayi >
                    yolcular.DM
                    [ayaktakiYolcular[i],
                        yandakiYolcu])
                {
                    enKucukSayi = yolcular.DM[ayaktakiYolcular[i], yandakiYolcu];
                    oturtulcakKişi = ayaktakiYolcular[i];
                    çıkarılcakKişi = i;
                }
            }

            oturanYolcuyuÇıkart(çıkarılcakKişi, enKucukSayi);
            return oturtulcakKişi;
        }

        static int[] arkaKoltukYerlesimi(int[] onKoltukYolcuları)
        {
            int ÇıkarılcakYolcu = 0;
            double enKucukDeger = 150;
            double toplam;
            int[]
                YerleşilenSıraYolcuları =
                    new int[4];
            for (int KoltukNumarası = 0;
                 KoltukNumarası < bosKoltukSayisi;
                 KoltukNumarası++)
            {
                toplam = yolcular.DM[ayaktakiYolcular[KoltukNumarası], onKoltukYolcuları[0]] +
                         yolcular.DM
                         [ayaktakiYolcular[KoltukNumarası],
                             onKoltukYolcuları[1]];
                if (enKucukDeger > toplam)
                {
                    enKucukDeger = toplam;
                    YerleşilenSıraYolcuları[0] = ayaktakiYolcular[KoltukNumarası];
                    ÇıkarılcakYolcu = KoltukNumarası;
                }
            }

            oturanYolcuyuÇıkart(ÇıkarılcakYolcu, enKucukDeger);
            int yandakiYolcu = YerleşilenSıraYolcuları[0];
            for (int KoltukNumarası = 1;
                 KoltukNumarası < 3;
                 KoltukNumarası++)
            {
                enKucukDeger =
                    150;
                for (int YolcuNumarası = 0;
                     YolcuNumarası < bosKoltukSayisi;
                     YolcuNumarası++)
                {
                    toplam = yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası - 1]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası], onKoltukYolcuları[KoltukNumarası + 1]] +
                             yolcular.DM[ayaktakiYolcular[YolcuNumarası],
                                 yandakiYolcu];
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
            for (int KoltukNumarası = 0;
                 KoltukNumarası < bosKoltukSayisi;
                 KoltukNumarası++)
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
            return YerleşilenSıraYolcuları;
        }

        static void oturanYolcuyuÇıkart(int çıkarılanYolcu, double enKucukDeger)
        {
            yolcular.koltukYolcuNumaraları[doluKoltukSayisi] = ayaktakiYolcular[çıkarılanYolcu];
            if (çıkarılanYolcu != 39)
            {
                for (int i = çıkarılanYolcu; i < bosKoltukSayisi - 1; i++)
                {
                    ayaktakiYolcular[i] = ayaktakiYolcular[i + 1];
                }
            }

            yolcular.koltukUzakliklari[doluKoltukSayisi] =
                enKucukDeger;
            doluKoltukSayisi++;
            bosKoltukSayisi--;
        }
    }

    internal class OtobüsProjesi
    {
        static void Main(string[] args)
        {
            string liste =
                    "İSMAİL SAMET YAHYA GAMZEGÜL MEVLÜT DERYA KAAN BÜŞRA HANDE İLKER NURGÜL MÜMÜN ŞEVKET MERVE YAĞMUR HALİL NAİL AHMET DİDEM YENER SEDA AYŞE FATMA ALİ SEÇKİN EKREM DEFNE RIDVAN DUHAN ABDULMELİK YÜCEL REŞAT ERDAL SERHAN FERHAN SAMİME AHMET GÜLŞAH ERTUNÇ FARUK"
                ;
            string[] isimListesi = liste.Split(' ');
            Otobüs şoför = new Otobüs(isimListesi);
            şoför.yolculariYerlestir();
        }
    }
}