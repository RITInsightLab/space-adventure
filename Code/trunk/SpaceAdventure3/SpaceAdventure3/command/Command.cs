using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAdventure3.command
{

    /**
     * Command superclass. Defines the execute() method that all commands
     * must implement. May also provide other functionality consistent
     * with all command subclasses
     * Author: Ross Kahn
     **/
    public abstract class Command
    {
        // If 'Travel Mode' is activated. Starts as 'true'
        // so the start-up script can run from Main
        protected static bool travel_mode = true;
        
        // All Command subclasses must implement this
        public abstract Boolean execute();

        // Turns off 'Travel Mode'
        public static void stopTravelMode()
        {
            travel_mode = false;
        }
    }
}
