using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditClass : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        private Classes _currentClass;
        public EditClass(Classes currentClass)
        {
            InitializeComponent();
            _currentClass = currentClass;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected async override void OnAppearing()
        {
            await _connection.CreateTableAsync<Classes>();

            ClassName.Text = _currentClass.ClassName;
            ClassStatus.SelectedItem = _currentClass.ClassStatus;
            ClassStart.Date = _currentClass.ClassStart;
            ClassEnd.Date = _currentClass.ClassEnd;
            ProfessorName.Text = _currentClass.ClassProfName;
            ProfessorPhone.Text = _currentClass.ClassProfNumber;
            ProfessorEmail.Text = _currentClass.ClassProfEmail;
            ClassNotes.Text = _currentClass.ClassNotes;
            if (_currentClass.Notifs == 1)
            {
                Notifs.On = true;
            }
            base.OnAppearing();
        }

        private async void SaveButton(object sender, EventArgs e)
        {
            _currentClass.ClassName = ClassName.Text;
            _currentClass.ClassStatus = (string)ClassStatus.SelectedItem;
            _currentClass.ClassStart = ClassStart.Date;
            _currentClass.ClassEnd = ClassEnd.Date;
            _currentClass.ClassProfName = ProfessorName.Text;
            _currentClass.ClassProfNumber = ProfessorPhone.Text;
            _currentClass.ClassProfEmail = ProfessorEmail.Text;
            _currentClass.ClassNotes = ClassNotes.Text;
            _currentClass.Notifs = Notifs.On == true ? 1 : 0;

            if (InputValidation.IsNull(ClassName.Text) && InputValidation.IsNull(ProfessorName.Text) &&
                InputValidation.IsNull(ProfessorPhone.Text))
            {
                if (InputValidation.CorrectEmail(ProfessorEmail.Text))
                {
                    if (_currentClass.ClassStart < _currentClass.ClassEnd)
                    {
                        await _connection.UpdateAsync(_currentClass);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error", "Start date must be before end date", "Okay");
                }
                else await DisplayAlert("Error", "Email address is invalid.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank", "Okay");
        }
    }
}