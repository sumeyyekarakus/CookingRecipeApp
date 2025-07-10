using System;
using System.Drawing;
using System.Windows.Forms;

namespace YemekTarifiAPP
{
    partial class TarifSilmeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblTarifSil;
        private TextBox txtTarifAdi;
        private Button btnTarifSil;
        private Control panelMenu;

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
            this.Text = "TarifSilmeForm";

           
            
            // Malzeme Sil Alanı
            lblTarifSil = new Label() { Text = "Tarif Sil ", Location = new Point(150, 100), AutoSize = true };
            txtTarifAdi = new TextBox() { Location = new Point(250, 100), Width = 120 };

            btnTarifSil = new Button() { Text = "SİL", Location = new Point(230, 200), Width = 120, BackColor = Color.IndianRed };
            btnTarifSil.Click += new EventHandler(this.btnTarifSil_Click);

            this.Controls.Add(lblTarifSil);
            this.Controls.Add(txtTarifAdi);
            this.Controls.Add(btnTarifSil);
        }

        #endregion
    }
}