#!/bin/bash
#extractvid.sh
if [ "$1" == '' ] || [ "$2" == '' ] || [ "$3" == '' ]; then
    echo "Usage: $0 <input folder> <output folder> <file extension>";
    exit;
fi
for file in "$1"/*."$3"; do
    destination="$2${file:${#1}:${#file}-${#1}-${#3}-1}";
    ffmpeg -i "$file" -ss 30 -t 10 "$destination-%d.jpg";
done
