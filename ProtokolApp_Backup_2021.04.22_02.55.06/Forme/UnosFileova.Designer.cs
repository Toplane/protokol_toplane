using Telerik.WinControls.UI;
namespace ProtokolApp
{
    partial class UnosFileova
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnosFileova));
            this.office2010SilverTheme1 = new Telerik.WinControls.Themes.Office2010SilverTheme();
            this.radOpenFilePredmet = new Telerik.WinControls.UI.RadOpenFileDialog();
            this.radOpenFileVeza = new Telerik.WinControls.UI.RadOpenFileDialog();
            this.radTextBoxPredmet = new Telerik.WinControls.UI.RadTextBox();
            this.radTextBoxVeza = new Telerik.WinControls.UI.RadTextBox();
            this.radLabelPredmet = new Telerik.WinControls.UI.RadLabel();
            this.radLabelVeza = new Telerik.WinControls.UI.RadLabel();
            this.radButtonFilePredmeta = new Telerik.WinControls.UI.RadButton();
            this.radButtonFileVeze = new Telerik.WinControls.UI.RadButton();
            this.radButtonUkloniPredmet = new Telerik.WinControls.UI.RadButton();
            this.radButtonUkloniFileVeze = new Telerik.WinControls.UI.RadButton();
            this.radButtonUnos = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radTextBoxPredmet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTextBoxVeza)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabelPredmet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabelVeza)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFilePredmeta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFileVeze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUkloniPredmet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUkloniFileVeze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUnos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radOpenFilePredmet
            // 
            this.radOpenFilePredmet.Filter = " PDF file | *.pdf;";
            // 
            // radOpenFileVeza
            // 
            this.radOpenFileVeza.Filter = " PDF file | *.pdf;";
            // 
            // radTextBoxPredmet
            // 
            this.radTextBoxPredmet.Location = new System.Drawing.Point(12, 27);
            this.radTextBoxPredmet.Name = "radTextBoxPredmet";
            this.radTextBoxPredmet.NullText = "Unesite opis predmeta...";
            this.radTextBoxPredmet.Size = new System.Drawing.Size(253, 20);
            this.radTextBoxPredmet.TabIndex = 0;
            this.radTextBoxPredmet.ThemeName = "Office2010Silver";
            // 
            // radTextBoxVeza
            // 
            this.radTextBoxVeza.Location = new System.Drawing.Point(12, 73);
            this.radTextBoxVeza.Name = "radTextBoxVeza";
            this.radTextBoxVeza.NullText = "Unesite opis veze...";
            this.radTextBoxVeza.Size = new System.Drawing.Size(253, 20);
            this.radTextBoxVeza.TabIndex = 1;
            this.radTextBoxVeza.ThemeName = "Office2010Silver";
            // 
            // radLabelPredmet
            // 
            this.radLabelPredmet.Location = new System.Drawing.Point(12, 49);
            this.radLabelPredmet.Name = "radLabelPredmet";
            this.radLabelPredmet.Size = new System.Drawing.Size(2, 2);
            this.radLabelPredmet.TabIndex = 2;
            // 
            // radLabelVeza
            // 
            this.radLabelVeza.Location = new System.Drawing.Point(12, 100);
            this.radLabelVeza.Name = "radLabelVeza";
            this.radLabelVeza.Size = new System.Drawing.Size(2, 2);
            this.radLabelVeza.TabIndex = 3;
            // 
            // radButtonFilePredmeta
            // 
            this.radButtonFilePredmeta.Location = new System.Drawing.Point(280, 27);
            this.radButtonFilePredmeta.Name = "radButtonFilePredmeta";
            this.radButtonFilePredmeta.Size = new System.Drawing.Size(139, 20);
            this.radButtonFilePredmeta.TabIndex = 4;
            this.radButtonFilePredmeta.Text = "Odaberite file predmeta";
            this.radButtonFilePredmeta.ThemeName = "Office2010Silver";
            this.radButtonFilePredmeta.Click += new System.EventHandler(this.radButtonFilePredmeta_Click);
            // 
            // radButtonFileVeze
            // 
            this.radButtonFileVeze.Location = new System.Drawing.Point(280, 73);
            this.radButtonFileVeze.Name = "radButtonFileVeze";
            this.radButtonFileVeze.Size = new System.Drawing.Size(139, 20);
            this.radButtonFileVeze.TabIndex = 5;
            this.radButtonFileVeze.Text = "Odaberite file veze";
            this.radButtonFileVeze.ThemeName = "Office2010Silver";
            this.radButtonFileVeze.Click += new System.EventHandler(this.radButtonFileVeze_Click);
            // 
            // radButtonUkloniPredmet
            // 
            this.radButtonUkloniPredmet.Location = new System.Drawing.Point(425, 27);
            this.radButtonUkloniPredmet.Name = "radButtonUkloniPredmet";
            this.radButtonUkloniPredmet.Size = new System.Drawing.Size(139, 20);
            this.radButtonUkloniPredmet.TabIndex = 5;
            this.radButtonUkloniPredmet.Text = "Ukloni file predmeta";
            this.radButtonUkloniPredmet.ThemeName = "Office2010Silver";
            this.radButtonUkloniPredmet.Visible = false;
            this.radButtonUkloniPredmet.Click += new System.EventHandler(this.radButtonUkloniPredmet_Click);
            // 
            // radButtonUkloniFileVeze
            // 
            this.radButtonUkloniFileVeze.Location = new System.Drawing.Point(425, 73);
            this.radButtonUkloniFileVeze.Name = "radButtonUkloniFileVeze";
            this.radButtonUkloniFileVeze.Size = new System.Drawing.Size(139, 20);
            this.radButtonUkloniFileVeze.TabIndex = 6;
            this.radButtonUkloniFileVeze.Text = "Ukloni file veze";
            this.radButtonUkloniFileVeze.ThemeName = "Office2010Silver";
            this.radButtonUkloniFileVeze.Visible = false;
            this.radButtonUkloniFileVeze.Click += new System.EventHandler(this.radButtonUkloniFileVeze_Click);
            // 
            // radButtonUnos
            // 
            this.radButtonUnos.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radButtonUnos.Location = new System.Drawing.Point(12, 124);
            this.radButtonUnos.Name = "radButtonUnos";
            this.radButtonUnos.Size = new System.Drawing.Size(253, 78);
            this.radButtonUnos.TabIndex = 7;
            this.radButtonUnos.Text = "Unesi";
            this.radButtonUnos.ThemeName = "Office2010Silver";
            this.radButtonUnos.Click += new System.EventHandler(this.radButtonUnos_Click);
            // 
            // UnosFileova
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 227);
            this.Controls.Add(this.radButtonUnos);
            this.Controls.Add(this.radButtonUkloniFileVeze);
            this.Controls.Add(this.radButtonUkloniPredmet);
            this.Controls.Add(this.radButtonFileVeze);
            this.Controls.Add(this.radButtonFilePredmeta);
            this.Controls.Add(this.radLabelVeza);
            this.Controls.Add(this.radLabelPredmet);
            this.Controls.Add(this.radTextBoxVeza);
            this.Controls.Add(this.radTextBoxPredmet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UnosFileova";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Unos fileova";
            this.ThemeName = "Office2010Silver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UnosFileova_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.radTextBoxPredmet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTextBoxVeza)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabelPredmet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabelVeza)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFilePredmeta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFileVeze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUkloniPredmet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUkloniFileVeze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonUnos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private Telerik.WinControls.Themes.Office2010SilverTheme office2010SilverTheme1;
        private Telerik.WinControls.UI.RadOpenFileDialog radOpenFilePredmet;
        private Telerik.WinControls.UI.RadOpenFileDialog radOpenFileVeza;
        private Telerik.WinControls.UI.RadTextBox radTextBoxPredmet;
        private Telerik.WinControls.UI.RadTextBox radTextBoxVeza;
        private Telerik.WinControls.UI.RadLabel radLabelPredmet;
        private Telerik.WinControls.UI.RadLabel radLabelVeza;
        private Telerik.WinControls.UI.RadButton radButtonFilePredmeta;
        private Telerik.WinControls.UI.RadButton radButtonFileVeze;
        private Telerik.WinControls.UI.RadButton radButtonUkloniPredmet;
        private Telerik.WinControls.UI.RadButton radButtonUkloniFileVeze;
        private Telerik.WinControls.UI.RadButton radButtonUnos;
    }
}
