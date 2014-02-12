using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceAdventure3.command;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using Microsoft.Kinect;


namespace SpaceAdventure3
{

    /**
     * This class is responsible for all speech recognition and microphones
     * 
     * Author: Ross Kahn
     **/
    class SpeechManager 
    {

        private RecognizerInfo recognizerInfo;
        private SpeechRecognitionEngine speechRecognitionEngine;

        private bool sensorActive;
        private KinectSensor sensor;        // For use ONLY if Kinect is being used as the microphone
        
        // For use with Travel Mode
        private Dictionary<string, Constants.PLANET> speechBindings;

        // For system actions, such as enabling Travel Mode
        private Dictionary<string, Constants.ACTION> actionBindings;

        public SpeechManager()
        {
            // No microphones have been initialized yet
            sensorActive = false;

            // If there's a Kinect available, set the recognizerInfo to the one it provides
            this.recognizerInfo = GetKinectRecognizer();
            if (this.recognizerInfo != null)
            {
                this.speechRecognitionEngine = new SpeechRecognitionEngine(this.recognizerInfo.Id);
                Console.WriteLine("Speech Recognition Engine set to Kinect Recognizer");
            }

            // Otherwise, use a new recognition engine for use with a default microhpone
            else
            {
                this.recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault();
                this.speechRecognitionEngine = new SpeechRecognitionEngine();
                Console.WriteLine("Speech Recognition Engine set to default microphone");
            }

            this.speechRecognitionEngine.SpeechRecognized += this.speechRecognized;
            this.speechRecognitionEngine.SpeechDetected += this.speechDetected;
        }

        /**
         * Initialize using Kinect built-in microphone array
         **/
        public void Initialize(KinectSensor newSensor)
        {
            SetSensor(newSensor);
            SetSpeechGrammar();
        }

        /**
         * Initialize with default microphone
         **/
        public void Initialize()
        {
            setSensor();
            SetSpeechGrammar();
        }

