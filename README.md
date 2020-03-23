# Paddle Whacker
A paddle-whacking game built from the ground up in Unity.
A unique take on old style of game, taking elements from pong and air hockey.



Can download and play on itch.io!: https://jeffreypersons.itch.io/paddle-whacker

## Sample Screenshots
### Start menu screen
![image](![image](https://user-images.githubusercontent.com/8084757/77353298-f28bdf00-6cfd-11ea-9506-a630c413fa19.png)

### Game play screen
![image](https://user-images.githubusercontent.com/8084757/77353342-08999f80-6cfe-11ea-9f65-1435332b644a.png)

### Game pause screen
![image](https://user-images.githubusercontent.com/8084757/77353348-0cc5bd00-6cfe-11ea-8098-5c8691cc452f.png)

### Game end screen
![image](https://user-images.githubusercontent.com/8084757/77353357-12bb9e00-6cfe-11ea-8a56-21d0c51af76f.png)

## How the AI works

![ai trajectory prediction demo](https://user-images.githubusercontent.com/8084757/74257777-265cf900-4caa-11ea-99fc-17729a5928f3.gif)

Basically, the ai runs in a seperate coroutine launched (after a response time delay) when its opponent (ie the player) hits the ball, and then approachs the point that the trajectory intersects with the x position of the ai's paddle.


### Implementation details for nerds
1) listen to opponent paddle hit event
2) on an opponent hit event, launch a coroutine after time N (ie 20 milliseconds, to simulate human reaction delay), in which the ai paddle does not move
3) when the delay is up, the trajectory of the ball is predicted by casting a ray from the ball's position, in the direction of the ball's velocity, casting an additional ray for each HorizontalWall hit, until the ray reaches the AI paddle's X position, or max number of iterations occurs
4) from the list of positions, we set the targetPosition to be the Y position of the final predicted position in the path
upon hitting the AI paddle, the targetPosition then switches to simply vertically aligning itself with the ball, until its opponent hits it again
5) repeat from step #2

Note that there is some randomness built in to the AI speed in addition to the response time delay, to give it a bit of a more human feel.
