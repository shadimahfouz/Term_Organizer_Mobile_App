using System;
using System.Collections.ObjectModel;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUTermOrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExamPage : ContentPage
    {

        private Classes _currentClass;
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Exams> _examList;
        public ExamPage(Classes currentClass)
        {
            InitializeComponent();
            ClassName.Text = currentClass.ClassName;
            _currentClass = currentClass;
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
            examlistView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ExamsTapped);
        }

        protected override async void OnAppearing()
        {
            ClassName.Text = _currentClass.ClassName;
            await _connection.CreateTableAsync<Exams>();
            var examlist =
                await _connection.QueryAsync<Exams>(
                    $"Select * From Exam Where ExamClass = '{_currentClass.ClassID}'");
            _examList = new ObservableCollection<Exams>(examlist);
            examlistView.ItemsSource = _examList;
            base.OnAppearing();
        }

        private async void AddExamButton(object sender, EventArgs e)
        {
            await _connection.CreateTableAsync<Exams>();
            var numExams =
                await _connection.QueryAsync<Exams>(
                    $"Select ExamType From Exam Where ExamClass = '{_currentClass.ClassID}'");
            if (numExams.Count == 2)
            {
                await DisplayAlert("Error", "You cannot add more than two exams, remove an exam and try again.", //Prevents users from adding more than one exam
                    "Okay");
            }

            else await Navigation.PushModalAsync(new AddExam(_currentClass));
        }

        private async void ExamsTapped(object sender, ItemTappedEventArgs e) //Brings up exam details for exam that was tapped
        {
            Exams exam = (Exams) e.Item;
            await Navigation.PushModalAsync(new ExamDetails(exam));
        }
    }
}