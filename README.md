# Paddle Whacker
A paddle whacking retro arcade game.

Features a [custom-made AI using prediction based forecasting](https://github.com/jeffreypersons/Paddle-Whacker#how-the-ai-work), touch support and a variety of different difficulty levels, as well as game length and sound settings.

Can play in-browser at itch.io: https://jeffreypersons.itch.io/paddle-whacker

Can download for Android at: https://play.google.com/store/apps/dev?id=8132485239530785081)


## Background
Tired by the lack of originality found in most 'hit the ball with a paddle' genre of 2d games, Jeff wondered how it could be improved on... Taking those ideas, he came up with _Paddle Whacker_ - a game that blends the vertical-only movement of Pong with an Air-Hockey inspired arena.


## Sample Screenshots
### Main menu screen
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/paddle%20whacker2%20-%20main%20menu.png)

### Settings panel in main menu
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/paddle%20whacker2%20-%20sound%20panel.png)

### Create game panel in main menu
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/paddle%20whacker2%20-%20start%20panel.png)

### Game play screen
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/v2%20gameplay.png)

### Game pause screen
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/v2%20pausemenu.png)

### Game end screen
![image](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddlewhacker-screenshots/v2%20endmenu.png)


## How the AI works

![paddle zones](https://user-images.githubusercontent.com/8084757/76711997-e6869880-66d1-11ea-8506-23550d968857.png)
AI behavior is now primarily driven by `PaddleZoneIntersection` events - triggered when a `Ball` intersects a `PaddleZone` (`BoxCollider2D` encapsulating each paddle, stretched from bottom to top of arena, as shown below).

The last hit zone gives us enough data to decide whether the ball is behind the AI paddle or not, and how to react to it.


### Implementation details for nerds:
* Each frame, Ai is always moving towards or present at `paddleTargetY`
* Upon `Ball` intersecting a `PaddleZone`, ball entry/exit position/velocity is recorded and event is triggered, received by AI, and used to infer the state of ball relative to paddle as follows:
  * _IF `ballIncoming` (approaching ai from opponents side)_: PredictBallPosition (target set to projected position)
  * _IF `ballBehind` (passed ai paddle next to goal)_: TryToHitBallFromHorizontalEdge (target set to avoid ball or hit it on the top/bottom paddle edge, if possible, as based on the projected position)
  * _ELSE (all other cases)_: TrackBall (vertically aligns the paddle with ball y position)
  
![ai trajectory prediction demo](https://raw.githubusercontent.com/jeffreypersons/Jeff-Images/master/paddle%20whacker%20improved%20ai.gif)

Basically, the ai runs in a seperate coroutine launched (after a response time delay) when its opponent (ie the player) hits the ball, and then approachs the point that the trajectory intersects with the x position of the ai's paddle.
