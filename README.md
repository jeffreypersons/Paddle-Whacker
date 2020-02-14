# Paddle Whacker
A paddle-whacking game built from ground up in Unity, initially as a pong-like game...
But ultimately on the path to becoming an air hockey game.

Can download and play on itch.io!: https://jeffreypersons.itch.io/paddle-whacker

## Sample Screenshots
### Start menu screen
![image](https://user-images.githubusercontent.com/8084757/74575511-22e0a080-4f3c-11ea-8ebf-6ec16f993b59.png)

### Game play screen
![image](https://user-images.githubusercontent.com/8084757/74575534-3855ca80-4f3c-11ea-95ac-511bb1741164.png)

### Game end screen
![image](https://user-images.githubusercontent.com/8084757/74575541-3e4bab80-4f3c-11ea-856f-14316fa9c670.png)


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
