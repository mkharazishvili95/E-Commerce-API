# E-Commerce-API
This is my WEB API project (on .Net 5) where users can register, login and shopping.
## Details
After registering and entering the system, the user is assigned a validation token,
which allows it to perform the following operations: Get all products list (which is added by admin), buy the product,The amount paid by the user goes to the bank account, whose model I have created as BankModel.
and get information about his Purchases. He can also get information about his balance and his profile.

And the administrator (who exists in the database as Admin123, Admin123@gmail.com) has the right to receive information about all users, as well as about a specific user according to his ID. Also sort Users and display by specific Shipping Address. He also has the right to block any User (except another admin), and also unblock him. A blocked user has no way to buy the product.
Admin can also add a new product or update an existing one. He also has the right to delete the product (after deleting the product, orders related to it are automatically canceled and the amount paid for this product is automatically returned to the customer, which is transferred from the bank account).
He also has the right to cancel the order and in this case the amount paid to the customer will be returned.

### User Register Form:
/api/User/Register
{
"firstName": "string",
  "lastName": "string",
  "age": int,
  "email": "string",
  "userName": "string",
  "password": "string",
  "shippingAddress": "string"
}

### User Login Form:
/api/User/Login
{
   "email" : "string",
  "username": "string",
  "password": "string"
}
/api/User/GetMyBalance     <= (User can know his own balance)
/api/User/GetMyProfile     <= (User can know his own profile information)

### Product:
/api/Product/GetAllProducts  <=(Get all product information, that is added by Admin)
/api/Product/BuyProduct                           <=(Buying product by User)
{
  "productId": int,                              <=(enter productId)
  "quantity": int                                <=(enter product quantity, how many pcs do you want to buy?)
}
/api/Product/GetMyPurchases                      <=(User Purchases list)   

### Admin:
/api/Admin/GetAllUsers                           <=(Getting information about all users who are registered)
/api/Admin/GetUserById=userId={id?}              <=(Getting information about User by this ID)
/api/Admin/SortUsersByAddress?address={address?}             <=(Getting information about customers who have a specific Shipping Address)
/api/Admin/BlockUser?userId={id?}                            <=(Block User by his id)
/api/Admin/UnblockUser?userId={id?}                          <=(Unblock User by his id)
/api/Admin/AddProduct                            <=(Add new product):
{
  "name": "string",
  "description": "string",
  "quantity": int,
  "productCategory": "string",   (Flowers, HomeAppliances, Accessories)
  "inStock": "string", (Yes/No)
  "price": double
}

​/api​/Admin​/UpdateProduct?productId={id?}                     <=(update existing product):
{
  "name": "string",
  "description": "string",
  "quantity": int,
  "productCategory": "string",
  "inStock": "string",
  "price": double
}
/api/Admin/DeleteProduct?productId={id?}                    <=(Delete product)
/api/Admin/PurchaseCancellation?purchaseId={id?}            <=(Delete purchase)

## What I have made:
I created models: User, Admin, Bank, Purchase.
I added two-factor authentication. There are Admin and User with different rights.
(I used the following Nuget packages: Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.Identity.EntityFrameworkCore)
I created a User registration and login form.I added validations to the RegisterModels 
(eg the user's age must be 18 years or older, also he can't be registered in the database with an existing user's Mail and UserName).(I used FluentValidator for it.)
I also added validation to the product addition service, which only admin has the right to add. I created services for models and wrote business logic. 
I connected the project to the database(SQL) I created under the name ECommerceSQL(I used: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools for it). Passwords of registered users are included in the database in a hashed state, for this I used: TweetinviAPI.
I added the logs. Logged user data goes to the database, which we access with the select * from dbo.Loggs code in SQL and see information about which User logged in, what role the User has and the time and when it was logged out.
I tested everything with Postman to return the correct status codes on requests. Everything works.
And finally, I created a NUnit project and made a FakeServices folder where I included the services in the project.
I wrote the tests and all 23 tests are successful.
Everything works perfectly.


