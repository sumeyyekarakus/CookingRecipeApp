
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace YemekTarifiAPP
{
    public partial class Form1 : Form
    {


        private TarifMaliyetHesaplama maliyet_Hesaplama;
        private Panel optionsPanel;
        private Panel malzemeSecimPanel;
        private CheckedListBox malzemeListBox;
        private FlowLayoutPanel searchResultPanel;
        private bool isMenuExpanded = false; // Menü başlangıçta kapalı (dar)
        private Button btnMenuToggle; // Object yerine Button olarak değiştirin
        private TextBox txtSearch;

        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";
        private Button btnGorTarifler;
        private string tarifAdi;

        public Form1()
        {
            InitializeComponent();
            InitializeMalzemeGiris(); // Önce malzeme giriş panelini oluştur
            CustomizeUI(); // Sonra UI'yi özelleştir
            AttachEvents();
            InitializeKategoriUI();
            InitializeSearchResultPanel();
            maliyet_Hesaplama = new TarifMaliyetHesaplama(connectionString);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(248, 249, 250);
        }

        private void InitializeMalzemeGiris()
        {
            // Ana malzeme seçim panelini oluştur
            malzemeSecimPanel = new Panel();
            malzemeSecimPanel.Location = new Point(150, 120);
            malzemeSecimPanel.Size = new Size(400, 300);
            malzemeSecimPanel.BackColor = Color.FromArgb(255, 245, 245);
            malzemeSecimPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(malzemeSecimPanel);

            // Panel başlığı ekle
            Label baslikLabel = new Label();
            baslikLabel.Text = "Malzeme Seçimi";
            baslikLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            baslikLabel.Location = new Point(5, 5);
            baslikLabel.AutoSize = true;
            malzemeSecimPanel.Controls.Add(baslikLabel);

            // CheckedListBox oluştur
            malzemeListBox = new CheckedListBox();
            malzemeListBox.Location = new Point(5, 30); // Başlığın altına yerleştir
            malzemeListBox.Size = new Size(390, 220);
            malzemeListBox.CheckOnClick = true;
            malzemeSecimPanel.Controls.Add(malzemeListBox);

            // Database'den malzemeleri çek
            LoadMalzemeler();

            // "Tarifleri Gör" butonunu panel içine ekle
            btnGorTarifler = new Button();
            btnGorTarifler.Text = "Tarifleri Gör";
            btnGorTarifler.Location = new Point(5, 260); // CheckedListBox'ın altına
            btnGorTarifler.Size = new Size(390, 30);
            btnGorTarifler.Click += BtnGorTarifler_Click;
            malzemeSecimPanel.Controls.Add(btnGorTarifler);

            // Başlangıçta paneli gizle
            malzemeSecimPanel.Visible = false;
        }
        private void LoadMalzemeler()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT MalzemeAdi FROM Malzemeler";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        malzemeListBox.Items.Add(reader["MalzemeAdi"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Malzemeler yüklenirken hata oluştu: " + ex.Message);
            }
        }
        private void BtnGorTarifler_Click(object sender, EventArgs e)
        {
            // Seçili malzemeleri al
            List<string> secilenMalzemeler = new List<string>();

            foreach (var item in malzemeListBox.CheckedItems)
            {
                secilenMalzemeler.Add(item.ToString());
            }

            if (secilenMalzemeler.Count > 0)
            {
                // Malzeme seçim panelini gizle ve dispose et
                malzemeSecimPanel.Visible = false;

                // Sonuç panelini oluştur
                FlowLayoutPanel mTarifPanel = new FlowLayoutPanel();
                mTarifPanel.Location = new Point(150, 120);
                mTarifPanel.Size = new Size(400, 200);
                mTarifPanel.BorderStyle = BorderStyle.FixedSingle;
                mTarifPanel.BringToFront();
                this.Controls.Add(mTarifPanel);

                SearchTariflerByMalzemeler(secilenMalzemeler, mTarifPanel);
            }
            else
            {
                MessageBox.Show("Lütfen en az bir malzeme seçin.");
            }
        }
        private void SearchTariflerByMalzemeler(List<string> malzemeler, FlowLayoutPanel panel)
        {
            try
            {
                panel.Visible = true;
                panel.BringToFront();

                if (panel.FindForm() != null)
                {
                    panel.FindForm().BringToFront();
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
            WITH TarifMalzemeSayilari AS (
                SELECT 
                    t.TarifID,
                    COUNT(DISTINCT tmi.MalzemeID) as ToplamMalzemeSayisi
                FROM Tarifler t
                INNER JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                GROUP BY t.TarifID
            ),
            EslesenMalzemeler AS (
                SELECT 
                    t.TarifID,
                    t.TarifAdi,
                    t.HazirlamaSuresi,
                    m.MalzemeID,
                    m.MalzemeAdi,
                    m.MalzemeBirim,
                    tmi.MalzemeMiktar as GerekliMiktar,
                    CAST(REPLACE(REPLACE(m.ToplamMiktar, ' ', ''), ',', '.') AS FLOAT) as ElimizdekiMiktar,
                    CASE 
                        WHEN CAST(REPLACE(REPLACE(m.ToplamMiktar, ' ', ''), ',', '.') AS FLOAT) >= tmi.MalzemeMiktar THEN 100
                        WHEN tmi.MalzemeMiktar > 0 THEN (
                            CAST(REPLACE(REPLACE(m.ToplamMiktar, ' ', ''), ',', '.') AS FLOAT) / tmi.MalzemeMiktar * 100
                        )
                        ELSE 0
                    END as MalzemeKarsilamaYuzdesi
                FROM Tarifler t
                INNER JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                INNER JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
                WHERE m.MalzemeAdi IN ({0})
            ),
            TarifOzeti AS (
                SELECT 
                    e.TarifID,
                    e.TarifAdi,
                    e.HazirlamaSuresi,
                    COUNT(e.MalzemeID) as EslesenMalzemeSayisi,
                    tms.ToplamMalzemeSayisi,
                    AVG(e.MalzemeKarsilamaYuzdesi) as OrtalamaKarsilamaYuzdesi,
                    STRING_AGG(
                        CONCAT(
                            e.MalzemeAdi, ' (',
                            CAST(e.ElimizdekiMiktar AS VARCHAR), '/',
                            CAST(e.GerekliMiktar AS VARCHAR),
                            ' ', e.MalzemeBirim, ' - %',
                            CAST(e.MalzemeKarsilamaYuzdesi AS DECIMAL(5,1)),
                            ')'
                        ),
                        CHAR(13) + CHAR(10)
                    ) as MalzemeDetaylari
                FROM EslesenMalzemeler e
                INNER JOIN TarifMalzemeSayilari tms ON e.TarifID = tms.TarifID
                GROUP BY e.TarifID, e.TarifAdi, e.HazirlamaSuresi, tms.ToplamMalzemeSayisi
            )
            SELECT 
                TarifAdi,
                HazirlamaSuresi,
                EslesenMalzemeSayisi,
                ToplamMalzemeSayisi,
                CAST(OrtalamaKarsilamaYuzdesi AS DECIMAL(5,2)) as KarsilamaYuzdesi,
                MalzemeDetaylari
            FROM TarifOzeti
            WHERE EslesenMalzemeSayisi > 0
            ORDER BY OrtalamaKarsilamaYuzdesi DESC, EslesenMalzemeSayisi DESC";

                    var sanitizedMalzemeler = malzemeler.Select(m => m.Replace("'", "''")).ToList();
                    string formattedQuery = string.Format(query, string.Join(",", sanitizedMalzemeler.Select(m => $"'{m}'")));

                    using (SqlCommand cmd = new SqlCommand(formattedQuery, conn))
                    {
                        panel.Controls.Clear();
                        panel.SuspendLayout();
                        panel.AutoScroll = true;
                        panel.BackColor = Color.FromArgb(255, 245, 245);
                        panel.BorderStyle = BorderStyle.Fixed3D;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            bool tarifBulundu = false;

                            while (reader.Read())
                            {
                                tarifBulundu = true;
                                string tarifAdi = reader["TarifAdi"].ToString();

                                // Maliyet hesaplama
                                var (malzemeDetaylari, toplamMaliyet) = maliyet_Hesaplama.HesaplaTarifMaliyeti(tarifAdi);

                                // Eksik malzeme maliyeti hesaplama
                                decimal eksikMalzemeMaliyeti = 0;
                                foreach (var malzeme in malzemeDetaylari)
                                {
                                    var elimizdekiMiktar = GetMevcutMiktar(malzeme.MalzemeAdi); // Bu metodu implement etmeniz gerekecek
                                    if (elimizdekiMiktar < malzeme.Miktar)
                                    {
                                        var eksikMiktar = malzeme.Miktar - elimizdekiMiktar;
                                        eksikMalzemeMaliyeti += maliyet_Hesaplama.HesaplaMalzemeMaliyeti(
                                            eksikMiktar,
                                            malzeme.Birim,
                                            malzeme.BirimFiyat
                                        );
                                    }
                                }

                                CreateRecipePanel(
                                    panel,
                                    tarifAdi,
                                    Convert.ToInt32(reader["HazirlamaSuresi"]),
                                    Convert.ToInt32(reader["EslesenMalzemeSayisi"]),
                                    Convert.ToInt32(reader["ToplamMalzemeSayisi"]),
                                    Convert.ToDecimal(reader["KarsilamaYuzdesi"]),
                                    eksikMalzemeMaliyeti,
                                    reader["MalzemeDetaylari"].ToString()
                                );
                            }

                            if (!tarifBulundu)
                            {
                                ShowNoRecipesFound(panel);
                                return;
                            }
                        }

                        panel.ResumeLayout();
                        panel.BringToFront();
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleSearchError(panel, ex);
            }
        }

        // Mevcut miktarı getiren yardımcı metod
        private double GetMevcutMiktar(string malzemeAdi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CAST(REPLACE(REPLACE(ToplamMiktar, ' ', ''), ',', '.') AS FLOAT) FROM Malzemeler WHERE MalzemeAdi = @MalzemeAdi";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }
        }

        private void CreateRecipePanel(
            FlowLayoutPanel panel,
            string tarifAdi,
            int hazirlamaSuresi,
            int eslesenMalzemeSayisi,
            int toplamMalzemeSayisi,
            decimal karsilamaYuzdesi,
            decimal eksikMalzemeMaliyeti,
            string malzemeDetaylari)
        {
            panel.BackColor = Color.FromArgb(255, 245, 245);
            var tarifPanel = new Panel
            {
                Width = 300,
                Height = 150,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık Label'ı
            var baslikLabel = new Label
            {
                Text = $"{tarifAdi} - {hazirlamaSuresi} dk",
                Font = new Font(DefaultFont, FontStyle.Bold),
                Width = 280,
                Location = new Point(10, 5)
            };

            // Yüzde Label'ı
            var yuzdeLabel = new Label
            {
                Text = $"Genel Durum: %{karsilamaYuzdesi:F1} ({eslesenMalzemeSayisi}/{toplamMalzemeSayisi} malzeme)",
                Width = 280,
                Location = new Point(10, 25)
            };

            // Maliyet Label'ı
            var maliyetLabel = new Label
            {
                Text = $"Eksik Malzeme Maliyeti: ₺{eksikMalzemeMaliyeti:F2}",
                Width = 280,
                Location = new Point(10, 45),
                ForeColor = eksikMalzemeMaliyeti > 0 ? Color.DarkRed : Color.DarkGreen
            };

            // Detay Label'ı
            var detayLabel = new Label
            {
                Text = malzemeDetaylari,
                Width = 280,
                Height = 40,
                Location = new Point(10, 65),
                AutoSize = false
            };

            // Detay Butonu
            var detayButton = new Button
            {
                Text = "Tarif Detayı",
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            detayButton.Location = new Point(tarifPanel.Width - detayButton.Width - 10, tarifPanel.Height - detayButton.Height - 10);

            detayButton.Click += (sender, e) =>
            {
                panel.Visible = false;
                var tarifDetayForm = new TarifDetayForm(tarifAdi);
                tarifDetayForm.Show();
            };

            // Panel rengini ayarla
            if (karsilamaYuzdesi >= 100)
            {
                tarifPanel.BackColor = Color.FromArgb(220, 255, 220); // Açık yeşil
            }
            else if (karsilamaYuzdesi >= 75)
            {
                tarifPanel.BackColor = Color.FromArgb(255, 255, 220); // Açık sarı
            }
            else
            {
                tarifPanel.BackColor = Color.FromArgb(255, 220, 220); // Açık kırmızı
            }

            // Kontrolleri panele ekle
            tarifPanel.Controls.AddRange(new Control[] { baslikLabel, yuzdeLabel, maliyetLabel, detayLabel, detayButton });
            panel.Controls.Add(tarifPanel);
        }
        private void ShowNoRecipesFound(FlowLayoutPanel panel)
        {
            panel.Visible = false;
            MessageBox.Show(
                "Seçilen malzemelerle eşleşen tarif bulunamadı.",
                "Sonuç Yok",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void HandleSearchError(FlowLayoutPanel panel, Exception ex)
        {
            panel.Visible = false;
            MessageBox.Show(
                $"Tarifler listelenirken bir hata oluştu: {ex.Message}",
                "Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            // Hata loglaması eklenebilir
            Debug.WriteLine($"Tarif arama hatası: {ex}");
        }
        private void InitializeKategoriUI()
        {
            // Ana TableLayoutPanel oluşturma
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Padding = new Padding(10);
            mainLayout.RowCount = 3;    // 3 satır
            mainLayout.ColumnCount = 3; // 3 sütun
            mainLayout.BackColor = Color.FromArgb(255, 245, 245); // Açık gri arkaplan

            // Satır ve sütun boyutlarını yüzdesel olarak ayarlama
            for (int i = 0; i < mainLayout.ColumnCount; i++)
            {
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            }

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140)); // Üst kısım için boşluk
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            this.Controls.Add(mainLayout);

            // Kategorileri özel renklerle oluşturma
            GroupBox tatliGroupBox = CreateStyledGroupBox(" Tatlılar");
            GroupBox etGroupBox = CreateStyledGroupBox(" Et Yemekleri");
            GroupBox zyGroupBox = CreateStyledGroupBox(" Zeytinyağlılar");
            GroupBox corbaGroupBox = CreateStyledGroupBox(" Çorbalar");
            GroupBox vjGroupBox = CreateStyledGroupBox(" Vejetaryen Seçimler");

            // FlowLayoutPanel'leri oluşturma
            FlowLayoutPanel tatliPanel = CreateFlowPanel();
            FlowLayoutPanel etPanel = CreateFlowPanel();
            FlowLayoutPanel zyPanel = CreateFlowPanel();
            FlowLayoutPanel corbaPanel = CreateFlowPanel();
            FlowLayoutPanel vjPanel = CreateFlowPanel();

            // GroupBox'lara FlowLayoutPanel'leri ekleme
            tatliGroupBox.Controls.Add(tatliPanel);
            etGroupBox.Controls.Add(etPanel);
            zyGroupBox.Controls.Add(zyPanel);
            corbaGroupBox.Controls.Add(corbaPanel);
            vjGroupBox.Controls.Add(vjPanel);

            // TableLayoutPanel'e GroupBox'ları yerleştirme ve gölge efekti ekleme
            AddToLayoutWithShadow(mainLayout, tatliGroupBox, 0, 1);
            AddToLayoutWithShadow(mainLayout, etGroupBox, 1, 1);
            AddToLayoutWithShadow(mainLayout, zyGroupBox, 0, 2);
            AddToLayoutWithShadow(mainLayout, corbaGroupBox, 1, 2);
            AddToLayoutWithShadow(mainLayout, vjGroupBox, 2, 1);
            mainLayout.SetRowSpan(vjGroupBox.Parent, 2); // Gölge paneli ile birlikte rowspan ayarı

            // Tarifleri yükleme
            GetTariflerByKategori("ÇORBA", corbaPanel);
            GetTariflerByKategori("ET YEMEĞİ", etPanel);
            GetTariflerByKategori("ZEYTİNYAĞLILAR", zyPanel);
            GetTariflerByKategori("VEJETARYEN", vjPanel);
            GetTariflerByKategori("TATLI", tatliPanel);
        }

        // Özelleştirilmiş GroupBox oluşturma metodu
        private GroupBox CreateStyledGroupBox(string title)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = title;
            groupBox.Dock = DockStyle.Fill;
            groupBox.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            groupBox.ForeColor = Color.FromArgb(255, 182, 193);
            groupBox.BackColor = Color.FromArgb(255, 245, 245);
            groupBox.Padding = new Padding(10);
            groupBox.Paint += (sender, e) =>
            {
                GroupBox box = sender as GroupBox;
                e.Graphics.Clear(box.BackColor);
                // Başlık çizimi
                if (!string.IsNullOrEmpty(box.Text))
                {
                    using (Font titleFont = new Font("Segoe UI", 12f, FontStyle.Bold))
                    {
                        SizeF textSize = e.Graphics.MeasureString(box.Text, titleFont);
                        using (SolidBrush brush = new SolidBrush(Color.White)) // Beyaz renk
                        {
                            e.Graphics.DrawString(
                                box.Text,
                                titleFont,
                                brush,
                                10, // x pozisyonu
                                0); // y pozisyonu
                        }
                    }
                }
            };

            return groupBox;
        }

        // Gölge efekti ile birlikte layout'a ekleme metodu
        private void AddToLayoutWithShadow(TableLayoutPanel layout, GroupBox box, int column, int row)
        {
            Panel shadowPanel = new Panel
            {
                BackColor = Color.FromArgb(147, 112, 219),
                Margin = new Padding(5),
                Padding = new Padding(3),
                Dock = DockStyle.Fill
            };

            box.BackColor = Color.FromArgb(216, 191, 216);
            shadowPanel.Controls.Add(box);
            layout.Controls.Add(shadowPanel, column, row);
        }

        // FlowLayoutPanel oluşturma metodu
        private FlowLayoutPanel CreateFlowPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(8),
                BackColor = Color.FromArgb(216, 191, 216),
                WrapContents = true
            };
        }
        public void GetTariflerByKategori(string kategori, FlowLayoutPanel panel)
        {
            string query = "SELECT TarifAdi, HazirlamaSuresi FROM Tarifler WHERE Kategori = @Kategori";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Kategori", kategori);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                panel.Controls.Clear();
                TarifMaliyetHesaplama maliyetHesaplama = new TarifMaliyetHesaplama(connectionString);

                while (reader.Read())
                {
                    string tarifAdi = reader["TarifAdi"].ToString();
                    int hazirlamaSuresi = Convert.ToInt32(reader["HazirlamaSuresi"]);
                    var (malzemeler, toplamMaliyet) = maliyetHesaplama.HesaplaTarifMaliyeti(tarifAdi);

                    Button tarifButton = new Button
                    {
                        Text = $"{tarifAdi} - {hazirlamaSuresi} dk - {toplamMaliyet:C}",
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        MinimumSize = new Size(230, 30),
                        MaximumSize = new Size(280, 40),
                        Margin = new Padding(5),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(255, 182, 193), // Pembe tonları
                        ForeColor = Color.White,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Padding = new Padding(10, 5, 10, 5),
                        Cursor = Cursors.Hand,
                        TextAlign = ContentAlignment.MiddleLeft,
                        UseVisualStyleBackColor = false // Arka plan rengini korumak için
                    };

                    tarifButton.Click += (sender, e) =>
                    {
                        TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                        tarifDetayForm.Show();
                    };

                    panel.Controls.Add(tarifButton);
                }

                reader.Close();
            }
        }

        private void AttachEvents()
        {

            btnTarifEkle.Click += new EventHandler(this.btnTarifEkle_Click);
            btnTarifSil.Click += new EventHandler(this.btnTarifSil_Click);
            btnTarifGuncelle.Click += new EventHandler(this.btnTarifGuncelle_Click);
            btnTarifOnerisi.Click += new EventHandler(this.btnTarifOnerisi_Click);
            btnMalzemelerim.Click += new EventHandler(this.btnMalzemelerim_Click);
        }
        // UI'yi özelleştirmek için yazdığımız fonksiyon
        private void CustomizeUI()
        {
            // Arka plan rengini ayarla
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan

            // Menü panelini özelleştir
            panelMenu.BackColor = Color.FromArgb(216, 191, 216); // Lila rengi
            panelMenu.Dock = DockStyle.Left; // Sol paneli konumlandır

            // Menü başlığını özelleştir
            labelMenuTitle.ForeColor = Color.White; // Başlık metni beyaz
            labelMenuTitle.Font = new Font("Arial", 14, FontStyle.Bold);

            // Toggle butonunu özelleştir
            btnMenuToggle.BackColor = Color.FromArgb(147, 112, 219); // Mavi-lila tonlarında buton
            btnMenuToggle.FlatStyle = FlatStyle.Flat;
            btnMenuToggle.ForeColor = Color.White;
            btnMenuToggle.Font = new Font("Arial", 10, FontStyle.Bold);
            btnMenuToggle.Location = new Point(10, 10);

            Button btnFilter = new Button();
            btnFilter.Size = new Size(30, 30); // Butonun boyutları
            btnFilter.Location = new Point(680, 80); // Arama butonunun yanına konumlandır
            btnFilter.BackColor = Color.Transparent;

            btnFilter.BackgroundImage = Image.FromFile("your_image_path");
            btnFilter.BackgroundImageLayout = ImageLayout.Stretch; // Resmi butonun boyutuna göre ger
            btnFilter.FlatStyle = FlatStyle.Flat; // Butonun stilini düz yap
            btnFilter.FlatAppearance.BorderSize = 0; // Kenarlık boyutunu sıfırla
            btnFilter.Text = "";

            // Filtre butonuna tıklama olayı ekleyin
            btnFilter.Click += new EventHandler(this.btnFilter_Click);

            this.Controls.Add(btnFilter); // Filtre butonunu forma ekle
            // Menü butonlarına renk ve stil ekleyelim
            Button[] buttons = { btnTarifEkle, btnTarifSil, btnTarifGuncelle, btnTarifOnerisi, btnMalzemelerim };

            foreach (Button button in buttons)
            {
                button.BackColor = Color.FromArgb(255, 182, 193); // Pembe tonları
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.Font = new Font("Arial", 10, FontStyle.Bold);
                button.Margin = new Padding(5);
            }

            // 1- Başlık
            Label labelTitle = new Label();
            labelTitle.Text = "YEMEK TARİFİ UYGULAMAM";
            labelTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            labelTitle.Location = new Point(150, 20); // Başlık konumu
            labelTitle.AutoSize = true;
            this.Controls.Add(labelTitle);

            // 2- Arama Kutusu (Placeholder metni manuel ekleniyor)
            txtSearch = new TextBox();  // Burada tanımlama yapıldı
            txtSearch.Text = "Yemek Ara...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Font = new Font("Arial", 12);
            txtSearch.Location = new Point(150, 80);
            txtSearch.Width = 400;
            this.Controls.Add(txtSearch);

            txtSearch.GotFocus += RemovePlaceholder;
            txtSearch.LostFocus += SetPlaceholder;

            Button btnSearch = new Button();
            btnSearch.Text = "Ara";
            btnSearch.Font = new Font("Arial", 12);
            btnSearch.Location = new Point(570, 80);
            btnSearch.Click += new EventHandler(this.btnSearch_Click);
            this.Controls.Add(btnSearch);


        }
        public void ShowOptions()
        {
            // Ana panel oluşturma
            optionsPanel = new Panel
            {
                Size = new Size(300, 380),
                Location = new Point(680, 120), // Filtre butonunun hemen altında
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245)
            };


            // Sıralama seçeneklerini liste olarak tanımlıyoruz
            string[] options = { "A'dan Z'ye sırala", "Süreye göre sırala", "En ucuz maliyete göre sırala", "En pahalı maliyete göre sırala", "Malzeme sayısına göre sırala", "Maliyet aralığına göre sırala", "Kategoriye göre sırala" };

            // Her bir seçenek için buton oluşturma
            for (int i = 0; i < options.Length; i++)
            {
                Button optionButton = new Button
                {
                    Text = options[i],
                    Size = new Size(250, 40),
                    Location = new Point(25, 20 + i * 50),  // Her butonun konumunu ayarlıyoruz
                    Tag = options[i]  // Seçenek ismini Tag özelliğine ekliyoruz
                };

                // Buton tıklama olayına işleyici ekle
                optionButton.Click += OptionButton_Click;

                // Panel'e buton ekle
                optionsPanel.Controls.Add(optionButton);
            }

            // Form'a veya ana kontrolünüze paneli ekleyin
            this.Controls.Add(optionsPanel);
            optionsPanel.BringToFront();
        }

        private void OptionButton_Click(object sender, EventArgs e)
        {
            optionsPanel.Visible = false;
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string option = clickedButton.Tag.ToString();

                // Seçilen butona göre fonksiyonları çalıştırma
                switch (option)
                {
                    case "A'dan Z'ye sırala":
                        alfabeSırala();
                        break;
                    case "Süreye göre sırala":
                        süreSırala();
                        break;
                    case "En ucuz maliyete göre sırala":
                        maliyetSırala(true);
                        break;
                    case "En pahalı maliyete göre sırala":
                        maliyetSırala(false);
                        break;
                    case "Malzeme sayısına göre sırala":
                        malzemesayıSırala();
                        break;
                    case "Maliyet aralığına göre sırala":
                        maliyetaralıkSırala();
                        break;
                    case "Kategoriye göre sırala":
                        kategoriSırala();
                        break;
                    default:
                        MessageBox.Show("Bilinmeyen seçenek!");
                        break;
                }
            }
        }

        private void alfabeSırala()
        {
            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 400),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2), // Ortada konumlandırma
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            string query = "SELECT TarifID, TarifAdi FROM Tarifler ORDER BY TarifAdi ASC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);

                int yOffset = 10; // Butonlar arası boşluk

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tarifID = reader.GetInt32(0);
                        string tarifAdi = reader.GetString(1);

                        Button tarifButton = new Button
                        {
                            Text = tarifAdi,
                            Size = new Size(250, 40),
                            Location = new Point(25, yOffset),
                            Tag = tarifID  // Tarif ID'sini Tag ile saklıyoruz
                        };

                        tarifButton.Click += (sender, e) =>
                        {
                            TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                            tarifDetayForm.Show();
                            panel.Visible = false;
                        };
                        panel.Controls.Add(tarifButton);

                        yOffset += 50;  // Butonlar arasındaki mesafeyi güncelle
                    }
                }
            }

            // Paneli forma veya ana kontrole ekleyin
            this.Controls.Add(panel);
            panel.BringToFront();
        }
        private void süreSırala()
        {
            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 400),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2), // Ortada konumlandırma
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            string query = "SELECT TarifID, TarifAdi, HazirlamaSuresi FROM Tarifler ORDER BY HazirlamaSuresi ASC"; // Süreye göre sıralama

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);

                int yOffset = 10; // Butonlar arası boşluk
                ToolTip toolTip = new ToolTip(); // Tooltip oluşturma

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tarifID = reader.GetInt32(0);
                        string tarifAdi = reader.GetString(1);
                        int hazirlamaSuresi = reader.GetInt32(2); // Hazırlama süresini al

                        // Uzun metni kısaltma ve buton oluşturma
                        string displayText = tarifAdi.Length > 20 ? tarifAdi.Substring(0, 20) + "..." : tarifAdi;

                        Button tarifButton = new Button
                        {
                            Text = $"{displayText} - Hazırlama Süresi: {hazirlamaSuresi} dk",
                            Size = new Size(250, 40),
                            Location = new Point(25, yOffset),
                            Tag = tarifID  // Tarif ID'sini Tag ile saklıyoruz
                        };

                        // Tooltip ekleme
                        toolTip.SetToolTip(tarifButton, $"{tarifAdi} - Hazırlama Süresi: {hazirlamaSuresi} dk");

                        tarifButton.Click += (sender, e) =>
                        {
                            TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                            tarifDetayForm.Show();
                            panel.Visible = false;
                        };
                        panel.Controls.Add(tarifButton);

                        yOffset += 50;  // Butonlar arasındaki mesafeyi güncelle
                    }
                }
            }

            // Paneli forma veya ana kontrole ekleyin
            this.Controls.Add(panel);
            panel.BringToFront();
        }
        private void maliyetSırala(bool ascending)
        {
            // Tarif maliyet hesaplama sınıfını başlat
            TarifMaliyetHesaplama maliyetHesaplama = new TarifMaliyetHesaplama(connectionString);

            // Tarifleri maliyete göre sıralama
            Dictionary<string, decimal> tarifMaliyetleri = new Dictionary<string, decimal>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TarifAdi FROM Tarifler";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tarifAdi = reader["TarifAdi"].ToString();
                        var maliyetDetay = maliyetHesaplama.HesaplaTarifMaliyeti(tarifAdi);
                        tarifMaliyetleri[tarifAdi] = maliyetDetay.toplamMaliyet;
                    }
                }
            }

            // Sıralama işlemi
            var sortedTarifler = ascending
                ? tarifMaliyetleri.OrderBy(t => t.Value)
                : tarifMaliyetleri.OrderByDescending(t => t.Value);

            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 400), // Panelin boyutları
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2), // Ekranın ortasına konumlandır
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            int yOffset = 10; // Butonlar arasındaki boşluk

            foreach (var tarif in sortedTarifler)
            {
                Button tarifButton = new Button
                {
                    Text = $"{tarif.Key} - Maliyet: {tarif.Value:C2}",
                    Size = new Size(250, 40),
                    Location = new Point(25, yOffset),
                    Tag = tarif.Key  // Tarif adını Tag olarak ekliyoruz
                };

                tarifButton.Click += (sender, e) =>
                {
                    TarifDetayForm tarifDetayForm = new TarifDetayForm(tarif.Key);
                    tarifDetayForm.Show();
                    panel.Visible = false;
                };
                panel.Controls.Add(tarifButton);
                yOffset += 50; // Her buton arasındaki boşluğu artır
            }

            // Paneli ekranda göstermek için form'a ekleyin
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void malzemesayıSırala()
        {
            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 400),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2), // Ortada konumlandırma
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true
            };

            string query = @"
                   SELECT t.TarifID, t.TarifAdi, COUNT(tmi.MalzemeID) AS MalzemeSayisi
                   FROM Tarifler t
                   LEFT JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                   GROUP BY t.TarifID, t.TarifAdi
                   ORDER BY MalzemeSayisi ASC"; // En azdan en çoğa sıralama

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);

                int yOffset = 10; // Butonlar arası boşluk
                ToolTip toolTip = new ToolTip();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tarifID = reader.GetInt32(0);
                        string tarifAdi = reader.GetString(1);
                        int malzemeSayisi = reader.GetInt32(2); // Malzeme sayısını al

                        Button tarifButton = new Button
                        {
                            Text = $"{tarifAdi} - Malzeme Sayısı: {malzemeSayisi}",
                            Size = new Size(250, 40),
                            Location = new Point(25, yOffset),
                            Tag = tarifID  // Tarif ID'sini Tag ile saklıyoruz
                        };

                        // Tooltip ekleme
                        toolTip.SetToolTip(tarifButton, $"{tarifAdi} - Malzeme Sayısı: {malzemeSayisi}");

                        tarifButton.Click += (sender, e) =>
                        {
                            TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                            tarifDetayForm.Show();
                            panel.Visible = false;
                        };
                        panel.Controls.Add(tarifButton);

                        yOffset += 50;
                    }
                }
            }
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void maliyetaralıkSırala()
        {
            // Kullanıcıdan maliyet aralığını almak için giriş kutuları oluşturma
            Form inputForm = new Form
            {
                Text = "Maliyet Aralığı Seçimi",
                Size = new Size(300, 200)
            };

            Label startLabel = new Label { Text = "Başlangıç Maliyeti:", Location = new Point(10, 20) };
            TextBox startTextBox = new TextBox { Location = new Point(150, 20), Width = 100 };

            Label endLabel = new Label { Text = "Bitiş Maliyeti:", Location = new Point(10, 60) };
            TextBox endTextBox = new TextBox { Location = new Point(150, 60), Width = 100 };

            Button submitButton = new Button { Text = "Göster", Location = new Point(100, 100) };

            submitButton.Click += (sender, e) =>
            {
                decimal startMaliyet, endMaliyet;

                // Maliyet girişlerini kontrol et
                if (decimal.TryParse(startTextBox.Text, out startMaliyet) && decimal.TryParse(endTextBox.Text, out endMaliyet))
                {
                    // Eğer geçerli bir aralıksa tarifleri göster
                    if (startMaliyet <= endMaliyet)
                    {
                        ShowTariflerWithinMaliyetRange(startMaliyet, endMaliyet);
                        inputForm.Close(); // Giriş formunu kapat
                    }
                    else
                    {
                        MessageBox.Show("Başlangıç maliyeti, bitiş maliyetinden küçük olmalıdır.");
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir maliyet değeri girin.");
                }
            };

            // Form elemanlarını ekle
            inputForm.Controls.Add(startLabel);
            inputForm.Controls.Add(startTextBox);
            inputForm.Controls.Add(endLabel);
            inputForm.Controls.Add(endTextBox);
            inputForm.Controls.Add(submitButton);

            // Formu göster
            inputForm.ShowDialog();
        }

        private void ShowTariflerWithinMaliyetRange(decimal startMaliyet, decimal endMaliyet)
        {
            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 400),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            // SQL bağlantı dizesi
            string connectionString = "Data Source=your_server Catalog=your_db_name;Integrated Security=True";
            var maliyetHesaplama = new TarifMaliyetHesaplama(connectionString);

            // ToolTip nesnesi oluştur
            ToolTip toolTip = new ToolTip();

            // SQL sorgusu, tüm tarifleri al
            string query = "SELECT TarifAdi FROM Tarifler";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                int yOffset = 10; // Butonlar arası boşluk

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tarifAdi = reader["TarifAdi"].ToString();

                        // Tarif maliyetini hesapla
                        var (malzemeler, toplamMaliyet) = maliyetHesaplama.HesaplaTarifMaliyeti(tarifAdi);

                        // Eğer toplam maliyet belirtilen aralıkta ise buton oluştur
                        if (toplamMaliyet >= startMaliyet && toplamMaliyet <= endMaliyet)
                        {
                            Button tarifButton = new Button
                            {
                                Text = $"{tarifAdi} - Maliyet: {toplamMaliyet:C2}", // Buton metnini tarif adı ve maliyet olarak ayarlıyoruz
                                Size = new Size(250, 40),
                                Location = new Point(25, yOffset),
                                Tag = tarifAdi // Tarif adını Tag ile saklıyoruz
                            };

                            // ToolTip ayarları
                            toolTip.SetToolTip(tarifButton, $"Toplam Maliyet: {toplamMaliyet:C2}"); // Toplam maliyeti göster

                            tarifButton.Click += (sender, e) =>
                            {
                                TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                                tarifDetayForm.Show();
                                panel.Visible = false;
                            };

                            panel.Controls.Add(tarifButton);
                            yOffset += 50; // Butonlar arasındaki mesafeyi güncelle
                        }
                    }
                }
            }

            // Paneli forma veya ana kontrole ekleyin
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private void kategoriSırala()
        {
            // Paneli temizleme ve yeniden oluşturma
            Panel panel = new Panel
            {
                Size = new Size(300, 270),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            string query = "SELECT DISTINCT Kategori FROM Tarifler"; // Kategorileri alma sorgusu

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                int yOffset = 10; // Butonlar arası boşluk

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string kategori = reader["Kategori"].ToString();

                        // Kategori butonu oluştur
                        Button kategoriButton = new Button
                        {
                            Text = kategori,
                            Size = new Size(250, 40),
                            Location = new Point(25, yOffset),
                            Tag = kategori // Kategori adını Tag ile saklıyoruz
                        };

                        // Kategoriye göre tarifleri gösterecek olan tıklama olayını ayarlama
                        kategoriButton.Click += (sender, e) =>
                        {
                            string selectedKategori = (sender as Button).Tag.ToString();
                            ShowTariflerByKategori(selectedKategori);
                            panel.Visible = false;
                        };

                        panel.Controls.Add(kategoriButton);
                        yOffset += 50; // Butonlar arasındaki mesafeyi güncelle
                    }
                }
            }

            // Paneli forma veya ana kontrole ekleyin
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        // Belirli bir kategoriye göre tarifleri gösteren metot
        private void ShowTariflerByKategori(string kategori)
        {
            // Tarifleri listelemek için yeni bir panel oluştur
            Panel tarifPanel = new Panel
            {
                Size = new Size(300, 400),
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 400) / 2),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 245, 245),
                AutoScroll = true // Kaydırma çubuğunu etkinleştir
            };

            string query = "SELECT TarifAdi FROM Tarifler WHERE Kategori = @Kategori";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Kategori", kategori);

                int yOffset = 10;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tarifAdi = reader["TarifAdi"].ToString();

                        // Tarif butonunu oluştur
                        Button tarifButton = new Button
                        {
                            Text = tarifAdi,
                            Size = new Size(250, 40),
                            Location = new Point(25, yOffset),
                            Tag = tarifAdi // Tarif adını Tag ile saklıyoruz
                        };

                        tarifButton.Click += (sender, e) =>
                        {
                            TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                            tarifDetayForm.Show();
                            tarifPanel.Visible = false;
                        };

                        tarifPanel.Controls.Add(tarifButton);
                        yOffset += 50;
                    }
                }
            }

            // Tarif panelini forma ekleyin
            this.Controls.Add(tarifPanel);
            tarifPanel.BringToFront();
        }

        private void InitializeSearchResultPanel()
        {
            searchResultPanel = new FlowLayoutPanel();
            searchResultPanel.Location = new Point(150, 120); // Panel konumu
            searchResultPanel.Size = new Size(400, 300); // Panel boyutu
            searchResultPanel.AutoScroll = true; // Kaydırma çubuğu eklenmesi
            this.Controls.Add(searchResultPanel); // Paneli forma ekliyoruz
        }
        public void SearchTarifler(string searchText)
        {
            malzemeSecimPanel.Visible = false;

            string query = @"
    WITH TarifMalzemeSayilari AS (
        SELECT 
            t.TarifID,
            t.TarifAdi,
            t.HazirlamaSuresi,
            COUNT(DISTINCT tmi.MalzemeID) as ToplamMalzemeSayisi
        FROM Tarifler t
        LEFT JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
        WHERE t.TarifAdi LIKE @searchText 
            OR t.Kategori LIKE @searchText 
            OR EXISTS (
                SELECT 1 
                FROM TarifMalzemeIliskisi tmi2 
                INNER JOIN Malzemeler m ON tmi2.MalzemeID = m.MalzemeID 
                WHERE tmi2.TarifID = t.TarifID 
                AND m.MalzemeAdi LIKE @searchText
            )
        GROUP BY t.TarifID, t.TarifAdi, t.HazirlamaSuresi
    ),
    EksikMalzemeler AS (
        SELECT 
            t.TarifID,
            SUM(
                CASE 
                    WHEN m.ToplamMiktar IS NULL OR CAST(m.ToplamMiktar AS FLOAT) < tmi.MalzemeMiktar
                    THEN (tmi.MalzemeMiktar - COALESCE(CAST(m.ToplamMiktar AS FLOAT), 0)) * m.BirimFiyat
                    ELSE 0 
                END
            ) as EksikMalzemeMaliyeti
        FROM TarifMalzemeSayilari t
        LEFT JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
        LEFT JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
        GROUP BY t.TarifID
    )
    SELECT 
        t.TarifID,
        t.TarifAdi,
        t.HazirlamaSuresi,
        t.ToplamMalzemeSayisi,
        COUNT(DISTINCT CASE 
            WHEN m.ToplamMiktar IS NOT NULL 
            AND CAST(m.ToplamMiktar AS FLOAT) >= tmi.MalzemeMiktar 
            THEN tmi.MalzemeID 
        END) as MevcutMalzemeSayisi,
        COALESCE(em.EksikMalzemeMaliyeti, 0) as EksikMalzemeMaliyeti
    FROM TarifMalzemeSayilari t
    LEFT JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
    LEFT JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
    LEFT JOIN EksikMalzemeler em ON t.TarifID = em.TarifID
    GROUP BY t.TarifID, t.TarifAdi, t.HazirlamaSuresi, t.ToplamMalzemeSayisi, em.EksikMalzemeMaliyeti";

            List<(Button Button, double Yuzde)> tarifButonlari = new List<(Button, double)>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                searchResultPanel.Controls.Clear();

                while (reader.Read())
                {
                    string tarifAdi = reader["TarifAdi"].ToString();
                    int hazirlamaSuresi = Convert.ToInt32(reader["HazirlamaSuresi"]);
                    int toplamMalzemeSayisi = Convert.ToInt32(reader["ToplamMalzemeSayisi"]);
                    int mevcutMalzemeSayisi = Convert.ToInt32(reader["MevcutMalzemeSayisi"]);
                    decimal eksikMalzemeMaliyeti = Convert.ToDecimal(reader["EksikMalzemeMaliyeti"]);

                    double malzemeYuzde = toplamMalzemeSayisi > 0
                        ? (double)mevcutMalzemeSayisi / toplamMalzemeSayisi * 100
                        : 0;

                    Button tarifButton = new Button();
                    tarifButton.Text = $"{tarifAdi} - {hazirlamaSuresi} dk\n" +
                                      $"Malzeme Uyumu: %{malzemeYuzde:F1}\n" +
                                      $"Eksik Malzeme Maliyeti: {eksikMalzemeMaliyeti:F2} TL";
                    tarifButton.Width = 250;
                    tarifButton.Height = 80; // Yüksekliği arttırdık
                    tarifButton.Margin = new Padding(5);
                    tarifButton.TextAlign = ContentAlignment.MiddleCenter;

                    if (malzemeYuzde >= 100)
                    {
                        tarifButton.BackColor = Color.FromArgb(144, 238, 144); // Açık yeşil
                    }
                    else if (malzemeYuzde >= 50)
                    {
                        tarifButton.BackColor = Color.FromArgb(255, 255, 153); // Açık sarı
                    }
                    else
                    {
                        tarifButton.BackColor = Color.FromArgb(255, 204, 204); // Açık kırmızı
                    }

                    tarifButton.Click += (sender, e) =>
                    {
                        TarifDetayForm tarifDetayForm = new TarifDetayForm(tarifAdi);
                        tarifDetayForm.Show();
                        searchResultPanel.Visible = false;
                    };

                    tarifButonlari.Add((tarifButton, malzemeYuzde));
                }
                reader.Close();
            }

            var siraliButonlar = tarifButonlari.OrderByDescending(x => x.Yuzde).ToList();

            foreach (var item in siraliButonlar)
            {
                searchResultPanel.Controls.Add(item.Button);
            }

            searchResultPanel.BringToFront();
            searchResultPanel.Visible = searchResultPanel.Controls.Count > 0;
        }
        // Placeholder'ı kaldıran olay (Textbox'a tıklandığında)
        private void RemovePlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text == "Yemek Ara...")
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;  // Kullanıcı metin yazarken siyah yap
            }
        }

        // Placeholder'ı geri ekleyen olay (Textbox'tan çıkıldığında)
        private void SetPlaceholder(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "Yemek Ara...";
                txt.ForeColor = Color.Gray;  // Placeholder metni için gri renk
            }
        }

        // Menü açıp kapatmayı sağlayan butonun olayı
        private void btnMenuToggle_Click(object sender, EventArgs e)
        {
            if (isMenuExpanded)
            {
                // Menü kapat (daralt)
                labelMenuTitle.Text = ""; // Başlık gizlenir
                panelMenu.Width = 0; // Panel daraltılır
                btnMenuToggle.Location = new Point(10, 10); // Toggle butonunu yerine al
                btnTarifEkle.Text = "";  // Menü butonlarının textlerini kaldır
                btnTarifSil.Text = "";
                btnTarifGuncelle.Text = "";
                btnTarifOnerisi.Text = "";
                btnMalzemelerim.Text = "";
                isMenuExpanded = false;
            }
            else
            {
                // Menü aç (genişlet)
                panelMenu.Width = 200;  // Panel genişletilir
                labelMenuTitle.Text = "MENÜ"; // Başlık tekrar gösterilir
                btnMenuToggle.Location = new Point(210, 10); // Toggle butonu sağa kayar
                btnTarifEkle.Text = "Tarif Ekle";  // Menü butonlarına metin eklenir
                btnTarifSil.Text = "Tarif Sil";
                btnTarifGuncelle.Text = "Tarif Güncelle";
                btnTarifOnerisi.Text = "Tarif Önerisi";
                btnMalzemelerim.Text = "Malzemelerim";
                isMenuExpanded = true;
            }
        }

        // Tarif Ekle butonuna tıklandığında yeni formu açar
        private void btnTarifEkle_Click(object sender, EventArgs e)
        {
            TarifEklemeForm tarifEkleForm = new TarifEklemeForm();
            tarifEkleForm.Show();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            // Önceki arama sonuçlarını gizle
            if (searchResultPanel != null)
                searchResultPanel.Visible = false;

            // Varsayılan metin kontrolü
            if (string.IsNullOrEmpty(searchText) || searchText == "Yemek Ara...")
            {
                // Malzeme seçim panelini göster ve öne getir
                malzemeSecimPanel.Visible = true;
                malzemeSecimPanel.BringToFront();
            }
            else
            {
                // Malzeme seçim panelini gizle
                malzemeSecimPanel.Visible = false;
                // Tarifleri arama metodu çağır
                SearchTarifler(searchText);
                searchResultPanel.BringToFront();
            }
        }
        // Tarif Sil butonuna tıklandığında yeni formu açar
        private void btnTarifSil_Click(object sender, EventArgs e)
        {
            TarifSilmeForm tarifSilForm = new TarifSilmeForm();
            tarifSilForm.Show();
        }

        // Tarif Güncelle butonuna tıklandığında yeni formu açar
        private void btnTarifGuncelle_Click(object sender, EventArgs e)
        {
            TarifGuncellemeForm tarifGuncelleForm = new TarifGuncellemeForm();
            tarifGuncelleForm.Show();
        }

        // Tarif Önerisi butonuna tıklandığında yeni formu açar
        private void btnTarifOnerisi_Click(object sender, EventArgs e)
        {
            TarifOnerisiForm tarifOnerisiForm = new TarifOnerisiForm();
            tarifOnerisiForm.Show();
        }


        // Malzemelerim butonuna tıklandığında yeni formu açar
        private void btnMalzemelerim_Click(object sender, EventArgs e)
        {
            MalzemelerimForm malzemelerimForm = new MalzemelerimForm();
            malzemelerimForm.Show();
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            ShowOptions();
        }

    }
    public class TarifMaliyetHesaplama
    {
        private readonly string connectionString;

        public TarifMaliyetHesaplama(string connString)
        {
            connectionString = connString;
        }

        public class MalzemeDetay
        {
            public string MalzemeAdi { get; set; }
            public double Miktar { get; set; }
            public string Birim { get; set; }
            public decimal BirimFiyat { get; set; }
            public decimal ToplamMaliyet { get; set; }
        }

        public (List<MalzemeDetay> malzemeler, decimal toplamMaliyet) HesaplaTarifMaliyeti(string tarifAdi)
        {
            var malzemeDetaylari = new List<MalzemeDetay>();
            decimal toplamMaliyet = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                SELECT 
                    m.MalzemeAdi,
                    tmi.MalzemeMiktar,
                    m.MalzemeBirim,
                    m.BirimFiyat
                FROM Tarifler t
                JOIN TarifMalzemeIliskisi tmi ON t.TarifID = tmi.TarifID
                JOIN Malzemeler m ON tmi.MalzemeID = m.MalzemeID
                WHERE t.TarifAdi = @TarifAdi";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var malzeme = new MalzemeDetay
                            {
                                MalzemeAdi = reader["MalzemeAdi"].ToString(),
                                Miktar = Convert.ToDouble(reader["MalzemeMiktar"]),
                                Birim = reader["MalzemeBirim"].ToString(),
                                BirimFiyat = Convert.ToDecimal(reader["BirimFiyat"])
                            };

                            // Birim dönüşümleri ve maliyet hesaplama
                            decimal malzemeMaliyeti = HesaplaMalzemeMaliyeti(
                                malzeme.Miktar,
                                malzeme.Birim,
                                malzeme.BirimFiyat
                            );

                            malzeme.ToplamMaliyet = malzemeMaliyeti;
                            malzemeDetaylari.Add(malzeme);
                            toplamMaliyet += malzemeMaliyeti;
                        }
                    }
                }
            }

            return (malzemeDetaylari, toplamMaliyet);
        }

        public decimal HesaplaMalzemeMaliyeti(double miktar, string birim, decimal birimFiyat)
        {
            // Birim dönüşümleri
            switch (birim.ToLower())
            {
                case "kg":
                case "kilo":
                    return birimFiyat * Convert.ToDecimal(miktar);

                case "gr":
                case "gram":
                    return birimFiyat * Convert.ToDecimal(miktar / 1000.0); // gramı kiloya çevir

                case "lt":
                case "litre":
                    return birimFiyat * Convert.ToDecimal(miktar);

                case "ml":
                case "mililitre":
                    return birimFiyat * Convert.ToDecimal(miktar / 1000.0); // mililitreyi litreye çevir

                case "adet":
                    return birimFiyat * Convert.ToDecimal(miktar);

                default:
                    return birimFiyat * Convert.ToDecimal(miktar);
            }
        }
    }
}