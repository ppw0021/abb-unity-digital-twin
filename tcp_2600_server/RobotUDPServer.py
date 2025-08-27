from socket import *
import threading
import json
import time
import requests
from requests.auth import HTTPDigestAuth
from bs4 import BeautifulSoup
import sys

# Network config
PORT = 9999

# Packet counts
packetSendCount = 0
robotRequestCount = 0

# If not connected to robot
DEMO = True

class RobotDemo:
    joints = [-170, -160, -150, -140, -130, -120]

    def ReturnXML(self):
        increaseRate = 5
        maxJoint = 180
        minJoint = -180
        for i in range(6):
            if (self.joints[i] > maxJoint):
                self.joints[i] = minJoint
            else:
                self.joints[i] += increaseRate
        return f'<?xml version="1.0" encoding="UTF-8"?><html xmlns="http://www.w3.org/1999/xhtml"><body><li class="ms-jointtarget"><span class="rax_1">{self.joints[0]}</span><span class="rax_2">{self.joints[1]}</span><span class="rax_3">{self.joints[2]}</span><span class="rax_4">{self.joints[3]}</span><span class="rax_5">{self.joints[4]}</span><span class="rax_6">{self.joints[5]}</span></li></body></html>'


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

def PrintPanel():
    print(
        f"\rServer: {gethostbyname(gethostname())}:{PORT} | "
        f"UDP Sent: {packetSendCount} | "
        f"Robot Recv: {robotRequestCount}",
        end="",
        flush=True
    )

def ListenForUDP(server):
    global packetSendCount
    PrintPanel()
    # print("Waiting to accept UDP connection")
    messageRecv, address = server.recvfrom(1024)
    
    # message = "Hello from server".encode()

    message = robotDataInstance.GetJSONData().encode()
    packetSendCount += 1
    # print(f"Received message: {messageRecv.decode()}")

    server.sendto(message, address)

def UDPThread():
    server = socket(AF_INET, SOCK_DGRAM)
    server.bind(('0.0.0.0', 9999))

    while True:
        ListenForUDP(server)
        # try:
        #     ListenForUDP(server)
        # except:
        #     # why the hell is this throwing an error every 10-20 seconds????
        #     print("Error?")

def ProcessXML(xmlIn):
    soup = BeautifulSoup(xmlIn, "html.parser")
    valuesToReturn = {}
    for i in range (1, 7):
        tag = soup.find("span", {"class": f"rax_{i}"})
        if tag:
            valuesToReturn[f"rax_{i}"] = float(tag.text)
    return valuesToReturn

def GetRobotData():
    global robotRequestCount
    demo = True
    updateRate = 0.1
    url = "http://192.168.125.1/rw/motionsystem/mechunits/ROB_1/jointtarget"

    with requests.Session() as session:  # session handles connection reuse + cleanup
        session.auth = HTTPDigestAuth("Default User", "robotics")
        session.headers.update({"Accept": "application/xml"})

        while True:
            robotRequestCount += 1
            startTime = time.time()
            if demo:
                stringResult = robotDemoInstance.ReturnXML()
                time.sleep(updateRate)
            else:
                response = session.get(url)   # uses same connection if possible
                stringResult = response.text
                response.close()  # releases the connection back to pool
                time.sleep(updateRate)

            rax_values = ProcessXML(stringResult)
            try:
                robotDataInstance.SetJSONData(
                    rax_values["rax_1"], rax_values["rax_2"], rax_values["rax_3"],
                    rax_values["rax_4"], rax_values["rax_5"], rax_values["rax_6"]
                )
            except KeyError:
                print("Failed")

            elapsedTime = round(time.time() - startTime, 4)
            # print(stringResult)
            # print(f"Took {elapsedTime}")
            PrintPanel()



robotDataInstance = RobotData()
robotDemoInstance = RobotDemo()


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