using Firebase.Database;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UD4T4AlejandroMartinez.MVVM.Models;

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

        private void PrintAlumno(Alumno alumno)
        {
            AlumnoActual = alumno;
        }

        private async Task LoadStudents()
        {
            var alumnos = await client.Child("Alumno").OnceAsync<Alumno>();
            Alumnos = new ObservableCollection<Alumno>(alumnos.Select(a => a.Object));

        }
    }
}
