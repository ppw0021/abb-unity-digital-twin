from requests import Session
from requests.auth import HTTPDigestAuth

class irb:
    def __init__(self, username='Default User', password='robotics'):
        self.username = username
        self.password = password
        self.session = Session() # create persistent HTTP communication
        self.session.auth = HTTPDigestAuth(self.username, self.password)