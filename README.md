A Sample to demonstrate the concept of EdgeModule with an Inner Device.

## NEED
* Ability to send Cloud to Device Sync and Async Messages to an IoTEdge Module 


https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-c2d-guidance
IoT Hub provides three options for device apps to expose functionality to a back-end app:

Direct methods for communications that require immediate confirmation of the result. Direct methods are often used for interactive control of devices such as turning on a fan.

Twin's desired properties for long-running commands intended to put the device into a certain desired state. For example, set the telemetry send interval to 30 minutes.

Cloud-to-device messages for one-way notifications to the device app.


IoT Edge Modules only supports Direct method and Module Twin's desired properties. However does not provide for Cloud to Device messages.

An approach taken with this sample is implement an inner device in an IoT Edge Module. In a way the inner device is a proxy to the Module. The application is as follows:

1. The Module supports a module method named "setinnerdevice". This mether accepts a device name and connectionstring as pay load
2. On setting the inner devicem, the module opens a IoT Device client connection direct with IoTHub.
3. A ReceiveC2DAsync method on thee devicee receives all meessages sent while the device was disconnected and anything sent while the device is connected.


To use this sample, need an  existing IoT Hub and an IoT Edge

* Build Module and deploy it to IoTEdge (assuming an Hub and Edge exists)
* Create an IoT Device for example D1
* Invoke Module Direct Method on MythicalEdgeModule with the method name "setinnerdevice" and payload as
```
{"devicename":"D1","connstring":"<DEVICE CONN STRING>"}
``` 
* Using IoTHub Explorer or Azure Portal, send Cloud2Device messages to device D1

Try sending before the device connects so you can also see past messages flowing to thee device