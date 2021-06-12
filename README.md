# :stars::ringed_planet: Planet Workshop :ringed_planet::stars:
Create varied planets and moons, and export equirectangular terrain heightmaps and textures for use in other applications

## Overview
Planet:<br>
<img src="https://github.com/carlpilot/Planet-Workshop/blob/main/Assets/plateau%20world.png" width=350><br>
Heightmap and texture:<br>
<img src="https://github.com/carlpilot/Planet-Workshop/blob/main/Assets/plateau%20world_height.png" width=500>
<img src="https://github.com/carlpilot/Planet-Workshop/blob/main/Assets/plateau%20world_texture.png" width=400>

## Details
- Customise features such as size, noise parameters, craters, colour and more
- Export greyscale height maps and full-colour texture maps as PNG files up to 8192x4096 pixels
- Generation carried out in 3D to eliminate distortions and discontinuities in exported maps
- Highly performant - mesh updates typically take under a second and exports under ten seconds for 2048x1024 output

## Features and To-do
- [x] Customisable noise scale, height and octaves
- [x] Varied surface colour
- [x] Random crater generation
- [x] Export heightmap and texture
- [x] Layering of noise and layering UI elements
- [x] Different types of noise (ridge, etc)
- [ ] Gradient colour picker for altitude
- [ ] Mixing between randomised and altitude-based colour maps
- [ ] Colour affected by crater presence
- [ ] Masks for noise and colour maps
- [ ] Save and load in a native format
- [ ] Control over random seed to allow full functionality of save/load
- [ ] Noise height curves for more varied surface
- [ ] Latitude-based colouring (ice caps, etc)
- [ ] Atmospheric and oceanic worlds
