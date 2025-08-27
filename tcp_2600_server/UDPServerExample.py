from socket import *
import threading

def ListenForUDP(server):
    print("Waiting to accept UDP connection")
    messageRecv, address = server.recvfrom(2024)
    
    message = "Hello from server".encode()

    print(f"Received message: {messageRecv.decode()}")

    server.sendto(message, address)


def UDPThread():
    server = socket(AF_INET, SOCK_DGRAM)
    server.bind(('0.0.0.0', 9999))

    while True:
        ListenForUDP(server)

threads = []
threads.append(threading.Thread(target=UDPThread))

for t in threads:
    t.start()

for t in threads:
    t.join()