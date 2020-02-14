# Paddle Whacker
A paddle-whacking game built from ground up in Unity, initially as a pong-like game...
But ultimately on the path to becoming an air hockey game.

Can download and play at: https://jeffreypersons.itch.io/paddle-whacker


## Implementation details for nerds: How the AI works

https://user-images.githubusercontent.com/8084757/74257777-265cf900-4caa-11ea-99fc-17729a5928f3.gif

1) listen to opponent paddle hit event
2) on an opponent hit event, launch a coroutine after time N (ie 20 milliseconds, to simulate human reaction delay), in which the ai paddle does not move
3) when the delay is up, the trajectory of the ball is predicted by casting a ray from the ball's position, in the direction of the ball's velocity, casting an additional ray for each HorizontalWall hit, until the ray reaches the AI paddle's X position, or max number of iterations occurs
4) from the list of positions, we set the targetPosition to be the Y position of the final predicted position in the path
upon hitting the AI paddle, the targetPosition then switches to simply vertically aligning itself with the ball, until its opponent hits it again
5) repeat from step #2

Note that there is some randomness built in to the AI speed in addition to the response time delay, to give it a bit of a more human feel.
