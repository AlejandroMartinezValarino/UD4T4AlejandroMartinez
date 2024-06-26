namespace UD4T4AlejandroMartinez.MVVM.Views;

using Firebase.Database;
using Firebase.Database.Query;
using MVVM.Models;
using UD4T4AlejandroMartinez.MVVM.ViewModels;

public partial class Day : ContentPage
{
    private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");

    /// <summary>
    /// Constructor de la p�gina de d�a.
    /// </summary>
    /// <param name="weekNumber">N�mero de la semana.</param>
    public Day(int weekNumber)
	{
		InitializeComponent();

        BindingContext = new DiaViewModel(weekNumber);
        LoadData();
    }

    /// <summary>
    /// M�todo invocado cuando se hace clic en el bot�n de guardar.
    /// Guarda los datos del d�a en Firebase.
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
            await this.DisplayAlert("Guardar", "Se han guardado los datos con �xito", "Ok");
        }
	}

    /// <summary>
    /// M�todo asincr�nico para cargar los datos del d�a desde Firebase.
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