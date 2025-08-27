import requests

ip = '192.168.125.1'
paths = ['rw', 'rw/panel', 'rw/rapid', 'doc', 'help', 'webdav']

for path in paths:
    url = f"http://{ip}/{path}"
    resp = requests.get(url, auth=('Default User', 'robotics'))
    print(f"{url}: {resp.status_code}")
