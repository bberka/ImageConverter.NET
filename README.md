# ImageConverter.NET
Simple console application and library project for converting images to different formats
Mostly supported DDS,PNG,WEBP conversions however other conversions can be added easily to library project.

## Usage
Simply open the app and provide necessary inputs through console

It will ask you input folder, output folder, output directory

All the image files inside input folder that is in supported format will be converted to desired format

By default it will use input and output folder next to exe file you can keep using default folders by simply skipping by enter

## Supported Formats
1. Png
2. Jpeg
3. Jpg
4. Bmp
5. WebP
6. Dds

## Error Handling
Console application has very little error handling it wont crash (most likely) it will print error message and wait for user input to exit

## Contributing 
Project is open source and open to pull requests and forks

If you want you can use this project as example and make one for yourself

You can simply add new formats to supported formats and pull request or fork the project to alter it with your needs

## Disclaimer
This project uses Magick.NET meaning anyone who wish to use it must comply with their terms of service.

I the owner of this project can not be hold responsible

## Reference
This project uses [Magick.NET](https://github.com/dlemstra/Magick.NET) as a reference to read and write image formats
