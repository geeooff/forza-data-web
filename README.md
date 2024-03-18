# Forza Data Web (_Work in progress_)

Forza Motorsport and Horizon Data-Out feature receiver web application, and some debug tools.

Based on Turn10 official documentation:
https://support.forzamotorsport.net/hc/en-us/articles/21742934024211-Forza-Motorsport-Data-Out-Documentation

## Objectives

The idea behind this Web Application was to provide web visitor access to Forza Motorsport or Forza Horizon telemetry from live, or recorded, game session.

Game data reading is made by a Web backend, parsed and routed via WebSocket to an rich UI Web Interface to provide real-time data on a game session.

Several game sessions can be read at same time by the backend, and, of course, several web users can read the same game session data.

## Progress

For the moment, game data can be observed using a simple console.

Samples can be recorded using the sample recorder program, and will be playable later.

No Web interface for the moment, only a placeholder razor app.

## Protocol compatibility

| Game                    | Sled     | Car Dash | Horizon specific | Motorsport specific |
|-------------------------|----------|----------|------------------|---------------------|
| Forza Motorsport 7      | Yes      | Yes      | N/A              | N/A                 |
| Forza Horizon 4         | Yes      | Yes      | Partial *        | N/A                 |
| Forza Horizon 5         | Yes      | Yes      | Partial *        | N/A                 |
| Forza Motorsport (2023) | Yes      | Yes      | N/A              | Yes                 |

_* Forza Horizon specific data is not documented. Only the car category (from community assumptions) is decoded, so thanks to them !_

## How to test

### Prerequisites

1. [Git](https://www.git-scm.com/downloads)
2. [Git LFS](https://git-lfs.github.com)
3. [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)

### Get sources

Clone (using Git) this repository to your Windows, Mac or Linux computer.
You will need to install Git LFS before cloning the repository to get large sample files.

### Enable Data-Out feature in game

> **Note for Windows as both game and receiving app**:
> Only Forza Motorsport (2023) allows to send data to 127.0.0.1.
> For earlier games you will need a separate compute to receive the data.

Start the game on your Windows computer or your Xbox.
Go to game options and look for _Data Out_ options :
1. `Data Out IP Address` : enter the IP Address of **the computer that will run the console program**
2. `Data Out IP Port` : enter the network port you want to **listen on this computer** (1024 to 65535)
3. `Data Out Packet Format` : choose **Sled** or **Car Dash**. You will get more data using Car Dash, if available.
4. `Data Out` : set to **ON**

### Run console program

Launch your command line to `Console` source directory.
Then run this command:
```
dotnet run --server <serverIpAddress> --port <port>
```

- `-s` / `--server` : the IP Address of your Xbox or Computer that runs the game.
- `-p` / `--port` : the network port you chose in-game

For example, if your Xbox or Windows Game Computer have 192.168.0.100 IP Address, and you chose 7777 network port to communicate:
```
dotnet run --server 192.168.0.100 --port 7777
```

_Note_ : If you want to quit the console program, just hit `CTRL+C` or `CTRL+Break`.

### Record samples

Launch your command line to `SampleRecorder` source directory.
Then run this command:
```
dotnet run --server <serverIpAddress> --port <port> --output <file>
```

**Arguments**

- `-s` or `--server` : the IP Address of your Xbox or Computer that runs the game.
- `-p` or `--port` : the network port you chose in-game
- `-o` or `--output` : the output file to record to

**Example**
```
dotnet run --server 192.168.0.100 --port 7777 --output sample.bin
```

_Note_ : If you want to quit the recorder program, just hit `CTRL+C` or `CTRL+Break`.
The output file will be deleted if no data is received.