import requests
import json

base_url = 'http://localhost:8786/api/order'  # Replace with your actual base URL


def AddTrader():
    new_trader = {
    "group": 0,
    "userId": 0,
    "displayName": "string",
    "userPwd": "string",
    "firstName": "string",
    "lastName": "string",
    "email": "string"
    }

    response = requests.post(f'{base_url}/add-new-trader', data=json.dumps(new_trader), headers={'Content-Type': 'application/json'})
    print(f'AddNewTrader response: {response.status_code}, {response.text}')

def Authenticate():
# Test AuthenticateTrader endpoint
    trader_info = {
        'userId' : 999999,
        'password': "abc", 
        'groupNum': 0
    }

    response = requests.post(f'{base_url}/authenticate', data=json.dumps(trader_info), headers={'Content-Type': 'application/json'})
    print(f'AuthenticateTrader response: {response.status_code}, {response.text}')


def NewOrder():
    # Test ProcessOrder endpoint
    new_order = {
    "newOrderID": 0,
    "platformOrderID": 0,
    "userName": "string",
    "userID": 0,
    "userGroup": 0,
    "authToken": 0,
    "instrument": "string",
    "orderPX": 0,
    "orderType": "STOP",
    "orderAction": "Buy",
    "quantity": 0
    }

    response = requests.post(f'{base_url}/process-order', data=json.dumps(new_order), headers={'Content-Type': 'application/json'})
    print(f'ProcessOrder response: {response.status_code}, {response.text}')