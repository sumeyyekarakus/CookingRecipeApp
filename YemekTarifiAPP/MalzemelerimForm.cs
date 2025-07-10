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
    public partial class MalzemelerimForm : Form
    {
        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";

        public MalzemelerimForm()
        {
            InitializeComponent();
            CustomizeUI();
            malzemelerListBox = new ListBox(); // ListBox'ı başlatıyoruz
            malzemelerListBox.Location = new System.Drawing.Point(200, 180);
            malzemelerListBox.Size = new System.Drawing.Size(500, 500);
            this.Controls.Add(malzemelerListBox); // ListBox'ı form üzerinde ekliyoruz
        }
        private ListBox malzemelerListBox;

        private void btnMalzemeListele_Click(object sender, EventArgs e)
        {
            // Tarifleri kategorilere ayırarak listelemek için fonksiyonu çağırıyoruz.
            GetMalzemeler(malzemelerListBox);
        }

        private void CustomizeUI()
        {
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan
        }
        public void GetMalzemeler(ListBox listBox)
        {


            // ListBox'ı temizle
            listBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Her kategori için tarifleri alalım


                // Kategoriye göre tarifleri almak için sorgu
                string query = "SELECT * FROM Malzemeler";
                SqlCommand command = new SqlCommand(query, connection);


                SqlDataReader reader = command.ExecuteReader();
                //MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat
                // Her kategori için tarifleri listeye ekleyelim
                while (reader.Read())
                {
                    int MalzemeID = Convert.ToInt32(reader["MalzemeID"]);
                    string MalzemeAdi = reader["MalzemeAdi"].ToString();
                    string ToplamMiktar = reader["ToplamMiktar"].ToString();
                    string MalzemeBirim = reader["MalzemeBirim"].ToString();
                    int BirimFiyat = Convert.ToInt32(reader["BirimFiyat"]);
                    listBox.Items.Add($"  ID: {MalzemeID} - {MalzemeAdi} - {ToplamMiktar} {MalzemeBirim} - {BirimFiyat} Tl"); // Tarifi listeye ekleyelim
                }

                // Kategoriler arası boşluk bırakalım
                listBox.Items.Add("");

                reader.Close();

            }
        }

        // Malzeme Ekleme Fonksiyonu
        private void btnMalzemeEkle_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Malzemeler (MalzemeID, MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat) VALUES (@MalzemeID, @MalzemeAdi, @ToplamMiktar, @MalzemeBirim, @BirimFiyat)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MalzemeID", txtMalzemeID.Text);
                command.Parameters.AddWithValue("@MalzemeAdi", txtMalzemeAdi.Text);
                command.Parameters.AddWithValue("@ToplamMiktar", txtToplamMiktar.Text);
                command.Parameters.AddWithValue("@MalzemeBirim", txtMalzemeBirim.Text);
                command.Parameters.AddWithValue("@BirimFiyat", txtBirimFiyat.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Malzeme başarıyla eklendi!");
                    // İşlem sonrası TextBox'ları temizleyin
                    txtMalzemeID.Text = string.Empty;
                    txtMalzemeAdi.Text = string.Empty;
                    txtToplamMiktar.Text = string.Empty;
                    txtMalzemeBirim.Text = string.Empty;
                    txtBirimFiyat.Text = string.Empty;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }

            }
        }

        // Malzeme Silme Fonksiyonu
        private void btnMalzemeSil_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Malzemeler WHERE MalzemeID = @MalzemeID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MalzemeID", txtMalzemeIDsil.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Malzeme başarıyla silindi!");
                    // İşlem sonrası yalnızca Malzeme Sil alanındaki TextBox'ı temizleyin
                    txtMalzemeIDsil.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        // Güncelleme işlemi için olay metodu
        private void btnMalzemeGuncelle_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Malzemeler SET MalzemeAdi = @MalzemeAdi, ToplamMiktar = @ToplamMiktar, MalzemeBirim = @MalzemeBirim, BirimFiyat = @BirimFiyat WHERE MalzemeID = @MalzemeID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MalzemeID", txtGuncelleMalzemeID.Text);
                command.Parameters.AddWithValue("@MalzemeAdi", txtGuncelleMalzemeAdi.Text);
                command.Parameters.AddWithValue("@ToplamMiktar", txtGuncelleToplamMiktar.Text);
                command.Parameters.AddWithValue("@MalzemeBirim", txtGuncelleMalzemeBirim.Text);
                command.Parameters.AddWithValue("@BirimFiyat", txtGuncelleBirimFiyat.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Malzeme başarıyla güncellendi!");
                    // İşlem sonrası TextBox'ları temizleyin
                    txtGuncelleMalzemeID.Text = string.Empty;
                    txtGuncelleMalzemeAdi.Text = string.Empty;
                    txtGuncelleToplamMiktar.Text = string.Empty;
                    txtGuncelleMalzemeBirim.Text = string.Empty;
                    txtGuncelleBirimFiyat.Text = string.Empty;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
    }
}
