using System;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassDetails : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Classes _currentClasses;
        public ClassDetails(Classes classes)
        {
            InitializeComponent();
            _currentClasses = classes;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected override void OnAppearing()
        {
            ClassName.Text = _currentClasses.ClassName;
            ClassStatus.Text = _currentClasses.ClassStatus;
            ClassStart.Text = _currentClasses.ClassStart.ToString("MM/dd/yyyy");
            ClassEnd.Text = _currentClasses.ClassEnd.ToString("MM/dd/yyyy");
            ProfessorName.Text = _currentClasses.ClassProfName;
            ProfessorNumber.Text = _currentClasses.ClassProfNumber;
            ProfessorEmail.Text = _currentClasses.ClassProfEmail;
            ClassNotes.Text = _currentClasses.ClassNotes;
            Notifs.Text = _currentClasses.Notifs == 1 ? "Yes" : "No";
            base.OnAppearing();
        }

        private async void EditClassButton(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditClass(_currentClasses));
        }

        private async void DropClassButton(object sender, EventArgs e)
        {
            var dropConfirm = await DisplayAlert("Alert", "Are you sure you want to drop this class?", "Yes", "No"); //Confirms deletion of class
            if (dropConfirm)
            {
                await _connection.DeleteAsync(_currentClasses);
                await Navigation.PopModalAsync();
            }
        }

        private async void ExamButton(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ExamPage(_currentClasses));
        }

        private async void ShareNotesButton(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest //Handles note sharing
            {
                Text = ClassNotes.Text,
                Title = "Share class notes."
            });
        }
    }
}