# ImageConverter.NET
Simple .NET CLI App to convert images to new formats and resize images.

## Commands
Usage: ImageConverter.NET.exe [command-name]

- [convert-image](#convert-image): Converts an images in given input directory tothe specified format.
- [resize-image](#resize-image): Resizes an image in the given input directory tothe specified dimensions.
- [list-formats](#list-formats): Lists supported image formats.

Options:
  -h, --help    Show help message
  --version     Show version


## Usage

### Convert Image
Usage: ImageConverter.NET convert-image

Converts an images in given input directory to the specified format.

Options:
--input <String>                         Path to the input directory. (Required)
--output <String>                        Path to the output directory. (Required)
--convert-to <String>                    Target format (e.g., 'png'). (Required)
--include-subdirectories=<true|false>    Include subdirectories. (Default: True)
--overwrite                              Overwrite existing files. (Default: False)
--new-width <Int32>                      Target width.
--new-height <Int32>                     Target height.
  

#### Batch example

Minimal usage
```batch
..\ImageConverter.NET.exe convert-image --input "input" --output "output" --convert-to "dds"
```

Extended usage
```batch
..\ImageConverter.NET.exe convert-image --input "input" --output "output" --convert-to "dds" --new-width 44 --new-height 44 --include-subdirectories --overwrite
```

### Resize Image
Usage: ImageConverter.NET resize-image

Resizes an image in the given input directory to the specified dimensions.

Options:
--input <String>     Path to the input directory. (Required)
--output <String>    Path to the output directory. (Required)
--width <Int32>      Target width. (Required)
--height <Int32>     Target height. (Required)
-h, --help           Show help message

Usage
```batch
..\ImageConverter.NET.exe resize-image --input "input" --output "output" --width 44 --height 56
```

### List Formats
Lists supported formats by Magick library to console you can also find supported formats [here](doc/formats.md)

## Things to know
- When input and output directory inputs are not valid app will use input directory and output directory in running directory
- App will accept any formatting as long as it is supported by Magick


## [Supported Formats](doc/formats.md)

## Error Handling
Error handling is very minimal and logging only to console.

App will not exit till user press a key so you can see errors in console.


## Disclaimer
This project uses Magick.NET meaning anyone who wish to use it must comply with their terms of service.

I the owner of this project can not be hold responsible

## Reference
This project uses [Magick.NET](https://github.com/dlemstra/Magick.NET) as a reference to read and write image formats