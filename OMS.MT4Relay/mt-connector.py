from flask import Flask, request

app = Flask(__name__)

@app.route('/process-order', methods=['POST'])
def process_order():

    print(request.json)

    ticket = request.json['platformOrderID']
    symbol = request.json['instrument']
    orderPx = request.json['orderPX']
    orderAction = request.json['orderAction']
    trader = request.json['userName']
    
    # Process the order here
    
    new_order = {
     "newOrderID": 0,
      "platformOrderID": ticket,
      "userName": trader,
      "userID": 0,
      "userGroup": 77,
      "authToken": 0,
      "instrument": symbol,
      "orderPX": orderPx,
      "orderType": "Market",
      "quantity": 10000
    }
    
    if int(orderAction) > 0 :
        new_order["orderAction"] = 1
    
    if int(orderAction) < 0 :
        new_order["orderAction"] = -1
        
    print(new_order)
    
    return 'Order processed successfully'

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
