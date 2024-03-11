namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Week : ContentPage
{
    /// <summary>
    /// Constructor de la clase Week. Inicializa los componentes de la página y crea botones para cada semana desde una fecha de inicio hasta una fecha de finalización.
    /// </summary>
    public Week()
	{
		InitializeComponent();
        var layout = (StackLayout)Content;

        var startDate = new DateTime(2023, 3, 18);
        var endDate = new DateTime(2023, 6, 17);
        var weeks = (endDate - startDate).Days / 7;

        for (int i = 1; i <= weeks; i++)
        {
            int weekNumber = i;
            var button = new Button { Text = $"Semana {weekNumber}", Margin = new Thickness(0, 10, 0, 0) };
            button.Clicked += (sender, e) => OnWeekClicked(weekNumber);
            layout.Children.Add(button);
        }

    }
    /// <summary>
    /// Método invocado cuando se hace clic en un botón de semana. Navega a la página Day correspondiente a la semana seleccionada.
    /// </summary>
    /// <param name="weekNumber">El número de la semana seleccionada.</param>
    private async void OnWeekClicked(int weekNumber)
    {
        await Navigation.PushAsync(new Day(weekNumber));
    }
}