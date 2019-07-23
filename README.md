# Steganography
[![Github All Releases](https://img.shields.io/github/downloads/PapyrusCompendium/Steganography/total.svg)]()
[![Github Issues](https://img.shields.io/github/issues/PapyrusCompendium/Steganography.svg)]()
[![Github Stars](https://img.shields.io/github/stars/PapyrusCompendium/Steganography.svg)]()
[![Github License](https://img.shields.io/github/license/PapyrusCompendium/Steganography.svg)]()

This is my own implementation of steganography, using a LSB (Least Significant Bit) method.  
The way this works is by taking the LSB for every byte in a pixel (There are four in this case ARGB)  
and changing the LSB to one bit of the data that you are trying to store. For example if I have  
one byte of data I would like to store, I could store it in two pixels. I can store 4 bits  
in one pixel without modifying the image enough that it is visible to the naked eye. In this  
implementation I am only storing string data, but this can be made to store anything from files  
to other images completely, like QR codes.  
[![Tool Screenshot](https://github.com/PapyrusCompendium/Steganography/blob/master/ScreenShot.png)]()

# How do I use this?
This version of my implementation is built for user advantage. I wanted this to be easy to use by  
anyone. Simply launch the executable (either compile or download the release) and select an input  
image. You can now select if you would like to use AES cryptography. If you choose to use AES, then  
make sure to enter an AES key! Now you can select convert, and process the image. At the point you  
convert the image you will see another image appear on the right hand side of the menu. The image  
on the right side is the encoded image, you can choose to download this from here. To decode the  
image simply follow the same process but selecting the decode radio button on the top, and not  
the encode radio button.

# Bugs and issues
If you encounter and bugs or issues, feel free to open an issue on GitHub. I will address these issues as soon as I can.
