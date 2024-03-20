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

### Sled vs Car Dash

_Sled_ data was initially for SimRacing motion platforms, so it has barely more than just movement data.

_Car Dash_ extends _Sled data_ with dashboard related metrics, like speed, gear, accelerator, brake, etc.

To see all available data fields, please have a look on the data structure files:
- [Sled structure](Core/ForzaSledDataStruct.cs)
- [Car Dash structure](Core/ForzaCarDashDataStruct.cs)
- [Horizon extras structure](Core/ForzaCarDashDataStruct.cs)
- [Motorsport extras structure](Core/ForzaCarDashDataStruct.cs)

### Game Support

| Game                    | Sled     | Car Dash | Horizon extras | Motorsport extras |
|-------------------------|----------|----------|----------------|-------------------|
| Forza Motorsport 7      | Yes      | Yes      | N/A            | N/A³              |
| Forza Horizon 4         | Yes¹     | Yes      | Partial²       | N/A               |
| Forza Horizon 5         | Yes¹     | Yes      | Partial²       | N/A               |
| Forza Motorsport (2023) | Yes      | Yes      | N/A            | Yes⁴              |

1. Forza Horizon enforces Car Dash data type
2. Forza Horizon extra data is not documented. Only the car category (from community assumptions) is decoded. Thanks to them !
3. Forza Motorsport extra data was introduced in 2023 edition, FM7 doesn't emit such data
4. Forza Motorsport (2023) extra data exposes tire wear and track ID

## How to run

Releases in GitHub are not there yet, but it's really easy to build yourself.

### Prerequisites

1. [Git](https://www.git-scm.com/downloads)
2. [Git LFS](https://git-lfs.github.com)
3. [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)

### Get sources

Clone (using Git) this repository to your Windows, Mac or Linux computer.

```shell
git clone https://github.com/geeooff/forza-data-web.git
cd forza-data-web
```

### Enable Data-Out feature in game

> **Note for Windows as both game and receiving app**:
> Only Forza Motorsport (2023) allows to send data to 127.0.0.1.
> For earlier games you will need a separate compute to receive the data.

Start the game on your Windows computer or your Xbox.
Go to game options and look for _Data Out_ options :
1. `Data Out IP Address` : enter the IP Address of **the computer that will run the console program**
2. `Data Out IP Port` : enter the network port you want to **listen on this computer** (1024 to 65535)
3. `Data Out Packet Format` : choose **Sled** (if available) or **Car Dash**
4. `Data Out` : set to **ON**

### Run console program

Launch your command line to `Console` source directory.
Then run this command:
```shell
dotnet run --server <serverIpAddress> --port <port>
```

- `-s` or `--server` : the IP Address of your Xbox or Computer that runs the game
- `-p` or `--port` : the network port you chose in the game

For example, if your Xbox or Windows Game Computer has 192.168.0.100 IP Address, and you chose 7777 network port to communicate:
```shell
dotnet run --server 192.168.0.100 --port 7777
```

The console will show `RACE` or `PAUSE` in the top left corner is race is on or not.

![image](docs/assets/screenshots/console.png)

_Note_ : If you want to quit the console program, just hit `CTRL+C` or `CTRL+Break`.

### Record samples

Launch your command line to `SampleRecorder` source directory.
Then run this command:
```shell
dotnet run --server <serverIpAddress> --port <port> --output <file>
```

**Arguments**

- `-s` or `--server` : the IP Address of your Xbox or Computer that runs the game
- `-p` or `--port` : the network port you chose in-game
- `-o` or `--output` : the output file to record to

**Example**
```shell
dotnet run --server 192.168.0.100 --port 7777 --output sample.bin
```

_Note_ : If you want to quit the recorder program, just hit `CTRL+C` or `CTRL+Break`.
The output file will be deleted if no data is received.