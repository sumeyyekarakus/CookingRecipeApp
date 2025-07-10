using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace YemekTarifiAPP
{
    public partial class TarifSilmeForm : Form
    {
        // Veritabanı bağlantı dizesi
        public string connectionString = "Data Source=your_server;Initial Catalog=your_db_name;Integrated Security=True";

        public TarifSilmeForm()
        {
            InitializeComponent();
            CustomizeUI();
        }

        private void CustomizeUI()
        {
            this.BackColor = Color.FromArgb(255, 245, 245); // Çok açık pembe bir arka plan

            // 1- Başlık
            Label labelTitle = new Label();
            labelTitle.Text = "YEMEK TARİFİ SİL";
            labelTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            labelTitle.Location = new Point(150, 20); // Başlık konumu
            labelTitle.AutoSize = true;
            this.Controls.Add(labelTitle);
        }

        // Tarif silme butonu tıklama olayı
        private void btnTarifSil_Click(object sender, EventArgs e)
        {
            string tarifAdi = txtTarifAdi.Text;

            if (string.IsNullOrEmpty(tarifAdi))
            {
                MessageBox.Show("Lütfen bir tarif adı giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Tarif adından TarifID'yi bulma sorgusu
                    string queryGetTarifID = "SELECT TarifID FROM Tarifler WHERE TarifAdi = @TarifAdi";
                    SqlCommand commandGetTarifID = new SqlCommand(queryGetTarifID, connection);
                    commandGetTarifID.Parameters.AddWithValue("@TarifAdi", tarifAdi);

                    object result = commandGetTarifID.ExecuteScalar();

                    if (result != null)
                    {
                        int tarifID = Convert.ToInt32(result);

                        // İlgili TarifMalzemeIliskisi verilerini sil
                        string queryDeleteMalzeme = "DELETE FROM TarifMalzemeIliskisi WHERE TarifID = @TarifID";
                        SqlCommand commandDeleteMalzeme = new SqlCommand(queryDeleteMalzeme, connection);
                        commandDeleteMalzeme.Parameters.AddWithValue("@TarifID", tarifID);
                        commandDeleteMalzeme.ExecuteNonQuery();

                        // Tarifler tablosundan tarifi sil
                        string queryDeleteTarif = "DELETE FROM Tarifler WHERE TarifID = @TarifID";
                        SqlCommand commandDeleteTarif = new SqlCommand(queryDeleteTarif, connection);
                        commandDeleteTarif.Parameters.AddWithValue("@TarifID", tarifID);
                        commandDeleteTarif.ExecuteNonQuery();

                        MessageBox.Show($"{tarifAdi} başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Belirtilen tarif adı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // TextBox'ı temizle
                    txtTarifAdi.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tarif silinirken bir hata oluştu: " + ex.Message);
                }
            }
        }
    }
}
