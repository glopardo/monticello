using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Exporter
{
    public partial class frmMain : Form
    {
        private int _puntoDeVenta;
        private Configuration _configuration;
        public frmMain()
        {
            InitializeComponent();
        }

        private SqlConnection GetDbConnection(Configuration conf)
        {
            if (conf != null)
            {
                var connString = string.Format("Data Source={0};Initial Catalog=MCRSPOS;MultipleActiveResultSets=true;User ID={1};Password={2}", _configuration.DatabaseName, _configuration.User, _configuration.Password);
                //DEBUG
                //Logger.Write(connString);

                var conn = new SqlConnection(connString);
                try
                {
                    conn.Open();
                    Logger.Write("Conexión a base de datos establecida");
                    return conn;
                }
                catch (Exception ex)
                {
                    Logger.Write("No se puede conectar a la base de datos - Error: " + ex.Message);
                    throw;
                }
            }
            Logger.Write("No se encontró archivo de configuración.");
            return null;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cbPuntoDeVenta.SelectedIndex = 0;
            dtpFechaDesde.Value = DateTime.Today.AddDays(-1);
            lblStatus.Visible = pbProcesando.Visible = false;
            dtpFechaHasta.Enabled = cbRango.Checked = true;

            try
            {
                _configuration = ConfigurationReader.Read("Config.ini");
                Logger.Write("Se inicia exportador de boletas de Monticello");
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void btnventasGenerales_Click(object sender, EventArgs e)
        {
            SetearPuntoDeVenta(cbPuntoDeVenta);
            pbProcesando.Visible = true;
            
            var bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            
            bw.RunWorkerAsync();
        }

        private void SetearPuntoDeVenta(ComboBox comboBox)
        {
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    _puntoDeVenta = 1;
                    break;
                case 1:
                    _puntoDeVenta = 2;
                    break;
                case 2:
                    _puntoDeVenta = 3;
                    break;
                case 3:
                    _puntoDeVenta = 4;
                    break;
                case 4:
                    _puntoDeVenta = 5;
                    break;
                case 5:
                    _puntoDeVenta = 6;
                    break;
                case 6:
                    _puntoDeVenta = 7;
                    break;
                case 7:
                    _puntoDeVenta = 8;
                    break;
                case 8:
                    _puntoDeVenta = 9;
                    break;
                case 9:
                    _puntoDeVenta = 10;
                    break;
                case 10:
                    _puntoDeVenta = 11;
                    break;
                case 11:
                    _puntoDeVenta = 13;
                    break;
                case 12:
                    _puntoDeVenta = 14;
                    break;
                case 13:
                    _puntoDeVenta = 15;
                    break;
                case 14:
                    _puntoDeVenta = 16;
                    break;
                case 15:
                    _puntoDeVenta = 17;
                    break;
                case 16:
                    _puntoDeVenta = 18;
                    break;
                case 17:
                    _puntoDeVenta = 19;
                    break;
                case 18:
                    _puntoDeVenta = 20;
                    break;
                case 19:
                    _puntoDeVenta = 21;
                    break;
                case 20:
                    _puntoDeVenta = 22;
                    break;
                case 21:
                    _puntoDeVenta = 24;
                    break;
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var connection = GetDbConnection(_configuration);
            GenerarArchivo(connection);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            dtpFechaDesde.Enabled = dtpFechaHasta.Enabled = btnVentasDetalle.Enabled = 
                btnventasGenerales.Enabled = cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
            pbProcesando.Visible = false;
        }

        private void GenerarArchivo(SqlConnection connection)
        {
            //EscribirTitulosColumnas("BoletasVentasMonticello.txt");

            lblStatus.Text = string.Empty;
            dtpFechaDesde.Enabled = dtpFechaHasta.Enabled = btnVentasDetalle.Enabled = btnventasGenerales.Enabled = cbRango.Enabled = cbPuntoDeVenta.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            var pathCarpetaArchivosExportados = ".\\Exportados\\";
            CrearDirectorioParaArchivosExportados(pathCarpetaArchivosExportados);
            var path = cbRango.Checked ? string.Format("{0}{1}-{2}_{3}_Monticello_BoletasDeVenta.csv",
                                                        pathCarpetaArchivosExportados,
                                                        dtpFechaDesde.Value.ToString("yyyyMMdd"), 
                                                        dtpFechaHasta.Value.ToString("yyyyMMdd"),
                                                        cbPuntoDeVenta.Text) : string.Format("{0}_Monticello_BoletasDeVenta.csv", dtpFechaDesde.Value.ToString("yyyyMMdd"));

            EscribirTitulosColumnas(path);

            //Calculo la cantidad de días a exportar
            var cantDias = cbRango.Checked ? (dtpFechaHasta.Value - dtpFechaDesde.Value).TotalDays : 1;
            var listaChecks = new List<MicrosCheck>();

            for (var i = 0; i < cantDias; i++)
            {
                //Por cada día itero y extraigo la información sumarizada
                try
                {
                    var command = connection.CreateCommand();
                    var fecha = dtpFechaDesde.Value.AddDays(i).ToString("yyyyMMdd");

                    Logger.Write(fecha);

                    var query = "SELECT CONVERT(VARCHAR(10), FCRBSNZDATE, 103) FCRBSNZDATEDDMMYYYY, CONVERT(VARCHAR(10), FCRBSNZDATE, 112) FCRBSNZDATEYYYYMMDD, CONVERT(VARCHAR(5), " +
                                "FCRBSNZDATE, 108) HORAMIN, FCRINVNUMBER, MICROSCHKNUM, SUBTOTAL1 SUBTOTALCONIVA, ROUND(SUBTOTAL1 / 1.19, 0) SUBTOTALSINIVA FROM MICROSDB.FCR_INVOICE_DATA " +
                                "WHERE CONVERT(VARCHAR(10), FCRBSNZDATE, 112) = '" + fecha + "' AND EXTRAFIELD5 = " + _puntoDeVenta + " ORDER BY FCRBSNZDATE DESC";

                    //DEBUG
                    //Logger.Write(query);                    //Logger.Write(query);

                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        //Itero por CABECERA CHECK uno por uno
                        var totalAlimentosPorCheck = 0;
                        var totalBebidasPorCheck = 0;
                        var totalTabacosPorCheck = 0;
                        var totalOtrosPorCheck = 0;

                        var microsCheck = new MicrosCheck()
                        {
                            CheckNumber = reader["MICROSCHKNUM"].ToString(),
                            Fecha = reader["FCRBSNZDATEDDMMYYYY"].ToString(),
                            TotalNeto = Convert.ToInt32(reader["SUBTOTALSINIVA"]),
                            VentasTotales = Convert.ToInt32(reader["SUBTOTALCONIVA"]),
                            Iva = Convert.ToInt32(reader["SUBTOTALCONIVA"]) - Convert.ToInt32(reader["SUBTOTALSINIVA"])
                        };

                        var totalIvaPorCheck = 0.0;

                        var chkNum = reader["MICROSCHKNUM"];
                        var queryChecks = "SELECT CHECKID, CHECKNUMBER, WORKSTATIONID, CHECKOPEN, TABLEOPEN, CHECKCLOSE, SPLITFROMCHECKNUM, SUBTOTAL, TAX, OTHER, PAYMENT, DUE " +
                                          "FROM MICROSDB.CHECKS WHERE CONVERT(VARCHAR(10), CHECKPOSTINGTIME, 112) = '" + reader["FCRBSNZDATEYYYYMMDD"] + 
                                          "' AND CONVERT(VARCHAR(5), CHECKPOSTINGTIME, 108) = '" + reader["HORAMIN"] + "' and CHECKNUMBER = " + chkNum; //Query para checks

                        //DEBUG
                        //Logger.Write(queryChecks);

                        command = connection.CreateCommand();
                        command.CommandText = queryChecks;
                        var subreaderChecks = command.ExecuteReader();

                        while (subreaderChecks.Read())
                        {
                            //Itero los detalles del check para acumular totales segregados
                            var checkId = subreaderChecks["CHECKID"];
                            var queryCheckDetail = "SELECT CHECK_DETAIL.CHECKDETAILID, FAMILY_GROUP.OBJECTNUMBER FAMGRP,CHECK_DETAIL.CHECKID, CHECK_DETAIL.DETAILINDEX, " +
                                                   "CHECK_DETAIL.DETAILTYPE, ROUND(CHECK_DETAIL.TOTAL / 1.19, 0) TOTALSINIVA, CHECK_DETAIL.TOTAL TOTAL, CHECK_DETAIL.SALESCOUNT, " +
                                                   "STRING_TABLE.STRINGTEXT, MENU_ITEM_DEFINITION.MENUITEMDEFID MENUITEMDEFID, MENU_ITEM_MASTER.OBJECTNUMBER OBJECTNUMBER " +
                                                   "FROM MICROSDB.CHECK_DETAIL CHECK_DETAIL INNER JOIN MICROSDB.MENU_ITEM_DETAIL MENU_ITEM_DETAIL ON CHECK_DETAIL.CHECKDETAILID = " +
                                                   "MENU_ITEM_DETAIL.CHECKDETAILID INNER JOIN MICROSDB.MENU_ITEM_DEFINITION MENU_ITEM_DEFINITION ON MENU_ITEM_DEFINITION.MENUITEMDEFID = " +
                                                   "MENU_ITEM_DETAIL.MENUITEMDEFID INNER JOIN MICROSDB.STRING_TABLE STRING_TABLE ON STRING_TABLE.STRINGNUMBERID = MENU_ITEM_DEFINITION.NAME1ID INNER JOIN " +
                                                   "MICROSDB.MENU_ITEM_MASTER MENU_ITEM_MASTER ON MENU_ITEM_MASTER.MENUITEMMASTERID = MENU_ITEM_DEFINITION.MENUITEMMASTERID " +
                                                   "INNER JOIN MICROSDB.FAMILY_GROUP FAMILY_GROUP ON FAMILY_GROUP.FAMGRPID = MENU_ITEM_MASTER.FAMGRPID " + 
                                                   "WHERE CHECKID = " + checkId + " AND TOTAL <> 0 AND MENU_ITEM_DEFINITION.MENUITEMCLASSID <> 1341 " +
                                                   "ORDER BY CHECK_DETAIL.DETAILINDEX";

                            //DEBUG
                            //Logger.Write(queryCheckDetail);

                            command = connection.CreateCommand();
                            command.CommandText = queryCheckDetail;

                            var subreaderDetail = command.ExecuteReader();

                            //CHECK DETAILS
                            while (subreaderDetail.Read())
                            {
                                var encontrado = false;
                                //PROCESO DETALLE POR CADA CHECK
                                var lFGAlimentos = new List<int>
                                {
                                    3, 60, 62, 64, 66, 101, 102, 103, 104, 105, 107, 108, 109, 110,
                                    111, 112, 114, 116, 117, 122, 123, 124, 125, 126, 132, 133, 160,
                                    161, 162, 163, 164, 167, 179, 171, 224, 250, 254, 262, 263, 301
                                };

                                var lFGBebidas = new List<int>
                                {
                                    1, 61, 120, 123, 125, 126, 128, 129, 165, 166, 168, 170, 200,
                                    201, 203, 204, 205, 206, 207, 209, 210, 211, 212, 213, 214, 215,
                                    216, 223, 225, 228, 230, 230, 256, 257, 258, 259, 260, 261, 263
                                };

                                var lFGTabacos = new List<int>
                                {
                                    121
                                };

                                foreach (var fgAlimento in lFGAlimentos)
                                {
                                    if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgAlimento)
                                    {
                                        totalAlimentosPorCheck += Convert.ToInt32(subreaderDetail["TOTAL"]);
                                        encontrado = true;
                                    }
                                }

                                if (!encontrado)
                                {
                                    foreach (var fgBebida in lFGBebidas)
                                    {
                                        if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgBebida)
                                        {
                                            totalBebidasPorCheck += Convert.ToInt32(subreaderDetail["TOTAL"]);
                                            encontrado = true;
                                        }
                                    }
                                }

                                if (!encontrado)
                                {
                                    foreach (var fgTabaco in lFGTabacos)
                                    {
                                        if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgTabaco)
                                        {
                                            totalTabacosPorCheck += Convert.ToInt32(subreaderDetail["TOTAL"]);
                                            encontrado = true;
                                        }
                                    }
                                }
                                
                                if (!encontrado)
                                {
                                    totalOtrosPorCheck += Convert.ToInt32(subreaderDetail["TOTAL"]);
                                }

                                totalIvaPorCheck += (Convert.ToInt32(subreaderDetail["TOTAL"]) / 1.19) * 0.19;
                            }

                            //PAGOS
                            var queryPayments = "SELECT CHECK_DETAIL.CHECKDETAILID, CHECK_DETAIL.CHECKID, CHECK_DETAIL.DETAILINDEX, " +
                                                "CHECK_DETAIL.DETAILTYPE, CHECK_DETAIL.TOTAL, CHECK_DETAIL.SALESCOUNT, TENDER_MEDIA_DETAIL.TENDMEDID " +
                                                "FROM CHECK_DETAIL INNER JOIN TENDER_MEDIA_DETAIL ON CHECK_DETAIL.CHECKDETAILID = TENDER_MEDIA_DETAIL.CHECKDETAILID " +
                                                "WHERE CHECKID = " + checkId + " AND TOTAL > 0 ORDER BY CHECK_DETAIL.DETAILINDEX";
                        }

                        microsCheck.Alimentos = Convert.ToInt32(totalAlimentosPorCheck / 1.19);
                        microsCheck.BebidasSAlcohol = Convert.ToInt32(totalBebidasPorCheck / 1.19);
                        microsCheck.Tabacos = Convert.ToInt32(totalTabacosPorCheck / 1.19);
                        microsCheck.Otros = Convert.ToInt32(totalOtrosPorCheck / 1.19);

                        //TOTALES
                        var totalNeto = Convert.ToInt32(totalAlimentosPorCheck / 1.19) + Convert.ToInt32(totalBebidasPorCheck / 1.19) + Convert.ToInt32(totalTabacosPorCheck / 1.19) + Convert.ToInt32(totalOtrosPorCheck / 1.19);
                        microsCheck.TotalNeto = totalNeto;
                        microsCheck.Iva = Convert.ToInt32(totalIvaPorCheck);
                        microsCheck.VentasTotales = totalNeto + Convert.ToInt32(totalIvaPorCheck);

                        Logger.Write(microsCheck.Fecha + " - " + Convert.ToDateTime(microsCheck.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") + " - " + microsCheck.CheckNumber);
                        listaChecks.Add(microsCheck);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = "Error al generar archivos";
                    Cursor.Current = Cursors.Default;
                    lblStatus.Visible = dtpFechaDesde.Enabled = dtpFechaHasta.Enabled = 
                        btnVentasDetalle.Enabled = btnventasGenerales.Enabled = 
                        cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
                    pbProcesando.Visible = false;
                    Logger.Write(ex.Message);
                }
            }

            for (var i = 0; i < cantDias; i++)
            {
                //Por cada día itero y extraigo la información sumarizada
                var fecha = dtpFechaDesde.Value.AddDays(i).ToString("yyyyMMdd");

                EscribirArchivoSumarizadoPorFecha(path, listaChecks, fecha);

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Archivos generados exitosamente";
                Logger.Write("Archivos generados exitosamente");
            }

            dtpFechaDesde.Enabled = dtpFechaHasta.Enabled = btnVentasDetalle.Enabled = 
                btnventasGenerales.Enabled = cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
            Cursor.Current = Cursors.Default;
            pbProcesando.Visible = false;
        }

        private void EscribirArchivoSumarizadoPorFecha(string path, List<MicrosCheck> listaChecks, string fecha)
        {
            //path = "BoletasVentasMonticello.txt";

            var desde = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Min(x => x.CheckNumber); // .First().CheckNumber;
            var hasta = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Max(x => x.CheckNumber); // .Last().CheckNumber;
            var nbol = listaChecks.Count(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha);
            var totalAlimentos = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.Alimentos);
            var totalBebidas = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.BebidasSAlcohol);
            var totalTabacos = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.Tabacos);
            var totalOtros = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.Otros);
            var totalNeto = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.TotalNeto);
            var totalIva = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => Convert.ToInt32(x.TotalNeto * 0.19));
            var total = listaChecks.Where(x => Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha).Sum(x => x.VentasTotales);

            var lineaAEscribir = string.Format("{0};10;{1};{2};{3};{4};{5};{6};{7};0;{8};0;{9};{10}\n", fecha, 
                desde, hasta, nbol, totalAlimentos, totalBebidas, totalTabacos, totalOtros, totalNeto, totalIva, total);

            using (var sw = File.AppendText(path))
            {
                sw.WriteLine(lineaAEscribir);
            }
        }

        private void EscribirTitulosColumnas(string path)
        {
            var titulos = "FECHA;;DESDE;HASTA;N° BOL;ALIMENTOS;BEBIDAS;TABACOS;OTROS;;VENTAS NETAS;VENTAS EXENTAS;IVA;VENTAS TOTALES\n";
            using (var file = new StreamWriter(path))
            {
                file.WriteLine(titulos);
            }
        }

        private void cbRango_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaHasta.Enabled = cbRango.Checked;
        }

        private void cbPuntoDeVenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
        }

        private void btnVentasDetalle_Click(object sender, EventArgs e)
        {

        }

        private void CrearDirectorioParaArchivosExportados(string path)
        {

        }
    }
}
