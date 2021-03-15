using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditExam : ContentPage
    {

        private Exams _exams;
        private SQLiteAsyncConnection _connection;
        public EditExam(Exams exams)
        {
            InitializeComponent();
            _exams = exams;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected async override void OnAppearing()
        {
            await _connection.CreateTableAsync<Exams>();

            ExamName.Text = _exams.ExamName;
            ExamStart.Date = _exams.ExamStart;
            ExamEnd.Date = _exams.ExamEnd;

            if (_exams.Notifs == 1) //Handles exam notifications
            {
                Notifs.On = true;
            }
            base.OnAppearing();
        }

        private async void SaveButton(object sender, EventArgs e)
        {
            _exams.ExamName = ExamName.Text;
            _exams.ExamStart = ExamStart.Date;
            _exams.ExamEnd = ExamEnd.Date;
            _exams.Notifs = Notifs.On == true ? 1 : 0;

            if (InputValidation.IsNull(ExamName.Text)) //Field entry validation before save
            {
                if (_exams.ExamStart < _exams.ExamEnd)
                {
                    await _connection.UpdateAsync(_exams);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must be before end date.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank.", "Okay");
        }
    }
}