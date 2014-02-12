using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceAdventure3
{
    public class Constants
    {

        /** Amount of time (in milliseconds) the user can zoom in or out from
         * the default zoom distance = MAX_ZOOM * TIMER_INTERVAL
         **/
        public const int MAX_ZOOM = 3;
        public const int TIMER_INTERVAL = 200;

        // Used for both speech recognition and script execution. Make sure
        // there are no typos! 
        public const string STR_TRAVELTO = "travel";
        public const string STR_SUN = "sun";
        public const string STR_MERCURY = "mercury";
        public const string STR_VENUS = "venus";
        public const string STR_EARTH = "earth";
        public const string STR_MARS = "mars";
        public const string STR_JUPITER = "jupiter";
        public const string STR_SATURN = "saturn";
        public const string STR_URANUS = "uranus";
        public const string STR_NEPTUNE = "neptune";
        public const string STR_PLUTO = "pluto";
        public const string STR_MOON = "moon";
        public const string STR_GALAXY = "galaxy";
        public const string STR_ENDOR = "endor";

        public enum PLANET {SUN, MERCURY, VENUS, EARTH, MARS, JUPITER, SATURN, URANUS, NEPTUNE, PLUTO, MOON, GALAXY, ENDOR};

        public enum ACTION { TRAVEL_TO, CLICK, RELEASE_CLICK, NEUTRAL, ZOOMIN, ZOOMOUT };

        public enum ZOOM_ACTION { ZOOMIN, ZOOMOUT, PAUSE, RESET };

        // Celestia's default zoom-in and zoom-out keyboard keys
        public const Keys ZOOMIN_KEY = Keys.Home;
        public const Keys ZOOMOUT_KEY = Keys.End;

    }
}
