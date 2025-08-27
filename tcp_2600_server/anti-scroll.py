import time
import sys

total = 0

for i in range(1, 11):  # count from 1 to 10
    total += i
    print(f"\rTotal: {total}", end="")  # overwrite the same line
    sys.stdout.flush()
    time.sleep(0.5)

print()  # move to a new line at the end
