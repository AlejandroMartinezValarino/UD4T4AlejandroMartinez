using UD4T4AlejandroMartinez.MVVM.ViewModels;

namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Student : ContentPage
{
	public Student()
	{
		InitializeComponent();
		BindingContext = new AlumnoViewModel();
	}

}