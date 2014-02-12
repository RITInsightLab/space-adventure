using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpaceAdventure3.command;
using MouseKeyboardLibrary;

namespace SpaceAdventure3
{
    /**
     * Catches (globally) all keyboard and mouse inputs. This is needed because
     * the Space Adventure C# program is not active during normal operation (Celestia
     * is). Because of that, normal keyboard input can't be used. 
     * 
     * This class is also needed to simulate keyboard presses and mouse clicks.
     * 
     * EXTENSIBILITY: To add a key binding, follow these steps:
     * 1. If the key binding represents a new planet to travel to, first follow
     *    all the steps in the TravelCommand.cs extensibility section. If the
     *    key binding represents a system action, follow the steps in the
     *    ActionCommand.cs extensibility section
     * 2. If the key binding represents a travel command, add an entry into
     *    travelBindings (in the KeyboardMouseManager constructor) with the
     *    key code and the new Constants.PLANET entry. If the key binding represents
     *    a system action, add an entry into actionBindings with the key code
     *    and the new Constants.ACTION entry.
     * 3. That's it!
     * 
     * Author: Ross Kahn
     **/
    public class KeyboardMouseManager 
    {

        // Represents travel-to destinations
        private Dictionary<Keys, Constants.PLANET> travelBindings;

        // Represents system actions (e.g. turning Travel Mode on)
        private Dictionary<Keys, Constants.ACTION> actionBindings;


        // From the MouseKeyboardLibrary external library
        MouseHook mouseHook = new MouseHook();
        KeyboardHook keyboardHook = new KeyboardHook();

        public KeyboardMouseManager()
        {
            travelBindings = new Dictionary<Keys, Constants.PLANET>();
            travelBindings.Add(Keys.D1, Constants.PLANET.MERCURY);
            travelBindings.Add(Keys.D2, Constants.PLANET.VENUS);
            travelBindings.Add(Keys.D3, Constants.PLANET.EARTH);
            travelBindings.Add(Keys.D4, Constants.PLANET.MARS);
            travelBindings.Add(Keys.D5, Constants.PLANET.JUPITER);
            travelBindings.Add(Keys.D6, Constants.PLANET.SATURN);
            travelBindings.Add(Keys.D7, Constants.PLANET.URANUS);
            travelBindings.Add(Keys.D8, Constants.PLANET.NEPTUNE);
            travelBindings.Add(Keys.D9, Constants.PLANET.SUN);
            travelBindings.Add(Keys.D0, Constants.PLANET.PLUTO);

            actionBindings = new Dictionary<Keys, Constants.ACTION>();
            actionBindings.Add(Keys.Tab, Constants.ACTION.TRAVEL_TO);
        }
        

        /**
         * Adds the event listener for key presses and starts the hooks
         **/
        public void start()
        {

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
  
            mouseHook.Start();
            keyboardHook.Start();
            
        }

        // Simulates the user holding down a specified keyboard key
        // Note that it's the user HOLDING the key, not pressing and releasing
        public static void simulateKeyDown(Keys toPress)
        {
           
            //Console.WriteLine("(KMM) Pressing key '" + toPress + "'...");
            KeyboardSimulator.KeyDown(toPress);
        }

        // Simulates a user releasing a key after pressing it
        public static void simulateKeyUp(Keys toRelease)
        {
            //Console.WriteLine("(KMM) Releasing key '" + toRelease + "'...");
            KeyboardSimulator.KeyUp(toRelease);

            
        }

        // Simulates the user holding down the right mouse button (if press is true)
        // or releasing the right mouse button (if press is false)
        public static void simulateMouseAction(bool press)
        {
            if (press)
            {
                //Console.WriteLine("(KMM) Mouse Pressed");
                MouseSimulator.MouseDown(MouseButton.Right);
            }
            else
            {
                //Console.WriteLine("(KMM) Mouse Released");
                MouseSimulator.MouseUp(MouseButton.Right);
            }
            
        }


        /**
         * The event handler for any key being pressed down. That includes simulated
         * key presses.
         **/
        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("(KMM) KeyDown Event: " + e.KeyCode);

            // Celestia's zoom in and zoom out key
            // TODO: This should probably use ZoomCommands
            if (e.KeyCode == Constants.ZOOMIN_KEY || e.KeyCode == Constants.ZOOMOUT_KEY) { return; }



            Command toDo;   // Command to execute

            if(actionBindings.Keys.Contains(e.KeyCode))
            {
                // Extract the ACTION enum for this key code using actionBindings
                Constants.ACTION action = actionBindings[e.KeyCode];

                // Construct a new ActionCommand using the ACTION enum as a parameter
                toDo = new ActionCommand(action);
            }

            else if (travelBindings.Keys.Contains(e.KeyCode))
            {
                // Extract the PLANET enum for this key code using travelBindings
                Constants.PLANET travelTo = travelBindings[e.KeyCode];

                // Construct a new TravelCommand using the PLANET enum as a parameter
                toDo = new TravelCommand(travelTo);   
            }
            else
            {
                // Have Celestia print out a message
                Console.WriteLine("(KeyboardMouseManager.cs) Command not specified for key " + e.KeyCode);
                return;
            }

            // Send the command to be executed
            main.doCommand(toDo);         
        }

    }
}
