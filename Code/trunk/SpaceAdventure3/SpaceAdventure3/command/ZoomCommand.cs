using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpaceAdventure3.command
{
    /**
     * Command class for SPECIFICALLY zooming in and out.
     * Note that Zoom Actions are meant to be idempotent... that is, multiple
     * of the same zoom actions can be performed sequentially without changing the
     * state of the system. Also note that all zoom action behaviors should expect
     * to be called from ANY OTHER zoom action. For instance, PAUSE may be called
     * immediately after RESET, ZOOMIN, or ZOOMOUT. Do not assume a zoom action
     * will always follow another specific zoom action.
     * 
     * IMPORTANT: This class assumes that ZOOMING OUT is negative and ZOOMING IN is positive
     * 
     * Author: Ross Kahn
     **/
    class ZoomCommand : Command
    {

        private static int zoomDistance = 0;    // Current zoom distance between -MAX_ZOOM and +MAX_ZOOM
        private static Timer keyTimer;          // Determines how long the zoom key is held down
        private static bool started = false;    // True if the keyTimer has been started


        private static Constants.ZOOM_ACTION curType;

        private Constants.ZOOM_ACTION zoomType;

        public ZoomCommand(Constants.ZOOM_ACTION param)
        {
            zoomType = param;
        }

        // Starts keyTimer. Instantiates the keyTimer if it hasn't been already
        private void start()
        {
            Console.WriteLine("(ZoomCommand.cs) Starting Zoom ");
            if (null == keyTimer)
            {
                keyTimer = new Timer();
                keyTimer.Interval = Constants.TIMER_INTERVAL;
                keyTimer.Start();
                keyTimer.Elapsed += keyTimer_Elapsed;
            }
            
            keyTimer.Enabled = true;
            started = true;
        }

        // Checks for a zoom-in lock, then starts zooming
        private bool zoomIn()
        {
            var checkMax = zoomDistance + 1;
            if (checkMax > Constants.MAX_ZOOM)
            {
                Console.WriteLine("(ZoomCommand.cs) Zoomin will not proceed");
                return false ;
            }

            start();
            KeyboardMouseManager.simulateKeyDown(Constants.ZOOMIN_KEY);
            return true;
        }

        // Checks for a zoom-out lock, then starts zooming
        private bool zoomOut()
        {
            var checkMax = Math.Abs(zoomDistance - 1);
            if (checkMax > Constants.MAX_ZOOM)
            {
                Console.WriteLine("(ZoomCommand.cs) Zoomout will not proceed");
                return false;
            }

            start();
            KeyboardMouseManager.simulateKeyDown(Constants.ZOOMOUT_KEY);
            return true;
        }

        // Stops zooming, but keeps track of the current zoom distance
        private bool pause()
        {
            // Stop both zooms
            KeyboardMouseManager.simulateKeyUp(Constants.ZOOMIN_KEY);
            KeyboardMouseManager.simulateKeyUp(Constants.ZOOMOUT_KEY);

            Console.WriteLine("(ZoomCommand.cs) Zoom Paused");
            if (null != keyTimer && started)
            {
                keyTimer.Enabled = false;
                started = false;
                return true;
            }
            else
            {
                started = false;
                return false;
            }
        }

        // Stops zooming and resets the current zoom distance.
        // Called when traveling to a new planet
        private bool reset()
        {

            if (null == keyTimer)
            {
                return false;
            }

            pause();
            started = false;
            zoomDistance = 0;
            return true;
        }

        /**
         * Event listener for a keyTimer 'tick'. At each tick, check if zooming
         * has reached its limit. If it has not, increment the current zoom distance
         * (if zooming in), or decrement the zoom distance (if zooming out). If
         * zooming has reached its limit, "lock" the zoom (i.e. pause zooming and
         * don't allow any more zooming in that direction). The lock is released
         * when zooming is reset, or when the user zooms in the opposite direction
         **/
        private static void keyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("(ZoomCommand.cs) " + curType +":"+zoomDistance);

            // Increment zoom distance
            if (curType == Constants.ZOOM_ACTION.ZOOMIN)
            {
                // Check zoom limits
                if (zoomDistance + 1 <= Constants.MAX_ZOOM)
                {
                    zoomDistance++;
                }
                else
                {
                    // Lock zooming in
                    Console.WriteLine("(ZoomCommand.cs) Locking zoomin... ");
                    Command pauseCommand = new ZoomCommand(Constants.ZOOM_ACTION.PAUSE);
                    main.doCommand(pauseCommand);
                }
            }

            // decrement zoom distance
            else if (curType == Constants.ZOOM_ACTION.ZOOMOUT)
            {
                // Check zoom limits
                if (Math.Abs(zoomDistance - 1) <= Constants.MAX_ZOOM)
                {
                    zoomDistance--;
                }
                else
                {
                    // Lock zooming out
                    Console.WriteLine("(ZoomCommand.cs) Locking zoomout... ");
                    Command pauseCommand = new ZoomCommand(Constants.ZOOM_ACTION.PAUSE);
                    main.doCommand(pauseCommand);
                }
            }
            else
            {
                // Somehow the timer was started with something other than Zoom-in or Zoom-out
                Console.WriteLine("ERROR ZoomCommand.cs - Timer started without valid zoomtype: " + curType);
                keyTimer.Stop();
            }
        }


        public override bool execute()
        {
            curType = zoomType;
            switch (zoomType)
            {
                case Constants.ZOOM_ACTION.PAUSE:
                    return pause();

                case Constants.ZOOM_ACTION.RESET:
                    return reset();

                case Constants.ZOOM_ACTION.ZOOMIN:
                    return zoomIn();

                case Constants.ZOOM_ACTION.ZOOMOUT:
                    return zoomOut();

                default:
                    Console.WriteLine("ERROR ZoomCommand.cs - Zoom Action not supported: " + zoomType);
                    return false;
            }
            
        }
    }
}
