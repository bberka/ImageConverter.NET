# ImageConverter.NET

A modern **.NET 9 CLI** tool for converting and resizing images using **Magick.NET**.  
It’s designed for developers who need an automated, scriptable, and observable image processing utility.

---

## Overview

**Key Features:**

- Convert or resize images in bulk
- Parallelized image processing for fast performance
- Structured logging with **Serilog** (console + file support)
- JSON export summaries for automation & testing
- Built with **Cocona** for beautiful CLI UX
- Powered by **Magick.NET** for format compatibility

---

## Global Options

| Option                      | Description                                                                                                                    |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| `--log-level <Level>`       | Sets the logging verbosity. _(Default: Information)_<br>Values: `Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal` |
| `--file-log <Path>`         | Enables file logging. Rolls daily, 10 MB max per file.                                                                         |
| `--max-parallelism <Int32>` | Limits image processing threads. _(Default: number of processors)_                                                             |
| `--summary <Path>`          | Exports operation statistics in JSON format.                                                                                   |

**Example:**

```bash
ImageConverter.NET.exe convert-image --input input --output output --convert-to webp --max-parallelism 4 --summary "reports/run.json" --file-log "logs/app.log"
```

---

## Commands

| Command                           | Description                                       |
| --------------------------------- | ------------------------------------------------- |
| [`convert-image`](#convert-image) | Converts images in a directory to another format. |
| [`resize-image`](#resize-image)   | Resizes images across a directory.                |
| [`list-formats`](#list-formats)   | Lists supported Magick.NET formats.               |
| `help`                            | Displays help information.                        |
| `version`                         | Displays version information.                     |

---

## Convert Image

```
Usage: ImageConverter.NET.exe convert-image [options]
```

Converts images in a folder to a specified format.

**Options:**

| Option                                  | Description                                          |
|-----------------------------------------| ---------------------------------------------------- |
| `--input <String>`                      | Path to the input directory. **(Required)**          |
| `--output <String>`                     | Path to the output directory. **(Required)**         |
| `--convert-to <String>`                 | Target format (e.g., png, jpg, webp). **(Required)** |
| `--include-subdirectories=<true,false>` | Include subdirectories. _(Default: true)_            |
| `--overwrite=<true,false>`              | Overwrite existing files. _(Default: false)_         |
| `--new-width <UInt32>`                  | Resize width (optional).                             |
| `--new-height <UInt32>`                 | Resize height (optional).                            |
| `--max-parallelism <Int32>`             | Limit concurrent conversions.                        |
| `--summary <Path>`                      | Export operation summary file.                       |
| `--log-level <Level>`                   | Override global logging level.                       |
| `--file-log <Path>`                     | Enable file logging.                                 |

### Convert Examples

Minimal:

```bash
ImageConverter.NET.exe convert-image --input in --output out --convert-to png
```

Advanced:

```bash
ImageConverter.NET.exe convert-image --input in --output out --convert-to jpg --new-width 128 --new-height 128 --overwrite --max-parallelism 4 --summary "reports/convert-summary.json" --file-log "logs/conversion.log"
```

---

## Resize Image

```
Usage: ImageConverter.NET.exe resize-image [options]
```

Resizes images to the given dimensions.

**Options:**

| Option                                  | Description                                  |
| --------------------------------------- | -------------------------------------------- |
| `--input <String>`                      | Input directory. **(Required)**              |
| `--output <String>`                     | Output directory. **(Required)**             |
| `--width <Int32>`                       | Target width. **(Required)**                 |
| `--height <Int32>`                      | Target height. **(Required)**                |
| `--include-subdirectories=<true,false>` | Include subfolders. _(Default: true)_        |
| `--overwrite=<true,false>`              | Overwrite existing files. _(Default: false)_ |
| `--max-parallelism <Int32>`             | Max threads for processing.                  |
| `--summary <Path>`                      | Export run summary JSON.                     |
| `--log-level <Level>`                   | Adjust verbosity.                            |
| `--file-log <Path>`                     | Record logs to file.                         |

### Resize Examples

```bash
ImageConverter.NET.exe resize-image --input in --output out --width 1024 --height 768 --include-subdirectories --summary "reports/resize-summary.json"
```

---

## List Formats

Lists all formats supported by **Magick.NET**.

```bash
ImageConverter.NET.exe list-formats
```

or with detailed logging:

```bash
ImageConverter.NET.exe list-formats --log-level Debug
```

A static list is also available at [doc/formats.md](doc/formats.md).

---

## Summary Export Example

When using `--summary reports/run.json`, a file like this is created:

```json
{
  "TotalFiles": 96,
  "Succeeded": 94,
  "Failed": 2,
  "ElapsedSeconds": 3.42,
  "Errors": [
    { "File": "broken.bmp", "Message": "Unsupported format" },
    { "File": "corrupt.jpg", "Message": "Invalid file header" }
  ]
}
```

Perfect for CI/CD and test verification.

---

## Notes

- Default parallelism = number of logical CPU cores.
- Operations are non‑blocking and run in parallel with safe exception handling.
- Invalid paths cause automatic fallback to local `input` and `output` directories.
- The application supports all **Magick.NET** file formats.

---

## Logging

| Target                    | Description                                          |
| ------------------------- | ---------------------------------------------------- |
| **Console**               | Always active. Color‑coded output.                   |
| **File (`--file-log`)**   | Optional. Daily rolling files (10 MB max).           |
| **Summary (`--summary`)** | Optional. JSON operation results for analysis/tests. |

Default log level: `Information`

---

## Error Handling

- Errors during processing are logged but **do not stop execution**.
- Full context is preserved in exported JSON summaries.
- Failed files are listed explicitly with messages.

---

## Disclaimer

This software uses **Magick.NET**.  
You must comply with **Magick.NET’s** licenses and usage terms.  
The developer of ImageConverter.NET is not responsible for third‑party license violations.

---

## References

- [.NET 9](https://dotnet.microsoft.com/)
- [Cocona](https://github.com/mayuki/Cocona)
- [Serilog](https://serilog.net/)
- [Magick.NET](https://github.com/dlemstra/Magick.NET)
