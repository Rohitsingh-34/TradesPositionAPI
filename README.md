# TradesPositionAPI

`TradesPositionAPI` is a .NET 8 Web API for managing stock trades and computing user positions in-memory. It provides endpoints to add trades, retrieve trades by user, and view current positions (net quantity and weighted average buy price) per stock.

## Quick summary

- Framework: .NET 8 (C# 12)
- Storage: In-memory (no database)
- APIs: `POST /trades`, `GET /trades/{user}`, `GET /positions/{user}`
- Swagger UI available in Development

---

## Run the project

Prerequisites:
- .NET 8 SDK installed

From the repository root run:

```bash
dotnet run --project TradesPositionAPI
```

The API will start (by default) on `https://localhost:{port}`. Open Swagger UI at `https://localhost:{port}/swagger` in development environment.
<img width="3151" height="1670" alt="image" src="https://github.com/user-attachments/assets/c7ac3d29-d1fc-47ff-ba78-64ccf7343308" />

---

## API Reference

All endpoints accept and return JSON.

### 1) Add a trade

- Method: `POST`
- URL: `/trades`
- Description: Add a new trade for a user. Only operations `BUY` and `SELL` are accepted (case-insensitive). The service normalizes `user` and `stock` to lowercase/uppercase internally.

Request body fields:
- `stock` (string) - stock symbol (e.g. `AAPL`)
- `operation` (string) - `BUY` or `SELL`
- `quantity` (integer) - number of shares
- `price` (decimal) - price per share
- `user` (string) - user identifier

Successful response:
- HTTP 200 OK
- Body: plain text message: `Trade Added to Database Successfully!`

Failure responses:
- HTTP 400 Bad Request when required fields are missing or `operation` is not `BUY`/`SELL`.

---

### 2) Get trades by user

- Method: `GET`
- URL: `/trades/{user}`
- Description: Returns the list of trades for the given user. If `{user}` is empty string the implementation returns all trades.

Response example:

```json
[
  {
    "tradeId": "GUID",
    "stock": "AAPL",
    "operation": "BUY",
    "quantity": 10,
    "price": 150.0,
    "user": "alice",
    "timestamp": "2025-11-30T12:34:56"
  }
]
```

---

### 3) Get positions by user

- Method: `GET`
- URL: `/positions/{user}`
- Description: Returns a list of positions for the specified user. Each position contains `stock`, `totalQuantity` and `averagePrice`.

Response example:

```json
[
  { "stock": "AAPL", "totalQuantity": 10, "averagePrice": 150.0 }
]
```

Fields:
- `stock` (string)
- `totalQuantity` (integer): net holdings = SUM(BUY quantities) - SUM(SELL quantities)
- `averagePrice` (double): weighted average price of BUY trades only = SUM(BUY price * quantity) / SUM(BUY quantity). If there are no BUY trades the average price is 0.0.

---

## Position calculation details

When a new trade is added the in-memory store recalculates the user's position for that stock:
- Collect all trades for the user and that stock.
- `totalQuantity` = SUM of BUY quantities - SUM of SELL quantities.
- `averagePrice` = (SUM over BUY trades of `price * quantity`) / (SUM over BUY trades of `quantity`). SELL trades do not affect the average price.

---

## Example workflow

1. Add a buy trade:

```http
POST /trades
{
  "stock": "AAPL",
  "operation": "BUY",
  "quantity": 10,
  "price": 150.0,
  "user": "alice"
}
```

2. Add another buy for the same stock:

```http
POST /trades
{
  "stock": "AAPL",
  "operation": "BUY",
  "quantity": 5,
  "price": 160.0,
  "user": "alice"
}
```

3. Position (`GET /positions/alice`) will return:

```json
[
  { "stock": "AAPL", "totalQuantity": 15, "averagePrice": 153.33333333333334 }
]
```

(averagePrice = (10*150 + 5*160) / (10 + 5) = 153.3333...)

If a sell is recorded:

```http
POST /trades
{
  "stock": "AAPL",
  "operation": "SELL",
  "quantity": 4,
  "price": 155.0,
  "user": "alice"
}
```

Position becomes:

```json
[
  { "stock": "AAPL", 
    "totalQuantity": 11, 
    "averagePrice": 153.33
  }
]
```

Note SELL reduced `totalQuantity` but does not change `averagePrice`.

---

## Notes and limitations

- Data is stored in-memory and will be lost on application restart. This project is intended as a simple demo.
- Usernames are normalized to lowercase and stock symbols are normalized to uppercase by the service.
- No authentication or authorization is implemented.
- No input validation beyond operation check; use careful clients or add validation for production.

---

## Project files of interest

- `Program.cs` — application startup and DI registration
- `Controllers/TradesController.cs` — endpoints for adding and querying trades
- `Controllers/PositionsController.cs` — endpoint for retrieving positions
- `Services/TradeService.cs` and `Services/PositionService.cs` — business logic
- `Storage/InMemoryStore.cs` — in-memory store and position calculation
- `Models/Trade.cs`, `Models/Position.cs` — data models
