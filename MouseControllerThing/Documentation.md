# Documentation

## Root
### The json root object, nice and simple
```c#
[Required] mappings: Portal[] //the list of all portal bindings
```

## Portal
### Defines a connection between 2 EdgeRanges
Moving the mouse into one of them remaps it to the range and position of the other
```c#
[Required] a: EdgeRange //the first edge
[Required] b: EdgeRange //the second edge
```

## EdgeRange
### Defines a pixel range on a given side of a given screen.
The pixel range goes between at most between at most 0 and *side length of screen* for obvious reasons
```c#
[Required] screen: i32 //the screen indices
[Required] side: Side //what side of the screen are we mapping
[Optional] begin: Anchor //what to start the range at (inclusive), defaults to 0% if not specified
[Optional] end: Anchor //what to end the range at (exclusive), defaults to 100% if not specified
```

## Anchor
### Defines a range on a given side of a given screen.
An Anchor is just a `string` with a special format. All anchors begin with an integer, directly followed by a unit.
| Unit    | symbol | Description                                                                            |
|---------|--------|----------------------------------------------------------------------------------------|
| Pixel   | px     | Specifies a pixel along the edge between 0 and the side length of the screen in pixels |
| Percent | %      | Specifies a percentage between 0% and 100% of the side length of the screen            |

## Side
### Defines a screen side
```c#
enum {
	Left, //left side of screen
	Right, //right side of screen
	Top, //top side of screen
	Bottom, //bottom side of screen
}
```

<br></br>
<br></br>
<br></br>

# Examples
## Single screen setup where the sides loop around, like in an [asteroids game][1]
```json
{
	"mappings": [
		{
			"a": {
				"screen": 0,
				"side": "Left"
			},
			"b": {
				"screen": 0,
				"side": "Right"
			}
		},
		{
			"a": {
				"screen": 0,
				"side": "Top"
			},
			"b": {
				"screen": 0,
				"side": "Bottom"
			}
		}
	]
}
```
<br></br>
## A 3 screen setup where no matter the resolution of the displays, it will map the edges 1 to 1
```json
{
	"mappings": [
		{
			"a": {
				"screen": 0,
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
				"side": "Right"
			},
			"b": {
				"screen": 2,
				"side": "Left"
			}
		}
	]
}
```
<br></br>
## A 3 screen setup where a large monitor maps the upper half of an edge to a upper right screen and the lower half to a lower right screen, with a 100 pixel vertical link between these smaller screens
```json
{
	"mappings": [
		{
			"a": {
				"screen": 0,
				"side": "Right",
				"begin": 0%,
				"end": 50%
			},
			"b": {
				"screen": 1,
				"side": "Left",
				"begin": 50%,
				"end": 100%
			}
		},
		{
			"a": {
				"screen": 0,
				"side": "Right",
				"begin": 50%,
				"end": 100%
			},
			"b": {
				"screen": 2,
				"side": "Left",
				"begin": 0%,
				"end": 50%
			}
		},
		{
			"a": {
				"screen": 1,
				"side": "Bottom",
				"end": 100px
			},
			"b": {
				"screen": 2,
				"side": "Top",
				"end": 100px
			}
		}
	]
}
```

[1]: https://www.google.com/search?q=asteroids+game
