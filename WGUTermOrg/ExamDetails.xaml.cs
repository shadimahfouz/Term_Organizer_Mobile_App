using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExamDetails : ContentPage
    {

        private Exams _exams;
        private SQLiteAsyncConnection _connection;
        public ExamDetails(Exams exams)
        {
            InitializeComponent();
            _exams = exams;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected override void OnAppearing()
        {
            ExamName.Text = _exams.ExamName;
            ExamStart.Text = _exams.ExamStart.ToString("MM/dd/yyyy");
            ExamEnd.Text = _exams.ExamEnd.ToString("MM/dd/yyyy");
            ExamType.Text = _exams.ExamType;
            Notifs.Text = _exams.Notifs == 1 ? "Yes" : "No";
            base.OnAppearing();
        }

        private async void EditExamButton(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditExam(_exams));
        }

        private async void DropExamButton(object sender, EventArgs e)
        {
            var dropConfirm = await DisplayAlert("Confirm", "Are you sure you want to drop this class?", "Yes", "No");
            if (dropConfirm)
            {
                await _connection.DeleteAsync(_exams);
                await Navigation.PopModalAsync();
            }
        }
    }
}