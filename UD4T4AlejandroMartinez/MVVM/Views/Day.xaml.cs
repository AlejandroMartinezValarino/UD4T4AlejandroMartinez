namespace UD4T4AlejandroMartinez.MVVM.Views;

using Firebase.Database;
using Firebase.Database.Query;
using MVVM.Models;
using UD4T4AlejandroMartinez.MVVM.ViewModels;

public partial class Day : ContentPage
{
    private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");

    /// <summary>
    /// Constructor de la página de día.
    /// </summary>
    /// <param name="weekNumber">Número de la semana.</param>
    public Day(int weekNumber)
	{
		InitializeComponent();

        BindingContext = new DiaViewModel(weekNumber);
        LoadData();
    }

    /// <summary>
    /// Método invocado cuando se hace clic en el botón de guardar.
    /// Guarda los datos del día en Firebase.
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Argumentos del evento.</param>
    public async void OnSaveClicked(object sender, EventArgs e)
	{
		var viewModel = BindingContext as DiaViewModel;

		if(viewModel != null)
		{
            var semana = new Semana
            {
                NumeroSemana = viewModel.Semana,
                Dias = viewModel.Dias
            };

            if (AppData.AlumnoActual.Semanas == null)
            {
                AppData.AlumnoActual.Semanas = new List<Semana>();
            }

            var semanaExistente = AppData.AlumnoActual.Semanas.FirstOrDefault(s => s.NumeroSemana == viewModel.Semana);
            if (semanaExistente != null)
            {
                semanaExistente.Dias = semana.Dias;
            }
            else
            {
                AppData.AlumnoActual.Semanas.Add(semana);
            }

            await client.Child("Alumno").Child(AppData.AlumnoActual.Id).PutAsync(AppData.AlumnoActual);
            await this.DisplayAlert("Guardar", "Se han guardado los datos con éxito", "Ok");
        }
	}

    /// <summary>
    /// Método asincrónico para cargar los datos del día desde Firebase.
    /// </summary>
    private async void LoadData()
    {
        var viewModel = BindingContext as DiaViewModel;
        var alumnos = await client.Child("Alumno").OnceAsync<Alumno>();

        if (alumnos != null && viewModel != null)
        {

            var alumno = alumnos.FirstOrDefault(a => a.Object.Id == AppData.AlumnoActual.Id);

            if (alumno != null && alumno.Object.Semanas != null)
            {
                var semana = alumno.Object.Semanas.FirstOrDefault(s => s.NumeroSemana == viewModel.Semana);

                if (semana != null)
                {
                    viewModel.Dias = semana.Dias;
                }
            }
        }
    }
}