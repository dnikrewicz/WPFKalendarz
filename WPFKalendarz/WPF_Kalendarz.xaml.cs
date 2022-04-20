using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;


namespace WPFKalendarz
{
    /// <summary>
    /// Interaction logic for WPF_Kalendarz.xaml
    /// </summary>
    public partial class WPF_Kalendarz : Window
    {
        public WPF_Kalendarz()
        {
            InitializeComponent();

            KalendarzEntities db = new KalendarzEntities();
            var docs = from d in db.Kalendarzs
                       select new
                       {
                           Monday = d.Monday,
                           Tuesday= d.Tuesday,
                           Wednesday= d.Wednesday,
                           Thursday= d.Thursday,
                           Friday= d.Friday,
                           Saturday= d.Saturday,
                           Sunday= d.Sunday

                       };
            foreach (var item in docs)
            {
                Console.WriteLine(item.Monday);
                Console.WriteLine(item.Tuesday);
                Console.WriteLine(item.Wednesday);
                Console.WriteLine(item.Thursday);
                Console.WriteLine(item.Friday);
                Console.WriteLine(item.Saturday);
                Console.WriteLine(item.Sunday);
            }
            this.gridKalendarz.ItemsSource = docs.ToList();
        }
        ///<summary>
        ///Dodanie taska do bazy danych
        ///</summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            KalendarzEntities db = new KalendarzEntities();

            Kalendarz kalendarzObject = new Kalendarz()
            {
                Monday = txtMonday.Text,
                Tuesday = txtTuesday.Text,
                Wednesday = txtWednesday.Text,
                Thursday = txtThursday.Text,
                Friday = txtFriday.Text,
                Saturday= txtSaturday.Text,
                Sunday= txtSunday.Text
            };
            db.Kalendarzs.Add(kalendarzObject);
            db.SaveChanges();
        }
        ///<summary>
        ///Wyswietla w aplikacji aktualne dane z bazy danych
        ///</summary>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            KalendarzEntities db = new KalendarzEntities();
            this.gridKalendarz.ItemsSource = db.Kalendarzs.ToList();
        }
        private int updatingKalendarzID = 0;
        private void gridKalendarz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.gridKalendarz.SelectedIndex >= 0)
            {
                if(this.gridKalendarz.SelectedItems.Count >= 0)
                {
                    if(this.gridKalendarz.SelectedItems[0].GetType() == typeof(Kalendarz))
                    {
                        Kalendarz d = (Kalendarz)this.gridKalendarz.SelectedItems[0];
                        this.txtMonday.Text = d.Monday;
                        this.txtTuesday.Text = d.Tuesday;
                        this.txtWednesday.Text = d.Wednesday;
                        this.txtThursday.Text = d.Thursday;
                        this.txtFriday.Text = d.Friday;
                        this.txtSaturday.Text = d.Saturday;
                        this.txtSunday.Text = d.Sunday;
                        this.updatingKalendarzID = d.Id;

                    }
                }
            }
        }
        ///<summary>
        ///Zaktualizowanie bazy danych
        ///</summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            KalendarzEntities db = new KalendarzEntities();

            var r = from d in db.Kalendarzs
                    where d.Id == this.updatingKalendarzID
                    select d;
            
            Kalendarz obj = r.SingleOrDefault();

            if (obj != null)
            {
                obj.Monday = this.txtMonday.Text;
                obj.Tuesday = this.txtTuesday.Text;
                obj.Wednesday = this.txtWednesday.Text;
                obj.Thursday = this.txtThursday.Text;
                obj.Friday = this.txtFriday.Text;
                obj.Saturday = this.txtSaturday.Text;
                obj.Sunday = this.txtSunday.Text;
            }
            db.SaveChanges();
        }


        ///<summary>
        ///Usuniecie danego elementu z bazy danych
        ///</summary>
        private void btnDelate_Click(object sender, RoutedEventArgs e)
        {
        MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to Delate?",
        "Delete task",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning,
        MessageBoxResult.No);

        if (messageBoxResult == MessageBoxResult.Yes)
        {
        KalendarzEntities db = new KalendarzEntities();
        var r = from d in db.Kalendarzs
              where d.Id == this.updatingKalendarzID
              select d;
        Kalendarz obj = r.SingleOrDefault();

        if (obj != null)
        {
          db.Kalendarzs.Remove(obj);
          db.SaveChanges();
        }
}







}
        string APIkey = "4d1851d33900614779731ee3dd48aa68";
        ///<summary>
        ///Przycisk odpowiedzialny za szukanie danej miejscowosci
        ///</summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            getWeather();
        }
        ///<summary>
        ///Sprawdza pogode w wybranej miejscowosci jezeli zostala dobrze wpisana
        ///</summary>
        void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", txtCity.Text, APIkey);
                try
                {
                    var json = web.DownloadString(url);
                    Kalendarz.root Info = JsonConvert.DeserializeObject<Kalendarz.root>(json);
                    double temp = Info.main.temp;
                    temp = temp - 273.15;
                    temp = Math.Round(temp, 2);
                    txtTemp.Text = temp.ToString() + " °C";
                }
                catch { txtTemp.Text = "Write City correctly"; }

            }
        }
    }
}
