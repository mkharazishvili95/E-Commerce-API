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
I created models: User, Product, BankModel, PurchaseModel.
I created 2Role-based authorization. (I used the following Nuget packages: Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Authentication.JwtBearer) I have Admin and User in the project. To receive services, a person must first register. I created a user registration model and a login model. I made validations for the registration model (eg the user's age must be 18 or older and the UserName and E-mail must not be identical to the UserName and E-Mail of the user in the database). For validations I used: FluentValidation. After user registration and logging in, log information is stored in the database (Select * from dbo.Loggs - outputs UserName, role and log-in date of the logged-in user), for this I created a Logs model.
After logging in, the user is assigned a token that is generated (I specified a conditional time of 365 days - 1 year)
And after writing the secret key of this token to JWT.IO, this token becomes valid, which allows the user to perform various operations and receive services.
I created the already mentioned Product model, and I also made validations for it. Only admin can add products. I also created services: AdminService, UserService, ProductService. I wrote business logic in it. Users who are already logged in can see their balance, as well as information on their profile. View the list of added products and buy any. After purchasing the products, the amount on his balance will be transferred to the bank account that I have created. He can also see his purchases.
The admin has the right to get information about all users, get information about specific users by Id, and also get information about users sorted by ShippingAddress.
He also has the right to block any user (except another admin). A blocked user is not allowed to purchase the product.
Admin also has the right to unblock a blocked user.
It can also add a new product or modify an existing one. Also delete the product (after deleting the product, if any customer has purchased this product, the money is automatically returned from the bank account). Also cancel any customer order. (After cancellation, the user will automatically get back the amount paid for this product, which will be transferred from the bank to his account)
I created a Context file with the name: ECommerceContext, where I included the models in the project that I wanted to transfer to the database. It also creates an admin in the database after adding migrations.
I connected the project with the database (SQL that I created under the name of E-CommerceSQL).
For this I used: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools.
The passwords entered in the database are in a hashed state, for this I used: TweetinviAPI.
I tested the project with Postman to output the status codes I expected and everything works fine.
I also created a NUnit project where I created FakeServices where I included the existing services in the project. I wrote the tests and all 23 tests ran successfully.
Everything works perfectly.
