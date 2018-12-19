using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FxLife
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window, INotifyPropertyChanged
    {
        public StartWindow()
        {
            InitializeComponent();
            DataContext = this;
            GetAlignment = Alignment.None;
            dBConn = new DBConnection();
            DelButton.Visibility = Visibility.Hidden;
            if (dBConn.GetReservedGame().Count() > 0)
                HaveReservedGame = true;
        }

        private int ReservNum = 1;
        private Alignment alignment;
        private Alignment GetAlignment
        {
            get => alignment;
            set
            {
                alignment = value;
                if (alignment == Alignment.None) ChoiceButton.IsEnabled = false;
                else ChoiceButton.IsEnabled = true;
            }
        }
        public bool HaveReservedGame
        {
            get => _haveReserv;
            set
            {
                _haveReserv = value;
                ResetCheckBoxes();
                OnPropertyChanged("HaveReservedGame");
            }
        }
        FxLifeWindow lifeWindow;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lifeWindow != null)
            {
                lifeWindow.Close();
            }
            lifeWindow = new FxLifeWindow(GetAlignment, ReservNum, this);
            lifeWindow.Show();
            this.Hide();
        }
        private DBConnection dBConn;
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ResetCheckBoxes();
            CheckBox cb = (CheckBox)sender;
            cb.IsChecked = true;
            if ((sender as CheckBox).Name == "AutoCheck")
            {
                alignment = Alignment.Auto;
            }
            else if ((sender as CheckBox).Name == "ManualChek")
            {
                alignment = Alignment.Manual;
            }
            else if ((sender as CheckBox).Name == "RnResCheck")
            {
                alignment = Alignment.Reserved;
                var source = dBConn.GetReservedGame();
                int[] temp = dBConn.GetAllIdReservedGames();
                ReservNum = temp[new Random().Next(0, temp.Length)];

            }
            else if ((sender as CheckBox).Name == "ResCheck")
            {
                reservLb.ItemsSource = dBConn.GetReservedGame();
            }
            ChoiceButton.IsEnabled = true;
        }
        private void ResetCheckBoxes()
        {
            alignment = Alignment.None;
            foreach (var i in grid.Children.OfType<CheckBox>())
                i.IsChecked = false;
            reservLb.ItemsSource = null;
        }

        private void reservLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (reservLb.SelectedIndex > -1)
            {
                alignment = Alignment.Reserved;
                ReservNum = IdByString(reservLb.SelectedItem.ToString());
                DelButton.Visibility = Visibility.Visible;
            }
            else DelButton.Visibility = Visibility.Hidden;
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            IdByString(reservLb.SelectedItem.ToString());
            if (reservLb.SelectedIndex > -1)
                dBConn.RemoveEntery(IdByString(reservLb.SelectedItem.ToString()));
            var source = dBConn.GetReservedGame();
            if (source.Count() == 0)
            {
                ChoiceButton.IsEnabled = false;
                HaveReservedGame = false;
            }
            reservLb.ItemsSource = source;
        }
        private int IdByString(string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(text.Reverse().ToArray().TakeWhile(x => x != '№').Reverse().ToArray());
            return Convert.ToInt32(sb.ToString());
        }
        private bool _haveReserv;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (dBConn.GetReservedGame().Count() > 0) 
                HaveReservedGame = true;
            else HaveReservedGame = false;
                ChoiceButton.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dBConn.Dispose();
        }
    }
}
