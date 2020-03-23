# Paddle Whacker
A paddle-whacking game built from the ground up in Unity.
A unique take on old style of game, taking elements from pong and air hockey.



Can download and play on itch.io!: https://jeffreypersons.itch.io/paddle-whacker

## Sample Screenshots
### Start menu screen
![image](https://user-images.githubusercontent.com/8084757/77353298-f28bdf00-6cfd-11ea-9506-a630c413fa19.png)

### Game play screen
![image](https://user-images.githubusercontent.com/8084757/77353342-08999f80-6cfe-11ea-9f65-1435332b644a.png)

### Game pause screen
![image](https://user-images.githubusercontent.com/8084757/77353348-0cc5bd00-6cfe-11ea-8098-5c8691cc452f.png)

### Game end screen
![image](https://user-images.githubusercontent.com/8084757/77353357-12bb9e00-6cfe-11ea-8a56-21d0c51af76f.png)


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
