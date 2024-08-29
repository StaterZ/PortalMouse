# PortalMouse Config Documentation

## Available Settings
### Root
#### The JSON root object
```json
{
	"mappings": Portal[] //a list of all portal bindings
}
```

### Portal
#### Defines a connection between 2 EdgeRanges
Moving the mouse into one of them remaps it to the range and position of the other
```json
{
	a: EdgeRange, //the first edge
	b: EdgeRange  //the second edge
}
```

### EdgeRange
#### Defines a pixel range on a given side of a given screen.
The pixel range goes between at most between at most 0 and *side length of screen* for obvious reasons
```json
{
	screen: i32,   // the screen indices
	side: Side,    // what side of the screen are we mapping
	begin: Anchor, // (optional) what to start the range at (inclusive), defaults to 0% if not specified
	end: Anchor    // (optional) what to end the range at (exclusive), defaults to 100% if not specified
}
```

### Anchor
#### Defines a range on a given side of a given screen.
An Anchor is just a `string` with a special format. All anchors begin with an integer, directly followed by a unit.

| Unit    | symbol | Description                                                                            |
| ------- | ------ | -------------------------------------------------------------------------------------- |
| Pixel   | px     | Specifies a pixel along the edge between 0 and the side length of the screen in pixels |
| Percent | %      | Specifies a percentage between 0% and 100% of the side length of the screen            |


### Side
#### Defines a screen side
```c#
enum {
	Left, //left side of screen
	Right, //right side of screen
	Top, //top side of screen
	Bottom, //bottom side of screen
}
```


## Examples
### Example 1: Looping
Single screen setup where the sides loop around.
```json
{
	"mappings": [
		{
			"a": {
				"screen": 1,
				"side": "Left"
			},
			"b": {
				"screen": 1,
				"side": "Right"
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Top"
			},
			"b": {
				"screen": 1,
				"side": "Bottom"
			}
		}
	]
}
```

### Example 2: 1-to-1 Edge Mapping
A 3 screen setup where no matter the resolution of the displays, it will map the edges 1-to-1.
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
				"screen": 2,
				"side": "Right"
			},
			"b": {
				"screen": 3,
				"side": "Left"
			}
		}
	]
}
```

### Example 3: Edge ranges
A 3 screen setup where a large monitor maps the upper half of an edge to a upper right screen and the lower half to a lower right screen, with a 100 pixel vertical link between these smaller screens.
```json
{
	"mappings": [
		{
			"a": {
				"screen": 1,
				"side": "Right",
				"begin": "0%",
				"end": "50%"
			},
			"b": {
				"screen": 2,
				"side": "Left",
				"begin": "50%",
				"end": "100%"
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Right",
				"begin": "50%",
				"end": "100%"
			},
			"b": {
				"screen": 3,
				"side": "Left",
				"begin": "0%",
				"end": "50%"
			}
		},
		{
			"a": {
				"screen": 2,
				"side": "Bottom",
				"end": "100px"
			},
			"b": {
				"screen": 3,
				"side": "Top",
				"end": "100px"
			}
		}
	]
}
```

### Example 4: Snapping
A 2 screen setup where the bottom part of a larger screen snaps up to the bottom of a smaller screen.
```json
{
	"mappings": [
		{
			"a": {
				"screen": 1,
				"side": "Right",
				"end": "1079px"
			},
			"b": {
				"screen": 2,
				"side": "Left",
				"end": "1079px"
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Right",
				"begin": "1079px"
			},
			"b": {
				"screen": 2,
				"side": "Left",
				"begin": "1079px",
				"end": "1080px"
			}
		}
	]
}
```
