using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Plugin.LocalNotifications;
using SQLite;
using Xamarin.Forms;

namespace WGUTermOrg
{
    [Table("Term")]
    public class Terms
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

    }
    [Table("Class")]
    public class Classes
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ClassID { get; set; }
        public int CTermID { get; set; }
        public string ClassName { get; set; }
        public string ClassStatus { get; set; }
        public DateTime ClassStart { get; set; }
        public DateTime ClassEnd { get; set; }
        public string ClassProfName { get; set; }
        public string ClassProfNumber { get; set; }
        public string ClassProfEmail { get; set; }
        public string ClassNotes { get; set; }
        public int Notifs { get; set; }
    }
    [Table("Exam")]
    public class Exams
    {
        [PrimaryKey, AutoIncrement, Column("_id")] 
        public int ExamID { get; set; }
        public string ExamName { get; set; }
        public DateTime ExamStart { get; set; }
        public DateTime ExamEnd { get; set; }
        public string ExamType { get; set; }
        public int ExamClass { get; set; }
        public int Notifs { get; set; }
    }

    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public ObservableCollection<Terms> _termslist;
        private bool pushNotifs = true;
        public MainPage()
        {
            InitializeComponent();
            _connection = DependencyService.Get<IAppDatabase>().GetConnection();
            termslistView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ClickOnTerm);
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Terms>();
            await _connection.CreateTableAsync<Classes>();
            await _connection.CreateTableAsync<Exams>();

            var termslist = await _connection.Table<Terms>().ToListAsync();
            var classlist = await _connection.Table<Classes>().ToListAsync();
            var examlist = await _connection.Table<Exams>().ToListAsync();

            if (!termslist.Any())
            {
                var preLoadTerm = new Terms();
                preLoadTerm.TermName = "Term 1";
                preLoadTerm.TermStart = new DateTime(2021, 01, 01);
                preLoadTerm.TermEnd = new DateTime(2021, 06, 01);
                await _connection.InsertAsync(preLoadTerm);
                termslist.Add(preLoadTerm);

                var preLoadClass = new Classes();
                preLoadClass.ClassName = "Class 1";
                preLoadClass.ClassStart = new DateTime(2021, 01, 01);
                preLoadClass.ClassEnd = new DateTime(2021, 02, 15);
                preLoadClass.ClassStatus = "Enrolled";
                preLoadClass.ClassProfName = "Adam Smith";
                preLoadClass.ClassProfNumber = "555-555-5555";
                preLoadClass.ClassProfEmail = "asmith@wgu.edu";
                preLoadClass.Notifs = 1;
                preLoadClass.ClassNotes = "This class was a challenge.";
                preLoadClass.CTermID = preLoadTerm.TermID;
                await _connection.InsertAsync(preLoadClass);

                var preLoadOA = new Exams();
                preLoadOA.ExamName = "Exam 1";
                preLoadOA.ExamStart = new DateTime(2021, 02, 15);
                preLoadOA.ExamEnd = new DateTime(2021, 02, 15);
                preLoadOA.ExamClass = preLoadClass.ClassID;
                preLoadOA.ExamType = "Objective";
                preLoadOA.Notifs = 1;
                await _connection.InsertAsync(preLoadOA);

                var preLoadPA = new Exams();
                preLoadPA.ExamName = "Project 1";
                preLoadPA.ExamStart = new DateTime(2021, 02, 01);
                preLoadPA.ExamEnd = new DateTime(2021, 02, 15);
                preLoadPA.ExamClass = preLoadClass.ClassID;
                preLoadPA.ExamType = "Performance";
                preLoadPA.Notifs = 1;
                await _connection.InsertAsync(preLoadPA);
            }

            if (pushNotifs == true)
            {
                pushNotifs = false;
                int classID = 0;
                foreach (Classes Class in classlist)
                {
                    classID++;
                    if (Class.Notifs == 1)
                    {
                        if (Class.ClassStart == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Class Alert", $"{Class.ClassName} starts today.", classID);
                        
                        if (Class.ClassEnd == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Class Alert", $"{Class.ClassName} will end today.", classID);
                    }
                }

                int examID = classID;
                foreach (Exams exams in examlist)
                {
                    examID++;
                    if (exams.Notifs == 1)
                    {
                        if (exams.ExamStart == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Exam Alert", $"{exams.ExamName} is today.", examID);

                        if (exams.ExamEnd == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Exam Alert", $"{exams.ExamName} is due today", examID);
                    }
                }
            }

            _termslist = new ObservableCollection<Terms>(termslist);
            termslistView.ItemsSource = _termslist;
            base.OnAppearing();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }

        private async void ClickOnTerm(object sender, ItemTappedEventArgs e)
        {
            Terms terms = (Terms)e.Item;
            await Navigation.PushModalAsync(new TermDetails(terms));
        }
    }
}
