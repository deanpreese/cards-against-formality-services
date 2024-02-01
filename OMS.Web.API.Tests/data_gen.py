import requests
import random
import time
import psycopg2
import psycopg2


 #Database connection parameters    
dbname = "orders"
user = "dean"
password = "abc"
host = "localhost"  

base_url = 'http://localhost:8786/api'  # Replace with your actual base URL

class DataGen:
    def __init__(self):
        print("Starting Data Gen")


    def generate_traders(self, traders, group):
        generated_traders = []
        for i in range(traders):
            new_trader = {
                "userId": 0,
                "group": group,
                "userPwd": "abc",
                "displayName": "Gen Display",
                "firstName": "Gen First",
                "lastName": "Gen Last",
                "email": "gen@example.com"
            }
            trader = requests.post(f'{base_url}/add-trader', json=new_trader)
            new_trader['userId'] = trader.json()['userId']
            generated_traders.append(new_trader)
            
        return generated_traders

    def verify_traders(self, traders):
        verified_traders = []
        for trader in traders:
            user_info = {
                "userID": trader['userId'],
                "groupNumber": trader['group'],
                "password": trader['userPwd']
            }
            tid = requests.post(f'{base_url}/authenticate-trader', json=user_info)
            if tid.json()['result'] == 0:
                print(f"Auth Failed.  {trader['userId']}")
            else:
                print(f"Auth Success.  {trader['userId']}")
                verified_traders.append(trader)
        return verified_traders

    def get_traders_from_db(self, dbname, user, password, host, number_of_traders):

        # Connect to your postgres DB
        conn = psycopg2.connect(
            dbname=dbname,  # replace with your database name
            user=user,  # replace with your username
            password=password,  # replace with your password
            host=host
        )

        # Open a cursor to perform database operations
        cur = conn.cursor()
        # Execute a query
        cur.execute(f'SELECT "UserID", "TraderGroup" FROM "UserProfiles" ORDER BY RANDOM() LIMIT {number_of_traders}')

        # Retrieve query results
        traders_from_db = cur.fetchall()

        db_traders = []
        for row in traders_from_db:
            
            user_info = {
                "userId": 0,
                "groupNumber": 0,
                "password": "abc"
            }
            
            user_info['userId'] = row[0]
            user_info['group'] = row[1]
            db_traders.append(user_info)

        #for t in db_traders:
        #    print(t)

        return db_traders


    def create_order(self, trader, action):
        return {
            "authToken": 1111111,
            "orderType": "MARKET",
            "platformOrderID": random.randint(1000000, 5000000),
            "userID": trader['userId'],
            "userGroup": trader['group'],
            "userName": "ME",
            "instrument": "DEMO",
            "orderPX": random.randint(10, 30),
            "orderAction": action,
            "quantity": 100
        }


    def generate_orders(self, new_traders, trades, groups):
        generated_orders = []
        for trader in new_traders:
            for group_number in range(groups + 1):
                for i in range(trades):
                    buy_order = self.create_order(trader, 'Buy')
                    sell_order = self.create_order(trader, 'Sell')
                    generated_orders.extend([buy_order, sell_order])
        random.shuffle(generated_orders)
        
        return generated_orders


    def run_data_with_existing_traders(self, traders, trades, groups):
        
        trader_list = self.get_traders_from_db(dbname, user, password, host, traders)
        order_list = self.generate_orders(trader_list, trades, groups)
       
        remaining_order_count = len(order_list); 
        for order in order_list:
            task = requests.post(f'{base_url}/order/process-order', json=order)
            time.sleep(0.25)

            remaining_order_count -= 1
            print(f"{remaining_order_count}    Order: {order['userID']} {order['orderAction']} {order['orderPX']} {order['quantity']}")
            while task.status_code != 200:
                time.sleep(0.5)


    def run_full_data_gen(self, traders, trades, groups):
        # Generate Traders
        trader_list = self.generate_traders(traders, groups)
        # Authenticate Trader
        verified_traders = self.verify_traders(trader_list)
        # Add Orders
        order_list = self.generate_orders(verified_traders, trades, groups)
        # Execute Orders
        for order in order_list:
            task = requests.post(f'{base_url}/order/process-order', json=order)
            while task.status_code != 200:
                time.sleep(0.25)
                

    def run_mt_data_gen(self, traders, trades, groups):
        # Generate Traders
        trader_list = self.generate_traders(traders, groups)
        # Authenticate Trader
        verified_traders = self.verify_traders(trader_list)
        # Add Orders
        order_list = self.generate_orders(verified_traders, trades, groups)
        # Execute Orders
        for order in order_list:
            task = requests.post(f'{base_url}/order/process-MT-order', json=order)
            while task.status_code != 200:
                time.sleep(0.25)                

# Run the script

traders = 10
trades = 3
groups = 1

data_gen = DataGen()

data_gen.run_full_data_gen(traders, trades, groups)
#data_gen.run_data_with_existing_traders(traders, trades, groups)