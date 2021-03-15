using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddExam : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Classes _classes;

        public AddExam(Classes classes)
        {
            InitializeComponent();
            _classes = classes;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Exams>();
            var objExam =
                await _connection.QueryAsync<Exams>(
                    $"Select Type From 'Exam' Where 'Classes' = '{_classes.ClassID} And Type = 'Objective'");

            var perfExam =
                await _connection.QueryAsync<Exams>(
                    $"Select Type From 'Exam' Where 'Classes' = '{_classes.ClassID} And Type = 'Performance'");

            if (objExam.Count == 0)
            {
                ExamType.Items.Add("Objective");
            }

            if (perfExam.Count == 0)
            {
                ExamType.Items.Add("Performance");
            }

            if (objExam.Count == 0)
            {
                ExamType.Items.Remove("Objective");
            }

            if (perfExam.Count == 0)
            {
                ExamType.Items.Remove("Performance");
            }
            base.OnAppearing();
        }

        private async void ImageButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void AddExamButton(object sender, EventArgs e)
        {
            var exam = new Exams();
            exam.ExamName = ExamName.Text;
            exam.ExamStart = ExamStart.Date;
            exam.ExamEnd = ExamEnd.Date;
            exam.ExamClass = _classes.ClassID;
            exam.ExamType = (string)ExamType.SelectedItem;

            if (InputValidation.IsNull(ExamName.Text))
            {
                if (exam.ExamStart < exam.ExamEnd)
                {
                    await _connection.InsertAsync(exam);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must be before end date.", "Okay");
            }
            else await DisplayAlert("Error", "Fields cannot be left blank.", "Okay");
        }
    }
}