using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD4T4AlejandroMartinez.MVVM.Models;

namespace UD4T4AlejandroMartinez.MVVM.ViewModels
{
    /// <summary>
    /// ViewModel para la página de día.
    /// </summary>
    public class DiaViewModel : INotifyPropertyChanged
    {
        private List<Dia> _dias;
        /// <summary>
        /// Lista de días.
        /// </summary>
        public List<Dia> Dias
        {
            get { return _dias; }
            set
            {
                if (_dias != value)
                {
                    _dias = value;
                    OnPropertyChanged(nameof(Dias));
                }
            }
        }

        /// <summary>
        /// Número de la semana.
        /// </summary>
        public int Semana { get; set; }

        /// <summary>
        /// Constructor de la clase DiaViewModel.
        /// </summary>
        /// <param name="weekNumber">Número de la semana.</param>
        public DiaViewModel(int weekNumber)
        {
            Semana = weekNumber;
            var startDate = new DateTime(2023, 3, 18).AddDays((weekNumber - 1) * 7);

            Dias = new List<Dia>();
            for (int i = 0; i < 5; i++)
            {
                var date = startDate.AddDays(i);
                Dias.Add(new Dia { DiaN = $"Día {date.Day}" });
            }
        }

        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método para invocar el evento PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambió.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
