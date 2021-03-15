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
            //Selects exams based on type and id relative to class id
            await _connection.CreateTableAsync<Exams>();
            var objExam =
                await _connection.QueryAsync<Exams>($"Select ExamType From Exam Where ExamClass = '{_classes.ClassID}' And ExamType = 'Objective'");

            var perfExam =
                await _connection.QueryAsync<Exams>($"Select ExamType From Exam Where ExamClass = '{_classes.ClassID}' And ExamType = 'Performance'");

            //This removes either exam selection type if one already exists
            if (objExam.Count == 0)
            {
                ExamType.Items.Add("Objective");
            }

            if (perfExam.Count == 0)
            {
                ExamType.Items.Add("Performance");
            }

            if (objExam.Count == 1)
            {
                ExamType.Items.Remove("Objective");
            }

            if (perfExam.Count == 1)
            {
                ExamType.Items.Remove("Performance");
            }
            base.OnAppearing();
        }

        private async void AddExamButton(object sender, EventArgs e)
        {
            var exam = new Exams();
            exam.ExamName = ExamName.Text;
            exam.ExamStart = ExamStart.Date;
            exam.ExamEnd = ExamEnd.Date;
            exam.ExamClass = _classes.ClassID;
            exam.ExamType = (string)ExamType.SelectedItem;

            if (InputValidation.IsNull(ExamName.Text)) //This validates entry fields before allowing save
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