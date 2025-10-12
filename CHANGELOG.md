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