using System.Drawing;
using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class TarifDetayForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTalimatlar;
        private ListBox listMalzemeler;  // Malzemeler için ListBox
        private Label lblMalzemeler;
        private TextBox txtTalimatlar;  // Talimatları göstermek için TextBox
        private Button btnKapat;

        /// <summary>
        /// Formun gerekli tasarımcı değişkenlerini temizlemek için kullanılır.
        /// </summary>
        /// <param name="disposing">Yönetilen kaynaklar mı yoksa yönetilmeyen kaynaklar mı serbest bırakılacak?</param>
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
        /// Tasarımcı desteği için gerekli metot - kod editörü ile bu metodun
        /// içeriğini değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTalimatlar = new System.Windows.Forms.Label();
            this.lblMalzemeler = new System.Windows.Forms.Label();
            this.listMalzemeler = new System.Windows.Forms.ListBox();
            this.txtTalimatlar = new System.Windows.Forms.TextBox();
            this.btnKapat = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // lblMalzemeler
            // 
            this.lblMalzemeler.AutoSize = true;
            this.lblMalzemeler.Location = new System.Drawing.Point(50, 20); // Başlangıç noktası
            this.lblMalzemeler.Name = "lblMalzemeler";
            this.lblMalzemeler.Size = new System.Drawing.Size(85, 20);
            this.lblMalzemeler.TabIndex = 0;
            this.lblMalzemeler.Text = "Malzemeler:";
            this.lblMalzemeler.Font = new Font("Arial", 12, FontStyle.Bold);
            this.lblMalzemeler.ForeColor = Color.DarkBlue;

            // 
            // listMalzemeler
            // 
            this.listMalzemeler.Location = new System.Drawing.Point(50, 50); // Malzemeleri gösterecek yer
            this.listMalzemeler.Name = "listMalzemeler";
            this.listMalzemeler.Size = new System.Drawing.Size(700, 100); // Genişlik ayarladık
            this.listMalzemeler.TabIndex = 1;
            this.listMalzemeler.Font = new Font("Arial", 10);
            this.listMalzemeler.ForeColor = Color.Black;
            this.listMalzemeler.BackColor = Color.LightGray; // Arka plan rengi gri

            // 
            // lblTalimatlar
            // 
            this.lblTalimatlar.AutoSize = true;
            this.lblTalimatlar.Location = new System.Drawing.Point(50, 170); // Malzemelerin altına yerleştirildi
            this.lblTalimatlar.Name = "lblTalimatlar";
            this.lblTalimatlar.Size = new System.Drawing.Size(85, 20);
            this.lblTalimatlar.TabIndex = 2;
            this.lblTalimatlar.Text = "Talimatlar:";
            this.lblTalimatlar.Font = new Font("Arial", 12, FontStyle.Bold);
            this.lblTalimatlar.ForeColor = Color.DarkBlue;

            // 
            // txtTalimatlar
            // 
            this.txtTalimatlar.Location = new System.Drawing.Point(50, 200); // Talimatların altına
            this.txtTalimatlar.Multiline = true;
            this.txtTalimatlar.Name = "txtTalimatlar";
            this.txtTalimatlar.Size = new System.Drawing.Size(700, 200); // Büyük bir alan verdik
            this.txtTalimatlar.ReadOnly = true;
            this.txtTalimatlar.ScrollBars = ScrollBars.Vertical; // Uzun talimatlar için kaydırma ekledik
            this.txtTalimatlar.Font = new Font("Arial", 10);
            this.txtTalimatlar.BackColor = Color.LightGray; // Arka plan rengi gri
            // 
            // btnKapat
            // 
            this.btnKapat.Text = "Kapat";
            this.btnKapat.Location = new System.Drawing.Point(630, 420); // Sağ alt köşeye yerleştirildi
            this.btnKapat.Size = new System.Drawing.Size(120, 40);
            this.btnKapat.BackColor = Color.LightCoral;
            this.btnKapat.Font = new Font("Arial", 10, FontStyle.Bold);
            this.btnKapat.ForeColor = Color.White;
            this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);

            // 
            // TarifDetayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.listMalzemeler);
            this.Controls.Add(this.lblMalzemeler);
            this.Controls.Add(this.lblTalimatlar);
            this.Controls.Add(this.txtTalimatlar);
            this.Controls.Add(this.btnKapat);
            this.Name = "TarifDetayForm";
            this.Text = "Tarif Detayları";
            this.BackColor = Color.FromArgb(255, 245, 245);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}