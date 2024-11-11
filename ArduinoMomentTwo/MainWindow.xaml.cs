using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArduinoMomentTwo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort currentPort;
        System.Timers.Timer aTimer;
        private delegate void updateDelegate(string txt);
        bool ArduinoPortFound = false;
        List<string> strings = new List<String>() { "0.1 sec", "0.5 sec", "1 sec" };
        public MainWindow()
        {
            InitializeComponent();
            Period.ItemsSource = strings;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 9600);
                    if (ArduinoDetected())
                    {
                        ArduinoPortFound = true;
                        break;
                    }
                    else
                    {
                        ArduinoPortFound = false;
                    }
                }
            }
            catch { }

            if (!ArduinoPortFound) return;
            System.Threading.Thread.Sleep(500);

            currentPort.BaudRate = 9600;
            currentPort.DtrEnable = true;
            currentPort.ReadTimeout = 1000;
            try
            {
                currentPort.Open();
            }
            catch { }

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimeEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            
        }

        public bool ArduinoDetected()
        {
            try
            {
                currentPort.Open();
                System.Threading.Thread.Sleep(1000);
                string returnMessage = currentPort.ReadLine();
                currentPort.Close();

                if (returnMessage.Contains("Arduino"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }

        }

        public void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
            try
            {
                currentPort.DiscardInBuffer();
                string strFromPort = currentPort.ReadLine();
                if (strFromPort.Contains("2"))
                {
                    main_lable.Dispatcher.BeginInvoke(new updateDelegate(updateTextBox), "Нажата кнопка 1");
                }
                if (strFromPort.Contains("3"))
                {
                    main_lable.Dispatcher.BeginInvoke(new updateDelegate(updateTextBox), "Нажата кнопка 2");
                }

            }
            catch { }
        }

        private void updateTextBox(string txt)
        {
            main_lable.Text = txt;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            aTimer.Enabled = false;
            currentPort.Close();
        }

        private void First_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
            switch ((string)Period.SelectedItem)
            {
                case "0.1 sec":
                    currentPort.Write("5");
                    break;
                case "0.5 sec":
                    currentPort.Write("6");
                    break;
                case "1 sec":
                    currentPort.Write("7");
                    break;
                default:
                    return;
            }
        }

        private void Second_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
            currentPort.Write("4");
        }
    }
}
