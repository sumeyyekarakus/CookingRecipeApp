using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace YemekTarifiAPP
{
    public partial class TarifGuncellemeForm : Form
    {
        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";
        private Dictionary<string, TextBox> malzemeMiktarTextBoxes = new Dictionary<string, TextBox>();
        private int currentY = 290; // Malzeme TextBox'ları için başlangıç Y koordinatı

        public TarifGuncellemeForm()
        {
            InitializeComponent();

            SetupAutoComplete();
            CustomizeUI();
        }
        private void CustomizeUI()
        {
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan

            // Başlık Label'ı oluştur
            Label lblBaslik = new Label
            {
                Text = "Yemek Tarifi Güncelle",
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
        private void SetupAutoComplete()
        {
            txtTarifAdi.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtTarifAdi.AutoCompleteSource = AutoCompleteSource.CustomSource;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TarifAdi FROM Tarifler";
                SqlCommand cmd = new SqlCommand(query, conn);

                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        autoComplete.Add(reader["TarifAdi"].ToString());
                    }
                }
                txtTarifAdi.AutoCompleteCustomSource = autoComplete;
            }

            txtTarifAdi.TextChanged += TarifAdi_TextChanged;
        }


        private void TarifAdi_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTarifAdi.Text)) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Tarif ve malzeme bilgilerini getir
                string query = @"
                    SELECT t.*, tm.MalzemeID, tm.MalzemeMiktar, m.MalzemeAdi 
                    FROM Tarifler t
                    LEFT JOIN TarifMalzemeIliskisi tm ON t.TarifID = tm.TarifID
                    LEFT JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID
                    WHERE t.TarifAdi = @TarifAdi";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TarifAdi", txtTarifAdi.Text);

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    // Temel tarif bilgilerini doldur
                    DataRow firstRow = dt.Rows[0];
                    txtKategori.Text = firstRow["Kategori"].ToString();
                    txtHazirlamaSuresi.Text = firstRow["HazirlamaSuresi"].ToString();
                    txtTalimatlar.Text = firstRow["Talimatlar"].ToString();

                    // Malzeme listesini temizle ve yeniden doldur
                    ClearMalzemeler();
                    clbMalzemeler.Items.Clear();

                    // Sadece bu tarife ait malzemeleri ekle
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!row.IsNull("MalzemeAdi"))
                        {
                            string malzemeAdi = row["MalzemeAdi"].ToString();
                            decimal malzemeMiktar = Convert.ToDecimal(row["MalzemeMiktar"]);

                            // Malzemeyi listeye ekle
                            clbMalzemeler.Items.Add(malzemeAdi, true);

                            // Miktar kontrolünü ekle
                            AddMalzemeMiktarControl(malzemeAdi, malzemeMiktar.ToString());
                        }
                    }
                }
            }
        }

        private void ClearMalzemeler()
        {
            // Miktar TextBox'larını ve labelları temizle
            foreach (var textBox in malzemeMiktarTextBoxes.Values)
            {
                this.Controls.Remove(textBox);
            }

            foreach (Control control in this.Controls.OfType<Label>().ToList())
            {
                if (control.Name?.StartsWith("lblMiktar_") == true)
                {
                    this.Controls.Remove(control);
                }
            }

            malzemeMiktarTextBoxes.Clear();
            currentY = 290;
        }

        private void AddMalzemeMiktarControl(string malzemeAdi, string miktar)
        {
            Label lblMiktar = new Label()
            {
                Text = $"{malzemeAdi} Miktarı:",
                Location = new System.Drawing.Point(400, currentY),
                AutoSize = true,
                Name = $"lblMiktar_{malzemeAdi}"
            };

            TextBox txtMiktar = new TextBox()
            {
                Location = new System.Drawing.Point(550, currentY),
                Width = 100,
                Name = $"txtMiktar_{malzemeAdi}",
                Text = miktar
            };

            this.Controls.Add(lblMiktar);
            this.Controls.Add(txtMiktar);
            malzemeMiktarTextBoxes[malzemeAdi] = txtMiktar;
            currentY += 30;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Tarif ID'sini al
                    string getTarifIdQuery = "SELECT TarifID FROM Tarifler WHERE TarifAdi = @TarifAdi";
                    SqlCommand cmdGetTarifId = new SqlCommand(getTarifIdQuery, connection, transaction);
                    cmdGetTarifId.Parameters.AddWithValue("@TarifAdi", txtTarifAdi.Text);
                    int tarifId = (int)cmdGetTarifId.ExecuteScalar();

                    // Tarif bilgilerini güncelle
                    string updateTarifQuery = @"
                UPDATE Tarifler 
                SET Kategori = @Kategori, 
                    HazirlamaSuresi = @HazirlamaSuresi, 
                    Talimatlar = @Talimatlar
                WHERE TarifID = @TarifID";

                    SqlCommand cmdTarif = new SqlCommand(updateTarifQuery, connection, transaction);
                    cmdTarif.Parameters.AddWithValue("@TarifID", tarifId);
                    cmdTarif.Parameters.AddWithValue("@Kategori", txtKategori.Text);
                    cmdTarif.Parameters.AddWithValue("@HazirlamaSuresi", txtHazirlamaSuresi.Text);
                    cmdTarif.Parameters.AddWithValue("@Talimatlar", txtTalimatlar.Text);
                    cmdTarif.ExecuteNonQuery();

                    // Mevcut malzeme ilişkilerini sil
                    string deleteMalzemeQuery = "DELETE FROM TarifMalzemeIliskisi WHERE TarifID = @TarifID";
                    SqlCommand cmdDelete = new SqlCommand(deleteMalzemeQuery, connection, transaction);
                    cmdDelete.Parameters.AddWithValue("@TarifID", tarifId);
                    cmdDelete.ExecuteNonQuery();

                    // Sadece CheckedListBox'ta işaretli olan malzemeleri güncelle
                    for (int i = 0; i < clbMalzemeler.Items.Count; i++)
                    {
                        string malzemeAdi = clbMalzemeler.Items[i].ToString();

                        // Eğer malzeme işaretli değilse, bu malzemeyi atla
                        if (!clbMalzemeler.GetItemChecked(i))
                            continue;

                        // Malzeme TextBox'ı var mı ve geçerli bir değer içeriyor mu kontrol et
                        if (malzemeMiktarTextBoxes.TryGetValue(malzemeAdi, out TextBox miktarTextBox))
                        {
                            string miktarText = miktarTextBox.Text;

                            if (!decimal.TryParse(miktarText, out decimal miktar))
                            {
                                throw new Exception($"Geçersiz miktar değeri: {miktarText} - Malzeme: {malzemeAdi}");
                            }

                            // Malzeme ID'sini al
                            string getMalzemeIdQuery = "SELECT MalzemeID FROM Malzemeler WHERE MalzemeAdi = @MalzemeAdi";
                            SqlCommand cmdGetMalzemeId = new SqlCommand(getMalzemeIdQuery, connection, transaction);
                            cmdGetMalzemeId.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                            int malzemeId = (int)cmdGetMalzemeId.ExecuteScalar();

                            // Malzeme ilişkisini ekle
                            string insertMalzemeQuery = @"
                        INSERT INTO TarifMalzemeIliskisi (TarifID, MalzemeID, MalzemeMiktar) 
                        VALUES (@TarifID, @MalzemeID, @MalzemeMiktar)";

                            SqlCommand cmdInsert = new SqlCommand(insertMalzemeQuery, connection, transaction);
                            cmdInsert.Parameters.AddWithValue("@TarifID", tarifId);
                            cmdInsert.Parameters.AddWithValue("@MalzemeID", malzemeId);
                            cmdInsert.Parameters.AddWithValue("@MalzemeMiktar", miktar);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Tarif başarıyla güncellendi!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


    }
}