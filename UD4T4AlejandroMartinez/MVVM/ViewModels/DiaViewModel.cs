using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD4T4AlejandroMartinez.MVVM.Models;

namespace UD4T4AlejandroMartinez.MVVM.ViewModels
{
    public class DiaViewModel : INotifyPropertyChanged
    {
        private List<Dia> _dias;
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
        public int Semana { get; set; }

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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
