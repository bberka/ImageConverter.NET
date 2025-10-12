# CHANGELOG

## [2.0.0] – 2025‑10‑12

### Overview

A major modernization of **ImageConverter.NET** — faster, cleaner, and easier to test and automate.

---

### Added

- **Parallelism Control:** `--max-parallelism` to limit concurrent processing.
- **Summary Export:** `--summary <path>` outputs a JSON report (processed, success, failed, duration, errors).
- **Serilog Logging:** structured, colorized output with `--log-level` and optional `--file-log`.
- **DI & Interface:** introduced `IImageConverterNet` for dependency injection and testing.
- **Result Models:** added `ImageConversionResult` and `ImageProcessError`.

---

### Changed

- Replaced `ImageConversionManager` with modern `ImageConverterNet`.
- Refactored commands for DI and Serilog logging.
- Improved path handling and extension parsing.
- Added single‑file `Convert` and `Resize` methods.
- Enhanced parallel stats tracking and error reporting.

---

### Removed

- Legacy `ConsoleLogger` and static method usage.
- Redundant I/O and hard‑coded paths.

## [1.0.1] – 2024‑02‑27

### Overview

Minor feature update extending the `resize-image` command for better control over batch processing.

---

### Added

- **`--include-subdirectories=<true|false>`**– Process images in subfolders. *(Default:true)*
- **`--overwrite=<true|false>`**– Allow overwriting existing files. *(Default:false)*

---

### Notes

- Both options align `resize-image` behavior with `convert-image`.
- No breaking changes; existing commands continue to work as before.

## [1.0.0] – Initial Release

### Overview

First fully working release of **ImageConverter.NET**, a cross‑platform .NET CLI tool for image format conversion and
resizing.

---

### Features

- **Command‑line interface** built with Cocona.
- **Magick.NET** integration for reliable image read/write.
- **Core Commands:**
    - `convert-image`– Convert images in a directory to a specified format.
    - `resize-image`– Resize images in a directory to set dimensions.
    - `list-formats`– Display all supported image formats.

---

### Notes

- Default input/output folders are auto‑created if not provided.
- Minimal error handling with console output.
- Runs on Windows, Linux, and macOS.