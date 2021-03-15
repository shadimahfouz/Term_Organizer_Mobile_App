using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerm : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        private Terms _term;
        public EditTerm(Terms currentTerm)
        {
            InitializeComponent();
            _term = currentTerm;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Terms>();
            TermName.Text = _term.TermName;
            TermStart.Date = _term.TermStart;
            TermEnd.Date = _term.TermEnd;
            base.OnAppearing();
        }

        private async void SaveButton(object sender, EventArgs e)
        {
            _term.TermName = TermName.Text;
            _term.TermStart = TermStart.Date;
            _term.TermEnd = TermEnd.Date;

            if (InputValidation.IsNull(_term.TermName)) //Field entry validation before save
            {
                if (_term.TermStart < _term.TermEnd)
                {
                    await _connection.UpdateAsync(_term);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must be before end date.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank.", "Okay");
        }
    }
}