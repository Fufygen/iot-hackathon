# Easterbot by Robo3 - IoT Hackathon projects
This repository includes projects created during the CEE Microsoft IoT hackathon 2016 event by our Robo3 team.
All projects are made with the Windows IoT extension for the UWP and run on Raspberry Pi2 with Explorer Hat and other sensors and devices specific to each project.

Because the IoT hackathon took place one week before the Easter, we got an idea how to perform folk customs with Windows IoT and Raspberry Pi2. One of the traditions on the Easter Monday in Slovakia is to water girls and women (see [Wikipedia for more info](https://en.wikipedia.org/wiki/%C5%9Amigus-Dyngus)) in order to wish them beauty and good health, but also to get rewards like painted eggs. For that, we decided to create a robot to water girls based on the detection in corridor, then retrieve the rewards with robotic hand.

- Easterbot - 4-wheeled bot with water hose for watering women on Easter Monday.
- Laser maze - maze created from 4 lasers, 4 photo resistors, 4 LEDs for detection of movement in the corridor. If a person is detected, i.e., interferes the laser beams, a photo is taken by the camera and analysed for number of girls in the corridor using the Project Oxford Face API. If any girl is detected, the easter bot is contacted through the network to take action.
- Robotic hand - prototype of robotic hand to retrieve a reward, e.g., painted eggs.

# Technologies used
- Windows IoT extension for the UWP
- [Microsoft Project Oxford Face API](https://www.projectoxford.ai/face)

# Devices and sensors used
- 4-wheeled chassis for the Easterbot
- 2 Microsoft power banks for powering the Easterbot - Microsoft Azure and Microsoft HoloLens
- a robotic hand with 5 motors for 4 joints and 1 for pincer
- lasers, photo resistors, relays, motor drivers, etc.  
