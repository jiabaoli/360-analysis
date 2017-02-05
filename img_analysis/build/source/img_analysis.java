import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import java.io.File; 
import hypermedia.video.*; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class img_analysis extends PApplet {

/*
** img_analysis.pde
**
**
*/




OpenCV opencv;

File dir;
File[] files;
IntDict videoname;

public void setup()
{
    
    noLoop();
    dir = new File("/Users/adampere/Dropbox (MIT)/youtube_360/images");
    files = dir.listFiles();
    videoname = new IntDict();

    println("img directory: " + dir.getAbsolutePath());
    println(files.length + " imgs");

    for(int i = 0; i < files.length; i++){
      String f = files[i].getAbsolutePath();
      //println(f);

      if(f.indexOf(".DS_Store") < 0) {
        PImage image = loadImage(f);
        image(image, 0, 0, 1280, 720);
        tint(255, 55);
        //println(f);

        f = f.substring(f.lastIndexOf("/")+1, f.lastIndexOf("-"));
        if(!videoname.hasKey(f)){
            videoname.set(f, 1);
        } else {
          videoname.increment(f);
        }
      }
    }

    String[] keys = videoname.keyArray();
    int min = videoname.get(keys[0]);
    int max = 0;
    int avg = 0;

    println("-----key count-----");
    for(int i = 0; i < keys.length; i++){
      int tv = videoname.get(keys[i]);

      if(tv < min) {
        min = tv;
      }
      if(tv > max){
        max = tv;
      }
      avg += tv;

      println(keys[i] + " : " + tv);
    }
    println("-------------------");

    avg = avg/keys.length;

    println("----file counts----");
    println("min: " + min);
    println("max: " + max);
    println("avg: " + avg);
    println("-------------------");





}
  public void settings() {  size(1280, 720); }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "img_analysis" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
