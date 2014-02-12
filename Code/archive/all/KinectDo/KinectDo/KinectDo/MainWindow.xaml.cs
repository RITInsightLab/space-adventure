using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KinectDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor = (from sensorToCheck in KinectSensor.KinectSensors
                               where sensorToCheck.Status == KinectStatus.Connected
                               select sensorToCheck).FirstOrDefault();



KinectAudioSource _kinectSource;
SpeechRecognitionEngine _speechEngine;
Stream _stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("This is what we have: " + SpeechRecognitionEngine.InstalledRecognizers().Count);
            listInstalledRecognizers.ItemsSource = SpeechRecognitionEngine.InstalledRecognizers();
            if (listInstalledRecognizers.Items.Count > 0)
                listInstalledRecognizers.SelectedItem = listInstalledRecognizers.Items[0];
        }

void BuildSpeechEngine(RecognizerInfo rec)
{
    _speechEngine = new SpeechRecognitionEngine(rec.Id);

    var choices = new Choices();
    choices.Add("venus");
    choices.Add("mars");
    choices.Add("earth");
    choices.Add("jupiter");
    choices.Add("sun");

    var gb = new GrammarBuilder { Culture = rec.Culture };
    gb.Append(choices);

    var g = new Grammar(gb);

    _speechEngine.LoadGrammar(g);
    //recognized a word or words that may be a component of multiple complete phrases in a grammar.
    _speechEngine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechEngineSpeechHypothesized);
    //receives input that matches any of its loaded and enabled Grammar objects.
    _speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_speechEngineSpeechRecognized);
    //receives input that does not match any of its loaded and enabled Grammar objects.
    _speechEngine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(_speechEngineSpeechRecognitionRejected);


    //C# threads are MTA by default and calling RecognizeAsync in the same thread will cause an COM exception.
    var t = new Thread(StartAudioStream);
    t.Start();
}

void StartAudioStream()
{
    sensor.Start();

    //Console.WriteLine("TomL: " + sensor.IsRunning);
    _kinectSource = sensor.AudioSource;
        _kinectSource.AutomaticGainControlEnabled = true;
        _kinectSource.EchoCancellationMode = EchoCancellationMode.None;
        _kinectSource.BeamAngleMode = BeamAngleMode.Adaptive;
    
    Console.WriteLine(" lakjsdflajsdlfjla " + _kinectSource.ToString());

    _stream = _kinectSource.Start();
    SpeechAudioFormatInfo bleh = new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null) ;


    _speechEngine.SetInputToAudioStream(_kinectSource.Start(), bleh);


    _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
}


        void _speechEngineSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.Write("\rSpeech Rejected: \t{0} \n", e.Result.Text);
        }

        void _speechEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String word = e.Result.Text;

            Process[] processes = Process.GetProcessesByName("celestia");

            Console.WriteLine(processes.Length + " IS THE LENGTH OF THE PROCESSES LIST");

            Process celestia = processes[0];

            if(word.Equals("venus")){
                SendKeys.SendWait("J");
                Console.WriteLine("DO THIS");
            }

            txtLastWord.Text = e.Result.Text;
        }

        void SpeechEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            txtList.Text = string.Format("{0} - Confidence={1}\n{2}", e.Result.Text, e.Result.Confidence, txtList.Text);
        }

private void BtnStartClick(object sender, RoutedEventArgs e)
{
    if (listInstalledRecognizers.SelectedItem == null) return;
    var rec = (RecognizerInfo)listInstalledRecognizers.SelectedItem;

    DisableUI();

    BuildSpeechEngine(rec);
}

        void DisableUI()
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            listInstalledRecognizers.IsEnabled = false;
        }

        void ActivateUI()
        {
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            listInstalledRecognizers.IsEnabled = true;
        }

        private void BtnStopClick(object sender, RoutedEventArgs e)
        {
            _kinectSource.Stop();
            _speechEngine.RecognizeAsyncStop();

            ActivateUI();

        }
    }
}
