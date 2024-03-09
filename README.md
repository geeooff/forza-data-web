# Forza Data Web (_Work in progress_)
Forza Motorsport 7 / Horizon 4 Data-Out feature receiver web application, and some debug tools.

Based on [Turn10 official specifications here](https://forums.forzamotorsport.net/turn10_postst128499_Forza-Motorsport-7--Data-Out--feature-details.aspx).

## Objectives

The idea behind this Web Application is to provide access to Forza Motorsport 7 or Forza Horizon 4 telemetry of a game session to Web users.

Game data reading is made by a Web backend, parsed and routed via WebSocket to an rich UI Web Interface to provide real-time data on a game session.

Several game sessions can be read at same time by the backend, and, of course, several web users can read the same game session data.

## Progress

For the moment, game data can be observed using a simple console.

Samples can be recorded using the sample recorder program, and will be playable later.

No Web interface for the moment.

## Protocol compatibility

| Game               | Sled     | Car Dash | _Horizon_ |
|--------------------|----------|----------|-----------|
| Forza Motorsport 7 | Yes      | Yes      | N/A       |
| Forza Horizon 4    | Yes      | Yes      | No*       |

_*_ Forza Horizon 4 is emitting 13 additional bytes. Only these are ignored.

## How to test

### Prerequisites

1. [Git](https://www.git-scm.com/downloads)
2. [Git LFS](https://git-lfs.github.com)
1. [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)


### Get sources

Clone (using Git) this repository to your Windows, Mac or Linux computer.
You will need to install Git LFS before cloning the repository to get large sample files.

### Enable Data-Out feature in game

Start Forza Motorsport 7 or Forza Horizon 4 on your Windows 10 computer or your Xbox.
Go to game options, then _HUD_ options. At the bottom of the screen :
1. `Data Out IP Address` : enter the IP Address of **the computer that will run the console program**
2. `Data Out IP Port` : enter the network port you want to **listen on this computer** (1024 to 65535)
3. `Data Out Packet Format` : choose **Sled** or **Car Dash**, but you will get more data using Car Dash format. Forza Horizon 4 does not have this option.
4. `Data Out` : set to **ON**

### Run console program

If you're playing on a Windows computer, you have to use another computer to run the console program, or you will receive nothing.

Launch your command line to `Console` source directory.
Then run this command:
```
dotnet run -ServerIpAddress <serverIpAddress> -Port <port>
```

- `-ServerIpAddress` : the IP Address of your Xbox or Computer that runs the game.
- `-Port` : the network port you chose in-game

For example, if your Xbox or Windows Game Computer have 192.168.0.10 IP Address, and you chose 7777 network port to communicate:
```
dotnet run -ServerIpAddress 192.168.0.10 -Port 7777
```

_Note_ : If you want to quit the console program, just hit `CTRL+C` or `CTRL+Break`.

### Record samples

Launch your command line to `SampleRecorder` source directory.
Then run this command:
```
dotnet run -ServerIpAddress <serverIpAddress> -Port <port> -Output <file>
```

**Arguments**

- `-ServerIpAddress` : the IP Address of your Xbox or Computer that runs the game.
- `-Port` : the network port you chose in-game
- `-Output` : the output file to record to

**Example**
```
dotnet run -ServerIpAddress 192.168.0.10 -Port 7777 -Output "Samples\Sample.bin"
```

_Note_ : If you want to quit the recorder program, just hit `CTRL+C` or `CTRL+Break`.
The output file will be deleted if no data is received.