import gab.opencv.*;
import processing.video.*;

OpenCV opencv;
Movie video;
String output = "{";

void setup() {
  size(640, 360);
  video = new Movie(this, "plane-diff-opt.avi");
  video.frameRate(30);
  opencv = new OpenCV(this, 640, 360);
  video.loop();
  video.play();  
}

void draw() {
  background(0);
  opencv.loadImage(video);
  opencv.calculateOpticalFlow();

  image(video, 0, 0);
  //translate(video.width,0);
  stroke(255,0,0);
  strokeWeight(.5);
  opencv.drawOpticalFlow();
  
  PVector aveFlow = opencv.getAverageFlow();
  int flowScale = 10;
  
  println(frameCount + " - aveFlow " + aveFlow);
  
  stroke(255, 255, 100);
  strokeWeight(3);
  line(video.width/2, video.height/2, video.width/2 + aveFlow.x*flowScale, video.height/2 + aveFlow.y*flowScale);
  if(frameCount < 300) {
     saveFrame("plane-diff-opt2-######.png"); 
     output =  output + "'frame-" + frameCount + "' : " + aveFlow + ",";
  } else if (frameCount == 301) {
     println(output); 
  }
}

void movieEvent(Movie m) {
  m.read();
}