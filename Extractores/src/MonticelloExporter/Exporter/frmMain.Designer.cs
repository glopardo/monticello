namespace Exporter
{
    partial class frmMain
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
            this.btnventasGenerales = new System.Windows.Forms.Button();
            this.btnVentasDetalle = new System.Windows.Forms.Button();
            this.gbFechas = new System.Windows.Forms.GroupBox();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbRango = new System.Windows.Forms.CheckBox();
            this.pbProcesando = new System.Windows.Forms.ProgressBar();
            this.cbPuntoDeVenta = new System.Windows.Forms.ComboBox();
            this.lvlPuntoDeVenta = new System.Windows.Forms.Label();
            this.gbFechas.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnventasGenerales
            // 
            this.btnventasGenerales.Location = new System.Drawing.Point(36, 252);
            this.btnventasGenerales.Name = "btnventasGenerales";
            this.btnventasGenerales.Size = new System.Drawing.Size(116, 50);
            this.btnventasGenerales.TabIndex = 5;
            this.btnventasGenerales.Text = "Ventas generales agrupadas";
            this.btnventasGenerales.UseVisualStyleBackColor = true;
            this.btnventasGenerales.Click += new System.EventHandler(this.btnventasGenerales_Click);
            // 
            // btnVentasDetalle
            // 
            this.btnVentasDetalle.Location = new System.Drawing.Point(179, 252);
            this.btnVentasDetalle.Name = "btnVentasDetalle";
            this.btnVentasDetalle.Size = new System.Drawing.Size(116, 50);
            this.btnVentasDetalle.TabIndex = 6;
            this.btnVentasDetalle.Text = "Ventas detalle agrupadas";
            this.btnVentasDetalle.UseVisualStyleBackColor = true;
            this.btnVentasDetalle.Click += new System.EventHandler(this.btnVentasDetalle_Click);
            // 
            // gbFechas
            // 
            this.gbFechas.Controls.Add(this.lblFechaHasta);
            this.gbFechas.Controls.Add(this.dtpFechaHasta);
            this.gbFechas.Controls.Add(this.lblFechaDesde);
            this.gbFechas.Controls.Add(this.dtpFechaDesde);
            this.gbFechas.Location = new System.Drawing.Point(12, 62);
            this.gbFechas.Name = "gbFechas";
            this.gbFechas.Size = new System.Drawing.Size(314, 97);
            this.gbFechas.TabIndex = 2;
            this.gbFechas.TabStop = false;
            this.gbFechas.Text = "Fechas";
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(53, 63);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(66, 13);
            this.lblFechaHasta.TabIndex = 3;
            this.lblFechaHasta.Text = "Fecha hasta";
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.CustomFormat = "dd/MM/yy";
            this.dtpFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaHasta.Location = new System.Drawing.Point(151, 57);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(92, 20);
            this.dtpFechaHasta.TabIndex = 3;
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Location = new System.Drawing.Point(53, 26);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(69, 13);
            this.lblFechaDesde.TabIndex = 1;
            this.lblFechaDesde.Text = "Fecha desde";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.CustomFormat = "dd/MM/yy";
            this.dtpFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaDesde.Location = new System.Drawing.Point(151, 20);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(92, 20);
            this.dtpFechaDesde.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(15, 224);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(52, 17);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "label1";
            // 
            // cbRango
            // 
            this.cbRango.AutoSize = true;
            this.cbRango.Location = new System.Drawing.Point(12, 165);
            this.cbRango.Name = "cbRango";
            this.cbRango.Size = new System.Drawing.Size(212, 17);
            this.cbRango.TabIndex = 4;
            this.cbRango.Text = "Habilitar búsqueda por rango de fechas";
            this.cbRango.UseVisualStyleBackColor = true;
            this.cbRango.CheckedChanged += new System.EventHandler(this.cbRango_CheckedChanged);
            // 
            // pbProcesando
            // 
            this.pbProcesando.Location = new System.Drawing.Point(12, 193);
            this.pbProcesando.Name = "pbProcesando";
            this.pbProcesando.Size = new System.Drawing.Size(314, 23);
            this.pbProcesando.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProcesando.TabIndex = 5;
            // 
            // cbPuntoDeVenta
            // 
            this.cbPuntoDeVenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPuntoDeVenta.FormattingEnabled = true;
            this.cbPuntoDeVenta.Items.AddRange(new object[] {
            "Bingo",
            "Pulse",
            "Capataz",
            "Suka club",
            "Johnny Rockets",
            "Main Game",
            "Carpentier",
            "Prive",
            "Banquetes",
            "Res Angostura",
            "Bar Movil",
            "Restaurant",
            "Bar Lounge",
            "Bar Piscina",
            "Room Service",
            "MiniBar",
            "Spa",
            "Arena Monticello",
            "Co&CO",
            "El Pescador",
            "Lucky 7",
            "Fuente Angostura"});
            this.cbPuntoDeVenta.Location = new System.Drawing.Point(152, 24);
            this.cbPuntoDeVenta.Name = "cbPuntoDeVenta";
            this.cbPuntoDeVenta.Size = new System.Drawing.Size(121, 21);
            this.cbPuntoDeVenta.TabIndex = 1;
            this.cbPuntoDeVenta.SelectedIndexChanged += new System.EventHandler(this.cbPuntoDeVenta_SelectedIndexChanged);
            // 
            // lvlPuntoDeVenta
            // 
            this.lvlPuntoDeVenta.AutoSize = true;
            this.lvlPuntoDeVenta.Location = new System.Drawing.Point(36, 31);
            this.lvlPuntoDeVenta.Name = "lvlPuntoDeVenta";
            this.lvlPuntoDeVenta.Size = new System.Drawing.Size(80, 13);
            this.lvlPuntoDeVenta.TabIndex = 7;
            this.lvlPuntoDeVenta.Text = "Punto de venta";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 328);
            this.Controls.Add(this.lvlPuntoDeVenta);
            this.Controls.Add(this.cbPuntoDeVenta);
            this.Controls.Add(this.pbProcesando);
            this.Controls.Add(this.cbRango);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.gbFechas);
            this.Controls.Add(this.btnVentasDetalle);
            this.Controls.Add(this.btnventasGenerales);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monticello Sales Exporter - v1.4";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbFechas.ResumeLayout(false);
            this.gbFechas.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnventasGenerales;
        private System.Windows.Forms.Button btnVentasDetalle;
        private System.Windows.Forms.GroupBox gbFechas;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.CheckBox cbRango;
        private System.Windows.Forms.ProgressBar pbProcesando;
        private System.Windows.Forms.ComboBox cbPuntoDeVenta;
        private System.Windows.Forms.Label lvlPuntoDeVenta;
    }
}

