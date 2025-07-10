using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Panel panelMenu;
        private Label labelMenuTitle;
        private Button btnTarifEkle;
        private Button btnTarifSil;
        private Button btnTarifGuncelle;
        private Button btnTarifOnerisi;
        private Button btnMalzemelerim;
        private Button btnTarifListele; // Listeleme butonu

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMenu = new System.Windows.Forms.Panel();
            this.labelMenuTitle = new System.Windows.Forms.Label();
            // Diğer kontrol elemanları ve özellikler

            // labelMenuTitle
            this.labelMenuTitle.AutoSize = true;
            this.labelMenuTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelMenuTitle.ForeColor = System.Drawing.Color.White;
            this.labelMenuTitle.Location = new System.Drawing.Point(20, 20); // Başlık konumu
            this.labelMenuTitle.Name = "labelMenuTitle";
            this.labelMenuTitle.Size = new System.Drawing.Size(56, 20); // Yazı boyutu
            this.labelMenuTitle.Text = "MENÜ";

            

            // Panel tanımlaması
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnMenuToggle = new System.Windows.Forms.Button();
            this.btnTarifEkle = new System.Windows.Forms.Button();
            this.btnTarifSil = new System.Windows.Forms.Button();
            this.btnTarifGuncelle = new System.Windows.Forms.Button();
            this.btnTarifOnerisi = new System.Windows.Forms.Button();
            this.btnMalzemelerim = new System.Windows.Forms.Button();

            // Panel ayarları
            // panelMenu'nun yüksekliğini artırın
            this.panelMenu.Height = 100; // panel yüksekliğini arttırın (örneğin 100)
            this.panelMenu.Controls.Add(this.labelMenuTitle); // Başlık doğru şekilde eklenmeli

            this.panelMenu.Controls.Add(this.btnMalzemelerim);
            this.panelMenu.Controls.Add(this.btnTarifOnerisi);
            this.panelMenu.Controls.Add(this.btnTarifGuncelle);
            this.panelMenu.Controls.Add(this.btnTarifSil);
            this.panelMenu.Controls.Add(this.btnTarifEkle);
            this.panelMenu.Location = new System.Drawing.Point(12, 12);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(0, 426);  // Başlangıçta dar
            this.panelMenu.BackColor = System.Drawing.Color.LightGray;

            // Toggle butonu ayarları (menüyü açıp kapatacak buton)
            this.btnMenuToggle.Location = new System.Drawing.Point(10, 10);
            this.btnMenuToggle.Name = "btnMenuToggle";
            this.btnMenuToggle.Size = new System.Drawing.Size(40, 40);
            this.btnMenuToggle.Text = "☰"; // Menü ikonu
            this.btnMenuToggle.Click += new System.EventHandler(this.btnMenuToggle_Click);



            // Menü butonlarının başlangıç pozisyonlarını dar halde gösterin
            this.btnTarifEkle.Location = new System.Drawing.Point(10, 60);
            this.btnTarifEkle.Name = "btnTarifEkle";
            this.btnTarifEkle.Size = new System.Drawing.Size(180, 40);
            this.btnTarifEkle.Text = "Tarif Ekle";

            this.btnTarifSil.Location = new System.Drawing.Point(10, 110);
            this.btnTarifSil.Name = "btnTarifSil";
            this.btnTarifSil.Size = new System.Drawing.Size(180, 40);
            this.btnTarifSil.Text = "Tarif Sil";

            this.btnTarifGuncelle.Location = new System.Drawing.Point(10, 160);
            this.btnTarifGuncelle.Name = "btnTarifGuncelle";
            this.btnTarifGuncelle.Size = new System.Drawing.Size(180, 40);
            this.btnTarifGuncelle.Text = "Tarif Güncelle";

            this.btnTarifOnerisi.Location = new System.Drawing.Point(10, 210);
            this.btnTarifOnerisi.Name = "btnTarifOnerisi";
            this.btnTarifOnerisi.Size = new System.Drawing.Size(180, 40);
            this.btnTarifOnerisi.Text = "Tarif Önerisi";

            this.btnMalzemelerim.Location = new System.Drawing.Point(10, 260);
            this.btnMalzemelerim.Name = "btnMalzemelerim";
            this.btnMalzemelerim.Size = new System.Drawing.Size(180, 40);
            this.btnMalzemelerim.Text = "Malzemelerim";

            // Form ayarları
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.btnMenuToggle);
            this.Text = "Yemek Tarifleri Uygulaması";
            this.ClientSize = new System.Drawing.Size(800, 450);

            this.btnTarifListele = new System.Windows.Forms.Button();
            this.SuspendLayout();

            
        
            this.ResumeLayout(false);


        }

        #endregion
    }
}

