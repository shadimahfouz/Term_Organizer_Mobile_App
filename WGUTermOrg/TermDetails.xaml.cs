using System;
using System.Collections.ObjectModel;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermDetails : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Classes> _classesList;
        private Terms _currentTerm;
        public TermDetails(Terms terms)
        {
            InitializeComponent();
            Title = terms.TermName;
            _currentTerm = terms;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
            classlistView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ClassesTapped);
        }

        protected override async void OnAppearing()
        {
            TermName.Text = _currentTerm.TermName;
            TermStart.Text = _currentTerm.TermStart.ToString("MM/dd/yyyy");
            TermEnd.Text = _currentTerm.TermEnd.ToString("MM/dd/yyyy");
            await _connection.CreateTableAsync<Classes>();
            var classlist = await _connection.QueryAsync<Classes>($"Select * From Class Where CTermID = '{_currentTerm.TermID}'");
            _classesList = new ObservableCollection<Classes>(classlist);
            classlistView.ItemsSource = _classesList;
            base.OnAppearing();
        }

        private async void ClassesTapped(object sender, ItemTappedEventArgs e) //Navigates to class detail page if class is tapped on screen
        {
            Classes classes = (Classes) e.Item;
            await Navigation.PushModalAsync(new ClassDetails(classes));
        }

        private async void AddClassButton(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddClass(_currentTerm));
        }

        private async void EditTermButton(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditTerm(_currentTerm));
        }

        private async void DropTermButton(object sender, EventArgs e)
        {
            var confirmDrop = await DisplayAlert("Alert", "Are you sure you want to drop this term?", "Yes", "No");
            if (confirmDrop)
            {
                await _connection.DeleteAsync(_currentTerm);
                await Navigation.PopModalAsync();
            }
        }
    }
}