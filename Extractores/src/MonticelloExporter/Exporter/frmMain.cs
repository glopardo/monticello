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
        private bool _imprimeDetalle;
        private int _puntoDeVenta;
        private Configuration _configuration;
        private int _minBol = 999999;
        private int _maxBol = 0;

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
                Logger.Write("ERROR: " + ex.Message);
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void btnventasGenerales_Click(object sender, EventArgs e)
        {
            _imprimeDetalle = false;
            if (PreClick())
            {
                var bw = new BackgroundWorker();
                bw.DoWork += bw_DoWork_VentasGenerales;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                bw.RunWorkerAsync();
            }
        }

        private void btnVentasDetalle_Click(object sender, EventArgs e)
        {
            _imprimeDetalle = true;
            if (PreClick())
            {
                var bw = new BackgroundWorker();
                bw.DoWork += bw_DoWork_VentasDetalle;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                bw.RunWorkerAsync();
            }
        }

        private bool PreClick()
        {
            if (dtpFechaHasta.Value < dtpFechaDesde.Value)
            {
                MessageBox.Show("La \'Fecha Desde\' no puede ser mayor a la \'Fecha Hasta\'");
                return false;
            }
            SetearPuntoDeVenta(cbPuntoDeVenta);
            return pbProcesando.Visible = true;
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

        void bw_DoWork_VentasGenerales(object sender, DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var connection = GetDbConnection(_configuration);
            GenerarArchivo(connection);
        }

        void bw_DoWork_VentasDetalle(object sender, DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var connection = GetDbConnection(_configuration);
            GenerarArchivo(connection);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            dtpFechaDesde.Enabled = btnVentasDetalle.Enabled = 
                btnventasGenerales.Enabled = cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
            dtpFechaHasta.Enabled = cbRango.Checked;
            pbProcesando.Visible = false;
        }

        private void GenerarArchivo(SqlConnection connection)
        {
            lblStatus.Text = string.Empty;
            dtpFechaDesde.Enabled = dtpFechaHasta.Enabled = btnVentasDetalle.Enabled = btnventasGenerales.Enabled = cbRango.Enabled = cbPuntoDeVenta.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            const string pathCarpetaArchivosExportados = ".\\Exportados\\";
            CrearDirectorioParaArchivosExportados(pathCarpetaArchivosExportados);
            var path = cbRango.Checked ? string.Format(_imprimeDetalle ? "{0}{1}-{2}_{3}_Monticello_BoletasDeVentaDetalle.csv" : "{0}{1}-{2}_{3}_Monticello_BoletasDeVenta.csv",
                                                        pathCarpetaArchivosExportados,
                                                        dtpFechaDesde.Value.ToString("yyyyMMdd"), 
                                                        dtpFechaHasta.Value.ToString("yyyyMMdd"),
                                                        cbPuntoDeVenta.Text) : string.Format(_imprimeDetalle ? "{0}{1}_{2}_Monticello_BoletasDeVentaDetalle.csv" : "{0}{1}_{2}_Monticello_BoletasDeVenta.csv",
                                                        pathCarpetaArchivosExportados,
                                                        dtpFechaDesde.Value.ToString("yyyyMMdd"),
                                                        cbPuntoDeVenta.Text.Replace(" ", ""));
            
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
                    var fechaMasUnDia = dtpFechaDesde.Value.AddDays(i + 1).ToString("yyyyMMdd");
                    var query = "SELECT PCWSID, CONVERT(VARCHAR(10), FCRBSNZDATE, 103) FCRBSNZDATEDDMMYYYY, CONVERT(VARCHAR(10), FCRBSNZDATE, 112) FCRBSNZDATEYYYYMMDD, CONVERT(VARCHAR(5), " +
                                "FCRBSNZDATE, 108) HORAMIN, FCRINVNUMBER, MICROSCHKNUM, ROUND(SUBTOTAL2 / 1.19, 0) DISC, SUBTOTAL8 SUBTOTALCONIVA, TAXTTL1 IVA, EXTRAFIELD3 EDINUMBER " +
                                "FROM MICROSDB.FCR_INVOICE_DATA WHERE ((CONVERT(VARCHAR(10), FCRBSNZDATE, 112) = '" + fecha + "' AND CONVERT(VARCHAR(5), FCRBSNZDATE, 108) > '08:01') " +
                                "OR ((CONVERT(VARCHAR(10), FCRBSNZDATE, 112) = '" + fechaMasUnDia + "' AND CONVERT(VARCHAR(5), FCRBSNZDATE, 108) < '08:00'))) " + " AND EXTRAFIELD5 = " +
                                _puntoDeVenta + " ORDER BY 1, FCRBSNZDATE DESC";

                    //DEBUG
                    //Logger.Write(query);

                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        //Itero por CABECERA CHECK uno por uno
                        //Totales para ventas generales
                        int totalAlimentosPorCheck = 0,
                            totalBebidasPorCheck = 0,
                            totalTabacosPorCheck = 0,
                            totalOtrosPorCheck = 0,
                            totalPropinasPorCheck = 0;

                        if (Convert.ToInt32(reader["FCRINVNUMBER"]) > _maxBol)
                        {
                            _maxBol = Convert.ToInt32(reader["FCRINVNUMBER"]);
                        }

                        if (Convert.ToInt32(reader["FCRINVNUMBER"]) < _minBol)
                        {
                            _minBol = Convert.ToInt32(reader["FCRINVNUMBER"]);
                        }

                        //Totales para ventas con detalle
                        //int totalVisa = 0, totalMaster = 0, totalAmerican = 0, totalDiners = 0,
                        //    totalRedCompra = 0, totalComplimentary = 0, totalCargoHab = 0,
                        //    totalCredEmpresa = 0, totalFact = 0, totalCheque = 0,
                        //    totalTransfDeposito = 0, totalEfectivo = 0, totalCigarros = 0;

                        var microsCheck = new MicrosCheck()
                        {
                            Pcws = reader["PCWSID"].ToString(),
                            CheckNumber = reader["MICROSCHKNUM"].ToString(),
                            EdiNumber = reader["EDINUMBER"].ToString(),
                            Fecha = reader["FCRBSNZDATEDDMMYYYY"].ToString(),
                            Hora = reader["HORAMIN"].ToString(),
                            TotalNeto = Convert.ToInt32(reader["SUBTOTALCONIVA"]) - Convert.ToInt32(reader["IVA"]),
                            VentasTotales = Convert.ToInt32(reader["SUBTOTALCONIVA"]),
                            Iva = Convert.ToInt32(reader["IVA"])
                        };

                        //var totalIvaPorCheck = 0.0;
                        var totalDescPorCheck = reader["DISC"];
                        var hora = reader["HORAMIN"].ToString();
                        var horaMasUnMinuto = DateTime.Parse(hora).AddMinutes(1).ToString("HH:mm");
                        var horaMenosUnMinuto = DateTime.Parse(hora).AddMinutes(-1).ToString("HH:mm");

                        var chkNum = reader["MICROSCHKNUM"];
                        var queryChecks = "SELECT CHECKID FROM MICROSDB.CHECKS WHERE CONVERT(VARCHAR(10), CHECKPOSTINGTIME, 112) = '" + reader["FCRBSNZDATEYYYYMMDD"] + 
                                          "' AND CONVERT(VARCHAR(5), CHECKPOSTINGTIME, 108) IN ('" + hora + "', '" + horaMenosUnMinuto + "', '" + horaMasUnMinuto + "') and CHECKNUMBER = " + chkNum; //Query para checks

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
                                                   "WHERE CHECKID = " + checkId + " AND TOTAL <> 0 " + "ORDER BY CHECK_DETAIL.DETAILINDEX";

                            //DEBUG
                            //Logger.Write(queryCheckDetail);

                            command = connection.CreateCommand();
                            command.CommandText = queryCheckDetail;

                            var subreaderDetail = command.ExecuteReader();

                            //CHECK DETAILS
                            var totalPorCheck = 0;
                            var totalSinIvaPorCheck = 0;

                            while (subreaderDetail.Read())
                            {
                                //DEBUG
                                //Logger.Write(microsCheck.EdiNumber + ";" + microsCheck.CheckNumber + ";" + subreaderDetail["FAMGRP"] + ";" + subreaderDetail["TOTALSINIVA"]);
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

                                var lFGOtros = new List<int>
                                {
                                    120, 125, 126, 130, 131, 302, 303
                                };

                                foreach (var fgAlimento in lFGAlimentos)
                                {
                                    if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgAlimento)
                                    {
                                        totalAlimentosPorCheck += Convert.ToInt32(subreaderDetail["TOTALSINIVA"]);
                                        encontrado = true;
                                    }
                                }

                                if (!encontrado)
                                {
                                    foreach (var fgBebida in lFGBebidas)
                                    {
                                        if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgBebida)
                                        {
                                            totalBebidasPorCheck += Convert.ToInt32(subreaderDetail["TOTALSINIVA"]);
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
                                            totalTabacosPorCheck += Convert.ToInt32(subreaderDetail["TOTALSINIVA"]);
                                            encontrado = true;
                                        }
                                    }
                                }

                                if (!encontrado)
                                {
                                    foreach (var fgOtros in lFGOtros)
                                    {
                                        if (Convert.ToInt32(subreaderDetail["FAMGRP"]) == fgOtros)
                                            totalOtrosPorCheck += Convert.ToInt32(subreaderDetail["TOTALSINIVA"]);
                                    }
                                }

                                totalSinIvaPorCheck += Convert.ToInt32(subreaderDetail["TOTALSINIVA"]);
                                totalPorCheck += Convert.ToInt32(subreaderDetail["TOTAL"]);
                            }

                            if (_imprimeDetalle)
                            {
                                //PAGOS
                                var queryPayments = "SELECT CHECK_DETAIL.CHECKDETAILID, CHECK_DETAIL.CHECKID, CHECK_DETAIL.DETAILINDEX, " +
                                                    "CHECK_DETAIL.DETAILTYPE, CHECK_DETAIL.TOTAL TOTAL, CHECK_DETAIL.SALESCOUNT, TENDER_MEDIA_DETAIL.TENDMEDID TENDMEDID, TENDER_MEDIA.OBJECTNUMBER TENDERMEDIA " +
                                                    "FROM CHECK_DETAIL INNER JOIN TENDER_MEDIA_DETAIL ON CHECK_DETAIL.CHECKDETAILID = TENDER_MEDIA_DETAIL.CHECKDETAILID " +
                                                    "INNER JOIN TENDER_MEDIA ON TENDER_MEDIA_DETAIL.TENDMEDID = TENDER_MEDIA.TENDMEDID " +
                                                    "WHERE CHECKID = " + checkId + " ORDER BY CHECK_DETAIL.DETAILINDEX";

                                //DEBUG
                                //Logger.Write(queryPayments);

                                command = connection.CreateCommand();
                                command.CommandText = queryPayments;

                                var subreaderPayments = command.ExecuteReader();
                                microsCheck.Payments = new Dictionary<int, int>();

                                while (subreaderPayments.Read())
                                {
                                    //DEBUG
                                    //Logger.Write(checkId + ": " + Convert.ToInt32(subreaderPayments["TENDERMEDIA"]) + ", " + subreaderPayments["TOTAL"]);
                                    if (!microsCheck.Payments.ContainsKey(Convert.ToInt32(subreaderPayments["TENDERMEDIA"])))
                                    {
                                        if (microsCheck.Payments.Count > 0)
                                            //Porque si tengo por ejemplo VISA y luego resto efectivo, debería restar a VISA el efectivo (?)
                                        {
                                            var totalPayment = Convert.ToInt32(subreaderPayments["TOTAL"]);
                                            var first = microsCheck.Payments.Keys.First();
                                            microsCheck.Payments[first] += totalPayment;

                                            if (totalPayment < 0)
                                                totalPropinasPorCheck += Math.Abs(totalPayment);
                                        }
                                        else if (Convert.ToInt32(subreaderPayments["TOTAL"]) > 0)
                                            microsCheck.Payments.Add(Convert.ToInt32(subreaderPayments["TENDERMEDIA"]), Convert.ToInt32(subreaderPayments["TOTAL"]));
                                    }
                                    else
                                        microsCheck.Payments[Convert.ToInt32(subreaderPayments["TENDERMEDIA"])] += Convert.ToInt32(subreaderPayments["TOTAL"]);
                                }
                            }
                        }

                        microsCheck.Alimentos = Convert.ToInt32(totalAlimentosPorCheck/* / 1.19*/);
                        microsCheck.BebidasSAlcohol = Convert.ToInt32(totalBebidasPorCheck/* / 1.19*/);
                        microsCheck.Tabacos = Convert.ToInt32(totalTabacosPorCheck/* / 1.19*/);
                        microsCheck.Otros = Convert.ToInt32(totalOtrosPorCheck/* / 1.19*/);;
                        microsCheck.Descuentos = Math.Abs(Convert.ToInt32(reader["DISC"]));
                        microsCheck.Propinas = totalPropinasPorCheck;
                        
                        //DEBUG
                        //Logger.Write(microsCheck.Fecha + " - " + Convert.ToDateTime(microsCheck.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") + " - " + microsCheck.EdiNumber);
                        //Logger.Write(microsCheck.EdiNumber + " - " + microsCheck.CheckNumber + " - " + microsCheck.Alimentos + " - " + microsCheck.BebidasSAlcohol);
                        listaChecks.Add(microsCheck);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = "Error al generar archivos";
                    Cursor.Current = Cursors.Default;
                    lblStatus.Visible = dtpFechaDesde.Enabled = btnVentasDetalle.Enabled = btnventasGenerales.Enabled = 
                        cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
                    dtpFechaHasta.Enabled = cbRango.Checked;
                    pbProcesando.Visible = false;
                    Logger.Write("ERROR: " + ex.Message);
                }
            }

            for (var i = 0; i < cantDias; i++)
            {
                //Por cada día itero y extraigo la información sumarizada
                var fecha = dtpFechaDesde.Value.AddDays(i).ToString("yyyyMMdd");
                var fechaMasUnDia = dtpFechaDesde.Value.AddDays(i + 1).ToString("yyyyMMdd");

                EscribirArchivoSumarizadoPorFecha(path, listaChecks, fecha, fechaMasUnDia);

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Archivos generados exitosamente";
                Logger.Write("Archivos generados exitosamente");
            }

            dtpFechaDesde.Enabled = btnVentasDetalle.Enabled = btnventasGenerales.Enabled = cbRango.Enabled = lblStatus.Visible = cbPuntoDeVenta.Enabled = true;
            dtpFechaHasta.Enabled = cbRango.Checked;
            Cursor.Current = Cursors.Default;
            pbProcesando.Visible = false;
        }

        private void EscribirArchivoSumarizadoPorFecha(string path, List<MicrosCheck> listaChecks, string fecha, string fechaMasUnDia)
        {
            //path = "BoletasVentasMonticello.txt";
            var listaChecksFiltrada = listaChecks.Where(x => (Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fecha && string.Compare(x.Hora, "08:01") > 0) ||
                        ((Convert.ToDateTime(x.Fecha, new CultureInfo("es-AR")).ToString("yyyyMMdd") == fechaMasUnDia && string.Compare(x.Hora, "08:00") < 0))).ToList();

            var listaTerminales = new List<string>();
            //var desde = listaChecksFiltrada.Min(x => x.EdiNumber);
            //var hasta = listaChecksFiltrada.Max(x => x.EdiNumber);
            foreach (var check in listaChecksFiltrada)
            {
                listaTerminales.Add(check.Pcws);
            }

            var listaTerminalesFiltrada = listaTerminales.Distinct().ToList().OrderBy(s => s);

            foreach (var terminal in listaTerminalesFiltrada)
            {
                //var desde = _minBol;
                //var hasta = _maxBol;
                var desde = listaChecksFiltrada.Where(x => x.Pcws == terminal).Min(x => x.EdiNumber);
                var hasta = listaChecksFiltrada.Where(x => x.Pcws == terminal).Max(x => x.EdiNumber);
                var nbol = listaChecksFiltrada.Count(x => x.Pcws == terminal);
                var totalAlimentos = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Alimentos);
                var totalBebidas = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.BebidasSAlcohol);
                var totalTabacos = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Tabacos);
                var totalExento = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Tabacos);
                var totalOtros = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Otros);
                var totalDesc = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Descuentos);
                var totalNeto = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.TotalNeto);
                var totalIva = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Iva);
                var totalPropinas = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.Propinas);
                var total = listaChecksFiltrada.Where(x => x.Pcws == terminal).Sum(x => x.VentasTotales);

                //totales de pagos
                int totalVisa = 0, totalMaster = 0, totalAmerican = 0, totalDiners = 0,
                    totalRedCompra = 0, totalComplimentary = 0, totalCargoHab = 0,
                    totalCredEmpresa = 0, totalFact = 0, totalCheque = 0,
                    totalTransfDeposito = 0, totalEfectivo = 0, totalCigarros = 0;

                if (_imprimeDetalle)
                {
                    foreach (var check in listaChecksFiltrada)
                    {
                        //debug
                        //Logger.Write("Visa: ");
                        //check.Payments.Where(x => x.Key == 205);
                        //foreach (var payment in check.Payments)
                        //{
                        //    Logger.Write(payment.Key + " - " + payment.Value);
                        //}
                        if (check.Pcws == terminal)
                        {
                            totalVisa += check.Payments.Where(x => x.Key == 205).Sum(x => x.Value);
                            totalMaster += check.Payments.Where(x => x.Key == 203).Sum(x => x.Value);
                            totalAmerican += check.Payments.Where(x => x.Key == 202).Sum(x => x.Value);
                            totalDiners += check.Payments.Where(x => x.Key == 204).Sum(x => x.Value);
                            totalRedCompra += check.Payments.Where(x => x.Key == 206).Sum(x => x.Value);
                            totalComplimentary += check.Payments.Where(x => x.Key == 321).Sum(x => x.Value);
                            totalCargoHab += check.Payments.Where(x => x.Key == 301).Sum(x => x.Value);
                            totalFact += check.Payments.Where(x => x.Key == 313).Sum(x => x.Value);
                            totalCheque += check.Payments.Where(x => x.Key == 42 || x.Key == 45 || x.Key == 41 || x.Key == 46 || x.Key == 313 || x.Key == 314).Sum(x => x.Value);
                            //DEBUG
                            //Logger.Write("VISA: " + check.Payments.Where(x => x.Key == 205).Sum(x => x.Value) + " (" + check.EdiNumber + ")");
                            //Logger.Write("MASTER: " + check.Payments.Where(x => x.Key == 203).Sum(x => x.Value) + " (" + check.EdiNumber + ")");
                            totalCredEmpresa += check.Payments.Where(x => x.Key == 5 || x.Key == 6 || x.Key == 7 || x.Key == 34 || x.Key == 35 || x.Key == 55 || x.Key == 70 || x.Key == 71 || x.Key == 72 || x.Key == 73
                                    || x.Key == 74 || x.Key == 75 || x.Key == 76 || x.Key == 77 || x.Key == 78 || x.Key == 79 || x.Key == 80 || x.Key == 81 || x.Key == 82 || x.Key == 83 || x.Key == 84 || x.Key == 85
                                    || x.Key == 86 || x.Key == 87 || x.Key == 88 || x.Key == 89 || x.Key == 90 || x.Key == 310 || x.Key == 311 || x.Key == 315).Sum(x => x.Value);
                            totalEfectivo += check.Payments.Where(x => x.Key == 39 || x.Key == 40 || x.Key == 41 || x.Key == 43 || x.Key == 44 || x.Key == 45 || x.Key == 46 || x.Key == 313 || x.Key == 314 || x.Key == 318 || x.Key == 319 || x.Key == 320).Sum(x => x.Value);
                            totalCigarros += check.Payments.Where(x => x.Key == 312).Sum(x => x.Value);
                        }
                    }
                }

                var lineaAEscribir = !_imprimeDetalle
                    ? string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}", fecha, _puntoDeVenta, terminal,
                        desde, hasta, nbol, totalAlimentos, totalBebidas, totalTabacos, totalOtros, totalDesc, totalNeto, totalExento, totalIva, total)
                    : string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}", fecha, _puntoDeVenta, terminal,
                    totalAlimentos, totalBebidas, totalTabacos, totalOtros, totalDesc, total, totalPropinas, totalVisa, totalMaster, totalAmerican, totalDiners, totalRedCompra,
                    totalCredEmpresa, totalComplimentary, totalCargoHab, totalFact, totalCheque, totalEfectivo, totalCigarros); ;

                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine(lineaAEscribir);
                }
            }
        }

        private void EscribirTitulosColumnas(string path)
        {
            var titulos = !_imprimeDetalle ? "FECHA;PUNTO DE VENTA;TERMINAL;DESDE;HASTA;N° BOL;ALIMENTOS;BEBIDAS;TABACOS;OTROS;DESCUENTOS;VENTAS NETAS;VENTAS EXENTAS;IVA;VENTAS TOTALES" :
                "FECHA;PUNTO DE VENTA;TERMINAL;ALIMENTOS;BEBIDAS;TABACOS;OTROS;DESCUENTOS;TOTAL VENTAS;PROPINAS;VISA;MASTER;AMERICAN;DINERS;RED COMPRA;CRED. EMPRESA;COMPLIMENTARY;CARGO HAB.;FACTURA;CHEQUES;EFECTIVO;CIGARROS";
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

        private void CrearDirectorioParaArchivosExportados(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
