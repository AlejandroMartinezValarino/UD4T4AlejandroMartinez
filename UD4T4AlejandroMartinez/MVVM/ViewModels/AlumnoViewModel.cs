using Firebase.Auth;
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
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UD4T4AlejandroMartinez.MVVM.Models;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace UD4T4AlejandroMartinez.MVVM.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión de datos de los alumnos.
    /// </summary>
    public class AlumnoViewModel : INotifyPropertyChanged
    {
        private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");
        private ObservableCollection<Alumno> _alumnos;

        /// <summary>
        /// Alumno actualmente seleccionado.
        /// </summary>
        public Alumno AlumnoActual { get; set; }

        /// <summary>
        /// Comando para imprimir los datos de un alumno.
        /// </summary>
        public ICommand PrintCommand { get; set; }

        /// <summary>
        /// Colección de alumnos.
        /// </summary>
        public ObservableCollection<Alumno> Alumnos
        {
            get => _alumnos;
            set
            {
                _alumnos = value;
                OnPropertyChanged(); // Notificar cambios en la propiedad
            }
        }

        /// <summary>
        /// Evento que se desencadena cuando cambia alguna propiedad del ViewModel.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método que se ejecuta cuando cambia alguna propiedad del ViewModel.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambió.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Constructor del ViewModel.
        /// </summary>
        public AlumnoViewModel()
        {
            Alumnos = new ObservableCollection<Alumno>();
            PrintCommand = new Command<Alumno>(PrintAlumno);
            LoadStudents();
        }

        /// <summary>
        /// Carga los datos de los alumnos desde Firebase.
        /// </summary>
        private async Task LoadStudents()
        {
            var alumnos = await client.Child("Alumno").OnceAsync<Alumno>();
            Alumnos = new ObservableCollection<Alumno>(alumnos.Select(a => a.Object));
        }

        /// <summary>
        /// Imprime los datos de un alumno.
        /// </summary>
        /// <param name="alumno">El alumno a imprimir.</param>
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

        /// <summary>
        /// Redimensiona una imagen.
        /// </summary>
        /// <param name="imageData">Datos de la imagen a redimensionar.</param>
        /// <param name="maxWidth">Ancho máximo deseado.</param>
        /// <param name="maxHeight">Alto máximo deseado.</param>
        /// <returns>Los datos de la imagen redimensionada.</returns>
        public byte[] ResizeImage(byte[] imageData, int maxWidth, int maxHeight)
        {
            using (var input = new MemoryStream(imageData))
            using (var inputStream = new SKManagedStream(input))
            using (var original = SKBitmap.Decode(inputStream))
            {
                int width, height;
                if (original.Width > original.Height)
                {
                    width = maxWidth;
                    height = (int)(original.Height * maxWidth / (float)original.Width);
                }
                else
                {
                    height = maxHeight;
                    width = (int)(original.Width * maxHeight / (float)original.Height);
                }

                using (var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
                using (var image = SKImage.FromBitmap(resized))
                using (var output = new MemoryStream())
                {
                    image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(output);
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Dibuja una imagen PNG en un canvas.
        /// </summary>
        /// <param name="canvas">El canvas donde se dibujará la imagen.</param>
        /// <param name="resourcePath">Ruta del recurso de la imagen.</param>
        /// <param name="x">Posición X de la imagen.</param>
        /// <param name="y">Posición Y de la imagen.</param>
        /// <param name="width">Ancho de la imagen.</param>
        /// <param name="height">Alto de la imagen.</param>
        public void DrawPng(SKCanvas canvas, string resourcePath, float x, float y, float width, float height)
        {
            using (var resourceStream = typeof(App).Assembly.GetManifestResourceStream(resourcePath))
            {
                if (resourceStream != null)
                {
                    using (var bitmap = SKBitmap.Decode(resourceStream))
                    {
                        if (bitmap != null)
                        {
                            var srcRect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
                            var destRect = new SKRect(x, y, x + width, y + height);
                            canvas.DrawBitmap(bitmap, srcRect, destRect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el mes correspondiente a la semana indicada.
        /// </summary>
        /// <param name="week">Número de la semana.</param>
        /// <returns>El nombre del mes correspondiente.</returns>
        public String MonthFromWeek(int week)
        {
            String month = "";
            switch(week)
            {
                case 1:
                case 2: month = "marzo";
                    break;
                case 3:
                case 4:
                case 5:
                case 6: month = "avril";
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11: month = "mayo";
                    break;
                case 12:
                case 13:
                case 14:
                case 15: month = "junio";
                    break;
                default: month = "error";
                    break;
            }
            return month;
        }

        /// <summary>
        /// Procesa una plantilla de PDF con los datos del estudiante y genera un nuevo PDF.
        /// </summary>
        /// <param name="originalTemplateStream">Stream de la plantilla PDF original.</param>
        /// <param name="modifiedTemplateFileName">Nombre del archivo para el PDF generado.</param>
        public void ProcessStudentTemplatePDF(Stream originalTemplateStream, string modifiedTemplateFileName)
        {
            var svg = new SKSvg();
            svg.Load(originalTemplateStream);
            var svgPicture = svg.Picture;
            var logoJuntaDeAndalucia = "UD4T4AlejandroMartinez.Resources.Raw.logo.png";

            float boxX, boxY, offsetX, offsetY, textX, textY;

            var pdfWidth = (int)svgPicture.CullRect.Width;
            var pdfHeight = (int)svgPicture.CullRect.Height;

            String month = "";

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pdfFilePath = Path.Combine(documentsPath, modifiedTemplateFileName);
            using (var pdfStream = File.OpenWrite(pdfFilePath))
            {
                using (var document = SKDocument.CreatePdf(pdfStream))
                {
                    using (var bitmap = new SKBitmap(pdfWidth, pdfHeight))
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        foreach(Semana semana in AlumnoActual.Semanas)
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
                            using (var webClient = new WebClient())
                            {
                                var imageData = webClient.DownloadData(AlumnoActual.FotoPath);
                                var resizedImageData = ResizeImage(imageData, 100, 100);
                                using (var imageStream = new MemoryStream(resizedImageData))
                                {
                                    var photoBitmap = SKBitmap.Decode(imageStream);
                                    if (photoBitmap != null)
                                    {
                                        canvas.DrawBitmap(photoBitmap, new SKPoint(812.79175f, 51.757161f));
                                    }
                                }
                            }
                            DrawPng(canvas, logoJuntaDeAndalucia, 200, 30, 200, 50);

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
                            boxX = 96;
                            boxY = 300;

                            textX = boxX + offsetX;
                            textY = boxY + offsetY;

                            for (int i = 0; i<semana.Dias.Count;i++)
                            {
                                canvas.DrawText($"{semana.Dias[i].DiaN}", textX, textY+(i*70f), textPaint);
                                canvas.DrawText($"{semana.Dias[i].Actividad}", textX+90f, textY + (i * 70f), textPaint);
                                canvas.DrawText($"{semana.Dias[i].Tiempo}", textX+490f, textY + (i * 70f), textPaint);
                                canvas.DrawText($"{semana.Dias[i].Observaciones}", textX+715f, textY + (i * 70f), textPaint);
                            }

                            textPaint = new SKPaint
                            {
                                Color = SKColors.Black,
                                TextSize = 15,
                                IsAntialias = true,
                                Typeface = SKTypeface.Default
                            };

                            textX = 79;
                            textY = 146.31217f;

                            month = MonthFromWeek(semana.NumeroSemana);

                            canvas.DrawText($"Semana {semana.NumeroSemana}: del {semana.Dias.First().DiaN} al {semana.Dias.Last().DiaN} de {month} de 2024", textX, textY, textPaint);

                            textPaint = new SKPaint
                            {
                                Color = SKColors.White,
                                TextSize = 15,
                                IsAntialias = true,
                                Typeface = SKTypeface.Default
                            };

                            textX = 130.02335f;
                            textY = 267.90348f;

                            canvas.DrawText("DIA", textX, textY, textPaint);

                            textX = 625.83663f;
                            textY = 267.90348f;

                            canvas.DrawText("TIEMPO EMPLEADO", textX, textY, textPaint);

                            textX = 850.60942f;
                            textY = 267.90348f;

                            canvas.DrawText("OBSERVACIONES", textX, textY, textPaint);

                            textX = 220.29111f;
                            textY = 267.90348f;

                            canvas.DrawText("ACTIVIDAD DESARROLLADA/PUESTO FORMATIVO", textX, textY, textPaint);
                                    using (var pdfCanvas = document.BeginPage(pdfWidth, pdfHeight))
                                    {
                                        pdfCanvas.Clear(SKColors.White);
                                        pdfCanvas.DrawBitmap(bitmap, 0, 0);
                                    }

                                    document.EndPage();
                                }

                                document.Close();
                            }
                    }
            }
            Application.Current.MainPage.DisplayAlert("Creación del pdf", "El pdf se ha creado correctamente", "Ok");
        }
    }
}
