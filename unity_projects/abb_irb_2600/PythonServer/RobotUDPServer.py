from socket import *
import threading
import json
import time
import requests
from requests.auth import HTTPDigestAuth
from bs4 import BeautifulSoup
import sys


# If not connected to robot
DEMO = True

class RobotData:
    jsonString = "{\"message\":\"no_data\"}"

    def SetJSONData(self, joint1, joint2, joint3, joint4, joint5, joint6):
        jsonDict = {
            "joint_1": joint1,
            "joint_2": joint2,
            "joint_3": joint3,
            "joint_4": joint4,
            "joint_5": joint5,
            "joint_6": joint6
        }
        self.jsonString = json.dumps(jsonDict)
        return
    
    def GetJSONData(self):
        return self.jsonString

def ListenForUDP(server):
    print("Waiting to accept UDP connection")
    messageRecv, address = server.recvfrom(1024)
    
    # message = "Hello from server".encode()

    message = robotDataInstance.GetJSONData().encode()

    print(f"Received message: {messageRecv.decode()}")

    server.sendto(message, address)

def UDPThread():
    server = socket(AF_INET, SOCK_DGRAM)
    server.bind(('0.0.0.0', 9999))

    while True:
        try:
            ListenForUDP(server)
        except:
            # why the hell is this throwing an error every 10-20 seconds????
            print("Error?")

def ProcessXML(xmlIn):
    soup = BeautifulSoup(xmlIn, "html.parser")
    valuesToReturn = {}
    for i in range (1, 7):
        tag = soup.find("span", {"class": f"rax_{i}"})
        if tag:
            valuesToReturn[f"rax_{i}"] = float(tag.text)
    return valuesToReturn

def GetRobotData():
    demo = DEMO
    updateRate = 0.01
    url = "http://192.168.0.20/rw/motionsystem/mechunits/ROB_1/jointtarget"
    # url = "/rw/motionsystem/mechunits/ROB_1/jointtarget"
    while True:
        # print("---------------------------------------------------")
        # print("Requesting Data")
        stringResult = ""
        startTime = time.time()
        if demo == True:
            #robotDataInstance.SetJSONData(1.1, 2.2, 3.3, 4.4, 5.5, 6.6)
            #print("New JSON: " + robotDataInstance.GetJSONData())
            #print("Updated")
            stringResult = '<?xml version="1.0" encoding="UTF-8"?><html xmlns="http://www.w3.org/1999/xhtml"><head><title>motionsystem</title><base href="http://192.168.0.20:80/rw/motionsystem/mechunits/ROB_1/jointtarget/"/></head><body><div class="state"><a href= "" rel="self"/> <ul> <li class="ms-jointtarget" title="ROB_1"> <span class="rax_1">1.1</span> <span class="rax_2">2.2</span> <span class="rax_3">3.3</span> <span class="rax_4">4.4</span> <span class="rax_5">-5.5</span> <span class="rax_6">-6.6</span> <span class="eax_a">0</span> <span class="eax_b">0</span> <span class="eax_c">0</span> <span class="eax_d">0</span> <span class="eax_e">0</span> <span class="eax_f">0</span> </li> </ul></div></body></html>'
            time.sleep(updateRate)
        if demo == False:
            stringResult = requests.get(url, auth=HTTPDigestAuth("Default User", "robotics"), headers={"Accept": "application/xml"}).text
            # print(f"Result: <{result.text}>")
            # jsonRecDict =
            time.sleep(updateRate)
        endTime = time.time()
        elapsedTime = round(endTime-startTime,4)
        rax_values = ProcessXML(stringResult)
        
        robotDataInstance.SetJSONData(rax_values["rax_1"], rax_values["rax_2"], rax_values["rax_3"], rax_values["rax_4"], rax_values["rax_5"], rax_values["rax_6"])
    
        # print(stringResult)
        print(f"Took {elapsedTime}")


robotDataInstance = RobotData()


try:
    robotDataInstance = RobotData()

    threads = []
    threads.append(threading.Thread(target=UDPThread, daemon=True))
    threads.append(threading.Thread(target=GetRobotData, daemon=True))
    for t in threads:
        t.start()

    for t in threads:
        t.join()

except KeyboardInterrupt:
    print("\nExiting cleanly...")
    sys.exit(0)