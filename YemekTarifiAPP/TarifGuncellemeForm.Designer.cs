using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class TarifGuncellemeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Bileşenler
        private System.Windows.Forms.TextBox txtTarifAdi;
        private System.Windows.Forms.TextBox txtKategori;
        private System.Windows.Forms.TextBox txtHazirlamaSuresi;
        private System.Windows.Forms.TextBox txtTalimatlar;
        private System.Windows.Forms.CheckedListBox clbMalzemeler;
        private System.Windows.Forms.Button btnGuncelle;
        private System.Windows.Forms.Label lblTarifAdi;
        private System.Windows.Forms.Label lblKategori;
        private System.Windows.Forms.Label lblMalzemeler;
        private System.Windows.Forms.Label lblHazirlamaSuresi;
        private System.Windows.Forms.Label lblTalimatlar;
        

        // Tasarımcı tarafından otomatik oluşturulan Dispose metodu
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Formun tasarım kısmı
        private void InitializeComponent()
        {
            

            this.txtTarifAdi = new System.Windows.Forms.TextBox();
            this.txtKategori = new System.Windows.Forms.TextBox();
            this.txtHazirlamaSuresi = new System.Windows.Forms.TextBox();
            this.txtTalimatlar = new System.Windows.Forms.TextBox();
            this.clbMalzemeler = new System.Windows.Forms.CheckedListBox();
            this.lblMalzemeler = new System.Windows.Forms.Label();
            this.btnGuncelle = new System.Windows.Forms.Button();
            this.lblTarifAdi = new System.Windows.Forms.Label();
            this.lblKategori = new System.Windows.Forms.Label();
            this.lblHazirlamaSuresi = new System.Windows.Forms.Label();
            this.lblTalimatlar = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // 
            // txtTarifAdi
            // 
            this.txtTarifAdi.Location = new System.Drawing.Point(150, 100);
            this.txtTarifAdi.Name = "txtTarifAdi";
            this.txtTarifAdi.Size = new System.Drawing.Size(200, 20);
            this.txtTarifAdi.TabIndex = 0;

            // 
            // txtKategori
            // 
            this.txtKategori.Location = new System.Drawing.Point(150, 140);
            this.txtKategori.Name = "txtKategori";
            this.txtKategori.Size = new System.Drawing.Size(200, 20);
            this.txtKategori.TabIndex = 1;

            // 
            // txtHazirlamaSuresi
            // 
            this.txtHazirlamaSuresi.Location = new System.Drawing.Point(150, 180);
            this.txtHazirlamaSuresi.Name = "txtHazirlamaSuresi";
            this.txtHazirlamaSuresi.Size = new System.Drawing.Size(200, 20);
            this.txtHazirlamaSuresi.TabIndex = 2;

            // 
            // txtTalimatlar
            // 
            this.txtTalimatlar.Location = new System.Drawing.Point(150, 220);
            this.txtTalimatlar.Multiline = true;
            this.txtTalimatlar.Name = "txtTalimatlar";
            this.txtTalimatlar.Size = new System.Drawing.Size(200, 100);
            this.txtTalimatlar.TabIndex = 3;

            // 
            // clbMalzemeler
            // 
            this.clbMalzemeler.FormattingEnabled = true;
            this.clbMalzemeler.Location = new System.Drawing.Point(150, 350);
            this.clbMalzemeler.Name = "clbMalzemeler";
            this.clbMalzemeler.Size = new System.Drawing.Size(200, 100);
            this.clbMalzemeler.TabIndex = 4;

            // 
            // btnGuncelle
            // 
            this.btnGuncelle.Location = new System.Drawing.Point(150, 490);
            this.btnGuncelle.Name = "btnGuncelle";
            this.btnGuncelle.Size = new System.Drawing.Size(100, 30);
            this.btnGuncelle.TabIndex = 5;
            this.btnGuncelle.Text = "Güncelle";
            this.btnGuncelle.UseVisualStyleBackColor = true;
            this.btnGuncelle.Click += new System.EventHandler(this.btnGuncelle_Click);

            // 
            // lblTarifAdi
            // 
            this.lblTarifAdi.AutoSize = true;
            this.lblTarifAdi.Location = new System.Drawing.Point(50, 100);
            this.lblTarifAdi.Name = "lblTarifAdi";
            this.lblTarifAdi.Size = new System.Drawing.Size(53, 13);
            this.lblTarifAdi.TabIndex = 6;
            this.lblTarifAdi.Text = "Tarif Adı:";

            // 
            // lblKategori
            // 
            this.lblKategori.AutoSize = true;
            this.lblKategori.Location = new System.Drawing.Point(50, 140);
            this.lblKategori.Name = "lblKategori";
            this.lblKategori.Size = new System.Drawing.Size(49, 13);
            this.lblKategori.TabIndex = 7;
            this.lblKategori.Text = "Kategori:";

            // 
            // lblHazirlamaSuresi
            // 
            this.lblHazirlamaSuresi.AutoSize = true;
            this.lblHazirlamaSuresi.Location = new System.Drawing.Point(50, 180);
            this.lblHazirlamaSuresi.Name = "lblHazirlamaSuresi";
            this.lblHazirlamaSuresi.Size = new System.Drawing.Size(86, 13);
            this.lblHazirlamaSuresi.TabIndex = 8;
            this.lblHazirlamaSuresi.Text = "Hazırlama Süresi:";

            // 
            // lblTalimatlar
            // 
            this.lblTalimatlar.AutoSize = true;
            this.lblTalimatlar.Location = new System.Drawing.Point(50, 220);
            this.lblTalimatlar.Name = "lblTalimatlar";
            this.lblTalimatlar.Size = new System.Drawing.Size(55, 13);
            this.lblTalimatlar.TabIndex = 9;
            this.lblTalimatlar.Text = "Talimatlar:";

            // 
            // lblMalzemeler
            // 
            this.lblMalzemeler.AutoSize = true;
            this.lblMalzemeler.Location = new System.Drawing.Point(50, 350);
            this.lblMalzemeler.Name = "lblMalzemeler";
            this.lblMalzemeler.Size = new System.Drawing.Size(55, 13);
            this.lblMalzemeler.TabIndex = 9;
            this.lblMalzemeler.Text = "Malzemeler:";

            // 
            // TarifGuncellemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 500);

            this.Controls.Add(this.lblTarifAdi);
            this.Controls.Add(this.lblKategori);
            this.Controls.Add(this.lblHazirlamaSuresi);
            this.Controls.Add(this.lblTalimatlar);
            this.Controls.Add(this.txtTarifAdi);
            this.Controls.Add(this.txtKategori);
            this.Controls.Add(this.txtHazirlamaSuresi);
            this.Controls.Add(this.txtTalimatlar);
            this.Controls.Add(this.clbMalzemeler);
            this.Controls.Add(this.lblMalzemeler);
            
            this.Controls.Add(this.btnGuncelle);
            this.Name = "TarifGuncellemeForm";
            this.Text = "Tarif Güncelle";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}