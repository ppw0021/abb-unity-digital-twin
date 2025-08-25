from socket import *
import threading
import json
import time
import requests
from requests.auth import HTTPDigestAuth

class RobotComMedium:
    ipAddress = ""
    port = 1234
    username = "Default User"
    password = "robotics"

    def __init__(self, ipAddress, port, username, password):
        self.ipAddress = ipAddress
        self.port = port
        self.username = username
        self.password = password

    def GetIpAddress(self):
        return self.ipAddress
    
    def GetPort(self):
        return self.port
    
    def GetUsername(self):
        return self.username
    
    def GetPassword(self):
        return self.password

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
    messageRecv, address = server.recvfrom(2024)
    
    # message = "Hello from server".encode()

    message = robotDataInstance.GetJSONData().encode()

    print(f"Received message: {messageRecv.decode()}")

    server.sendto(message, address)

def UDPThread():
    server = socket(AF_INET, SOCK_DGRAM)
    server.bind(('0.0.0.0', 9999))

    while True:
        ListenForUDP(server)

def GetRobotData():
    demo = False
    updateRate = 1
    url = "http://192.168.0.20/rw/motionsystem/mechunits/ROB_1/jointtarget"
    # url = "/rw/motionsystem/mechunits/ROB_1/jointtarget"
    while True:
        if demo == True:
            robotDataInstance.SetJSONData(1.1, 2.2, 3.3, 4.4, 5.5, 6.6)
            #print("New JSON: " + robotDataInstance.GetJSONData())
            #print("Updated")
            time.sleep(updateRate)
        if demo == False:
            startTime = time.time()
            result = requests.get(url, auth=HTTPDigestAuth("Default User", "robotics"), headers={"Accept": "application/xml"})
            print(f"Result: <{result.text}>")
            endTime = time.time()
            elapsedTime = endTime-startTime
            print(f"Took {elapsedTime}")
            # jsonRecDict =
            time.sleep(updateRate)


robotDataInstance = RobotData()

threads = []
threads.append(threading.Thread(target=UDPThread))
threads.append(threading.Thread(target=GetRobotData))
for t in threads:
    t.start()

for t in threads:
    t.join()