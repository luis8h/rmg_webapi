import os
import sys
from PIL import Image

# this script converts all images in the specified path to webp but still keeps
# the original files using the original_ prefix

if (len(sys.argv) != 2):
    print("path not set ... give path to images as command line argument")
    exit(1)

path = sys.argv[1]

for filename in os.listdir(path):
    print("conterting: " + filename)

    # rename original files
    fullname_old = path + "/" + filename
    fullname_orig = path + "/" + "original_" + filename
    os.rename(fullname_old, fullname_orig);

    # convert to webp files
    fullname_conv = os.path.splitext(fullname_old)[0] + ".webp"
    image = Image.open(fullname_orig).convert("RGB")
    image.save(fullname_conv, "webp")

