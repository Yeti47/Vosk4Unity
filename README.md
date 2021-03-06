# Vosk4Unity
Vosk4Unity is a module for the Unity Engine that provides a simple way to integrate speech recognition into your application. 
It is based on and powered by the [Vosk speech recognition engine](https://github.com/alphacep/vosk-api).

Vosk4Unity can be used with any Vosk-compatible speech recognition model and even provides an in-editor GUI for downloading and managing models. This allows for a very quick and easy setup.

## Target Platform Support
The current development version only supports Windows x64 as the target build platform. Eventually, there will be two separate releases. One for targeting Windows 64-bit platforms and one for Windows x86 platforms. Any other target platforms, such as Linux, Android or macOS are currently not supported.

If you do need to build for 32-bit Windows clients, you can use the following work-around for now:
- Download the Windows 32-bit release of the official vosk-api [here](https://github.com/alphacep/vosk-api/releases).
- Go into the folder 'Assets/lib' and replace the .dll files with the ones you downloaded
- Recompile
