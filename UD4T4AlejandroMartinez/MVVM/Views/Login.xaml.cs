using Firebase.Database;
using Firebase.Database.Query;
using UD4T4AlejandroMartinez.MVVM.Models;

namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Login : ContentPage
{
    private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");

    /// <summary>
    /// Constructor de la p�gina de inicio de sesi�n.
    /// </summary>
    public Login()
	{
		InitializeComponent();
	}

    /// <summary>
    /// M�todo invocado cuando se cambia el interruptor de roles (alumno/profesor).
    /// Actualiza el texto de la etiqueta de rol y la visibilidad del bot�n de registro seg�n el valor del interruptor.
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Argumentos del evento.</param>
    public void OnRoleSwitchToggled(object sender, ToggledEventArgs e)
    {
        roleLabel.Text = e.Value ? "Profesor" : "Alumno";
        registerButton.IsVisible = !e.Value;
    }

    /// <summary>
    /// M�todo invocado cuando se hace clic en el bot�n de registro.
    /// Navega a la p�gina de registro.
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Argumentos del evento.</param>
    public void OnRegisterClick(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Register());
    }

    /// <summary>
    /// M�todo invocado cuando se hace clic en el bot�n de inicio de sesi�n.
    /// Realiza la autenticaci�n y navega a la p�gina correspondiente (Week para alumnos, Student para profesores).
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Argumentos del evento.</param>
    public async void OnLoginClick(object sender, EventArgs e)
    {
        if (roleLabel.Text.Equals("Alumno"))
        {
            var firebaseAlumno = (await client.Child("Alumno")
                .OnceAsync<Alumno>())
                .FirstOrDefault(a => a.Object.Nombre == NombreEntry.Text);

            if (firebaseAlumno != null && firebaseAlumno.Object.Contrase�a == Encrypt.GetSha256(PasswordEntry.Text))
            {
                AppData.AlumnoActual = firebaseAlumno.Object;
                AppData.AlumnoActual.Id = firebaseAlumno.Key;
                await Navigation.PushAsync(new Week());
            }
            else
            {
                await this.DisplayAlert("Error", "Te has equivocado en el nombre de usuario o contrase�a", "Ok");
            }
        }
        else
        {
            var profesor = (await client.Child("Profesor")
                .OnceAsync<Profesor>())
                .FirstOrDefault(a => a.Object.Nombre == NombreEntry.Text)?.Object;
            if (profesor != null && profesor.Contrase�a == Encrypt.GetSha256(PasswordEntry.Text))
            {
                await Navigation.PushAsync(new Student());
            }
            else
            {
                await this.DisplayAlert("Error", "Te has equivocado en el nombre de usuario o contrase�a", "Ok");
            }
        }
    }
}