using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddClass : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Terms _terms;
        public AddClass(Terms terms)
        {
            InitializeComponent();
            _terms = terms;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        private async void AddClassButton(object sender, EventArgs e)
        {
            var classes = new Classes();
            classes.ClassName = ClassName.Text;
            classes.ClassStart = ClassStart.Date;
            classes.ClassEnd = ClassEnd.Date;
            classes.ClassStatus = (string)ClassStatus.SelectedItem;
            classes.ClassProfName = ProfessorName.Text;
            classes.ClassProfNumber = ProfessorNumber.Text;
            classes.ClassProfEmail = ProfessorEmail.Text;
            classes.Notifs = Notifs.On == true ? 1 : 0;
            classes.ClassNotes = ClassNotes.Text;
            classes.CTermID = _terms.TermID;

            if (InputValidation.IsNull(ClassName.Text) && InputValidation.IsNull(ProfessorName.Text) && //This validates entry fields before save
                InputValidation.IsNull(ProfessorNumber.Text))
            {
                if (InputValidation.CorrectEmail(ProfessorEmail.Text)) //This specifically validates email entry before save
                {
                    if (classes.ClassStart < classes.ClassEnd)
                    {
                        await _connection.InsertAsync(classes);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error", "Start date must be before end date.", "Okay");
                }
                else await DisplayAlert("Error", "Email is invalid.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank.", "Okay");

        }
    }
}