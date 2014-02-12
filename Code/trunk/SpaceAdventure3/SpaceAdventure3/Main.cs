using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SpaceAdventure3.command;
using System.Collections.Concurrent;

namespace SpaceAdventure3
{

    public class main
    {
        /**
         * Main method. Initializes all input managers and starts them.
         * Then, executes the start-up script (right now it just goes
         * to Earth. To make a better one, look at the Extensibility section
         * in TravelCommand.cs)
         * 
         * Author: Ross Kahn
         **/
        public static void Main()
        {

            KeyboardMouseManager keymanager = new KeyboardMouseManager();
            keymanager.start();

            KinectManager kinectManager = new KinectManager();
            SpeechManager speechManager = new SpeechManager();

            kinectManager.Initialize();

            if (KinectManager.isConnected())
            {
                // Start the speech manager with the Kinect microphone array
                speechManager.Initialize(kinectManager.getSensor());
            }
            else
            {
                // Start the speech manager with the default system microphone because
                // no kinect was detected
                speechManager.Initialize();
            }

            // Start-up script. Works just like a regular travel command
            TravelCommand command = new TravelCommand(Constants.PLANET.EARTH);
            doCommand(command);

            // Keeps the application running. Since this program is event-driven,
            // there needs to be a non-blocking main loop as the program waits
            // for events to come in. That's what Application.Run() does
            try
            {
               Application.Run();  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
      
        }


        /**
         * The main thread is responsible for executing commands.
         * 
         * EXTENSIBILITY: If there is behavior that you would like to do
         * before (or after) any command is executed, do it here. Also,
         * it may be wise in the future to make this thread-safe
         **/
        public static void doCommand(Command c)
        { 
            c.execute();
        }
       
        
    }
}
