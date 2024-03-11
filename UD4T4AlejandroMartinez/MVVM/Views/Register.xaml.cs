using Firebase.Auth.Providers;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using UD4T4AlejandroMartinez.MVVM.Models;
using System.IO;

namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Register : ContentPage
{
    /// <summary>
    /// Indica si se ha seleccionado una imagen.
    /// </summary>
    private bool imagenSeleccionada= false;

    /// <summary>
    /// Cliente para interactuar con la base de datos Firebase.
    /// </summary>
    private FirebaseClient client = new FirebaseClient("https://ud4t4-5f0c2-default-rtdb.europe-west1.firebasedatabase.app/");

    /// <summary>
    /// Ruta de almacenamiento en Firebase Storage.
    /// </summary>
    private string rutaStorage = "ud4t4-5f0c2.appspot.com";
    
    /// <summary>
    /// Nombre del nuevo archivo de imagen.
    /// </summary>
    private String newFile;

    /// <summary>
    /// Token de autenticación.
    /// </summary>
    private string token = String.Empty;

    /// <summary>
    /// Constructor de la clase Register.
    /// </summary>
    public Register()
	{
		InitializeComponent();
	}
    /// <summary>
    /// Método invocado cuando la página está a punto de mostrarse.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (string.IsNullOrEmpty(token))
        {
            await ObtenerToken();
        }
    }
    /// <summary>
    /// Maneja el evento de clic en el botón para elegir una foto.
    /// </summary>
    public async void OnChoosePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Por favor elige una foto"
            });

            if (result != null)
            {
                newFile = Path.Combine(FileSystem.CacheDirectory, result.FileName);
                using (var stream = await result.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);
                profileImage.Source = ImageSource.FromFile(newFile);
                imagenSeleccionada = true;
            }
        }catch(Exception ex)
        {
            await this.DisplayAlert("Error", ex.Message, "Ok");
        }
    }
    /// <summary>
    /// Maneja el evento de clic en el botón de registro.
    /// Verifica que ninguno de los campos estén vacios, que la contraseña y verificación correspondan y crea el nuevo alumno en Firebase.
    /// Luego te lleva a la página de login.
    /// </summary>
    public async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {

            if (!string.IsNullOrEmpty(nombreEntry.Text) &&
                !string.IsNullOrEmpty(contraseñaEntry.Text) &&
                !string.IsNullOrEmpty(reescribeContraseñaEntry.Text) &&
                !string.IsNullOrEmpty(centroDocenteEntry.Text) &&
                !string.IsNullOrEmpty(profesorSeguimientoEntry.Text) &&
                !string.IsNullOrEmpty(centroTrabajoEntry.Text) &&
                !string.IsNullOrEmpty(tutorTrabajoEntry.Text) &&
                !string.IsNullOrEmpty(cicloFormativoEntry.Text) &&
                !string.IsNullOrEmpty(gradoEntry.Text) &&
                imagenSeleccionada)
            {
                if (contraseñaEntry.Text.Equals(reescribeContraseñaEntry.Text))
                {
                    using (var uploadStream = File.OpenRead(newFile))
                    {
                        var storageImageTask = new FirebaseStorage(rutaStorage, new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(token),
                            ThrowOnCancel = true
                        })
                        .Child("fotos-perfil")
                        .Child(nombreEntry.Text)
                        .PutAsync(uploadStream);

                        var rutaImage = await storageImageTask;

                        await client.Child("Alumno").PostAsync(new Alumno
                        {
                            Nombre = nombreEntry.Text,
                            Contraseña = Encrypt.GetSha256(contraseñaEntry.Text),
                            CentroDocente = centroDocenteEntry.Text,
                            ProfesorSeguimiento = profesorSeguimientoEntry.Text,
                            CentroTrabajo = centroTrabajoEntry.Text,
                            TutorTrabajo = tutorTrabajoEntry.Text,
                            CicloFormativo = cicloFormativoEntry.Text,
                            Grado = gradoEntry.Text,
                            FotoPath = rutaImage
                        });
                        await Navigation.PushAsync(new Login());
                }
                }
                else
                {
                    await this.DisplayAlert("Error", "Te has equivocado al reescribir la contraseña", "Ok");
                }
            }
            else
            {
                await this.DisplayAlert("Error", "Rellene todos los campos por favor", "Ok");
            }
        }catch (Exception ex)
        {
            await this.DisplayAlert("Error", ex.Message, "Ok");
        }
    }
    /// <summary>
    /// Obtiene el token de autenticación.
    /// </summary>
    public async Task ObtenerToken()
    {
        try
        {
            string authDomain = "ud4t4-5f0c2.firebaseapp.com";
            string apiKey = "AIzaSyDOGKyXLXGbj4GqDI7I-BxXLpiSQNsa4IU";
            string email = "user@gmail.com";
            string password = "123456";

            var client = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = apiKey,
                AuthDomain = authDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
            var credenciales = await client.SignInWithEmailAndPasswordAsync(email, password);
            token = await credenciales.User.GetIdTokenAsync();
        }catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}