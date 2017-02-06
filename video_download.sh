
#  download youtube playlist
##
## for every item in the list,
## only download the single best file that has video & sound
####
#### playlist url: "mecha_01 vr"
######
youtube-dl -citkvf best https://www.youtube.com/playlist?list=PLrOsmPkJI7vQBRWZcSbNKDKjLqNjrovCK



#  extract 1 frame per second from vide0
##
##
##
####
####
######
ffmpeg -i foo.avi -r 1 -s -f image2 foo-%03d.jpeg



#  extract images from video
##
## extract 1 frame per second from each video in directory and saves it to a folder
##
####
####
######
See extractvid.sh

# call using
./extractvid.sh <input folder> <output folder> <file extension>

# e.g.
./extractvid.sh videos images mp4
