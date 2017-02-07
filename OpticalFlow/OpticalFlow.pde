import gab.opencv.*;
import processing.video.*;

OpenCV opencv;
Movie video;
String output = "{";

void setup() {
  size(1280,720);
  video = new Movie(this, "chute.mp4");
  opencv = new OpenCV(this, 1280, 720);
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
  opencv.drawOpticalFlow();
  
  PVector aveFlow = opencv.getAverageFlow();
  int flowScale = 10;
  
  println(frameCount + " - aveFlow " + aveFlow);
  
  stroke(255, 255, 100);
  strokeWeight(1);
  line(video.width/2, video.height/2, video.width/2 + aveFlow.x*flowScale, video.height/2 + aveFlow.y*flowScale);
  if(frameCount < 300) {
     saveFrame("lion-######.png"); 
     output =  output + "'frame-" + frameCount + "' : " + aveFlow + ",";
  } else if (frameCount == 301) {
     println(output); 
  }
}

void movieEvent(Movie m) {
  m.read();
}