        /**
         * Unfortunately, the kinect needs special configuration to be used
         * for speech recognition alongside motion recognition. There may be 
         * ways to decouple this with a slightly different architecture
         * 
         * CAUTION: The exact ordering of this method took a VERY long time
         * to figure out. I still don't understand exactly what all of it does.
         * DO NOT MAKE CHANGES TO IT WITHOUT SAVING A BACKUP.
         * -Ross
         **/
        private bool SetSensor(KinectSensor newSensor)
        {
            if (this.speechRecognitionEngine == null)
            {
                return false;
            }

            if (this.sensor != null)
            {
                this.sensorActive = false;
                this.speechRecognitionEngine.RecognizeAsyncStop();
            }

            this.sensor = newSensor;
            if (this.sensor != null)
            {
                this.sensor.AudioSource.NoiseSuppression = true;
                this.sensor.AudioSource.AutomaticGainControlEnabled = false;
                this.sensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;

                System.IO.Stream s = this.sensor.AudioSource.Start();

                // Set the Kinect AudioSource stream as the audio stream for the speech recognition engine.
                this.speechRecognitionEngine.SetInputToAudioStream(
                    s,
                                                                   new SpeechAudioFormatInfo(
                                                                       EncodingFormat.Pcm,
                                                                       16000,
                                                                       16,
                                                                       1,
                                                                       32000,
                                                                       2,
                                                                       null));
                this.sensorActive = true;

                // Begin speech recognition.
                if (this.speechRecognitionEngine.Grammars.Count > 0)
                {
                    this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
                Console.WriteLine("Kinect configured successfully with audio stream");
                return true;
            }
            return false;
        }

        /**
         * Microphone configuration with default system microphone. Compared
         * to the Kinect microphone array, very straight-forward
         **/
        private void setSensor()
        {
            try
            {
                this.speechRecognitionEngine.SetInputToDefaultAudioDevice();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to set audio sensor to the computer's default audio device. Speech recognition is disabled.");
                return;
            }
            sensorActive = true;
            Console.WriteLine("Sensor configured successfully to default audio device");
        }

        /**
         * Load into the recognizer all of the words that should be recognized
         * by the system
         * 
         * EXTENSIBILITY: To add new words, follow these steps:
         * 1. If you want a new planet to go to, first follow all the steps in 
         *    extensibility section of TravelCommand.cs. If you want a new system
         *    action, first follow all the steps in ActionCommand.cs
         * 2. For traveling, add your newly created string const and PLANET enum (made in step 1)
         *    to speechBindings (below). For an action, make sure you have a lowercase
         *    string constant (see STR_TRAVELTO in Constants.cs for an example) in
         *    Constants.cs. Then add that string and ACTION enum (made in step 1)
         *    to actionBindings (below). 
         * 3. That's it! Assuming you did step 1 correctly
         **/
        private void SetSpeechGrammar()
        {
            speechBindings = new Dictionary<string, Constants.PLANET>();
            speechBindings.Add(Constants.STR_SUN, Constants.PLANET.SUN);
            speechBindings.Add(Constants.STR_MERCURY, Constants.PLANET.MERCURY);
            speechBindings.Add(Constants.STR_VENUS, Constants.PLANET.VENUS);
            speechBindings.Add(Constants.STR_EARTH, Constants.PLANET.EARTH);
            speechBindings.Add(Constants.STR_MARS, Constants.PLANET.MARS);
            speechBindings.Add(Constants.STR_JUPITER, Constants.PLANET.JUPITER);
            speechBindings.Add(Constants.STR_SATURN, Constants.PLANET.SATURN);
            speechBindings.Add(Constants.STR_URANUS, Constants.PLANET.URANUS);
            speechBindings.Add(Constants.STR_NEPTUNE, Constants.PLANET.NEPTUNE);
            speechBindings.Add(Constants.STR_PLUTO, Constants.PLANET.PLUTO);
            speechBindings.Add(Constants.STR_MOON, Constants.PLANET.MOON);
            speechBindings.Add(Constants.STR_GALAXY, Constants.PLANET.GALAXY);
            speechBindings.Add(Constants.STR_ENDOR, Constants.PLANET.ENDOR);

            actionBindings = new Dictionary<string, Constants.ACTION>();
            actionBindings.Add(Constants.STR_TRAVELTO, Constants.ACTION.TRAVEL_TO);

            if (this.speechRecognitionEngine != null)
            {
                // Pauses everything while a new grammar is being loaded
                this.speechRecognitionEngine.RecognizeAsyncStop();
                this.speechRecognitionEngine.UnloadAllGrammars();

                // Create a Grammar for the speech recognizer using English
                var gb = new GrammarBuilder();
                gb.Culture = this.recognizerInfo.Culture;


                // Load all the strings in speechBindings and actionBindings into the recognizer
                var choices = new Choices();
                foreach (string choice in speechBindings.Keys)
                {
                    choices.Add(choice);
                }
                foreach (string choice in actionBindings.Keys)
                {
                    choices.Add(choice);
                }

                gb.Append(choices);

                var g = new Grammar(gb);

                // Load the created Grammar into the speech recognition engine
                this.speechRecognitionEngine.LoadGrammar(g);

                // Only do this if the microphone is connected
                if (sensorActive)
                {
                    this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        /**
         * Event handler for speech recognition. Converts the recognized word
         * into a Command with the word's associated Enum in the grammar dictionaries
         **/
        private void speechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Command toDo;                   // Command to generate
            string text = e.Result.Text;    // The word that was recognized

            // If an Action word was recognized
            if (actionBindings.Keys.Contains(text))
            {
                // Create new ActionCommand with the ACTION value in actionBindings
                Constants.ACTION action = actionBindings[text];
                toDo = new ActionCommand(action);
            }

            // If a Travel word was recognized
            else if (speechBindings.Keys.Contains(text))
            {
                // Create new TravelCommand with the PLANET value in speechBindings
                Constants.PLANET travelTo = speechBindings[text];
                toDo = new TravelCommand(travelTo);
            }
            else
            {
                //Console.WriteLine("Speech command not recognized: " + text);
                return;
            }

            Console.WriteLine("Speech Recognized! : " + text);
            main.doCommand(toDo);
        }

        /**
         * This was for testing
         **/
        private void speechDetected(object sender, SpeechDetectedEventArgs e)
        {
            //Console.WriteLine("*Speech Detected"); ;
        }
        

        //Author: Anonymous from Internet. DO NOT CHANGE
        public static RecognizerInfo GetKinectRecognizer()
        {
            // Check for the Kinect language recognizer for a particular culture and return it.
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return
                    "True".Equals(value, StringComparison.OrdinalIgnoreCase)
                    && "en-US".Equals(r.Culture.Name, StringComparison.OrdinalIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }
    }
}
