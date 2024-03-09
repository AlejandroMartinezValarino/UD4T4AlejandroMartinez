using Firebase.Database;
using Firebase.Database.Query;
using UD4T4AlejandroMartinez.MVVM.Models;

namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Login : ContentPage
{
    private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");
    public Login()
	{
		InitializeComponent();
	}

    public void OnRoleSwitchToggled(object sender, ToggledEventArgs e)
    {
        roleLabel.Text = e.Value ? "Profesor" : "Alumno";
        registerButton.IsVisible = !e.Value;
    }
    public void OnRegisterClick(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Register());
    }
    public async void OnLoginClick(object sender, EventArgs e)
    {
        if (roleLabel.Text.Equals("Alumno"))
        {
            var firebaseAlumno = (await client.Child("Alumno")
                .OnceAsync<Alumno>())
                .FirstOrDefault(a => a.Object.Nombre == NombreEntry.Text);

            if (firebaseAlumno != null && firebaseAlumno.Object.Contraseña == Encrypt.GetSha256(PasswordEntry.Text))
            {
                AppData.AlumnoActual = firebaseAlumno.Object;
                AppData.AlumnoActual.Id = firebaseAlumno.Key;
                await Navigation.PushAsync(new Week());
            }
            else
            {
                await this.DisplayAlert("Error", "Te has equivocado en el nombre de usuario o contraseña", "Ok");
            }
        }
        else
        {
            var profesor = (await client.Child("Profesor")
                .OnceAsync<Profesor>())
                .FirstOrDefault(a => a.Object.Nombre == NombreEntry.Text)?.Object;
            if (profesor != null && profesor.Contraseña == Encrypt.GetSha256(PasswordEntry.Text))
            {
                await Navigation.PushAsync(new Student());
            }
            else
            {
                await this.DisplayAlert("Error", "Te has equivocado en el nombre de usuario o contraseña", "Ok");
            }
        }
    }
}