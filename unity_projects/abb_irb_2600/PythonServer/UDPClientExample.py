from socket import *
import time

start = time.time()
client = socket(AF_INET, SOCK_DGRAM)

# 1 second timeout
client.settimeout(1)

message = "Hello from client"

messageToSend = message.encode()

addr = ("127.0.0.1", 9999)

number = [1,2,3,4]

client.sendto(messageToSend, addr)
print(f"Sent {messageToSend} to {addr[0]}:{addr[1]}")

data, server = client.recvfrom(1024)

recvMessage = data.decode()

end = time.time()
elapsed = round(((end - start) * 1000), 5)

print(f"Message received: {recvMessage}, took {elapsed}ms")