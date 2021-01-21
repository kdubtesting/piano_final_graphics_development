//Kenneth Weeks, cs 680, Final Project, 11.17.20
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace piano
{
    public partial class Form1 : Form
    {
        //These are the global variables
        double[] camera = { 6.0, 6.0, -1.0 };

        //These will be some basic planes that are used for the program
        double[] yPlanes = { 5.0, 4.0, 2.0, 1.0 };
        double[] zPlanes = { 0.75, 1.0, 2.0 };

        //This will save the pixels for each key that will be used later for the animations
        Pixel[,] pixelsForKeys = new Pixel[13, 10000];

        //This will count the actual pixels used for each key
        int[] count = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //This will be used in the animation to track the white keys
        double topPlane = 4.0;
        double nextPlane = 3.75;
        double amount = -0.25;

        //This will track the black keys as the white keys are moving
        double blackPlane = 4.0;
        double bamount = -0.125;
        double[] blackSides = new double[5];

        //This will be for the black keys
        double topPlaneB = 5.0;
        double nextPlaneB = 5.0;
        double amountB = -0.5;

        //This will keep track of the key presses and trigger the different animations
        bool endPress = false;
        bool keyPress = false;
        //This will be for the key presses that are taken when something is being animated
        String keyQueue = "";
        int currChar = 0;

        //This will be the bitmap used
        Bitmap bm = null;

        //These are the denominators that will be used to translate screen to user coordiante
        double xDenom = 0;
        double yDenom = 0;

        //These are the sounds of each key press
        SoundPlayer[] sounds = { new SoundPlayer(piano.Properties.Resources.middlec), 
            new SoundPlayer(piano.Properties.Resources.dkey), 
            new SoundPlayer(piano.Properties.Resources.ekey),
            new SoundPlayer(piano.Properties.Resources.fkey),
            new SoundPlayer(piano.Properties.Resources.gkey), 
            new SoundPlayer(piano.Properties.Resources.akey), 
            new SoundPlayer(piano.Properties.Resources.bkey), 
            new SoundPlayer(piano.Properties.Resources.fifthc),
            new SoundPlayer(piano.Properties.Resources.csharp),
            new SoundPlayer(piano.Properties.Resources.dsharp),
            new SoundPlayer(piano.Properties.Resources.fsharp),
            new SoundPlayer(piano.Properties.Resources.gsharp),
            new SoundPlayer(piano.Properties.Resources.asharp)};

        //This will be used in the general animation, to key track of the keys
        int key = 0;

        public Form1()
        {
            InitializeComponent();
            //Set the bitmap here
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            //Set the denominators here
            xDenom = (double)pictureBox1.Width;
            yDenom = (double)pictureBox1.Height;

        }

        public bool x_plane(double plane, double xp, double yp, Bitmap bm, int j, int i, bool tick)
        {
            //Determine the xcoordinate t for the specific plane given by the parameter
            double xcoord = (plane - camera[0]) / (((-1.0) * camera[0]) + xp);

            double ycoord = (camera[1] * (1.0 - xcoord)) + (yp * xcoord);
            double zcoord = (camera[2] * (1.0 - xcoord));

            //What if we start middle
            //Go left
            //Then Go right,
            //that ensures that no matter the angle the correct plane is hit first
            //We will need to order the checks from middle, left, and right

            //Black Keys sides
            //as long as the order is taken care of in the array, this should work
            if (plane >= 2.75 && plane <= 8.25)
            {
                if ((ycoord >= 4.0 && ycoord <= 5.0)
                    && (zcoord >= 2.0 && zcoord <= 5.0))
                {
                    //Populate the array associated with the specific keys here
                    if (!tick)
                    {
                        if (plane == 3.25)
                        {
                            pixelsForKeys[8, count[8]] = new Pixel(j, i);
                            count[8]++;
                            blackSides[0] = 3.25;
                        }

                        if (plane == 4.25)
                        {
                            pixelsForKeys[9, count[9]] = new Pixel(j, i);
                            count[9]++;
                            blackSides[1] = 4.25;
                        }

                        if (plane == 6.25 || plane == 5.75) 
                        {
                            pixelsForKeys[10, count[10]] = new Pixel(j, i);
                            count[10]++;
                            blackSides[2] = 6.25;
                        }

                        if (plane == 6.75) 
                        {
                            pixelsForKeys[11, count[11]] = new Pixel(j, i);
                            count[11]++;
                            blackSides[3] = 6.75;
                        }

                        if (plane == 7.75) 
                        {
                            pixelsForKeys[12, count[12]] = new Pixel(j, i);
                            count[12]++;
                            blackSides[4] = 7.75;
                        }
                    }
                    bm.SetPixel(j, i, Color.FromArgb(255, 41, 41, 41));
                    return true;
                }
            }

            //This will be the side panels between the front panel and the border, so it will be close to the keys
            if (plane == 2.0 || plane == 10.0)
            {
                if ((ycoord <= 4.0 && ycoord >= 2.0) && (zcoord >= 0.75 && zcoord <= 5.0))
                {
                    bm.SetPixel(j, i, Color.FromArgb(255, 63, 42, 20));
                    return true;
                }
            }


            return false;
        }

        public bool z_plane(double plane, double xp, double yp, Bitmap bm, int j, int i, bool tick)
        {
            //Find the z coordinate t for the given plane
            double zcoord = 1.0 - (plane / camera[2]);

            double xcoord = (camera[0] * (1.0 - zcoord)) + (xp * zcoord); //X
            double ycoord = (camera[1] * (1.0 - zcoord)) + (yp * zcoord); //Y

            if (plane == 0.75)
            {
                //Two Front Panels
                if (((xcoord >= 1.0 && xcoord <= 2.0) || (xcoord <= 11.0 && xcoord >= 10.0)) && (ycoord >= 1.0 && ycoord <= 4.0))
                {
                    bm.SetPixel(j, i, Color.FromArgb(255, 82, 54, 27));
                    return true;
                }

                //Front facing panel
                if ((xcoord >= 2.0 && xcoord <= 10.0) && (ycoord >= 1.0 && ycoord <= 2.0))
                {
                    bm.SetPixel(j, i, Color.FromArgb(255, 82, 54, 27));
                    return true;
                }
            }
            else if (plane == 1.0)
            {
                //These are the fronts of the white keys
                if ((xcoord <= 10.0 && xcoord >= 2.0) && (ycoord <= 4.0 && ycoord >= 2.0))
                {
                    double floorX = Math.Floor(xcoord);

                    //Populate the array for the pixels used in the animation
                    if (!tick)
                    {
                        pixelsForKeys[(int)(floorX - 2), count[(int)(floorX - 2)]] = new Pixel(j, i);
                        count[(int)(floorX - 2)]++;
                    }

                    double ceilX = Math.Ceiling(xcoord);

                    //Create the division in the keys
                    if ((xcoord <= floorX + 0.03) || (xcoord >= ceilX - 0.03))
                    {
                        bm.SetPixel(j, i, Color.Black);
                    }
                    else
                    {
                        bm.SetPixel(j, i, Color.WhiteSmoke);
                    }

                    return true;
                }
            }
            else if (plane == 2.0)
            {
                //These are the fronts of the black keys
                if (((xcoord >= 2.75 && xcoord <= 3.25) ||
                        (xcoord >= 3.75 && xcoord <= 4.25) ||
                        (xcoord >= 5.75 && xcoord <= 6.25) ||
                        (xcoord >= 6.75 && xcoord <= 7.25) ||
                        (xcoord >= 7.75 && xcoord <= 8.25))
                        && (ycoord >= 4.0 && ycoord <= 5.0))
                {
                    //Populate the proper array for the black keys here
                    if (xcoord >= 2.75 && xcoord <= 3.25) 
                    {
                        if (!tick)
                        {
                            pixelsForKeys[8, count[8]] = new Pixel(j, i);
                            count[8]++;
                        }
                    }
                    else if (xcoord >= 3.75 && xcoord <= 4.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[9, count[9]] = new Pixel(j, i);
                            count[9]++;
                        }
                    }
                    else if (xcoord >= 5.75 && xcoord <= 6.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[10, count[10]] = new Pixel(j, i);
                            count[10]++;
                        }
                    }
                    else if (xcoord >= 6.75 && xcoord <= 7.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[11, count[11]] = new Pixel(j, i);
                            count[11]++;
                        }
                    }
                    else if (xcoord >= 7.75 && xcoord <= 8.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[12, count[12]] = new Pixel(j, i);
                            count[12]++;
                        }
                    }
                    bm.SetPixel(j, i, Color.Black);
                    return true;
                }
            }

            return false;
        }

        public bool y_plane(double plane, double xp, double yp, Bitmap bm, int j, int i, bool tick)
        {
            //Find the y coordnate t for the given plane
            double ycoord = (plane - camera[1]) / (((-1.0) * camera[1]) + yp);

            double xcoord = (camera[0] * (1.0 - ycoord)) + (xp * ycoord);
            double zcoord = (camera[2] * (1.0 - ycoord));

            double[] xplanes = null;
            int k = 0;

            if (plane == 5.0)
            {
                //Top face of the black keys
                if (((xcoord >= 2.75 && xcoord <= 3.25) ||
                        (xcoord >= 3.75 && xcoord <= 4.25) ||
                        (xcoord >= 5.75 && xcoord <= 6.25) ||
                        (xcoord >= 6.75 && xcoord <= 7.25) ||
                        (xcoord >= 7.75 && xcoord <= 8.25))
                        && (zcoord >= 2.0 && zcoord <= 5.0))
                {
                    //Populate the array for the black keys here
                    if (xcoord >= 2.75 && xcoord <= 3.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[8, count[8]] = new Pixel(j, i);
                            count[8]++;
                        }
                    }
                    else if (xcoord >= 3.75 && xcoord <= 4.25) 
                    {
                        if (!tick)
                        {
                            pixelsForKeys[9, count[9]] = new Pixel(j, i);
                            count[9]++;
                        }
                    }
                    else if (xcoord >= 5.75 && xcoord <= 6.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[10, count[10]] = new Pixel(j, i);
                            count[10]++;
                        }
                    }
                    else if (xcoord >= 6.75 && xcoord <= 7.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[11, count[11]] = new Pixel(j, i);
                            count[11]++;
                        }
                    }
                    else if (xcoord >= 7.75 && xcoord <= 8.25)
                    {
                        if (!tick)
                        {
                            pixelsForKeys[12, count[12]] = new Pixel(j, i);
                            count[12]++;
                        }
                    }

                    bm.SetPixel(j, i, Color.FromArgb(255, 61, 61, 61));
                    return true;
                }
                else
                {
                    //Check the z plane, it is given in the first conditional
                    if (z_plane(2.0, xp, yp, bm, j, i, tick))
                    {
                        return true;
                    }

                    //In the x direction, there is a list of x planes to choose from, so this will be the same as the main loop here
                    bool xFind = false;
                    //Start middle, go left, then go right
                    xplanes = new double[10] { 5.75, 4.25, 3.75, 3.25, 2.75, 6.25, 6.75, 7.25, 7.75, 8.25 };

                    while (k < xplanes.Length && xFind == false)
                    {
                        xFind |= x_plane(xplanes[k], xp, yp, bm, j, i, tick);
                        k++;
                    }

                    //If the plane is found;
                    if (xFind)
                    {
                        return true;
                    }
                }
            }
            else if (plane == 4.0)
            {
                //Top face of the keys
                if ((xcoord <= 10.0 && xcoord >= 2.0) && (zcoord >= 1.0 && zcoord <= 5.0))
                {
                    double floorX = Math.Floor(xcoord);

                    //Populate the array for the pixels used in the animation\
                    if(!tick) 
                    {
                        pixelsForKeys[(int)(floorX - 2), count[(int)(floorX - 2)]] = new Pixel(j, i);
                        count[(int)(floorX - 2)]++;
                    }
                    
                    double ceilX = Math.Ceiling(xcoord);
                    //Add the dividing lines here
                    if ((xcoord <= floorX + 0.05) || (xcoord >= ceilX - 0.05))
                    {
                        bm.SetPixel(j, i, Color.Black);
                    }
                    else
                    {
                        bm.SetPixel(j, i, Color.White);
                    }

                    return true;
                }
                else if (((xcoord >= 1.0 && xcoord <= 2.0) || (xcoord <= 11.0 && xcoord >= 10.0)) && (zcoord >= 0.75 && zcoord <= 5.0))
                {
                    //This is the side panel of the keys
                    bm.SetPixel(j, i, Color.FromArgb(255, 120, 80, 39));
                    return true;
                }
                else
                {
                    //Front face of keys
                    if (z_plane(1.0, xp, yp, bm, j, i, tick))
                    {
                        return true;
                    }

                    bool xFind = false;
                    //Left to Right
                    xplanes = new double[2] { 2.0, 10.0 };
                    while (k < xplanes.Length && xFind == false)
                    {
                        xFind |= x_plane(xplanes[k], xp, yp, bm, j, i, tick);
                        k++;
                    }

                    if (xFind)
                    {
                        return true;
                    }
                }
            }
            else if (plane == 2.0)
            {
                //Connecting keys to front panel
                if ((xcoord >= 2.0 && xcoord <= 10.0) && (zcoord >= 0.75 && zcoord <= 1.0))
                {
                    bm.SetPixel(j, i, Color.FromArgb(255, 63, 42, 20));
                    return true;
                }
                else
                {
                    if (z_plane(0.75, xp, yp, bm, j, i, tick))
                    {
                        return true;
                    }
                }
            }
            else if (plane == 1.0)
            {
                //floor of piano
                if ((xcoord <= 11.0 && xcoord >= 1.0) && (zcoord >= 0.75 && zcoord <= 5.0))
                {
                    bm.SetPixel(j, i, Color.OrangeRed);
                    return true;
                }
                else
                {
                    if (z_plane(0.75, xp, yp, bm, j, i, tick))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Create the piano here
            for (int i = 0; i < pictureBox1.Height; i++)
            {
                for (int j = 0; j < pictureBox1.Width; j++)
                {

                    double xp = (double)(12 * j) / xDenom;
                    double yp = 12.0 - (((double)(12.0 * i)) / yDenom);

                    bool checker = false;

                    int k = 0;
                    while (k < yPlanes.Length && checker != true)
                    {
                        checker |= y_plane(yPlanes[k], xp, yp, bm, j, i, false);
                        k++;
                    }

                    //So everything failed in the function
                    if (checker == false)
                    {
                        bm.SetPixel(j, i, Color.Gray);
                    }
                }
            }

            pictureBox1.Image = bm;
        }

        private void animate_Click(object sender, EventArgs e)
        {
            //Enable the animations to begin
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (key < 8)
            {
                //Animate the white keys
                timer1.Interval = 16; 
                nextPlane += amount;
                blackPlane += bamount;

                if (nextPlane == 2.0 || nextPlane == 4.0)
                {
                    //This conditional will determine the direction of the key
                    if (nextPlane == 2.0) 
                    {
                        //This will determine the sound;
                        sounds[key].Play();
                    }
                    amount *= -1;
                    bamount *= -1;
                }

                //Call the animation of the white keys
                animate_key();
            }
            else 
            {
                //Animate the black keys
                timer1.Interval = 60;
                nextPlaneB += amountB;

                if (nextPlaneB == 4.0 || nextPlaneB == 5.0) 
                {
                    //This conditional will determine the direction of the key
                    if (nextPlaneB == 4.0)
                    {
                        //This will determine the sound;
                        sounds[key].Play();
                    }
                    amountB *= -1;
                }

                //Call the animation of the black keys
                animate_bkey();
            }

            //This is for the black keys
            if (nextPlaneB == 5.0 && key >= 8 && keyPress == false)
            {
                //This will be used when the keys are being oscilated through with the "Move Keys button"
                key++;
            }

            if (nextPlaneB == 5.0 && key >= 8 && keyPress == true)
            {
                //This will keep track of when the black keys are pressed
                if (currChar < keyQueue.Length && !endPress)
                {
                    //Check to see if there are keys in the queue that has been populate while a previous animation was running
                    if (currChar + 1 == keyQueue.Length)
                    {
                        endPress = true;
                    }
                    //Search the specific key pressed here
                    keyPresses(keyQueue[currChar]);

                }
                else
                {
                    //End the animtion here
                    timer1.Enabled = false;
                    //textBox1.Enabled = true;
                    keyPress = false;
                }
            }

            //This is for the white keys
            if (nextPlane == 4.0 && key < 8 && keyPress == false)
            {
                key++;
            }

            if (nextPlane == 4.0 && key < 8 && keyPress == true)
            {
                //This will keep track of when the white keys are pressed
                if (currChar < keyQueue.Length && !endPress)
                {
                    //Check to see if there are keys in the queue that has been populate while a previous animation was running
                    if (currChar + 1 == keyQueue.Length)
                    {
                        endPress = true;
                    }
                    //Search the specific key pressed here
                    keyPresses(keyQueue[currChar]);
                        
                }
                else 
                {
                    //End the animation here
                    timer1.Enabled = false;
                    //textBox1.Enabled = true;
                    keyPress = false;
                }
                

            }

            //This will be for all keys
            if (key == 13)
            {
                //restart the oscillation here
                key = 0;
            }
        }

        public void animate_bkey() 
        {
            //This is for black keys
            for (int i = 0; i < count[key]; i++) 
            {
                double xp = (double)(12 * pixelsForKeys[key, i].getJ()) / xDenom;
                double yp = 12.0 - (((double)(12.0 * pixelsForKeys[key, i].getI())) / yDenom);

                //Check if it intersects with the top plane

                //Evaluate the top of the keys first
                if (topPlaneB - nextPlaneB == 0)
                {
                    //This will be for when the key has returned to it's rest poisiton
                    double t = (5.0 - camera[1]) / (((-1.0) * camera[1]) + yp);

                    double xcoord = (camera[0] * (1.0 - t)) + (xp * t);
                    double zcoord = (camera[2] * (1.0 - t));

                    if (((xcoord >= 2.75 && xcoord <= 3.25) 
                        || (xcoord >= 3.75 && xcoord <= 4.25)
                        || (xcoord >= 5.75 && xcoord <= 6.25)
                        || (xcoord >= 6.75 && xcoord <= 7.25)
                        || (xcoord >= 7.75 && xcoord <= 8.25)) && zcoord >= 2.0 && zcoord <= 5.0)
                    {

                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 61, 61, 61));

                        continue;
                    }
                }
                else 
                {
                    //This will track and color the keys as it is moving through it's motion
                    double t = (7.5 - (1.5 * (camera[1])) + (((topPlaneB - nextPlaneB) / 2.0) * camera[2]) - (((topPlaneB - nextPlaneB) / 2.0) * 5.0)) / ((-1.5 * camera[1]) + (1.5 * yp) + (((topPlaneB - nextPlaneB) / 2.0) * camera[2]));

                    double xcoord = (camera[0] * (1.0 - t)) + (xp * t);
                    double ycoord = (camera[1] * (1.0 - t)) + (yp * t);
                    double zcoord = (camera[2] * (1.0 - t));

                    if (((xcoord >= 2.75 && xcoord <= 3.25)
                        || (xcoord >= 3.75 && xcoord <= 4.25)
                        || (xcoord >= 5.75 && xcoord <= 6.25)
                        || (xcoord >= 6.75 && xcoord <= 7.25)
                        || (xcoord >= 7.75 && xcoord <= 8.25)) && zcoord >= 2.0 && zcoord <= 5.0 && ycoord <= 5.0 && ycoord >= nextPlaneB)
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 61, 61, 61));
                        continue;
                    }
                }

                //THis will be for the sides of the keys as it moving
                double t2 = (blackSides[(int)(key - 8)] - camera[0]) / (((-1.0) * camera[0]) + xp);

                double ycoord2 = (camera[1] * (1.0 - t2)) + (yp * t2);
                double zcoord2 = (camera[2] * (1.0 - t2));

                //Need to account for the slope as the key is moving to determine the proper y boundaries
                double slope = (5.0 - nextPlaneB) / (5.0 - 2.0);
                double b = (nextPlaneB) - (slope * 2);
                double boundY = ((slope) * zcoord2) + b;

                if (zcoord2 <= 5.0 && zcoord2 >= 2.0 && ycoord2 >= 4.0 && ycoord2 <= boundY) 
                {
                    bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 41, 41, 41));
                    continue;
                }

                //This will be the front of the black keys
                double t3 = 1.0 - (2.0 / camera[2]);

                double xcoord3 = (camera[0] * (1.0 - t3)) + (xp * t3);
                double ycoord3 = (camera[1] * (1.0 - t3)) + (yp * t3);
                     
                if (((xcoord3 >= 2.75 && xcoord3 <= 3.25)
                        || (xcoord3 >= 3.75 && xcoord3 <= 4.25)
                        || (xcoord3 >= 5.75 && xcoord3 <= 6.25)
                        || (xcoord3 >= 6.75 && xcoord3 <= 7.25)
                        || (xcoord3 >= 7.75 && xcoord3 <= 8.25)) && (ycoord3 <= nextPlaneB && ycoord3 >= 4.0))
                {

                    bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                   
                    continue;
                }

                //Fill in the holes in the back
                bool checkInter = false;

                //Next loop through the x planes of the black keys (check the sides)
                if (blackSides[(int)(key - 8)] < 6)
                {
                    //Keys to the left of the middle
                    checkInter |= x_plane(blackSides[(int)(key - 8)] - 1, xp, yp, bm, pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), true);
                }
                else
                {
                    //Keys to the right of the moddile
                    checkInter |= x_plane(blackSides[(int)(key - 8)] + 1, xp, yp, bm, pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), true);
                }

                //See if the pixels that do not interset with the black keys intersect with the top plane of the white keys
                if (checkInter == false) 
                {
                    
                    checkInter |= y_plane(4, xp, yp, bm, pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), true);
                }

                //Otherwise paint it to the background
                if (checkInter == false) 
                {
                    bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Gray);
                }

                
            }

            //Set the image here
            pictureBox1.Image = bm;
        }

        public void animate_key()
        {
            //Animate the black keys
            for (int i = 0; i < count[key]; i++)
            {
                //This is the screen points
                //double xp = (double)(10 * pixelsForKeys[key, i].getJ()) / xDenom;
                //double yp = 10.0 - ((double)pixelsForKeys[key, i].getI() / yDenom);

                double xp = (double)(12 * pixelsForKeys[key, i].getJ()) / xDenom;
                double yp = 12.0 - (((double)(12.0 * pixelsForKeys[key, i].getI())) / yDenom);

                //This will be the z plane for the face of the black keys
                //This will extend as the white keys move up and down
                double t = 1.0 - (2.0 / camera[2]);

                double xcoord = (camera[0] * (1.0 - t)) + (xp * t); //X
                double ycoord = (camera[1] * (1.0 - t)) + (yp * t); //Y

                //Check to see if that point has intersected with the planes given
                if (((xcoord >= (key + 2) && xcoord <= (key + 2.25)) || (xcoord >= (key + 2.75) && xcoord <= (key + 3))) && (ycoord >= blackPlane && ycoord <= 4.0))
                {
                    if (!(xcoord >= 2 && xcoord <= 2.25) && !(xcoord >= 4.75 && xcoord <= 5.0) && !(xcoord >= 5.0 && xcoord <= 5.25) && !(xcoord >= 8.75 && xcoord <= 9.0) && !(xcoord >= 9.0 && xcoord <= 9.25) && !(xcoord >= 9.75 && xcoord <= 10.0))
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                        continue;
                    }
                }

                //Now test the two side planes of the black keys
                //Have to check both sides of the keys, just incase
                double t1a = (key + 2.25 - camera[0]) / (((-1.0) * camera[0]) + xp);

                double ycoorda = (camera[1] * (1.0 - t1a)) + (yp * t1a);
                double zcoorda = (camera[2] * (1.0 - t1a));

                if ((ycoorda >= blackPlane && ycoorda <= 4.0) && (zcoorda >= 2.0 && zcoorda <= 5.0))
                {
                    if (!(key + 2.25 == 2.25) && !(key + 2.25 == 5.25))
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 41, 41, 41));
                        continue;
                    }
                }

                //This is the opposite side of the black key as related to the plane above
                double t1b = (key + 2.75 - camera[0]) / (((-1.0) * camera[0]) + xp);

                double ycoordb = (camera[1] * (1.0 - t1b)) + (yp * t1b);
                double zcoordb = (camera[2] * (1.0 - t1b));

                if ((ycoordb >= blackPlane && ycoordb <= 4.0) && (zcoordb >= 2.0 && zcoordb <= 5.0))
                {
                    if (!(key + 2.75 == 4.75) && !(key + 2.75 == 8.75))
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 41, 41, 41));
                        continue;
                    }
                }

                //This will test for the top plane in the animation
                if (topPlane - nextPlane == 0)
                {
                    //This is the test for when the key has returned to rest at the top
                    double t2 = (4.0 - camera[1]) / (((-1.0) * camera[1]) + yp);

                    double xcoord2 = (camera[0] * (1.0 - t2)) + (xp * t2);
                    double zcoord2 = (camera[2] * (1.0 - t2));

                    if (xcoord2 >= 2.0 && xcoord2 <= 10.0 && zcoord2 >= 1.0 && zcoord2 <= 5.0)
                    {
                        if (xcoord2 <= (key + 2) + 0.04 || xcoord2 >= (key + 3) - 0.04)
                        {
                            bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                        }
                        else
                        {
                            bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.White);
                        }
                        continue;
                    }
                }
                else
                {
                    //Top face of the key as it is moving through it's motion.
                    //This function will track it
                    double t2 = (16 - (4 * camera[1]) + ((topPlane - nextPlane) * camera[2]) - ((topPlane - nextPlane) * 5)) / ((-4 * camera[1]) + (4 * yp) + ((topPlane - nextPlane) * camera[2]));

                    double xcoord2 = (camera[0] * (1 - t2)) + (xp * t2);
                    double zcoord2 = (camera[2] * (1 - t2));
                    double ycoord2 = (camera[1] * (1 - t2)) + (yp * t2);

                    if (xcoord2 >= (key + 2) && xcoord2 <= (key + 3) && zcoord2 >= 1.0 && zcoord2 <= 5.0 && ycoord2 >= nextPlane && ycoord2 <= 4.0)
                    {
                        if (xcoord2 <= (key + 2) + 0.03 || xcoord2 >= (key + 3) - 0.03)
                        {
                            bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                        }
                        else
                        {
                            bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.White);
                        }
                        continue;
                    }
                }

                //This is the front face of the keys
                double t3 = 1.0 - (1.0 / camera[2]);

                double xcoord3 = (camera[0] * (1.0 - t3)) + (xp * t3);
                double ycoord3 = (camera[1] * (1.0 - t3)) + (yp * t3);

                //This will determine how the keys will be moving
                if ((xcoord3 <= (key + 3) && xcoord3 >= (key + 2)) && (ycoord3 <= nextPlane && ycoord3 >= 2.0))
                {
                    if (xcoord3 >= (key + 3) - 0.03 || xcoord3 <= (key + 2) + 0.03)
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                    }
                    else
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.WhiteSmoke);
                    }

                    continue;
                }

                //Color the pixels that have not intersected with anything to another plane in the set
                if ((xcoord3 == (key + 3) || xcoord3 == (key + 2)) && (key + 2 != 2.0 || key + 3 != 10.0))
                {
                    bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.Black);
                }
                else
                {
                    if (key + 2 == 2.0 || key + 3 == 10.0)
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.FromArgb(255, 63, 42, 20));
                    }
                    else
                    {
                        bm.SetPixel(pixelsForKeys[key, i].getJ(), pixelsForKeys[key, i].getI(), Color.LightSlateGray);
                    }
                }
            }

            //This ends the loop, and sets the background
            pictureBox1.Image = bm;

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!keyPress)
            {
                //If something hasn't been pressed, just start a regular animation
                endPress = false;
                keyQueue += e.KeyChar;
                //textBox1.Enabled = false;
                keyPresses(e.KeyChar);
            }
            else 
            {
                //Otherwise save everything to a queue
                keyQueue += e.KeyChar;
            } 
        }

        public void keyPresses(char c) 
        {
            //This will return the key on the piano associated with the key pressed on the keyboard
            switch (c) 
            {
                case 'A':
                case 'a':
                    key = 0;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'S':
                case 's':
                    key = 1;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'D':
                case 'd':
                    key = 2;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'F':
                case 'f':
                    key = 3;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'G':
                case 'g':
                    key = 4;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'H':
                case 'h':
                    key = 5;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'J':
                case 'j':
                    key = 6;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'K':
                case 'k':
                    key = 7;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'W':
                case 'w':
                    key = 8;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'E':
                case 'e':
                    key = 9;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'T':
                case 't':
                    key = 10;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'Y':
                case 'y':
                    key = 11;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                case 'U':
                case 'u':
                    key = 12;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
                default:
                    key = 0;
                    keyPress = true;
                    timer1.Enabled = true;
                    currChar++;
                    break;
            }
        }

        private void stopKeys_Click(object sender, EventArgs e)
        {
            //Stop the animation here, and restart the variables of those animations here
            timer1.Enabled = false;
            if (key < 8)
            {
                //White Keys
                blackPlane = 4;
                nextPlane = 4;
                animate_key();
                nextPlane = 3.75;
                amount = -0.25;
                bamount = -0.125;
            }
            else
            {
                //Black Keys
                nextPlaneB = 5;
                animate_bkey();
                amountB = -0.5;
            }

        }
    }

    //This will be used to save the pixels of each key
    class Pixel
    {
        private int j = 0;
        private int i = 0;

        public Pixel(int j, int i)
        {
            this.j = j;
            this.i = i;
        }

        //These functions will return the pixels stored for a specific key
        public int getI()
        {
            return this.i;
        }

        public int getJ()
        {
            return this.j;
        }
    }
}
