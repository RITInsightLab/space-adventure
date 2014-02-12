using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace SpaceAdventure3.command
{
    /**
     * Command for executing a system action. As of Space Adventure 3.0, this
     * only includes turning on Travel Mode... however, more actions can be 
     * added in the future
     * 
     * EXTENSIBILITY: To add system actions, follow these steps:
     * 1. Add the new action name to Constants.ACTION enum in Constants.cs
     * 2. Add an appropriate switch case in this class's execute() method
     * 3. Add all action behavior in that switch case
     * 
     * Author: Ross Kahn
     **/
    class ActionCommand: Command
    {

        private string scriptPath;
        private Constants.ACTION action;

        /**
         * Pass in the action to perform, as defined in Constants.cs
         **/
        public ActionCommand(Constants.ACTION action)
        {
            this.action = action;
            
        }

        public override bool execute()
        {

            // Stop zooming (if applicable) once another action is executed
            main.doCommand( new ZoomCommand(Constants.ZOOM_ACTION.PAUSE));


            // Define behavior for all actions
            switch (action)
            {
                case(Constants.ACTION.TRAVEL_TO):
                    scriptPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\celestia\\travel\\travel.cel";
                    travel_mode = true;

                    Console.WriteLine("(ActionCommand.cs) Travel Mode On");
                    System.Diagnostics.Process.Start(scriptPath);
                    return true;

                default:
                    Console.WriteLine("(ActionCommand.cs) Action Command not supported: " + action);
                    return false;
            }
        }
    }
}
