using Firebase.Database;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UD4T4AlejandroMartinez.MVVM.Models;
using SKSvg = SkiaSharp.SKSvg;

namespace UD4T4AlejandroMartinez.MVVM.ViewModels
{
    public class AlumnoViewModel : INotifyPropertyChanged
    {
        private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");
        private ObservableCollection<Alumno> _alumnos;
        public Alumno AlumnoActual { get; set; }
        public ICommand PrintCommand { get; set; }

        public ObservableCollection<Alumno> Alumnos
        {
            get => _alumnos;
            set
            {
                _alumnos = value;
                OnPropertyChanged(); // Notificar cambios en la propiedad
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AlumnoViewModel()
        {
            Alumnos = new ObservableCollection<Alumno>();
            PrintCommand = new Command<Alumno>(PrintAlumno);
            LoadStudents();
        }
        private async Task LoadStudents()
        {
            var alumnos = await client.Child("Alumno").OnceAsync<Alumno>();
            Alumnos = new ObservableCollection<Alumno>(alumnos.Select(a => a.Object));
        }

        private void PrintAlumno(Alumno alumno)
        {
            AlumnoActual = alumno;
            var svgFileName = "UD4T4AlejandroMartinez.Resources.Raw.Digital_FichaSemanalAlumno.svg";
            using var resourceStream = typeof(App).Assembly.GetManifestResourceStream(svgFileName);
            if (resourceStream != null)
            {
                ProcessStudentTemplatePDF(resourceStream, $"FichasSemanal_{alumno.Nombre}.pdf");
            }
        }
        public void ProcessStudentTemplatePDF(Stream originalTemplateStream, string modifiedTemplateFileName)
        {
            var svg = new SKSvg();
            svg.Load(originalTemplateStream);
            var svgPicture = svg.Picture;

            var pdfWidth = (int)svgPicture.CullRect.Width;
            var pdfHeight = (int)svgPicture.CullRect.Height;

            using (var bitmap = new SKBitmap(pdfWidth, pdfHeight))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);
                canvas.DrawPicture(svgPicture);

                var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 20,
                    IsAntialias = true,
                    Typeface = SKTypeface.Default
                };

                float boxX, boxY, offsetX, offsetY, textX, textY;



                boxX = 79;
                boxY = 178.74251f;

                offsetX = 10;
                offsetY = 5;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Centro docente: {AlumnoActual.CentroDocente}", textX, textY, textPaint);

                boxX = 79;
                boxY = 195.60775f;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Profesor/a responsable del seguimiento: {AlumnoActual.ProfesorSeguimiento}", textX, textY, textPaint);

                boxX = 79;
                boxY = 227.47298f;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Alumno/a: {AlumnoActual.Nombre}", textX, textY, textPaint);

                boxX = 597;
                boxY = 227.47298f;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Ciclo formativo: {AlumnoActual.CicloFormativo}", textX, textY, textPaint);

                textX = 910.30341f;

                canvas.DrawText($"Grado: {AlumnoActual.Grado}", textX, textY, textPaint);

                boxX = 597;
                boxY = 178.74251f;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Centro de trabajo colaborador: {AlumnoActual.CentroTrabajo}", textX, textY, textPaint);

                boxX = 597;
                boxY = 195.60775f;

                textX = boxX + offsetX;
                textY = boxY + offsetY;

                canvas.DrawText($"Tutor/a centro de trabajo: {AlumnoActual.TutorTrabajo}", textX, textY, textPaint);

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var pdfFilePath = Path.Combine(documentsPath, modifiedTemplateFileName);
                using (var pdfStream = File.OpenWrite(pdfFilePath))
                {
                    using (var document = SKDocument.CreatePdf(pdfStream))
                    {
                        using (var pdfCanvas = document.BeginPage(pdfWidth, pdfHeight))
                        {
                            pdfCanvas.Clear(SKColors.White);
                            pdfCanvas.DrawBitmap(bitmap, 0, 0);

                            // Aquí puedes agregar más texto en el canvas PDF
                        }

                        document.EndPage();
                        document.Close();
                    }
                }
            }
        }

    }
}
