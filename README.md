# beachstickball  
A 2-player LAN multiplayer game where the goal is to keep the ball from hitting the ground.  

## Demo  
[![Watch the demo](https://www.youtube.com/watch?v=ofU3sq9DWwA)](https://www.youtube.com/watch?v=ofU3sq9DWwA)

## Concept  
This game was made with Unity for the multiplayer project for Prof. Fodor's 3D game programming class. The game is based on a minigame found in A Short Hike with the same name. This is my attempt at recreating the minigame but with multiplayer involved. 

## Features  
- Multiplayer with Netcode for Game Objects  
- UI for hosting and joining a game within a LAN  
- Ball physics synced server-side  
- A cool peanut that tracks your score and follows the ball  

## How to Run  
- Clone the project
- Run and build the project in Unity twice - one for the host and another for the client
- Setup host instance FIRST: press host button and note IP address in bottom left
- Join with client instance SECOND: input said IP address in input field and press client button
- Read instructions and play the game

- If all else fails, I've also added the [build file here](https://drive.google.com/drive/folders/1HS45Z8PUpDI1Ryit6QgYP9z1vROsJKTy?usp=sharing). This contains the .exe to run the application - it's titled "multiplyer.exe". Run it twice - for host and client - and follow instructions for setup above.

## NetCode from Unity tutorial:  
* For multiplayer tutorial: https://learn.unity.com/tutorial/get-started-with-netcode-for-gameobjects#66881213edbc2a14bb67be9e
* For spawning multiple player prefabs: https://docs-multiplayer.unity3d.com/netcode/current/basics/object-spawning/
