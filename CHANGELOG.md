# CHANGELOG

## [2.0.0] – 2025‑10‑12

### Major Update

**ImageConverter.NET v2.0.0** introduces a complete modernization of the app — faster, testable, observable.

---

### Added

- **Parallelism Control**
  - New option `--max-parallelism` to control thread concurrency.
  - Defaults to the number of logical processor cores.

- **Summary Export**
  - New option `--summary <path>` outputs a structured JSON summary:
    - Total files processed
    - Success/failure counts
    - Processing duration
    - Detailed error list

- **Serilog Logging System**
  - Switched to structured logging with colorized console output.
  - Added support for `--file-log` with rolling file output (10 MB, daily rotation).
  - Runtime control of verbosity via `--log-level`.

- **Dependency Injection & Interfaces**
  - Introduced `IImageConverterNet` for cleaner architecture and testability.
  - Removed static logic; services are fully injectable.

- **Result Models**
  - Added `ImageConversionResult` and `ImageProcessError` models for internal logging and testing.

---

### Changed

- `ImageConversionManager` replaced by modern `ImageConverterNet` class.
- Commands refactored to leverage DI and structured logging.
- Significantly improved console output consistency.

---

### Removed

- Legacy `ConsoleLogger`
- Static method access patterns (replaced by DI)
- Redundant sync I/O paths

---

### Example

```bash
ImageConverter.NET.exe convert-image --input in --output out --convert-to png --max-parallelism 4 --summary "summary.json" --log-level Debug
```
