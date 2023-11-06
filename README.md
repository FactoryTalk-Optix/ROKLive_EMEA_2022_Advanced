# ROKLive_EMEA_2022_Advanced
ROKLive 2022 EMEA Full Project, script to restore initial state is included at DT

## Cloning the repository

1. Click on the green `CODE` button in the top right corner
2. Select `HTTPS` and copy the provided URL
3. Open FT Optix IDE
4. Click on `Open` and select the `Remote` tab
5. Paste the URL from step 2
6. Click `Open` button in the bottom right corner to start the cloning process

## Preparing the lab
This demo can be initialized by executing the [PrepareEmptyVersion](./main/ProjectFiles/NetSolution/PrepareEmptyVersion.cs) script, this will blank some parts of the project in order to complete the Advanced Lab

## Running the demo
### MQTT
MQTT Connectivity is configured using the sample broker at [test.mosquitto.org](https://test.mosquitto.org/) and can publish and receive messages via the `my_topic` topic

### OPC/UA
#### Client
The application is configured to connect to the OPC-UA demo server provided by the UA foundation [UANSI C SERVER](https://www.unified-automation.com/downloads/opc-ua-servers.html) which simulates a boiler tank

#### Server
Application exposes three motors that can be started and stopped via an OPC-UA Client, these can be found in the Model folder. Recommended client: [UaExpert](https://www.unified-automation.com/downloads/opc-ua-servers.html)
