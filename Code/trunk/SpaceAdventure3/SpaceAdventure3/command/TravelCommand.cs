using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SpaceAdventure3.command
{
    public class TravelCommand:Command
    {
         /**
         * Command class for "traveling" to different planetary bodies.
         * To expand functionlity for traveling to new bodies, follow these steps:
          * 
          * EXTENSIBILITY:
          * 1. Add a new entry for the Planetary Body (PB) in Constants.PLANET
          * 2. Add a new const string entry in Constants.cs
          * 2. Add an appropriate case to the switch statement in the TravelCommand constructor
          * 3. Make sure there is a .cel script in the '...\celestia\travel' directory corresponding
          *    EXACTLY to the filename you provide in the switch statement
          * 4. Place a goto line in your .cel script (look at the other scripts for reference)
         * 
          * Author: Ross Kahn
         * **/

        private string scriptPath;  // Path to the .cel script for traveling

        /**
         * Constructs a full filepath to the .cel script based on the passed-in enum
         **/
        public TravelCommand(Constants.PLANET travelTo)
        {
            scriptPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\celestia\\travel\\";
            string filename = "";
            switch (travelTo)
            {
                case Constants.PLANET.SUN:
                    filename = Constants.STR_SUN;
                    break;

                case Constants.PLANET.MERCURY:
                    filename = Constants.STR_MERCURY;
                    break;

                case Constants.PLANET.VENUS:
                    filename = Constants.STR_VENUS;
                    break;

                case Constants.PLANET.EARTH:
                    filename = Constants.STR_EARTH;
                    break;

                case Constants.PLANET.MARS:
                    filename = Constants.STR_MARS;
                    break;

                case Constants.PLANET.JUPITER:
                    filename = Constants.STR_JUPITER;
                    break;

                case Constants.PLANET.SATURN:
                    filename = Constants.STR_SATURN;
                    break;

                case Constants.PLANET.URANUS:
                    filename = Constants.STR_URANUS;
                    break;

                case Constants.PLANET.NEPTUNE:
                    filename = Constants.STR_NEPTUNE;
                    break;

                case Constants.PLANET.PLUTO:
                    filename = Constants.STR_PLUTO;
                    break;

                case Constants.PLANET.MOON:
                    filename = Constants.STR_MOON;
                    break;

                case Constants.PLANET.GALAXY:
                    filename = Constants.STR_GALAXY;
                    break;

                case Constants.PLANET.ENDOR:
                    filename = Constants.STR_ENDOR;
                    break;

                default:
                    scriptPath = null;   // Not a valid travel command
                    break;
            }

            // Complete the file path
            if (scriptPath != null)
            {
                scriptPath = scriptPath + filename + ".cel";
            }
        }

        
        public override bool execute()
        {

            if (scriptPath == null || !File.Exists(scriptPath))
            {
                return false;       // Not a valid travel command
            }

            // Traveling can only happen if travel mode is on
            if (travel_mode)
            {

                Command.stopTravelMode();
                main.doCommand(new ZoomCommand(Constants.ZOOM_ACTION.RESET));

                try
                {
                    //Console.WriteLine("Reflection: "+ System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                    //Console.WriteLine("Normal: " + Directory.GetParent(Environment.CurrentDirectory).Parent.FullName);

                    // Start a new process to execute the .cel script
                    System.Diagnostics.Process.Start(scriptPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to execute Celestia script: \n" + e.Message + "\n" + e.StackTrace);
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                    return false;
                }
                return true;     // Returns if the file exists
            }

            else
            {
                return false;
            }

            
            
        }
    }
}

﻿