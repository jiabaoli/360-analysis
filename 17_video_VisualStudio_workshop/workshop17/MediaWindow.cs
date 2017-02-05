using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using C_sawapan_media;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace workshop17
{
    public class MediaWindow
    {
        public int Width = 0;       //width of the viewport in pixels
        public int Height = 0;      //height of the viewport in pixels
        public double MouseX = 0.0; //location of the mouse along X
        public double MouseY = 0.0; //location of the mouse along Y


        // An image object to hold the latest camera frame
        VBitmap Videoimage = new VBitmap(120, 120);
        //this object represents a single video input device [camera or video file]
        VideoIN Video = new VideoIN();


        //initialization function. Everything you write here is executed once in the begining of the program
        public void Initialize()
        {
            //intialize Video capturing from primary camera [0] at a low resolution
            VideoIN.EnumCaptureDevices();
            Video.StartCamera(VideoIN.CaptureDevices[0], 160, 120);
            // Video.StartVideoFile(@"C:\Users\pan\Desktop\out2.avi");
            Video.SetResolution(80, 60);

            //create a sound buffer that can hold 0.3 seconds of sound for playback
            sound = SoundOUT.TheSoundOUT.AddEmptyFreqSample(0.3, 0.3);
            //play the sound once (loop==false). We will change the contents of the sound buffer an dplay it repeatedly to respond to the video movement
            sound.Play(false);
        }

        SoundSampleFreq sound; //sound buffer variable. It is created inside the Initialize() method

        double MotionXSmooth = 0.0; //we'll use these two variables to track a rolling average of the center of movement over time
        double MotionYSmooth = 0.0;
        LinkedList<Vector2d> mpoints = new LinkedList<Vector2d>(); //this is a list in which we are going to add the points of the centroid of movement over time in order to visualize the trajectory


        //animation function. This contains code executed 20 times per second.
        public void OnFrameUpdate()
        {
            if (!Video.IsVideoCapturing) return; //make sure that there is a camera connected and running
            if (Video.NeedUpdate) Video.UpdateFrame(true);  //recalculate the video frame if the camera got a new one


            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, Video.ResX, 0.0, Video.ResY, -1.0, 1.0);

            Videoimage.FromVideo(Video);                    //update the video image and draw it [just for debugging now]
            Videoimage.Draw(0.0, 0.0, Video.ResX, Video.ResY, 0.2);


            //..................................................MAIN CODE AREA
            //draw each video pixel by a point whose size is proportional to the brightness of the pixel
            //Loop through the array of pixels in the Video object. The dimensions of this array are Video.ResX x Video.ResY
            for (int j = 0; j < Video.ResY; ++j)
            {
                for (int i = 0; i < Video.ResX; ++i)
                {
                    GL.PointSize((float)((Video.Pixels[j, i].V) * 20.0));   //set the current point graphic radius to 20 times the brightness of the pixel [i,j]
                    GL.Color4(1.0, 1.0, 1.0, 1.0);                          //set the color to white

                    GL.Begin(PrimitiveType.Points);                         //draw a point at coordinates (i,j) with the above set color and size
                    GL.Vertex2(i, j);
                    GL.End();
                }
            }

            //draw edges and gradient here....

            //visualize movement by taking the difference in brightness between the current frame pixels and that of the previous frame.
            for (int j = 0; j < Video.ResY; ++j)
            {
                for (int i = 0; i < Video.ResX; ++i)
                {
                    double diff = Math.Abs(Video.Pixels[j, i].V - Video.Pixels[j, i].V0);       //compute the absolute value of the difference between the current and previous brightness values of the pixel [i,j]
                    GL.PointSize((float)(diff * 50.0));                                         //visualize the brightness difference as a white point
                    GL.Color4(1.0, 0.0, 0.0, 0.5);

                    GL.Begin(PrimitiveType.Points);
                    GL.Vertex2(i, j);
                    GL.End();
                }
            }

            //visualize the optical flow vector field
            GL.Color4(0.0, 0.0, 0.0, 0.5);  //set current color to transparent black
            GL.LineWidth(1.0f);             //set line width to 1 pixel
            GL.Begin(PrimitiveType.Lines);  //enter line drawing mode
            for (int j = 0; j < Video.ResY; ++j)
            {
                for (int i = 0; i < Video.ResX; ++i)
                {
                    GL.Vertex2(i, j);                       //emit the two line endpoints, from the pixel location p=(i,j) to the endpoint p+movement*50
                    GL.Vertex2(i + Video.Pixels[j, i].mx * 50.0, j + Video.Pixels[j, i].my * 50.0);
                }
            }
            GL.End();                       //end line drawing mode
            

            //Calculate and visualize the centroid of movement and the average direction of motion
            double mx = 0.0;        //movement centroid coordinates
            double my = 0.0;
            double mtotal = 0.0;    //sum of total movement at all pixels
            double avgDX = 0.0;     //average x component of movement
            double avgDY = 0.0;     //average y component of movement

            for (int j = 0; j < Video.ResY; ++j)
            {
                for (int i = 0; i < Video.ResX; ++i)
                {
                    double mmag = Math.Sqrt(Video.Pixels[j, i].mx * Video.Pixels[j, i].mx + Video.Pixels[j, i].my * Video.Pixels[j, i].my); //movement vector magnitude at pixel (i,j)
                    mx += i * mmag;         //the coordinates of the centroid are simply the sum of coordinates of the pixels multiplied by the amount of movement at each pixel
                    my += j * mmag;

                    mtotal += mmag;         //accumulate all the movement values

                    avgDX += Video.Pixels[j, i].mx;  //accumulate the movement compoennt values in order to calculate the average direction of movement
                    avgDY += Video.Pixels[j, i].my;
                }
            }

            mx /= mtotal; //divide the centroid components by the total movement
            my /= mtotal;

            avgDX /= (double)(Video.ResY* Video.ResX); //divide compoennts of movement with total number of pixels in order to compute averages
            avgDY /= (double)(Video.ResY * Video.ResX);

            GL.PointSize((float)(5.0 + mtotal * 0.5));  //draw the movement centroid (mx,my)
            GL.Color4(0.0, 0.0, 0.0, 0.5);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(mx, my);
            GL.End();


            GL.Color4(0.0, 0.0, 0.0, 1.0);  //draw the average movement direction
            GL.LineWidth(5.0f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(mx, my);
            GL.Vertex2(mx + avgDX * 460.0, my + avgDY * 460.0);
            GL.End();


            //compute rolling average of the movement centroid and visualize its trajectory
            MotionXSmooth = MotionXSmooth * 0.9 + mx * 0.1; 
            MotionYSmooth = MotionYSmooth * 0.9 + my * 0.1;


            //draw the rolling average of the centroid, which is less sensitive to abrupt movements and noise
            GL.PointSize(10.0f);
            GL.Color4(0.0, 0.0, 0.0, 1.0);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(MotionXSmooth, MotionYSmooth);
            GL.End();
            
            //add the current centroid coordinates to a list of points that represents the trajectory f the centroid
            mpoints.AddLast(new Vector2d(MotionXSmooth, MotionYSmooth));
            if (mpoints.Count > 500) mpoints.RemoveFirst(); //makje sure the list does not exceed 500 values. If it does remove the first entry so that we don;t end up with an ever increasing list consuming all the memory available

            //draw the centroid trajectory
            GL.Color4(0.0, 0.0, 0.0, 1.0);
            GL.LineWidth(1.0f);
            GL.Begin(PrimitiveType.LineStrip); //start drawing a polyline
            foreach (Vector2d v in mpoints)
            {
                GL.Vertex2(v);
            }
            GL.End(); //end polyline drawing

            //synthesize sound that responds to movement
            if (!sound.IsPlaying) //make sure that the current sound sample has finished playing before overwriting its contents
            {
                sound.SilenceAllFrequencies(); //flatten the spectrum of the sound sample

                double motmag = Math.Sqrt(avgDX * avgDX + avgDY * avgDY); //compute the average movement magnitude
                avgDX /= motmag;                                          //normalize the average movement vector (avgDX, avgDY)
                avgDY /= motmag;

                double vol = motmag;                            
                if (vol > 0.3) vol = 0.3;
                sound.SetFreq(200.0 + (1.0 + avgDY) * 800.0, vol); //add a tone of volume=0.3 and whose frequency depends on the y component of the average movement vector

                sound.BuildSoundSample();                       //reconstruct the sound sample's waveform from the frequency spectrum

                sound.Pan = 2.0 * ((mx / Video.ResX) - 0.5);    //set the pan (left/right speaker volume) to be proportional to the x compoennt of average movement
                sound.Play(false);                              //start playing the new sound
            }


        }
    }
}
