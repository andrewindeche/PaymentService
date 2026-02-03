# SUBSCRIPTION BASED PAYMENT APP

|Tool                | Description                    | Tags for tools used                                                                                               |
| ------------------- | ------------------------------ | ---------------------------------------------------------------------------------------------------- |
| 1.GitHub| Version Control| [Version-Control]; [Repo];|
| 2.DotNet |  C# Based Backend Framework| [dotnet]; [C#];|
| 3.Flutterwave | Payment Gateway| [Payment]; [API];|
| 4.AfricasTalking | SMS Gateway | [SMS]; [Gateway];|
| 5.Microsoft SQL Server | SQL DB | [SQL]; [DB]; |
| 6.Ngrok | API Gateway | [Tunnel]; [API]; |

## <h1> Description</h1>
<p>Payment is a critical part of any marketplace or Saas. In this hypothetical scenario, a user 
needs to pay 20USD a month to access premium content. After a month they will have to renew 
their subscription. The API enables a user to activate_premium after login and recieve SMS confirmations</p>

## <h1> Features</h1>
<ul>
<li>An endpoint for user to activate premium </li>
<li>An endpoint to view user details including payment status </li>
<li>An automatic way a user status(is_premium) toggles based off their payment </li>
<li>Webhook to send callback after premium is activated so that Flutterwave sends SMS</li>
<li>Ngrok API Gateway to tunnel URL to Flutterwave</li>
</ul>

## <h1> Setting Up Project
<ol>

Create an account on Flutterwave and Africastalking and Use examplesettings.json to set env/settings variables

<li>Clone the repository </li>

```bash
git clone git@github.com:andrewindeche/PaymentService.git
cd your-repo
```
<li>Restore dependencies</li>

```bash
dotnet restore
```
<li>Update configuration</li>
    
  use examplesettings.json for reference
  Open appsettings.json (and/or appsettings.Development.json)</li>
  Update values such as connection strings, API keys, or environment-specific settings
<li>Run Docker Instance of MsSql</li>
  Start the Docker server;

  ```bash
  docker run -d \
  --name sqlserver \
  -e 'ACCEPT_EULA=Y' \
  -e 'SA_PASSWORD=password' \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
  ```
<li>Migrate Db</li>

```bash
dotnet ef migrations add InitUserDb
dotnet ef database update
```

<li>Run the API</li>

  ```bash
  dotnet run
  ```
</ol>

## <h1> App Workflow
<li>Login → 

```bash
curl -X POST http://127.0.0.1:5117/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'
```
</li>

<li>
Sanity check →

```bash
TOKEN=$(curl -s -X POST http://127.0.0.1:5117/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}' | jq -r .token)

curl -i http://127.0.0.1:5117/secure \
  -H "Authorization: Bearer $TOKEN"
```
</li>

<li> Activate user to premium → 

```bash
curl -i -X POST http://127.0.0.1:5117/api/payment/activate-premium/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```
 </li>

<li>Webhook → Flutterwave posts result to /api/webhook/flutterwave ->

```bash
 curl -X POST https://abcd1234.ngrok.io/api/webhook/flutterwave \
  -H "Content-Type: application/json" \
  -d '{
    "status":"successful",
    "txRef":"TX123",
    "id":1,
    "customer":{
      "id":1,
      "email":"admin@example.com",
      "phoneNumber":"+254700000000"
    }
  }'
```
 </li>
<li>SMS → Africa’s Talking sandbox sends confirmation text to user’s phone number.</li>

# <h1> Documentation </h1>
<p>Read through the flutterwave documentation here: https://developer.flutterwave.com/docs </p>
<p>Read through Africastalking documentation here: https://developers.africastalking.com/</p>