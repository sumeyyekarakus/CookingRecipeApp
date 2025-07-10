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
    public partial class TarifEklemeForm : Form
    {
        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";
        private const int MAX_MALZEME_SAYISI = 10; // Maksimum seçilebilecek malzeme sayısı
        private int currentY = 480; // Başlangıç Y koordinatı

        public TarifEklemeForm()
        {
            InitializeComponent();
            CustomizeUI();
            YerTutucuAyarla(txtTarifAdi, "Tarif Adı");
            YerTutucuAyarla(txtKategori, "Kategori");
            YerTutucuAyarla(txtHazirlamaSuresi, "Hazırlama Süresi");
        }
        private void CustomizeUI()
        {
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan

            // Başlık Label'ı oluştur
            Label lblBaslik = new Label
            {
                Text = "Yemek Tarifi Ekle",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64), // Koyu gri renk
                AutoSize = true
            };

            // Label'ı formun üst kısmında ortalı yerleştir
            lblBaslik.Location = new Point(
                (this.ClientSize.Width - lblBaslik.Width) / 2,
                20
            );

            // Form yeniden boyutlandırıldığında başlığı ortala
            this.Resize += (sender, e) =>
            {
                lblBaslik.Location = new Point(
                    (this.ClientSize.Width - lblBaslik.Width) / 2,
                    20
                );
            };

            this.Controls.Add(lblBaslik);
        }

        // Tarif ekleme butonunu güncelleme
        private void btnTarifEkle_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // 1. Önce tarifi ekle
                    string tarifQuery = @"INSERT INTO Tarifler 
                (TarifID, TarifAdi, Kategori, HazirlamaSuresi, Talimatlar) 
                VALUES (@TarifID, @TarifAdi, @Kategori, @HazirlamaSuresi, @Talimatlar)";

                    SqlCommand cmdTarif = new SqlCommand(tarifQuery, connection, transaction);
                    cmdTarif.Parameters.AddWithValue("@TarifID", txtTarifID.Text);
                    cmdTarif.Parameters.AddWithValue("@TarifAdi", txtTarifAdi.Text);
                    cmdTarif.Parameters.AddWithValue("@Kategori", txtKategori.Text);
                    cmdTarif.Parameters.AddWithValue("@HazirlamaSuresi", txtHazirlamaSuresi.Text);
                    cmdTarif.Parameters.AddWithValue("@Talimatlar", txtTalimatlar.Text);

                    cmdTarif.ExecuteNonQuery();

                    // 2. Her bir seçili malzeme için TarifMalzemeIliskisi tablosuna veri ekle
                    foreach (var malzeme in malzemeMiktarTextBoxes)
                    {
                        string malzemeAdi = malzeme.Key;
                        string miktarText = malzeme.Value.Text;

                        // Miktar değerini kontrol et
                        if (!decimal.TryParse(miktarText, out decimal miktar))
                        {
                            throw new Exception($"Geçersiz miktar değeri: {miktarText} - Malzeme: {malzemeAdi}");
                        }

                        // Malzeme ID'sini al - LIKE kullanarak daha esnek arama yapalım
                        string getMalzemeIdQuery = "SELECT MalzemeID FROM Malzemeler WHERE MalzemeAdi LIKE @MalzemeAdi";
                        SqlCommand cmdGetMalzemeId = new SqlCommand(getMalzemeIdQuery, connection, transaction);
                        cmdGetMalzemeId.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);

                        object malzemeIdResult = cmdGetMalzemeId.ExecuteScalar();

                        if (malzemeIdResult == null)
                        {
                            // Eğer malzeme bulunamazsa, hata yerine debug bilgisi yazdıralım
                            MessageBox.Show($"Debug - Malzeme bulunamadı: {malzemeAdi}");
                            continue;
                        }

                        int malzemeId = (int)malzemeIdResult;

                        // TarifMalzemeIliskisi tablosuna veri ekle
                        string insertTarifMalzemeQuery = @"INSERT INTO TarifMalzemeIliskisi
                    (TarifID, MalzemeID, MalzemeMiktar) 
                    VALUES (@TarifID, @MalzemeID, @MalzemeMiktar)";

                        SqlCommand cmdTarifMalzeme = new SqlCommand(insertTarifMalzemeQuery, connection, transaction);
                        cmdTarifMalzeme.Parameters.AddWithValue("@TarifID", txtTarifID.Text);
                        cmdTarifMalzeme.Parameters.AddWithValue("@MalzemeID", malzemeId);
                        cmdTarifMalzeme.Parameters.AddWithValue("@MalzemeMiktar", miktar);

                        cmdTarifMalzeme.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Tarif ve malzemeler başarıyla eklendi!");

                    // Formu temizle
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata: " + ex.Message);

                    // Debug bilgisi
                    MessageBox.Show("Debug - Seçili malzemeler: " +
                        string.Join(", ", malzemeMiktarTextBoxes.Keys));
                }
            }
        }

        private void ClearForm()
        {
            txtTarifAdi.Text = "Tarif Adı";
            txtKategori.Text = "Kategori";
            txtHazirlamaSuresi.Text = "Hazırlama Süresi";
            txtTalimatlar.Clear();

            // Malzeme seçimlerini temizle
            foreach (int i in clbMalzemeler.CheckedIndices)
            {
                clbMalzemeler.SetItemChecked(i, false);
            }

            // Miktar textbox'larını temizle
            foreach (var textBox in malzemeMiktarTextBoxes.Values)
            {
                this.Controls.Remove(textBox);
            }
            malzemeMiktarTextBoxes.Clear();

            // Malzeme kontrollerini temizle
            foreach (var malzeme in malzemeMiktarTextBoxes.Keys.ToList())
            {
                // Label'ları temizle
                Control[] labels = this.Controls.Find($"lblMiktar_{malzeme}", true);
                foreach (Control label in labels)
                {
                    this.Controls.Remove(label);
                }

                // TextBox'ları temizle
                this.Controls.Remove(malzemeMiktarTextBoxes[malzeme]);
            }

            malzemeMiktarTextBoxes.Clear();
            currentY = 480; // Y koordinatını sıfırla
        }


        // Veritabanından Malzemeleri ComboBox'a yükler
        private void LoadMalzemeler()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MalzemeAdi FROM Malzemeler";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clbMalzemeler.Items.Add(reader["MalzemeAdi"].ToString());
                }

                clbMalzemeler.Items.Add("Diğer");
            }
        }

        // Malzeme ComboBox'ında "Diğer" seçilirse, yeni malzeme alanı açılır
        private void clbMalzemeler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbMalzemeler.SelectedItem?.ToString() == "Diğer")
            {
                txtYeniMalzeme.Visible = true;
                txtToplamMiktar.Visible = true;
                txtMalzemeBirim.Visible = true;
                txtBirimFiyat.Visible = true;
                btnMalzemeEkle.Visible = true;

                // Placeholder texts
                YerTutucuAyarla(txtYeniMalzeme, "Malzeme Adı");
                YerTutucuAyarla(txtToplamMiktar, "Toplam Miktar");
                YerTutucuAyarla(txtMalzemeBirim, "Birim (kg, lt, adet)");
                YerTutucuAyarla(txtBirimFiyat, "Birim Fiyat");
            }
            else
            {
                txtYeniMalzeme.Visible = false;
                txtToplamMiktar.Visible = false;
                txtMalzemeBirim.Visible = false;
                txtBirimFiyat.Visible = false;
                btnMalzemeEkle.Visible = false;
            }
        }


        private void btnMalzemeEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtYeniMalzeme.Text) ||
                txtYeniMalzeme.Text == "Malzeme Adı")
            {
                MessageBox.Show("Lütfen geçerli bir malzeme adı giriniz.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Son MalzemeID'yi bul
                    string maxIdQuery = "SELECT ISNULL(MAX(MalzemeID), 0) + 1 FROM Malzemeler";
                    SqlCommand maxIdCommand = new SqlCommand(maxIdQuery, connection, transaction);
                    int yeniMalzemeID = (int)maxIdCommand.ExecuteScalar();

                    string yeniMalzemeAdi = txtYeniMalzeme.Text.Trim(); // Malzeme adını al ve boşlukları temizle

                    // Malzemeler tablosuna veri ekleme
                    string insertQuery = @"INSERT INTO Malzemeler 
                (MalzemeID, MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat) 
                VALUES (@MalzemeID, @MalzemeAdi, 0, @MalzemeBirim, @BirimFiyat)";

                    SqlCommand cmd = new SqlCommand(insertQuery, connection, transaction);
                    cmd.Parameters.AddWithValue("@MalzemeID", yeniMalzemeID);
                    cmd.Parameters.AddWithValue("@MalzemeAdi", yeniMalzemeAdi);
                    cmd.Parameters.AddWithValue("@MalzemeBirim", txtMalzemeBirim.Text == "Birim (kg, lt, adet)" ? "Birim" : txtMalzemeBirim.Text);
                    cmd.Parameters.AddWithValue("@BirimFiyat", txtBirimFiyat.Text == "Birim Fiyat" ? 0 : Convert.ToDecimal(txtBirimFiyat.Text));

                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                    // CheckedListBox'ı güncelle
                    int indexOfDiger = clbMalzemeler.Items.IndexOf("Diğer");
                    if (indexOfDiger != -1)
                    {
                        clbMalzemeler.Items.Insert(indexOfDiger, yeniMalzemeAdi); // "Diğer" seçeneğinden önce ekle
                        int yeniMalzemeIndex = indexOfDiger;
                        

                        currentY += 30; // Y koordinatını güncelle
                    }
                    else
                    {
                        clbMalzemeler.Items.Add(yeniMalzemeAdi);
                        clbMalzemeler.SetItemChecked(clbMalzemeler.Items.Count - 1, true);
                    }

                    MessageBox.Show("Yeni malzeme başarıyla eklendi ve seçildi!");

                    // Alanları temizle
                    txtYeniMalzeme.Text = "Malzeme Adı";
                    txtToplamMiktar.Text = "Toplam Miktar";
                    txtMalzemeBirim.Text = "Birim (kg, lt, adet)";
                    txtBirimFiyat.Text = "Birim Fiyat";

                    // Alanları gizle
                    txtYeniMalzeme.Visible = false;
                    txtToplamMiktar.Visible = false;
                    txtMalzemeBirim.Visible = false;
                    txtBirimFiyat.Visible = false;
                    btnMalzemeEkle.Visible = false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
        private void clbMalzemeler_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Seçili malzeme sayısını kontrol et
            int checkedCount = clbMalzemeler.CheckedItems.Count;
            if (e.NewValue == CheckState.Checked && checkedCount >= MAX_MALZEME_SAYISI)
            {
                e.NewValue = CheckState.Unchecked;
                MessageBox.Show($"En fazla {MAX_MALZEME_SAYISI} malzeme seçebilirsiniz!");
                return;
            }

            string selectedMalzeme = clbMalzemeler.Items[e.Index].ToString();

            if (e.NewValue == CheckState.Checked)
            {
                if (selectedMalzeme != "Diğer")
                {
                    // Malzeme için label oluştur
                    Label lblMiktar = new Label()
                    {
                        Text = $"{selectedMalzeme} Miktarı:",
                        Location = new Point(400, currentY),
                        AutoSize = true,
                        Name = $"lblMiktar_{selectedMalzeme}" // Add this line to set the Name property
                    };

                    // Miktar için TextBox oluştur
                    TextBox txtMiktar = new TextBox()
                    {
                        Location = new Point(550, currentY),
                        Width = 100,
                        Name = $"txtMiktar_{selectedMalzeme}"
                    };

                    // Form'a ekle
                    this.Controls.Add(lblMiktar);
                    this.Controls.Add(txtMiktar);

                    // Sözlüğe kaydet
                    malzemeMiktarTextBoxes[selectedMalzeme] = txtMiktar;

                    // Bir sonraki kontrol için Y koordinatını güncelle
                    currentY += 30;
                }
            }
            else if (e.NewValue == CheckState.Unchecked && malzemeMiktarTextBoxes.ContainsKey(selectedMalzeme))
            {
                // Label'ı bul ve kaldır
                Control[] labels = this.Controls.Find($"lblMiktar_{selectedMalzeme}", true);
                foreach (Control label in labels)
                {
                    this.Controls.Remove(label);
                }

                // TextBox'ı kaldır
                this.Controls.Remove(malzemeMiktarTextBoxes[selectedMalzeme]);
                malzemeMiktarTextBoxes.Remove(selectedMalzeme);

                // Kalan kontrollerin konumlarını yeniden düzenle
                ReorganizeMalzemelerUI();
            }
        }
        private void ReorganizeMalzemelerUI()
        {
            currentY = 480; // Başlangıç Y koordinatını sıfırla

            // Tüm seçili malzemelerin kontrollerini yeniden düzenle
            foreach (string malzeme in malzemeMiktarTextBoxes.Keys)
            {
                // Label'ı güncelle
                Control[] labels = this.Controls.Find($"lblMiktar_{malzeme}", true);
                if (labels.Length > 0)
                {
                    labels[0].Location = new Point(400, currentY);
                }

                // TextBox'ı güncelle
                malzemeMiktarTextBoxes[malzeme].Location = new Point(550, currentY);

                currentY += 30;
            }
        }


        // Yer tutucu ayarlayan metod
        private void YerTutucuAyarla(TextBox textBox, string yerTutucu)
        {
            textBox.Text = yerTutucu;
            textBox.ForeColor = Color.Gray;

            // Kullanıcı metin kutusuna tıkladığında yer tutucuyu temizle
            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == yerTutucu)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            // Kullanıcı metin kutusundan ayrıldığında ve metin boşsa yer tutucuyu geri getir
            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = yerTutucu;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }


    }

}
