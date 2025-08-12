# CentralLogStreamHandler

CentralLogStreamHandler is a Windows Service for centralized, concurrent log stream handling via Inter-Process Communication (IPC) using named pipes. It efficiently collects log messages from multiple sources and writes them to specified log files, supporting batching and asynchronous file I/O for high performance.

## Features
- Centralized log collection via named pipes
- Supports up to 10 concurrent IPC connections (configurable)
- Asynchronous, batched log writing for performance
- Customizable log file paths and log types
- Robust error handling and fallback logging

## Requirements
- Windows OS
- .NET Framework 4.8.1
- Administrator privileges to install/start Windows services

## Installation
1. Clone this repository:
   ```sh
git clone https://github.com/yourusername/CentralLogStreamHandler.git
```
2. Open the solution in Visual Studio.
3. Build the project (ensure .NET Framework 4.8.1 is installed).
4. Install the service using the command line (run as Administrator):
   ```sh
sc create CentralLogStreamHandler binPath= "<path-to-built-exe>"
```
   Or use InstallUtil:
   ```sh
InstallUtil.exe <path-to-built-exe>
```
5. Start the service:
   ```sh
sc start CentralLogStreamHandler
```

## Usage
- Send log messages to the service via named pipe `CentralLogStreamHandler`.
- Log messages must be in JSON format, e.g.:
  ```json
  {
    "log": "Your log message",
    "log_path": "C:\\Logs\\app.log",
    "LogType": "Info"
  }
  ```
- Ensure all backslashes in paths are escaped (`\\`).
- The service writes logs asynchronously and batches entries for efficiency.

## Configuration
- Change `MaxConcurrentConnections` and `DefaultLogFilePath` in `Servicemain.cs` as needed.
- Log files are created automatically if they do not exist.

## Logging Details
- All log entries are timestamped and include log type.
- Critical errors and JSON parse failures are logged to the default log file.

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License
This project is licensed under the MIT License.

## Contact
For questions or support, open an issue on GitHub or contact the maintainer.
