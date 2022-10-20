# InputManager
Improved package for Unity Input System

## Feature
- Easily handle inputs utilizing the Action of Input System

- the Action of Input System makes it easy to handle input from a keyboard or several types of gamepads

- Focus on handling BUTTON input

- Mouse and touchpad input is not supported <br>(can be used together if you make it on your own)

## Install

### Install Dependency
Window -> Package Manager -> AddPackage fom git url
|Package|version|URL|
|---|---|---|
|InputSystem|1.4.3|com.unity.inputsystem@1.4.3
|UniTask|2.3.1|https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask|

### Install InputManager
Window -> Package Manager -> AddPackage fom git url
https://github.com/elicxir/InputManager.git

## How to Use

### InputData
InputData is data that holds input information for that frame.

SingleInputData can be retrieved based on InputData and id.
SingleInputData is information about a single button input.

```Csharp
//Get Latest InputData
var inputdata = GetInputData();
```


### SingleInputData
SingleInputData is information about a single button input.

```Csharp
//Get Latest InputData
var inputdata = GetInputData();

//Get Single Input Data of id:"button1"

var button1 = inputdata.GetSingleInputData("button1");

//Get Arrow Key Input as Vector2
var arrowinput = inputdata.GetArrowInput();
```

SingleInputData contains the following information

|  Property (readonly)  |  Type  | Description |
| ---- | ---- | ---- |
|  ID  |  string  |Corresponding Action name|
|  isPressed  |  bool  |Whether the button is pressed or not|
|  isPressedThisFrame  |  bool  |Whether or not the moment the button is pressed|
|  isReleasedThisFrame  |  bool  |Whether or not the moment the button is released|
|  PressTimer  |  float  |Time since isPressedThisFrame last set to True|
|  HoldTimer  |  float  |Time to keep pressing |
|  ReleaseTimer  |  float  |Time since isReleasedThisFrame last set to True|

(All time units are in seconds. )

