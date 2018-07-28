# Forza Data Web (_Work in progress_)
Forza Motorsport Data-Out feature receiver web application, and some debug tools.

Based on [Turn10 official specifications here](https://forums.forzamotorsport.net/turn10_postst128499_Forza-Motorsport-7--Data-Out--feature-details.aspx).

## Objectives

The idea behind this Web Application is to provide access to Forza Motorsport 7 telemetry of a game session to Web users.

FM7 data reading is made by a Web backend, parsed and routed via WebSocket to an rich UI Web Interface to provide real-time data on a game session.

Several game sessions can be read at same time by the backend, and, of course, several web users can read the same game session data.

## Progress

For the moment, FM7 data can be observed using a simple console.

No Web interface for the moment.

## How to test

### Prerequisites

1. [.NET Core SDK](https://www.microsoft.com/net/download/windows)
2. Optional: [Git](https://www.git-scm.com/downloads)

### Get sources

Clone (using Git) or download this repository to your Windows, Mac or Linux computer.

### Enable Data-Out feature in game

Start Forza Motorsport 7 on your Windows 10 computer or Xbox.
Go to _HUD_ options, then, at the bottom :
1. Enter the IP Address of **the computer that will run the console program**
2. Enter the network port you want to **listen on this computer** (1024 to 65535)
3. Set _Data Out_ to **ON**

### Run console program

If you're playing on a Windows computer, you have to use another computer to run the console program, or you will receive nothing.

Launch your command line to `Console` directory of sources.
Then run this command:
```
dotnet run -Port <port> -ServerIpAddress <serverIpAddress>
```

Replace `<port>` with the network port you chose in-game and `<serverIpAddress>` with the IP Address of your Xbox or Computer that runs the game.

For example, if your Xbox or Windows Game Computer have 192.168.0.10 IP Address, and you chose 7777 network port to communicate:
```
dotnet run -Port 7777 -ServerIpAddress 192.168.0.10
```

You should observe data now.

If you want to quit the console program, just press `CTRL+C` or `CTRL+Break`.