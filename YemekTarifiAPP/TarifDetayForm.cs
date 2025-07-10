using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static YemekTarifiAPP.Form1;

namespace YemekTarifiAPP
{
    public partial class TarifDetayForm : Form
    {


        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";
        private readonly string tarifAdi;

        private readonly TarifMaliyetHesaplama maliyetHesaplayici;
        private Label lblMaliyetler;
        private ListBox listMaliyetler;
        public TarifDetayForm(string tarifAdi)
        {
            InitializeComponent();
            this.tarifAdi = tarifAdi;
            maliyetHesaplayici = new TarifMaliyetHesaplama(connectionString);
            InitializeMaliyetComponents();
            LoadTarifDetaylari();
            DisplayMaliyetPanel();
        }

        // Tarifi veritabanından çekip detayları gösterme
        private void LoadTarifDetaylari()
        {
            string query = @"
                  SELECT t.Talimatlar, m.MalzemeAdi, tmi.MalzemeMiktar, m.MalzemeBirim
                  FROM Tarifler t
                  JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                  JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
                  WHERE t.TarifAdi = @TarifAdi";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                listMalzemeler.Items.Clear();  // Malzeme listesini temizle

                string talimatlar = string.Empty;

                while (reader.Read())
                {

                    // Talimatlar sadece bir kez alınacak
                    if (string.IsNullOrEmpty(talimatlar))
                    {
                        talimatlar = reader["Talimatlar"].ToString();

                    }

                    // Malzemeleri listMalzemeler'e ekleyelim
                    string malzemeAdi = reader["MalzemeAdi"].ToString();
                    float malzemeMiktar = Convert.ToSingle(reader["MalzemeMiktar"]);
                    string malzemeBirim = reader["MalzemeBirim"].ToString();

                    listMalzemeler.Items.Add($"{malzemeAdi}: {malzemeMiktar} {malzemeBirim}");
                }

                // Talimatları TextBox'a ekliyoruz
                txtTalimatlar.Text = talimatlar;
                reader.Close();
            }
        }
        private void InitializeMaliyetComponents()
        {
            // Malzeme listesini sola kaydır ve daralt
            this.listMalzemeler.Size = new Size(350, 100);

            // Maliyet başlığı
            this.lblMaliyetler = new Label
            {
                AutoSize = true,
                Location = new Point(420, 20), // Malzemelerin yanına
                Size = new Size(85, 20),
                Text = "Maliyetler:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Maliyet listesi
            this.listMaliyetler = new ListBox
            {
                Location = new Point(420, 50), // Malzemelerin yanına
                Size = new Size(330, 100),
                Font = new Font("Arial", 10),
                ForeColor = Color.Black,
                BackColor = Color.LightGray
            };

            // Talimatlar bölümünü tam genişliğe çıkar
            this.txtTalimatlar.Size = new Size(700, 200);

            // Yeni kontrolleri forma ekle
            this.Controls.Add(lblMaliyetler);
            this.Controls.Add(listMaliyetler);
        }

        private void DisplayMaliyetPanel()
        {
            // Maliyet listesini temizle
            listMaliyetler.Items.Clear();

            // Maliyetleri hesapla
            var (malzemeler, toplamMaliyet) = maliyetHesaplayici.HesaplaTarifMaliyeti(tarifAdi);

            // Her malzemenin maliyetini listeye ekle
            foreach (var malzeme in malzemeler)
            {
                string maliyetText = $"{malzeme.MalzemeAdi}: {malzeme.Miktar} {malzeme.Birim}";
                string fiyatText = $"Birim: {malzeme.BirimFiyat:C2} - Toplam: {malzeme.ToplamMaliyet:C2}";
                listMaliyetler.Items.Add(maliyetText);
                listMaliyetler.Items.Add($"    {fiyatText}");
                listMaliyetler.Items.Add(""); // Boş satır ekle
            }

            // Toplam maliyeti en alta ekle
            listMaliyetler.Items.Add("------------------------");
            listMaliyetler.Items.Add($"Toplam Maliyet: {toplamMaliyet:C2}");
        }
        // Kapat butonuna tıklandığında formu kapatma
        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}