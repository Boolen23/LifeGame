using fxLife;
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
using System.Windows.Threading;

namespace FxLife
{
    /// <summary>
    /// Логика взаимодействия для FxLifeWindow.xaml
    /// </summary>
    public partial class FxLifeWindow : Window
    {
        public FxLifeWindow(Alignment alignment, int reservNum, StartWindow startW)
        {
            InitializeComponent();
            startWindow = startW;

            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            DBConn = new DBConnection();
            currAligment = alignment;
            InitializeCells(reservNum);
            InitializeGrid();
            LogLb.ItemsSource = DBConn.GetLogs();
            NewChoice = false;
            DBConn.Log(EnumToString(alignment), "Новая игра");
        }
        private StartWindow startWindow;
        private bool NewChoice;
        private void InitializeCells(int resNum)
        {
            _cells = new CellCollection(CellCount);
            switch (currAligment)
            {
                case Alignment.Auto:
                    {
                        _cells.AutoInitializeCells();
                        break;
                    }
                case Alignment.Manual:
                    {
                        mainGrid.MouseMove += MainGrid_MouseMove;
                        _cells.ManualInitializeCells();
                        break;
                    }
                case Alignment.Reserved:
                    {
                        _cells.LoadFromString(DBConn.ReservedData(resNum));
                        InitializeGrid();
                        break;
                    }
            }
        }
        DBConnection DBConn;
        Alignment currAligment;
        double CellSize = 16;
        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point cur = e.GetPosition(mainGrid);
                int numX = Convert.ToInt32(cur.X / CellSize);
                int numY = Convert.ToInt32(cur.Y / CellSize);
                _cells[numX, numY].IsAlive = true;
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_cells.EndUpdateLife())
            {
                _timer.IsEnabled = false;
                MessageBox.Show("Game over");
            }
        }

        void startButton_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.MouseMove -= MainGrid_MouseMove;
            _timer.Start();
        }

        void stopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
        private void InitializeGrid()
        {
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < CellCount; i++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                mainGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int row = 0; row < CellCount; row++)
            {
                for (int column = 0; column < CellCount; column++)
                {
                    Ellipse ellipse = new Ellipse();
                    Grid.SetColumn(ellipse, column);
                    Grid.SetRow(ellipse, row);
                    ellipse.DataContext = _cells[column, row];
                    mainGrid.Children.Add(ellipse);
                    ellipse.Style = Resources["lifeStyle"] as Style;
                }
            }
        }
        public static int CellCount = 50;
        CellCollection _cells;
        DispatcherTimer _timer = new DispatcherTimer();

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DBConn.SaveGame(_cells.SaveData);
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            NewChoice = true;
            startWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!NewChoice)
            {
                DBConn.Log(" ", "Выход из игры");
                DBConn.SaveGame(_cells.SaveData);
                startWindow.Close();
            }
            DBConn.Dispose();
        }
        private string EnumToString(Alignment alig)
        {
            switch (alig)
            {
                case Alignment.Manual: return "Ручная расстановка";
                case Alignment.Auto: return "Автоматическая расстановка";
                case Alignment.Reserved: return "Сохраненная игра";
            }
            throw new NullReferenceException();
        }
    }
    public enum Alignment { None, Auto, Manual, Reserved }
}
