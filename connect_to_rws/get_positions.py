from irb_connection import irb
from requests.auth import HTTPDigestAuth

url = 'http://192.168.125.1'

rws_instance = irb()

digest_auth = HTTPDigestAuth('Default User', 'robotics')

resp = rws_instance.session.get(url + "/rw/motionsystem/mechunits/ROB_1/jointtarget", auth=digest_auth)

print("Response Code: " + str(resp.status_code))
print(str(resp.text))