import gab.opencv.*;
import processing.video.*;

Movie video;
OpenCV opencv;
boolean firstRun = true;
int ind = 0;
int offset = 1;
float alpha = 1;
void setup() {
  size(1280, 720);
  video = new Movie(this, "bjork.avi");
  opencv = new OpenCV(this, 1280, 720);
 
  opencv.startBackgroundSubtraction(5, 3, 0.5);

  video.loop();
  video.play();
}

void draw() {
  //image(video, 0, 0); 
 
  opencv.loadImage(video);
  
  opencv.updateBackground();
  
  opencv.dilate();
  opencv.erode();

  noFill();
  stroke(255, 0, 0);
  strokeWeight(3);
  for (Contour contour : opencv.findContours()) {
    //stroke(0, 255, 0);
   // contour.draw();
    
    stroke(255, 0, 0, alpha);
   // strokeWeight(alpha);
    beginShape();
    for (PVector point : contour.getPolygonApproximation().getPoints()) {
      vertex(point.x, point.y);
    }
    endShape();
  }
  
  
  if(firstRun) {
    saveFrame("bjork-######.png");
     alpha+=1;
  }
  
  ind++;
  if(ind+offset > 80) {
     ind = 0; 
     println("loop");
     firstRun = false;
  }
  //delay(10);
  
}

void movieEvent(Movie m) {
  m.read();
}