# DEEP_DIVE
Intro to Virtual Reality Final Project Spring 2023 at UF. 

Fish System:
Currently the fish (boid) system works, but there are improvements that need to be done.
1. Increase efficiency of compute shader
2. Allow for separation of boids into flocks that will not intermingle
  This will likely be done through the use of a new flock class which will the call the compute shader, and modifying the BoidManager to become a FlockManager which has a list of all the flocks. 
3. Allow for boids to head to a target position
4. Allow for boids to head away from a predator position
