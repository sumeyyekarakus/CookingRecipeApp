using System;
using System.Drawing;
using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class MalzemelerimForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button btnMalzemeListele; // Listeleme butonu
        private Control panelMenu;
        private TextBox txtMalzemeID, txtMalzemeAdi, txtToplamMiktar, txtMalzemeBirim, txtBirimFiyat, txtMalzemeIDsil;
        private Button btnMalzemeEkle, btnMalzemeSil;
        private Label lblMalzemeID, lblMalzemeAdi, lblToplamMiktar, lblMalzemeBirim, lblBirimFiyat, lblMalzemeSil;
        private TextBox txtGuncelleMalzemeID, txtGuncelleMalzemeAdi, txtGuncelleToplamMiktar, txtGuncelleMalzemeBirim, txtGuncelleBirimFiyat;
        private Button btnMalzemeGuncelle;
        private Label lblGuncelleMalzemeID, lblGuncelleMalzemeAdi, lblGuncelleToplamMiktar, lblGuncelleMalzemeBirim, lblGuncelleBirimFiyat;

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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "MalzemelerimForm";


            // Form ayarları
            this.Controls.Add(this.panelMenu);
            this.Text = "MALZEMELERİM";
            this.ClientSize = new System.Drawing.Size(800, 450);

            this.btnMalzemeListele = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // btnMalzemeListele
            // 
            this.btnMalzemeListele.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            this.btnMalzemeListele.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMalzemeListele.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.btnMalzemeListele.ForeColor = System.Drawing.Color.White;
            this.btnMalzemeListele.Location = new System.Drawing.Point(200, 130); // Butonun konumu
            this.btnMalzemeListele.Name = "btnMalzemeListele";
            this.btnMalzemeListele.Size = new System.Drawing.Size(120, 40);
            this.btnMalzemeListele.TabIndex = 0;
            this.btnMalzemeListele.Text = "Malzemelerim";
            this.btnMalzemeListele.UseVisualStyleBackColor = false;
            this.btnMalzemeListele.Click += new System.EventHandler(this.btnMalzemeListele_Click); // Butonun tıklanma olayını ekliyoruz.

            // Form elemanlarını ekliyoruz
            this.Controls.Add(this.btnMalzemeListele);
            this.ResumeLayout(false);


            // Malzeme Ekle Alanları
            lblMalzemeID = new Label() { Text = "Malzeme ID", Location = new Point(800, 100), AutoSize = true };
            txtMalzemeID = new TextBox() { Location = new Point(800, 120), Width = 120 };

            lblMalzemeAdi = new Label() { Text = "Malzeme Adı", Location = new Point(800, 150), AutoSize = true };
            txtMalzemeAdi = new TextBox() { Location = new Point(800, 170), Width = 120 };

            lblToplamMiktar = new Label() { Text = "Toplam Miktar", Location = new Point(800, 200), AutoSize = true };
            txtToplamMiktar = new TextBox() { Location = new Point(800, 220), Width = 120 };

            lblMalzemeBirim = new Label() { Text = "Malzeme Birim", Location = new Point(800, 250), AutoSize = true };
            txtMalzemeBirim = new TextBox() { Location = new Point(800, 270), Width = 120 };

            lblBirimFiyat = new Label() { Text = "Birim Fiyat", Location = new Point(800, 300), AutoSize = true };
            txtBirimFiyat = new TextBox() { Location = new Point(800, 320), Width = 120 };

            btnMalzemeEkle = new Button() { Text = "EKLE", Location = new Point(800, 350), Width = 120, BackColor = Color.LightGreen };
            btnMalzemeEkle.Click += new EventHandler(this.btnMalzemeEkle_Click);

            // Malzeme Sil Alanı
            lblMalzemeSil = new Label() { Text = "Malzeme Sil (ID)", Location = new Point(800, 500), AutoSize = true };
            txtMalzemeIDsil = new TextBox() { Location = new Point(800, 520), Width = 120 };

            btnMalzemeSil = new Button() { Text = "SİL", Location = new Point(800, 550), Width = 120, BackColor = Color.IndianRed };
            btnMalzemeSil.Click += new EventHandler(this.btnMalzemeSil_Click);

            // Malzeme Güncelleme Alanları
            lblGuncelleMalzemeID = new Label() { Text = "Malzeme ID", Location = new Point(1000, 100), AutoSize = true };
            txtGuncelleMalzemeID = new TextBox() { Location = new Point(1000, 120), Width = 120 };

            lblGuncelleMalzemeAdi = new Label() { Text = "Malzeme Adı", Location = new Point(1000, 150), AutoSize = true };
            txtGuncelleMalzemeAdi = new TextBox() { Location = new Point(1000, 170), Width = 120 };

            lblGuncelleToplamMiktar = new Label() { Text = "Toplam Miktar", Location = new Point(1000, 200), AutoSize = true };
            txtGuncelleToplamMiktar = new TextBox() { Location = new Point(1000, 220), Width = 120 };

            lblGuncelleMalzemeBirim = new Label() { Text = "Malzeme Birim", Location = new Point(1000, 250), AutoSize = true };
            txtGuncelleMalzemeBirim = new TextBox() { Location = new Point(1000, 270), Width = 120 };

            lblGuncelleBirimFiyat = new Label() { Text = "Birim Fiyat", Location = new Point(1000, 300), AutoSize = true };
            txtGuncelleBirimFiyat = new TextBox() { Location = new Point(1000, 320), Width = 120 };

            btnMalzemeGuncelle = new Button() { Text = "GÜNCELLE", Location = new Point(1000, 350), Width = 120, BackColor = Color.LightBlue };
            btnMalzemeGuncelle.Click += new EventHandler(this.btnMalzemeGuncelle_Click);

            // Form elemanlarını ekliyoruz
            this.Controls.Add(lblMalzemeID);
            this.Controls.Add(txtMalzemeID);
            this.Controls.Add(lblMalzemeAdi);
            this.Controls.Add(txtMalzemeAdi);
            this.Controls.Add(lblToplamMiktar);
            this.Controls.Add(txtToplamMiktar);
            this.Controls.Add(lblMalzemeBirim);
            this.Controls.Add(txtMalzemeBirim);
            this.Controls.Add(lblBirimFiyat);
            this.Controls.Add(txtBirimFiyat);
            this.Controls.Add(btnMalzemeEkle);
            this.Controls.Add(lblMalzemeSil);
            this.Controls.Add(txtMalzemeIDsil);
            this.Controls.Add(btnMalzemeSil);
            this.Controls.Add(lblGuncelleMalzemeID);
            this.Controls.Add(txtGuncelleMalzemeID);
            this.Controls.Add(lblGuncelleMalzemeAdi);
            this.Controls.Add(txtGuncelleMalzemeAdi);
            this.Controls.Add(lblGuncelleToplamMiktar);
            this.Controls.Add(txtGuncelleToplamMiktar);
            this.Controls.Add(lblGuncelleMalzemeBirim);
            this.Controls.Add(txtGuncelleMalzemeBirim);
            this.Controls.Add(lblGuncelleBirimFiyat);
            this.Controls.Add(txtGuncelleBirimFiyat);
            this.Controls.Add(btnMalzemeGuncelle);
        }
    }

        #endregion
    }
