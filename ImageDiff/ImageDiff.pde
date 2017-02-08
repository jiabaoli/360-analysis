 import gab.opencv.*;

OpenCV opencv;
PImage[] frames;
PImage output;
int ind = 0;
int offset = 1;
boolean firstRun = true;

//PImage colorDiff;
void setup() {
  String path = sketchPath("")+"img/bjork/"; 
  File[] files = listFiles(path);
  frames =new PImage[files.length];
  size(1280,720);

  print(path+"\n"); 
  print(files.length+"\n"); //how many files are here
  println();
  
  //opencv = new OpenCV(this, before);    
  //opencv.diff(after);
  //grayDiff = opencv.getSnapshot(); 
  
  for(int i=0;i<files.length;i++) {
    //println(files[i]);
    opencv = new OpenCV(this, loadImage(files[i].getAbsolutePath()) );
    opencv.adaptiveThreshold(591,10);
    //opencv.threshold(10);
    frames[i]= opencv.getOutput();
  }
  
  println("*******opencv done*********");
}

void draw() {
  if(ind%offset == 0 && ind+offset < frames.length) {
    opencv = new OpenCV(this, frames[ind]);  
    //opencv.threshold(2);
    opencv.diff(frames[ind+offset]);
    output = opencv.getSnapshot();
    
    pushMatrix();
    scale(0.5);
    image(frames[ind], 0, 0);
    image(frames[ind + offset], frames[ind].width, 0);
//  image(colorDiff, 0, before.height);
    image(output, frames[ind].width, frames[ind].height);
    popMatrix();
  }
  
  if(firstRun) {
    saveFrame("people-######.png");
  }
  
  ind++;
  if(ind+offset > frames.length) {
     ind = 0; 
     println("loop");
     firstRun = false;
  }
  delay(10);
}