# DEEP_DIVE
A virtual reality SCUBA diving experience created as a final project for CIS4930 Intro to Virtual Reality in Spring 2023 at UF. 

Developed by [Gabriel Aldous](https://github.com/Sn00pyW00dst0ck), [Ryan Porter](https://github.com/RyPort), and [Chung To](https://github.com/CC-0000).


## 

## Fish System:
The custom fish simulation system is based on the concept of boids. 

The following resources were used to develop the fish simulation system:
1. https://www.reddit.com/r/Unity3D/comments/7bvx9d/made_a_vertex_animation_shader_for_a_fish/
2. https://www.kodeco.com/5671826-introduction-to-shaders-in-unity
3. https://on-demand.gputechconf.com/gtc/2014/presentations/S4117-fast-fixed-radius-nearest-neighbor-gpu.pdf
4. https://en.wikipedia.org/wiki/Prefix_sum
5. https://en.wikipedia.org/wiki/Counting_sort

Possible future fish system improvements:
1. Allow for separation of boids into flocks that will not intermingle. This will require a major re-write, as the boid system heavily relies on the GPU and compute shaders and errors are thrown when running multiple instances of those.
2. Multi-Color boids / different random colors for each boid. 
3. Allow for boids to head to a target position
4. Allow for boids to head away from a predator position
