// Emgu CV
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using PdfSharp.Drawing;
// PDF
using PdfSharp.Pdf;
using PdfSharp; // Aquí és on resideix GlobalFontSettings
using PdfSharp.Fonts; // Necessari per a IFontResolver
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using iTextSharp.text; // Para PDF
using iTextSharp.text.pdf;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing; // <--- IMPRESCINDIBLE per a fer servir Bitmaps

namespace TriatgePeces
{
    public partial class frmTriatge1 : Form
    {
        // Variables globals
        private Mat? _imatgeActual;
        private List<Peca> _llistaPeces = new List<Peca>();

        public frmTriatge1()
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
            }

            InitializeComponent();
        }

        // --- BLOC 1: VISIÓ ---

        private void btnGris1_Click(object sender, EventArgs e)
        {
            if (_imatgeActual == null) return;
            Mat gris = new Mat();

            CvInvoke.CvtColor(_imatgeActual, gris, ColorConversion.Bgr2Gray);

            //_imatgeActual = gris;


            // EXAMEN INICI: Implementeu la invocació per a  convertir la _imatgeActual a Mat gris = new Mat();



            // EXAMEN FI

            // Aquí ja teniu l'enviament de la vostra imatge grisa (gris) al visor
            ActualitzarVisor(gris);
        }

        private void btnSuavitzat1_Click(object sender, EventArgs e)
        {
            if (_imatgeActual == null) return;
            Mat suavitzat = new Mat();

            CvInvoke.GaussianBlur(_imatgeActual, _imatgeActual, new System.Drawing.Size(5, 5), 0);

            // probar sino borrar
            /*
            if (pbImatge1.Image != null) { 
            
                pbImatge1 .Dispose();
                pbImatge1.Image = null;

            }
            */

            string path = Path.Combine(Path.GetTempPath(), "temp_suave.png");

            if (File.Exists(path)) File.Delete(path);

            _imatgeActual.Save(path);


            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                pbImatge1.Image = System.Drawing.Image.FromStream(fs);
            }

            // EXAMEN INICI: Implementeu la invocació per a  convertir la _imatgeActual a Mat suavitzat = new Mat();



            // EXAMEN FI

            // Aquí ja teniu l'enviament de la vostra imatge suavitzada (suavitzat) al visor
            ActualitzarVisor(suavitzat);
        }

        private void btnSegmentacio1_Click(object sender, EventArgs e)
        {
            if (_imatgeActual == null) return;
            Mat binaria = new Mat();

            CvInvoke.Threshold(_imatgeActual, _imatgeActual, 150, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

            // probar
            string path = Path.Combine(Path.GetTempPath(), "3_threshold.png");
            _imatgeActual.Save(path);
            pbImatge1.Image = System.Drawing.Image.FromFile(path);


            // EXAMEN INICI: Implementeu la invocació Threshold per a segmentar la _imatgeActual a Mat binaria = new Mat();



            // EXAMEN FI

            // Aquí ja teniu l'enviament de la vostra imatge segmentada (binaria) al visor
            ActualitzarVisor(binaria);
        }

        private void btnContorn1_Click(object sender, EventArgs e)
        {
            if (_imatgeActual == null)
            {
                MessageBox.Show("Heu de carregar una imatge primer.");
                return;
            }

            // Treballem sobre una còpia per no perdre la imatge original neta si cal
            // comentada

             Mat imatgeColor = _imatgeActual.Clone();

            // Suposem que la imatge actual ja ha estat processada (Gris -> Suavitzat -> Threshold)
            // Si no, caldria fer el processament aquí abans de FindContours

            // (VectorOfVectorOfPoint contorns = new VectorOfVectorOfPoint())


            //Mat jerarquia = new Mat();

            // EXAMEN INICI: Invoca la llibreria per a trobar els contorns de la figura



            // EXAMEN FI

            VectorOfVectorOfPoint contorns = new VectorOfVectorOfPoint();

            CvInvoke.FindContours(_imatgeActual, contorns, null,
                Emgu.CV.CvEnum.RetrType.External,
                Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            

            for (int i = 0; i < contorns.Size; i++)
                {
                    // Filtre d'àrea per evitar soroll (ajustable segons la foto)
                    double area = CvInvoke.ContourArea(contorns[i]);
                    //if (area < 500) continue;

                  //  VectorOfPoint aprox = new VectorOfPoint();


                    if (area > 1500) {
                        VectorOfPoint approx = new VectorOfPoint();

                        double perimetre = CvInvoke.ArcLength(contorns[i], true);

                        CvInvoke.ApproxPolyDP(contorns[i], approx, 0.02 * perimetre, true);
                        int vertices = approx.Size;
                        string tipus = "Desconegut  ";

                        if (vertices == 3) tipus = "Triangle";
                        else if (vertices == 4) tipus = "Rectangle";
                        else if (vertices == 5) tipus = "Pentàgon";
                        else if (vertices > 5) tipus = "Cercle";

                       lblPoligon1.Text = $"Detectat: {tipus} ({approx.Size} vèrt.)";
                   // lblPoligon1.Text = $"Detectat: {tipus} ({vertices} vèrt.)";

                    System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(contorns[i]);
                        CvInvoke.Rectangle(_imatgeActual, rect, new MCvScalar(0, 0, 255), 3);

                    }

                    if (pbImatge1.Image != null)
                    {

                        pbImatge1.Image.Dispose();
                        pbImatge1.Image = null;

                    }

                    string path = Path.Combine(Path.GetTempPath(), "resultat_contorns.png");
                    _imatgeActual.Save(path);


                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        pbImatge1.Image = System.Drawing.Image.FromStream(fs);
                    }



                    // 1. Aproximació de polígons per comptar vèrtexs




                    // EXAMEN INICI: Invoca la llibreria per a trobar els contorns de la figura i comptar els vèrtex aprox.Size



                    // EXAMEN FI

                    // 2. Classificació segons el nombre de vèrtexs

                    //string tipus = "";

                    // EXAMEN INICI
                    //


                    // EXAMEN FI

                    // Actualitzem el label amb l'última detecció

                    // lblPoligon1.Text = $"Detectat: {tipus} ({aprox.Size} vèrt.)";
                }

                // Mostrem el resultat final amb els rectangles dibuixats
                pbImatge1.Image = imatgeColor.ToBitmap();
            }
        

        // --- BLOC 2: GESTIÓ I PDF ---

        private void btnAfegir1_Click(object sender, EventArgs e)
        {
            // Lògica simplificada per a l'examen: afegeix la darrera peça detectada
            if (lblPoligon1.Text.Contains("Detectat:"))
            {
                //string[] partes = lblPoligon1.Text.Split(' ');
                string nom = lblPoligon1.Text.Split(':')[1].Split('(')[0].Trim();
                string tipo = nom;

                Peca peca = new Peca
                {
                    Tipus = tipo,
                    Area = 0,
                    Data = DateTime.Now.ToString("dd//MM/yyyy HH:mm")


                };

                _llistaPeces.Add(peca);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _llistaPeces;

               // RefrescarGraella(_llistaPeces);


                // string nom = lblPoligon1.Text.Split(':')[1].Split('(')[0].Trim();

                // EXAMEN INICI: Afegeix una péça (Objecte Peca.cs) passant el tipus de la peça (nom). L'ària i la data poden quedar buits.



                // EXAMEN FI

                // Crida a funció per a refrescar el contingut de la graella

            }
        }

        private void tbCerca1_TextChanged(object sender, EventArgs e)
        {
            // Filtre LINQ dinàmic

            // EXAMEN INICI: Apliqueu el filtre dinàmic per a refrescar el que es mostra a la graella DataGridView


            string texto = tbCerca1.Text.ToLower();
            var filtrados = _llistaPeces
                 .Where(f => f.Tipus.ToLower().Contains(texto))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtrados;


            // EXAMEN FI

            // Crida a funció per a refrescar el contingut de la graella
            // RefrescarGraella(filtrada);
        }

        private void btnInforme1_Click(object sender, EventArgs e)
        {
            if (_llistaPeces.Count == 0) return;
            Document doc = new Document();


            PdfWriter.GetInstance(doc, new FileStream("Resultats.pdf", FileMode.Create));

            doc.Open();


            doc.Add(new Paragraph("FIGURES TROBADES:"));
            doc.Add(new Paragraph("Total detectats: " + _llistaPeces.Count));
            doc.Add(new Paragraph(" "));


            foreach (var f in _llistaPeces)
            {
                doc.Add(new Paragraph("- Tipus: " + f.Tipus + " | Data: " + f.Data));
            }

            doc.Close(); // tancar
            MessageBox.Show("PDF Generat correctament!");
           



            // 1. Verifiquem si hi ha dades a la graella (que representa la cerca actual)

            // EXAMEN INICI



            // EXAMEN FI

            // 2. Recorrem les files del DataGridView en lloc de la llista global
            // EXAMEN INICI



            // EXAMEN FI

            // 3. Afegim el recompte total al final de l'informe
            // EXAMEN INICI



            // EXAMEN FI
        }

        // Funcions auxiliars
        private void ActualitzarVisor(Mat m)
        {
            _imatgeActual = m.Clone();
            pbImatge1.Image = _imatgeActual.ToBitmap();
        }

        private void RefrescarGraella(List<Peca> dades)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dades;
        }

        private void btnCarregar1_Click(object sender, EventArgs e)
        {
            if (ofdObrir1.ShowDialog() == DialogResult.OK)
            {
                _imatgeActual = CvInvoke.Imread(ofdObrir1.FileName);
                pbImatge1.Image = _imatgeActual.ToBitmap();
            }
        }
    }
}