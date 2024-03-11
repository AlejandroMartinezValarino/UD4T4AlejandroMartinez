using UD4T4AlejandroMartinez.MVVM.ViewModels;

namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Student : ContentPage
{
    /// <summary>
    /// Constructor de la clase Student. Inicializa los componentes de la página y establece el contexto de enlace con una instancia de AlumnoViewModel.
    /// </summary>
    public Student()
	{
		InitializeComponent();
		BindingContext = new AlumnoViewModel();
	}

}