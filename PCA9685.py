#!/usr/bin/python

import time
import math
import smbus
import keyboard

# ============================================================================
# Raspi PCA9685 16-Channel PWM Servo Driver
# ============================================================================

class PCA9685:

  # Registers/etc.
  __SUBADR1            = 0x02
  __SUBADR2            = 0x03
  __SUBADR3            = 0x04
  __MODE1              = 0x00
  __PRESCALE           = 0xFE
  __LED0_ON_L          = 0x06
  __LED0_ON_H          = 0x07
  __LED0_OFF_L         = 0x08
  __LED0_OFF_H         = 0x09
  __ALLLED_ON_L        = 0xFA
  __ALLLED_ON_H        = 0xFB
  __ALLLED_OFF_L       = 0xFC
  __ALLLED_OFF_H       = 0xFD

  def __init__(self, address=0x40, debug=False):
    self.bus = smbus.SMBus(1)
    self.address = address
    self.debug = debug
    if (self.debug):
      print("Reseting PCA9685")
    self.write(self.__MODE1, 0x00)
	
  def write(self, reg, value):
    "Writes an 8-bit value to the specified register/address"
    self.bus.write_byte_data(self.address, reg, value)
    if (self.debug):
      print("I2C: Write 0x%02X to register 0x%02X" % (value, reg))
	  
  def read(self, reg):
    "Read an unsigned byte from the I2C device"
    result = self.bus.read_byte_data(self.address, reg)
    if (self.debug):
      print("I2C: Device 0x%02X returned 0x%02X from reg 0x%02X" % (self.address, result & 0xFF, reg))
    return result
	
  def setPWMFreq(self, freq):
    "Sets the PWM frequency"
    prescaleval = 25000000.0    # 25MHz
    prescaleval /= 4096.0       # 12-bit
    prescaleval /= float(freq)
    prescaleval -= 1.0
    if (self.debug):
      print("Setting PWM frequency to %d Hz" % freq)
      print("Estimated pre-scale: %d" % prescaleval)
    prescale = math.floor(prescaleval + 0.5)
    if (self.debug):
      print("Final pre-scale: %d" % prescale)

    oldmode = self.read(self.__MODE1);
    newmode = (oldmode & 0x7F) | 0x10        # sleep
    self.write(self.__MODE1, newmode)        # go to sleep
    self.write(self.__PRESCALE, int(math.floor(prescale)))
    self.write(self.__MODE1, oldmode)
    time.sleep(0.005)
    self.write(self.__MODE1, oldmode | 0x80)

  def setPWM(self, channel, on, off):
    "Sets a single PWM channel"
    self.write(self.__LED0_ON_L+4*channel, on & 0xFF)
    self.write(self.__LED0_ON_H+4*channel, on >> 8)
    self.write(self.__LED0_OFF_L+4*channel, off & 0xFF)
    self.write(self.__LED0_OFF_H+4*channel, off >> 8)
    if (self.debug):
      print("channel: %d  LED_ON: %d LED_OFF: %d" % (channel,on,off))
	  
  def setServoPulse(self, channel, pulse):
    "Sets the Servo Pulse,The PWM frequency must be 50HZ"
    pulse = pulse*4096/20000        #PWM frequency is 50HZ,the period is 20000us
    self.setPWM(channel, 0, int(pulse))
  def setMotorPulse(self, channel, pulse):
    "Sets the Servo Pulse,The PWM frequency must be 50HZ"
    pulse = pulse*4096/20000       #PWM frequency is 50HZ,the period is 20000us
    self.setPWM(channel, 0, int(pulse))
    
def press(key):
    if key == "up":
        print("up pressed")
    elif key == "down":
        print("down pressed")
    elif key == "left":
        print("left pressed")
    elif key == "right":
        print("right pressed")



if __name__=='__main__':
 
  pwm = PCA9685(0x40, debug=False)
  pwm.setPWMFreq(50)
  
  #for i in range(1500,500,-1):
      #print (i)
      #pwm.setServoPulse(0,i)
  
  #constants for servo midpoints
  servo0mid = 1500 #rotation
  servo1mid = 900 #trigger
  servo2mid = 1800 #elevation
  
  #variables for changing servo position
  servo0current = servo0mid #rotation
  servo1current = servo1mid #trigger
  servo2current = servo2mid #elevation
  
  b = True # bool for trigger
  
  pwm.setServoPulse(0,servo0mid)
  print("pwm0 ",servo0mid," init")
  pwm.setServoPulse(1,servo1mid)#900 safe 840 fire
  print("pwm1 ",servo1mid," init")
  pwm.setServoPulse(2,servo2mid) #1400 $1900
  print("pwm2 ",servo2mid," init") 
  
  while True:
    val = input("")
    print(val)
    if val == "w":
        servo2current = servo2current - 10
        pwm.setServoPulse(2,servo2current)
    if val == "a":
        pwm.setServoPulse(0,servo0current)
    if val == "s":
        servo2current = servo2current + 10
        pwm.setServoPulse(2,servo2current)
    if val == "d":
        pwm.setServoPulse(0,servo0current)
        
    if val == "ww":
        servo2current = servo2current - 200
        pwm.setServoPulse(2,servo2current)
    if val == "aa":
        pwm.setServoPulse(0,servo0current)
    if val == "ss":
        servo2current = servo2current + 200
        pwm.setServoPulse(2,servo2current)
    if val == "dd":
        pwm.setServoPulse(0,servo0current)
    if val == " ":
        if b == True:
            pwm.setServoPulse(1,820)
            print("Fire")
            b = False
        elif b == False:
            pwm.setServoPulse(1,900)
            print("Cease Fire")
            b = True
