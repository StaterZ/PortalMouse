# Documentation

## Root
### The json root object, nice and simple
```c#
[Required] mappings: Mapping[] //the list of all mappings
[Optional] correctionDist: i32 //the max number of pixels/frame windows can move the cursor. if it moves faster than the supplied value, the program will attempt to stop windows from teleporting the cursor. if no value is supplied, corrections will be disabled
```

## Mapping
### Defines a connection between 2 EdgeRanges
> Moving the mouse into one of them remaps it to the range and position of the other
```c#
[Required] a: EdgeRange //the first edge
[Required] b: EdgeRange //the second edge
```

## EdgeRange
### Defines a pixel range on a given side of a given screen.
> The pixel range goes between at most between at most 0 and *side length of screen* for obvious reasons
```c#
[Required] screen: i32 //the screen indices
[Required] side: Side //what side of the screen are we mapping
[Optional] begin: i32 //what pixel to start the range at (inclusive), defaults to 0 if not specified
[Optional] end: i32 //what pixel to end the range at (exclusive), defaults to side length of screen if not specified
```

## Side
### Defines a screen side
```c#
enum {
	Left, //Left side of screen
	Right, //Right side of screen
	Top, //Top side of screen
	Bottom, //Bottom side of screen
}
```

<br></br>
<br></br>
<br></br>

# Examples
## Single screen setup where the sides loop around like an [asteroids game][1]
```json
{
	"mappings": [
		{
			"a": {
				"screen": 0,
				"side": "Top"
			},
			"b": {
				"screen": 0,
				"side": "Bottom"
			}
		},
		{
			"a": {
				"screen": 0,
				"side": "Left"
			},
			"b": {
				"screen": 0,
				"side": "Right"
			}
		}
	]
}
```
<br></br>
## A 3 screen setup where no matter the resolution of the displays it will map the edges 1 to 1
```json
{
	"mappings": [
		{
			"a": {
				"screen": 1,
				"side": "Left"
			},
			"b": {
				"screen": 2,
				"side": "Right"
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Right"
			},
			"b": {
				"screen": 0,
				"side": "Left"
			}
		}
	]
}
```
<br></br>
## A 3 screen 1080p setup where the left monitor maps the first half of pixels to a upper right screen and the other half to a lower right screen
```json
{
	"mappings": [
		{
			"a": {
				"screen": 0,
				"side": "Right",
				"begin": 0,
				"end": 540
			},
			"b": {
				"screen": 1,
				"side": "Left",
				"begin": 540,
				"end": 1080
			}
		},
		{
			"a": {
				"screen": 0,
				"side": "Right",
				"begin": 540,
				"end": 1080
			},
			"b": {
				"screen": 2,
				"side": "Left",
				"begin": 0,
				"end": 540
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Bottom"
			},
			"b": {
				"screen": 2,
				"side": "Top"
			}
		}
	]
}
```

[1]: https://letmegooglethat.com/?q=asteroids+game
