# JoyShockLibrary
The Sony PlayStation's DualShock 4, DualSense, Nintendo Switch Joy-Cons (used in pairs), and Nintendo Switch Pro Controller have much in common. They have many of the features expected of modern game controllers. They also have an incredibly versatile and underutilised input that their biggest rival (Microsoft's Xbox One controller) doesn't have: the gyro.

My goal with JoyShockLibrary is to enable game developers to support DS4, DS, Joy-Cons, and Pro Controllers natively in PC games. I've compiled the library for Windows, but it uses platform-agnostic tools, and my hope is that other developers would be able to get it working on other platforms (such as Linux or Mac) without too much trouble.

## Contents
* **[Releases](#releases)**
* **[Reference](#reference)**
  * **[Structs](#structs)**
  * **[Functions](#functions)**
* **[Known and Perceived Issues](#known-and-perceived-issues)**
* **[Backwards Compatibility](#backwards-compatibility)**
* **[Credits](#credits)**
* **[Helpful Resources](#helpful-resources)**
* **[License](#license)**

## Releases
The latest version of JoyShockLibrary can always be found [here](https://github.com/JibbSmart/JoyShockLibrary/releases). Included is a 64-bit dll and a 32-bit dll, both for Windows, and JoyShockLibrary.h and JoyShockLibrary.cs for using the dll in C/C++ and C\# (Unity), respectively. The .cs file isn't up to date with the latest features, but should provide a starting point for your use.

## Reference
*JoyShockLibrary.h* has everything you need to use the library, but here's a breakdown of everything in there.

### Structs
**struct JOY\_SHOCK\_STATE** - This struct contains the state for all the sticks, buttons, and triggers on the controller. If you're just using JoyShockLibrary to be able to use Joy-Cons, Pro Controllers, DualSenses, and DualShock 4s similarly to how you'd use other devices, this has everything you need to know.
* **int buttons** contains the states of all the controller's buttons with the following masks:
  * ```0x00001``` - d-pad ```up```
  * ```0x00002``` - d-pad ```down```
  * ```0x00004``` - d-pad ```left```
  * ```0x00008``` - d-pad ```right```
  * ```0x00010``` - ```+``` on Nintendo devices, ```Options``` on DS4
  * ```0x00020``` - ```-``` on Nintendo devices, ```Share``` on DS4
  * ```0x00040``` - ```left-stick click``` on Nintendo devices, ```L3``` on DS4
  * ```0x00080``` - ```right-stick click``` on Nintendo devices, ```R3``` on DS4
  * ```0x00100``` - ```L``` on Nintendo devices, ```L1``` on DS4
  * ```0x00200``` - ```R``` on Nintendo devices, ```R1``` on DS4
  * ```0x00400``` - ```ZL``` on Nintendo devices, ```L2``` on DS4
  * ```0x00800``` - ```ZR``` on Nintendo devices, ```R2``` on DS4
  * ```0x01000``` - the South face-button: ```B``` on Nintendo devices, ```⨉``` on DS4
  * ```0x02000``` - the East face-button: ```A``` on Nintendo devices, ```○``` on DS4
  * ```0x04000``` - the West face-button: ```Y``` on Nintendo devices, ```□``` on DS4
  * ```0x08000``` - the North face-button: ```X``` on Nintendo devices, ```△``` on DS4
  * ```0x10000``` - ```Home``` on Nintendo devices, ```PS``` on DS4
  * ```0x20000``` - ```Capture``` on Nintendo devices, ```touchpad click``` on DS4, ```Create``` on DS5
  * ```0x40000``` - ```SL``` on Nintendo Joy-Cons, ```Mic``` on DS5
  * ```0x80000``` - ```SR``` on Nintendo Joy-Cons
* **float lTrigger** - how far has the left trigger been pressed? This will be 1 or 0 on Nintendo devices, which don't have analog triggers
* **float rTrigger** - how far has the right trigger been pressed? This will be 1 or 0 on Nintendo devices, which don't have analog triggers
* **float stickLX, stickLY** - left-stick X axis and Y axis, respectively, from -1 to 1
* **float stickRX, stickRX** - right-stick X axis and Y axis, respectively, from -1 to 1

**struct IMU_STATE** - Each supported device contains an IMU which has a 3-axis accelerometer and a 3-axis gyroscope. IMU\_STATE is where you find that info.
* **float accelX, accelY, accelZ** - accelerometer X axis, Y axis, and Z axis, respectively, in g (g-force).
* **float gyroX, gyroY, gyroZ** - gyroscope angular velocity X, Y, and Z, respectively, in dps (degrees per second), when correctly calibrated.

**struct MOTION_STATE** - The MOTION_STATE reports the orientation of the device as calculated using a sensor fusion solution to combine gyro and accelerometer data.
* **float quatW, quatX, quatY, quatZ** - a quaternion representing the orientation of the device.
* **float accelX, accelY, accelZ** - local acceleration after accounting for and removing the effect of gravity.
* **float gravX, gravY, gravZ** - local gravity direction.

#### Tip
Quaternions are useful if you want to try and represent the device's 3D orientation in-game, with one major limitation: "yaw drift". Small errors will accumulate over time so that the quaternion orientation no longer matches the controller's real orientation. The gravity direction is used to counter this error in the roll and pitch axes, but there's nothing for countering the error in the yaw axis.

Quaternions are **not** recommended for mouse-like aiming or cursor control. The gravity correction applied to the quaternion is not useful for these cases, and only introduces more error. For these, it's much better to use the calibrated gyro angular velocities. To make sure you account for every motion sensor update, either set a callback where you can respond to new motion input as it comes in, or poll the controller using **JslGetAndFlushAccumulatedGyro** to get a good average angular velocity since you last called this function.

### Functions

All these functions *should* be thread-safe, and none of them should cause any harm if given the wrong or out-of-date deviceId. So even if a device gets disconnected between calling "JslStillConnected" and "JslGetSimpleState", the latter will just report all the sticks, triggers, and buttons untouched, and you'll detect the disconnection *next time* you call "JslStillConnected".

**int JslConnectDevices()** - Register any connected devices. Returns the number of devices connected, which is helpful for getting the handles for those devices with the next function. As of version 3, this will not interrupt current connections. So you can call this function at any time to check for new connections. To only call it when necessary, only do this when your OS notifies you of a new connection (eg WM_DEVICECHANGE on Windows).

**int JslGetConnectedDeviceHandles(int\* deviceHandleArray, int size)** - Fills the array *deviceHandleArray* of size *size* with the handles for all connected devices, up to the length of the array. Use the length returned by *JslConnectDevices* to make sure you've got all connected devices' handles.

**void JslDisconnectAndDisposeAll()** - Disconnect devices, no longer polling them for input.

**bool JslStillConnected(int deviceId)** - Returns **true** if the controller with the given id is still connected.

**JOY\_SHOCK\_STATE JslGetSimpleState(int deviceId)** - Get the latest button + trigger + stick state for the controller with the given id.

**IMU\_STATE JslGetIMUState(int deviceId)** - Get the latest accelerometer + gyroscope state for the controller with the given id.

**MOTION\_STATE JslGetMotionState(int deviceId)** - Get the latest motion state for the controller with the given id.

**TOUCH\_STATE JslGetTouchState(int deviceId, bool previous = false)** - Get the latest or previous touchpad state for the controller with the given id. Only DualShock 4 and DualSense support this.

**bool JslGetTouchpadDimension(int deviceId, int &sizeX, int &sizeY)** - Get the dimension of the touchpad. This is useful to abstract the resolution of different touchpads.

**int JslGetButtons(int deviceId)** - Get the latest button state for the controller with the given id. If you want more than just the buttons, it's more efficient to use JslGetSimpleState.

**float JslGetLeftX/JslGetLeftY/JslGetRightX/JslGetRightY(int deviceId)** - Get the latest stick state for the controller with the given id. If you want more than just a single stick axis, it's more efficient to use JslGetSimpleState.

**float JslGetLeftTrigger/JslGetRightTrigger(int deviceId)** - Get the latest trigger state for the controller with the given id. If you want more than just a single trigger, it's more efficient to use JslGetSimpleState.

**float JslGetGyroX/JslGetGyroY/JslGetGyroZ(int deviceId)** - Get the latest angular velocity for a given gyroscope axis. If you want more than just a single gyroscope axis velocity, it's more efficient to use JslGetIMUState.

**void JslGetAndFlushAccumulatedGyro(int deviceId, float& gyroX, float& gyroY, float& gyroZ)** - Get gyro input that has accumulated since you last called this function on the device with the given id. This is an average angular velocity. Highly recommended if you're polling controllers instead of using the callbacks (_JslSetCallback_) to get new gyro data the moment it is received, to give a better account of how the controller has been turned since you last polled it. If no new motion data has come in since you last called it, it will repeat the same values, to avoid stuttery motion input when your game's framerate is higher than the controller's reporting rate (common with Switch controllers on high end PCs, for example).

**void JslSetGyroSpace(int deviceId, int gyroSpace)** - Players have different ideas about what the front of the controller is, or what the controller's rest position should be. This can make it hard to decide whether to use the controller's yaw or roll axis for turning in-game or moving a cursor horizontally. To address this, there are 3 popular "spaces" for gyro controls:
- 0 = Local Space - just pick yaw or roll and potentially give the player the option to change it. Or add them together. Up to you. It's not doing anything, so this is the default.
- 1 = World Space - use the gravity calculation to figure out which way is "up", and use that to calculate new axes of rotation for horizontal and vertical movement. Adapts to player preferences, but acquires some error from depending so strongly on the gravity calculation.
- 2 = Player Space - pretty much as adaptive as World Space, but uses gravity more loosely, so it doesn't acquire any error from errors in the gravity calculation. A great default for standard controllers (where the screen isn't attached to the controller). But it's not the default here.

JslSetGyroSpace lets you choose one of those spaces, and the transformation will automatically be applied when you request gyro (JslGetGyro* functions), get the IMU_STATE, or receive the IMU_STATE in a callback.

**float JslGetAccelX/JslGetAccelY/JslGetAccelZ(int deviceId)** - Get the latest acceleration for a given axis. If you want more than just a accelerometer axis, it's more efficient to use JslGetIMUState.

**int JslGetTouchId(int deviceId, bool secondTouch=false)** - Get the last touch's id, which is a value in range of 0-127 that automaticaly increments whenever a new touch appears, for the controller with the given id. Only DualShock 4s support this. If you want more than just a touch's id, it's more efficient to use JslGetTouchState.

**bool JslGetTouchDown(int deviceId, bool secondTouch=false)** - Get the latest state of the touch being present on a touchpad for the controller with the given id. Only DualShock 4s support this. If you want more than just a presence of touch, it's more efficient to use JslGetTouchState.

**float JslGetTouchX/JslGetTouchY(int deviceId, bool secondTouch=false)** - Get the latest touch state for the controller with the given id. Only DualShock 4s support this. If you want more than just a single touch axis, it's more efficient to use JslGetTouchState.

**float JslGetStickStep(int deviceId)** - Different devices use different size data types and different ranges on those data types when reporting stick axes. For some calculations, it may be important to know the limits of the current device and work around them in different ways. This gives the smallest step size between two values for the given device's analog sticks.

**float JslGetTriggerStep(int deviceId)** - Some devices have analog triggers, some don't. For some calculations, it may be important to know the limits of the current device and work around them in different ways. This gives the smallest step size between two values for the given device's triggers, or 1.0 if they're actually just binary inputs.

**float JslGetTriggerStep(int deviceId)** - Some devices have analog triggers, some don't. For some calculations, it may be important to know the limits of the current device and work around them in different ways. This gives the smallest step size between two values for the given device's triggers, or 1.0 if they're actually just binary inputs.

**float JslGetPollRate(int deviceId)** - Different devices report back new information at different rates. For the given device, this gives how many times one would usually expect the device to report back per second.

**float JslGetTimeSinceLastUpdate(int deviceId)** - Getting the time since the last update was received (in seconds) can be helpful for communicating a poor connection to the user (which can help communicate to wireless players that they need to move closer to the console or in some other way improve the connection, as [Fly Together](https://youtu.be/BjksCGFknKo?t=782) communicates so well in this example).

**void JslResetContinuousCalibration(int deviceId)** - JoyShockLibrary has helpful functions for calibrating the gyroscope by averaging out its input over time. This deletes all calibration data that's been accumulated, if any, this session.

**void JslStartContinuousCalibration(int deviceId)** - Start collecting gyro data, recording the ongoing average and using that to offset gyro output.

**void JslPauseContinuousCalibration(int deviceId)** - Stop collecting gyro data, but don't delete it.

**void JslSetAutomaticCalibration(int deviceId, bool enabled)** - Enable to have gyro automatically recalibrated when the controller is at rest or held very still. Disable to return to manual calibration using the above functions.

**void JslGetCalibrationOffset(int deviceId, float& xOffset, float& yOffset, float& zOffset)** - Get the calibrated offset value for the given device's gyro. You don't have to use it; all gyro output for this device is already being offset by this vector before leaving JoyShockLibrary.

**void JslSetCalibrationOffset(int deviceId, float xOffset, float yOffset, float zOffset)** - Manually set the calibrated offset value for the given device's gyro.

**JSL_AUTO_CALIBRATION JslGetAutoCalibrationStatus(int deviceId)** - Get whether auto calibration is enabled, whether we think the controller is currently being held still, and how confident we are in its auto calibration. If you want to prompt the user to manually calibrate their controller, you can use auto calibration, and read these values to show to the user whether the controller is at rest as well as progress for calibration. Then, you can either leave auto calibration enabled (when confidence is high, auto calibration becomes difficult to trigger accidentally), or just disable auto calibration to prevent further changes without the user triggering it manually again.

**void JslSetCallback(void(\*callback)(int, JOY\_SHOCK\_STATE, JOY\_SHOCK\_STATE, IMU\_STATE, IMU\_STATE, float))** - Set a callback function by which JoyShockLibrary can report the current state for each device. This callback will be given the *deviceId* for the reporting device, its current button + trigger + stick state, its previous button + trigger + stick state, its current accelerometer + gyro state, its previous accelerometer + gyro state, and the amount of time since the last report for this device (in seconds).

**void JslSetTouchCallback(void(\*callback)(int, TOUCH\_STATE, TOUCH\_STATE, float))** - Set a callback function by which JoyShockLibrary can report the current touchpad state for each device. Only DualShock 4s will use this. This callback will be given the *deviceId* for the reporting device, its current and previous touchpad states, and the amount of time since the last report for this device (in seconds).

**void JslSetConnectCallback(void(\*callback)(int))** - Set a callback function when a new device has been connected. This is *not* watching for new connections. You still need to explicitly call JslConnectDevices. From there, this callback will be called for each *new* device that is connected, giving you the opportunity to set default settings on those devices. It gives the unique deviceId for the newly connected device. This deviceId may be re-used if the device is connecting through the same port as a previous device.

**void JslSetDisconnectCallback(void(\*callback)(int, bool))** - Set a callback function when a device has been disconnected. When this is called, the given deviceId is no longer useful to you -- all JslGet* functions will give empty/neutral results as there's no device at that id to read from. The bool your callback receives is whether or not this disconnect was due to a timeout. Otherwise, it's an explicit disconnection (device physically disconnected).

**JSL_SETTINGS JslGetControllerInfoAndSettings(int deviceId)** - Read a bunch of info from the controller in one go instead of requesting each thing one at a time. Gyro space, colour (for LED on PlayStation controllers, for the controller itself for Switch controllers), whether it's calibrating, etc. See JoyShockLibrary.h for more info.

**int JslGetControllerType(int deviceId)** - What type of controller is this device?
  1. Left Joy-Con
  2. Right Joy-Con
  3. Switch Pro Controller
  4. DualShock 4
  5. DualSense

**int JslGetControllerSplitType(int deviceId)** - Is this a half-controller or full? If half, what kind?
  1. Left half
  2. Right half
  3. Full controller

**int JslGetControllerColour(int deviceId)** - Get the colour of the controller. Only Nintendo devices support this. Others will report white.

**void JslSetLightColour(int deviceId, int colour)** - Set the light colour on the given controller. Only DualShock 4 and the DualSense support this. Players will often prefer to be able to disable the light, so make sure to give them that option, but when setting players up in a local multiplayer game, setting the light colour is a useful way to uniquely identify different controllers.

**void JslSetPlayerNumber(int deviceId, int number)** - Set the lights that indicate player number. This only works on Nintendo devices and the DualSense. NOTE: The DualSense sets each LED through a bitmask. Use the ```DS5_PLAYER_#``` definitions in the header file to get PS5-style lightbar formats.

Player 1: ```--x--```
Player 2: ```-x-x-```
Player 3: ```x-x-x```
Player 4: ```xx-xx```
Player 5: ```xxxxx```

**void JslSetRumble(int deviceId, int smallRumble, int bigRumble)** - DualShock 4s have two types of rumble, and they can be set at the same time with different intensities. These can be set from 0 to 255. Nintendo devices support rumble as well, but totally differently. They call it "HD rumble", and it's a great feature, but JoyShockLibrary doesn't yet support it.

## Known and Perceived Issues
### Bluetooth connectivity
Joy-Cons and Pro Controllers are normally only be connected by Bluetooth. Some Bluetooth adapters can't keep up with these devices, resulting in laggy input. This is especially common when more than one device is connected (such as when using a pair of Joy-Cons). There is nothing JoyShockMapper or JoyShockLibrary can do about this.

There is experimental support for connecting supported Switch controllers by USB now. Please let me know if you have any issues (or success!) with it.

### Gyro poll rate on Nintendo devices
The Nintendo devices report every 15ms, but their IMUs actually report every 5ms. Every 15ms report includes the last 3 gyro and accelerometer reports. When creating the latest IMU state for Nintendo devices, JoyShockLibrary averages out those 3 gyro and accelerometer reports, so that it can best include all that information in a sensible format. For things like controlling a cursor on a plane, this should be of little to no consequence, since the result is the same as adding all 3 reports separately over shorter time intervals. But for representing real 3D rotations of the controller, this causes the Nintendo devices to be *slightly* less accurate than they could be, because we're combining 3 rotations in a simplistic way.

In a future version I hope to either combine the 3 rotations in a way that works better in 3D, or to add a way for a single controller event to report several IMU events at the same time.

## Backwards Compatibility
JoyShockLibrary v2 changes the gyro and accelerometer axes from previous versions. Previous versions were inconsistent between gyro and accelerometer. When upgrading to JoyShockLibrary v2, in order to maintain previous behaviour:
* Invert Gyro X
* Swap Accel Z and Y
* Then invert Accel Z

JoyShockLibrary v3 makes some small changes to the behaviour of some functions:
* **JslConnectDevices** no longer calls **JslDisconnectAndDisposeAll** before looking for connections. Instead of reconnecting all devices, it'll only make new connections, without disrupting current connections. If you were counting on the old behaviour, call JslDisconnectAndDisposeAll before calling JslConnectDevices.
* Using a newer version of GamepadMotionHelpers, if you enable auto calibration, it will be applied more quickly at first, and then less quickly when it has more confidence in the current calibration. You can reset these at any time.

## Credits
I'm Jibb Smart, and I made JoyShockLibrary. JoyShockLibrary has also benefited from the contributions of:
* Romeo Calota (Linux support + general portability improvements)
* RollinBarrel (touchpad support)
* Robin (wireless DS4/5 support)
* And others

JoyShockLibrary uses substantial portions of mfosse's [JoyCon-Driver](https://github.com/mfosse/JoyCon-Driver), a [vJoy](http://vjoystick.sourceforge.net/site/) feeder for most communication with Nintendo devices, building on it with info from dekuNukem's [Nintendo Switch Reverse Engineering](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/) page in order to (for example) unpack all gyro and accelerometer samples from each report.

JoyShockLibrary's DualShock 4 support would likely not be possible without the info available on [PSDevWiki](https://www.psdevwiki.com/ps4/Main_Page) and [Eleccelerator Wiki](http://eleccelerator.com/wiki/index.php?title=DualShock_4). chrippa's [ds4drv](https://github.com/chrippa/ds4drv) was also a handy reference for getting rumble and lights working right away, and some changes have been made while referencing Ryochan7's [DS4Windows](https://github.com/Ryochan7/DS4Windows).

This software depends on signal11's [HIDAPI](https://github.com/signal11/hidapi) to connect to USB and Bluetooth devices.

The gravity calculation and gyro calibration is handled by another library of mine, [GamepadMotionHelpers](https://github.com/jibbsmart/gamepadmotionhelpers). Making it a separate library means you can make use of its robust sensor fusion calculation and automatic calibration options regardless of what you're using to read from the controller itself.

## Helpful Resources
* [GyroWiki](http://gyrowiki.jibbsmart.com) - All about good gyro controls for games:
  * Why gyro controls make gaming better;
  * How developers can do a better job implementing gyro controls;
  * How to use JoyShockLibrary;
  * How gamers can play any PC game with gyro controls using [JoyShockMapper](https://github.com/Electronicks/JoyShockMapper). Legacy versions use JoyShockLibrary to read from supported controllers, but the standard version uses SDL2 to support more controllers.

## License
JoyShockLibrary is licensed under the MIT License - see [LICENSE.md](LICENSE.md).
