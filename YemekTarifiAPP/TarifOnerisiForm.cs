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

namespace YemekTarifiAPP
{
    public partial class TarifOnerisiForm : Form
    {
        public string connectionString = "Data Source=your_server;Initial Catalog=your_database_name;Integrated Security=True";
        
        public TarifOnerisiForm()
        {
            InitializeComponent();
            LoadTarifOnerileri();
            CustomizeUI();
        }
        private void CustomizeUI()
        {
            // Arka plan rengini ayarla
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan

            // 1- Başlık
            Label labelTitle = new Label();
            labelTitle.Text = "YEMEK TARİFİ ÖNERİLERİ";
            labelTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            labelTitle.Location = new Point(75, 20); // Başlık konumu
            labelTitle.AutoSize = true;
            this.Controls.Add(labelTitle);
        }
            private void LoadTarifOnerileri()
        {
            string query = @"
                SELECT 
                    t.TarifID,
                    t.TarifAdi,
                    CASE 
                        WHEN SUM(CASE 
                            WHEN m.ToplamMiktar < tmi.MalzemeMiktar THEN 1 
                            ELSE 0 
                        END) > 0 THEN 'Kırmızı'
                        ELSE 'Yeşil'
                    END AS TarifDurumu,
                    SUM(CASE 
                        WHEN m.ToplamMiktar < tmi.MalzemeMiktar 
                        THEN (tmi.MalzemeMiktar - m.ToplamMiktar) * m.BirimFiyat
                        ELSE 0
                    END) AS EksikMalzemeMaliyeti
                FROM Tarifler t
                JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
                GROUP BY t.TarifID, t.TarifAdi;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tarifAdi = reader["TarifAdi"].ToString();
                        string tarifDurumu = reader["TarifDurumu"].ToString();
                        decimal eksikMalzemeMaliyeti = Convert.ToDecimal(reader["EksikMalzemeMaliyeti"]);

                        // Her tarif için bir panel oluştur
                        Panel tarifPanel = new Panel
                        {
                            Width = 400,
                            Height = 70,
                            Margin = new Padding(5),
                            BackColor = (tarifDurumu == "Kırmızı") ? Color.Red : Color.Green
                        };

                        // Tarif adı label
                        Label lblTarifAdi = new Label
                        {
                            Text = tarifAdi,
                            Font = new Font("Arial", 14, FontStyle.Bold),
                            AutoSize = true,
                            Location = new Point(10, 10)
                        };
                        tarifPanel.Controls.Add(lblTarifAdi);

                        // Eksik malzeme maliyeti
                        if (tarifDurumu == "Kırmızı")
                        {
                            Label lblEksikMaliyet = new Label
                            {
                                Text = $"Eksik Malzeme Maliyeti: {eksikMalzemeMaliyeti:C2}",
                                Font = new Font("Arial", 10),
                                Location = new Point(10, 50),
                                AutoSize = true
                            };
                            tarifPanel.Controls.Add(lblEksikMaliyet);
                        }

                        // Panelleri flowLayoutPanel1'e ekleyelim
                        flowLayoutPanel1.Controls.Add(tarifPanel);
                    }
                }
            }
        }
    }
}