using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {

        public MainPage _MainPage;
        private SQLiteAsyncConnection _connection;
        public AddTerm(MainPage mainPage)
        {
            InitializeComponent();
            _MainPage = mainPage;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        private async void AddTermButton(object sender, EventArgs e)
        {
            var terms = new Terms();
            terms.TermName = TermName.Text;
            terms.TermStart = TermStart.Date;
            terms.TermEnd = TermEnd.Date;

            if (InputValidation.IsNull(terms.TermName))
            {
                if (terms.TermStart < terms.TermEnd)
                {
                    await _connection.InsertAsync(terms);
                    _MainPage._termslist.Add(terms);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must be before end date.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank.", "Okay");
        }
    }
}