namespace UD4T4AlejandroMartinez.MVVM.Views;

public partial class Week : ContentPage
{
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
    private async void OnWeekClicked(int weekNumber)
    {
        await Navigation.PushAsync(new Day(weekNumber));
    }
}