# Products API

## Management

- Allows some folks ("Product Managers") to:
- Add Products
- Update Products (Make price adjustments, inventory adjustments, etc.)
- Remove Products from the available catlog, etc.

## Browsing

- Allows our customers ("Customer") to retrieve a list of products so they can add them to their cart.


## The Orders API Is a Client

When an order is placed, it needs to verify if the items in the cart being processed:

- Are still available
- Are the same price and decide what to do if the price is different

## If We Were Sharing Databases Across the Project in this Mono Repo

a) I wouldn't throw that out. Services shouldn't share databases, but this is an App. But it still isn't great.
b) We could just have the products API add and modify stuff in the database, and the Orders API (or other APIs in in APP
   can access that.