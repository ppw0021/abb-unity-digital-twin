from irb_connection import irb
from requests.auth import HTTPDigestAuth

url = 'http://192.168.125.1'

rws_instance = irb()

digest_auth = HTTPDigestAuth('Default User', 'robotics')

payload = {'ctrl-state': 'motoron'}
resp = rws_instance.session.post(url + "/rw/panel/ctrlstate?action=setctrlstate", auth=digest_auth, data=payload)

if resp.status_code == 204:
    print("Response: " + str(resp.status_code))
    print("Executed code: " + str(payload))
else:
    print(resp.status_code)
    print("Error, Controller might be in manual mode")