

[UnprocessedRequestsQueue](https://github.com/bitministry/persistence-safetynet/blob/master/Turnit.GenericStore.WebApi.Common/UnprocessedRequestsQueue.cs)


that is a light weight SAFETY NET for .net web API requests.
it steps in when your data layer fails with an unexpected exception, and retries the processing at later time.
no installation nor configuration needed.

# Turnit Store task, as it came in

https://gitlab.tsolutions.co/external/practice-exercises/turnit-generic-store

## General issues
There are some performance and maintainability problems with the current solution.

Firstly, client has reported that the applications memory consumption grows very fast.
Also the code performance and maintainability is not so good - code duplication and bad code.

## Assignments
You are free to use any additional libraries, patterns, etc that you find fit (except replacing NHibernate). Database changes are not required.

### Task 1
Add functionality to add/remove products to categories.

Implement methods `PUT /products/{productId}/category/{categoryId}` and `DELETE /products/{productId}/category/{categoryId}`

### Task 2
Add functionality to change products availability in different stores.

Implement methods `POST /products/{productId}/book` and `POST /store/{storeId}/restock`

### Task 3
Find and fix the memory leak issue.
Improve the overall code quality in `GET /products/*` methods.



## Overview
This is a simple product inventory application. Main concepts are **Product**, **Category** and **Store**.
Product may belong to multiple categories or it can be uncategorized. 

Each store (warehouse) have it's own availability of products.

## API methods

### GET /categories
Should return list of all the available categories in alphabetical order.

Response:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string"
  }
]
```

### GET /products
Should return list of all the available products per category (also uncategorized).

Response:
```
[
  {
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "products": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "string",
        "availability": [
          {
            "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "availability": 0
          }
        ]
      }
    ]
  }
]
```

### GET /products/by-category/{categoryId}
Should return list af all the available products in the specific category.

Response:
```
[
  {
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "products": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "string",
        "availability": [
          {
            "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "availability": 0
          }
        ]
      }
    ]
  }
]
```

### PUT /products/{productId}/category/{categoryId}
Adds specific product to the category. Should be idempotent.

### DELETE /products/{productId}/category/{categoryId}
Removes specific product from the category. Should be idempotent.

### POST /products/{productId}/book
Marks products as booked in the store. This basically means that this quantity is sold (or reserved) to some client
and it should not be available as general availability.  

One request can contain quantities for multiple stores.

Request:
```
[
  {
    "storeId": "4f10a98a-e65b-11ec-a1ac-24ee9a88c06f",
    "quantity": 100
  }
]
```

### GET /stores
Should return list of all the available stores.

Response:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string"
  }
]
```

### POST /store/{storeId}/restock
Adds specified quantity of products to the stores availability.
This is used to increase the availability of products in specific store.

One request can contain quantities for multiple products (in the same store).

Request:
```
[
  {
    "productId": "4f10a98a-e65b-11ec-a1ac-24ee9a88c06f",
    "quantity": 100
  }
]
```

## Technical
Database layer is implemented using NHibernate and PostgreSQL database.

Application can be run with docker compose `docker-compose up` (docker-compose file is in the root directory). Application is exposed on the port 5000 and PostgreSQL is exposed on port 5632.

When running application locally (without docker, in dev environment), 
docker compose can be used to serve database and then can use connection string:
`Server=localhost;Port=5632;Database=turnit_store;User ID=postgres;Password=postgres`.

Application image can be built with (when using with docker-compose) `docker-compose build web`.

Swagger is accessible from the url http://localhost:5000/swagger.
