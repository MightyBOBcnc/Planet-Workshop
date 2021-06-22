[ PLANET WORKSHOP ]
     version 1

For most up-to-date information, check the GitHub page:
https://github.com/carlpilot/Planet-Workshop

TO EDIT THE PLANET:
	- See the Options descriptions below for more detail on each individual option.
	- After making changes, click Update Planet to refresh the planet including the new changes

TO EXPORT THE PLANET:
	Exporting heightmap and texture:
		- These will be exported as equirectangular maps
		- In Export Options, set the desired resolutions (resolution = width of texture)
		- Set the name for the exported files
		- Click "Export Height & Texture" (no update needed)
	Exporting a 3D model:
		- Set the desired mesh resolution in Planet Options
		- Un-check Randomise Seed (unless you want the planet to be randomised before export)
		- Update Planet
		- Export Options -> "Export Planet Mesh"
		
TO SAVE YOUR WORK AS A PROJECT:
	- Enter a planet name in the "Name" box of the left-side menu
	- Click "Save"

TO LOAD A SAVED PROJECT:
	- Click "Load Planet" in the left-side menu to open the load dropdown
	- Select the planet to load from the list (you may need to scroll down)

OPTIONS:
	LEFT-SIDE MENU:
		NAME:
			Input the name for the saved project file
		SAVE:
			Saves the planet as a project (.pws) file
		LOAD:
			Lists the saved project files
			(Click on a saved file to load it into the editor)
	RIGHT-SIDE MENU:
		PLANET OPTIONS:
			RADIUS:
				Controls the size of the planet.
				The planet will appear the same size but surface features will be larger or smaller
			MESH RESOLUTION:
				Controls how detailed the 3D planet appears as well as the resolution of any exported 3D models
				Resolutions above 255 are not recommended except for immediate export due to high computational load
			SEED:
				The seed for the planet generation algorithm - can be any text
			RANDOMISE SEED:
				Whether or not to generate a random seed for each update
				If checked, the planet will look different on every update even though the options do not change
		NOISE OPTIONS:
			ADD NOISE LAYER:
				Creates a new noise layer. Layers are added together to form the final shape
			NOISE LAYERS:
				DELETE THIS LAYER:
					Deletes this noise layer
				NAME:
					A name which will appear on the dropdown, for your convenience
				TYPE:
					The overall shape of noise in this layer
					Types:
						- Perlin: Standard, uniformly varying terrain
						- Ridge: Tall, snaking ridgelines with varied terrain below
						- Plateaus: Flat, low-lying terrain with steep mesas
				SCALE:
					The overall size of the noise - smaller values lead to more local variation
				HEIGHT:
					The amplitude of the noise - larger is taller
				OCTAVES:
					How many layers of this noise should be added together at decreasing scale and height
					More octaves produces more detailed noise whereas less octaves produces smoother noise
		CRATER OPTIONS:
			NUM. CRATERS:
				How many craters the planet has
			CRATER MAX. SIZE:
				The maximum size of each crater
				The size of each crater is chosen randomly, up to this value
			CRATER DEPTH MULTIPLIER:
				Controls how deep the craters are
		COLOUR OPTIONS:
			RANDOM COLOURING:
				RANDOM SURFACE COLOUR 1 & 2:
					The two colours that will be randomly blended between across the entire surface of the planet
				RANDOM COLOUR SMOOTHNESS:
					How smoothly to blend between the random colours - from 0 (sharp delineations) to 1 (pure Perlin noise)
			COLOUR NOISE OPTIONS:
				COLOUR NOISE SCALE:
					The scale of the noise used for the random colours and for blending (compare noise layer noise scale)
				COLOUR NOISE OCTAVES:
					How many octaves of noise to use for random colours and for blending (compare noise layer octaves)
			HEIGHT COLOURING:
				LOW COLOUR:
					The colour to be used for low-lying areas
				HIGH COLOUR:
					The colour to be used for the highest terrain
			COLOUR BLENDING:
				GRADIENT BLENDING:
					Bias between random and height-based colour maps from all random to all height-based
				GRADIENT BLENDING NOISE:
					Amount of noise to apply to the blending to create patches of random and height-based colour
					0 = no noise, evenly blended
					Respects colour noise scale and octaves parameters
				GRADIENT BLENDING SMOOTHNESS:
					How smoothly to apply the gradient blending noise
					0 = sharp delineations, 1 = smooth noise
			CRATER COLOURING:
				CRATER BOTTOM COLOUR:
					A colour to apply to the bottom of craters
				CRATER COLOUR STRENGTH:
					How strongly to apply the crater bottom colour
		EXPORT OPTIONS:
			NAME:
				Name of the exported files
			HEIGHTMAP RESOLUTION:
				The resolution (in pixels of width) of the exported equirectangular heightmap
			TEXTURE RESOLUTION:
				The resolution (in pixels of width) of the exported equirectangular texture
			EXPORT HEIGHT & TEXTURE:
				Export the heightmap and texture map according to the options currently set
			EXPORT PLANET MESH:
				Export the 3D model of the planet AS OF THE LAST UPDATE
				(Will not take into account any non-Updated changes)

CHANGELOG:
	v1:
		- Initial release