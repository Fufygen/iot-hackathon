# Easterbot by Robo3 - IoT Hackathon projects
This repository includes projects created during the CEE Microsoft IoT hackathon 2016 event by our Robo3 team.
All projects use the Windows IoT extension for the UWP and run on Raspberry Pi2 with Explorer Hat, together with other sensors and devices specific to each project.

Because the IoT hackathon took place one week before the Easter, we got an idea how to perform folk customs with Windows IoT and Raspberry Pi2. One of the traditions on the Easter Monday in Slovakia is about boys throwing water on girls (see [Wikipedia for more info](https://en.wikipedia.org/wiki/%C5%9Amigus-Dyngus)) in order to wish them beauty and good health, but also to get rewards like painted eggs. For that, we decided to create a robot to water girls based on the face detection in a corridor leading to our laboratory at university. If a girl is detected in the corridor, we control the easterbot with a stearing wheel and splash water on command. After that, a robotic hand is used to retrieve the rewards.

- Easterbot - 4-wheeled bot with water hose for splashing water on girls on Easter Monday. A Genius 3 MT steering wheel is used for controlling the bot. A python script reads input from the wheel and sends commands through Wifi to the bot running Windows UWP background app. Received commands are used for pulse width modulation of output to control individual motors on the bot.
- Laser maze - maze created from 4 lasers, 4 photo resistors, 4 LEDs for detection of movement in the corridor. If a person is detected, i.e., interferes the laser beams, a photo is taken by the USB camera connected to the RPi2 and analysed for number of girls in the corridor using the Project Oxford Face API. If any girl is detected, the easter bot is contacted through the network to take action.
- Robotic hand - prototype of robotic hand to retrieve a reward, e.g., painted eggs (see photo: easterwhip). The hand is controlled with the keyboard connected to the RPi2 (numkeypad 1 and 3 for motor no. 1, 4 and 7 for motor no. 2, 5 and 8 for motor no. 3, 6 and 9 for motor no. 4, keys O and P for the pincer). Commands issued by the keyboard are recorded, may be saved or restored from the memory card, and replayed automatically.

# Technologies used
- Windows IoT extension for the UWP
- [Microsoft Project Oxford Face API](https://www.projectoxford.ai/face)
- [BusProviders](https://github.com/ms-iot/BusProviders)

# Devices and sensors used
- 3 Raspberry Pi2 boards
- 2 Explorer hat shields
- 2wd chassis for the Easterbot (see photos: easterbot 1 and 2)
- 4wd chassis later replaced the original 2wd chassis for the Easterbot (see photos: easterbot 3 and 4)
- 2 unique Microsoft power banks for powering the Easterbot - Microsoft Azure and Microsoft HoloLens :)
- water pump
- robotic hand with 5 motors for 4 joints and 1 for pincer (see photos of hand)
- keyboard for controlling the hand
- USB webcam for taking pictures 
- Genius 3 MT steering wheel for controlling the Easterbot
- lasers, photo resistors, relays, motor drivers, LEDs lots of cables, batteries, etc.  

# Files
- src - contains source code for all projects
- photos - pictures taken during the development of our projects, named after the projects
