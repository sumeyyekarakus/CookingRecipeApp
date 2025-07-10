using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class TarifEklemeForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTarifID;
        private TextBox txtTarifID;
        private Label lblTarifAdi;
        private TextBox txtTarifAdi;
        private Label lblKategori;
        private TextBox txtKategori;
        private Label lblHazirlamaSuresi;
        private TextBox txtHazirlamaSuresi;
        private Label lblTalimatlar;
        private TextBox txtTalimatlar;
        private Button btnTarifEkle;

        
        private TextBox txtYeniMalzeme;
        private TextBox txtToplamMiktar;
        private Button btnMalzemeEkle;

        private CheckedListBox clbMalzemeler; // Malzemeler için CheckedListBox
        private Dictionary<string, TextBox> malzemeMiktarTextBoxes; // Her malzeme için miktar giriş alanı
        private TextBox txtMalzemeBirim;
        private TextBox txtBirimFiyat;
        private Control panelMenu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()

        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "TarifEklemeForm";

           
            
            // Tarif Ekle Alanları (Mevcut kodlar)
            lblTarifID = new Label() { Text = "Tarif ID", Location = new Point(200, 100), AutoSize = true };
            txtTarifID = new TextBox() { Location = new Point(200, 120), Width = 120 };

            lblTarifAdi = new Label() { Text = "Tarif Adı", Location = new Point(200, 150), AutoSize = true };
            txtTarifAdi = new TextBox() { Location = new Point(200, 170), Width = 120 };

            lblKategori = new Label() { Text = "Kategori", Location = new Point(200, 200), AutoSize = true };
            txtKategori = new TextBox() { Location = new Point(200, 220), Width = 120 };

            lblHazirlamaSuresi = new Label() { Text = "Hazırlama Süresi", Location = new Point(200, 250), AutoSize = true };
            txtHazirlamaSuresi = new TextBox() { Location = new Point(200, 270), Width = 120 };

            lblTalimatlar = new Label() { Text = "Talimatlar", Location = new Point(200, 300), AutoSize = true };
            txtTalimatlar = new TextBox()
            {
                Location = new Point(200, 320),
                Width = 400,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnTarifEkle = new Button() { Text = "EKLE", Location = new Point(200, 700), Width = 120, BackColor = Color.LightGreen };
            btnTarifEkle.Click += new EventHandler(this.btnTarifEkle_Click);

            // Malzeme Seçimi (CheckedListBox)
            // Malzeme seçimi için label'ı güncelle
        Label lblMalzemeler = new Label()
        {
            Text = $"Malzemeler (En fazla {MAX_MALZEME_SAYISI} malzeme seçebilirsiniz)",
            Location = new Point(200, 450),
            AutoSize = true
        };

            // CheckedListBox konumunu güncelle
            clbMalzemeler = new CheckedListBox()
            {
                Location = new Point(200, 480),
                Width = 150,
                Height = 200  // Yüksekliği artırıldı
            };

            // Event handler'ları güncelle
            clbMalzemeler.ItemCheck += new ItemCheckEventHandler(this.clbMalzemeler_ItemCheck);


            // Form'a ekleme
            this.Controls.Add(lblTarifID);
            this.Controls.Add(txtTarifID);
            this.Controls.Add(lblTarifAdi);
            this.Controls.Add(txtTarifAdi);
            this.Controls.Add(lblKategori);
            this.Controls.Add(txtKategori);
            this.Controls.Add(lblHazirlamaSuresi);
            this.Controls.Add(txtHazirlamaSuresi);
            this.Controls.Add(lblTalimatlar);
            this.Controls.Add(txtTalimatlar);
            this.Controls.Add(btnTarifEkle);
            this.Controls.Add(lblMalzemeler);
            this.Controls.Add(clbMalzemeler);

            // Malzeme miktarları için TextBox'lar
            malzemeMiktarTextBoxes = new Dictionary<string, TextBox>();
            LoadMalzemeler();  // Veritabanından malzemeleri yükler

            // Yeni malzeme ekleme alanları
        txtYeniMalzeme = new TextBox()
        {
            Location = new Point(800, 450),
            Width = 150,
            Visible = false
        };

            txtToplamMiktar = new TextBox()
            {
                Location = new Point(800, 480),
                Width = 150,
                Visible = false
            };

            txtMalzemeBirim = new TextBox()
            {
                Location = new Point(800, 510),
                Width = 150,
                Visible = false
            };

            txtBirimFiyat = new TextBox()
            {
                Location = new Point(800, 540),
                Width = 150,
                Visible = false
            };

            btnMalzemeEkle = new Button()
            {
                Text = "Yeni Malzeme Ekle",
                Location = new Point(800, 570),
                Width = 150,
                Visible = false
            };

            this.Controls.Add(txtYeniMalzeme);
            this.Controls.Add(txtToplamMiktar);
            this.Controls.Add(txtMalzemeBirim);
            this.Controls.Add(txtBirimFiyat);
            this.Controls.Add(btnMalzemeEkle);

            // Event handlers
            clbMalzemeler.SelectedIndexChanged += new EventHandler(this.clbMalzemeler_SelectedIndexChanged);
            btnMalzemeEkle.Click += new EventHandler(this.btnMalzemeEkle_Click);
        }
    }
}

        #endregion