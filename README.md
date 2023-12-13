# ImageConverter.NET
Simple console application and library project for converting some types of images to other types. 
Mostly supported DDS,PNG,WEBP conversions however other conversions can be added easily to library project.

## Usage
1. App will ask you to enter a choice of conversion
2. You have to enter input directory, if you leave it blank it will use "input" directory in current directory
3. You have to enter output directory, if you leave it blank it will use "output" directory in current directory
4. App will check if required files exists in input folder
5. App will check if items with same name exists in output folder. If it does it will give error
6. Then will go proceed with the action and convert the images
7. When conversion is done it will print how many files is converted 

## Supported Conversion
1. Convert DDS to PNG
2. Convert PNG to DDS
3. Convert WEBP to PNG
4. Convert WEBP to DDS

## Reference
This project uses [Magick.NET](https://github.com/dlemstra/Magick.NET) as a reference to read and write image formats
