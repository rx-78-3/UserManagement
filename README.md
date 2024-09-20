# Instructions

## Structure
All the project's items are located in **src\UserManagement** directory. The frontend isn't added to main solution and is placed in **user-management-web-ui** directory and can be opened separately.

## Running
You can run the project by selecting **docker-compose** Startup Item. Database initializes automatically. After starting you will be redirected to **http://localhost:8080/#/login** page which is the root page of frontend.

## Login
You can log in with follows credentials:
Username: someUser1
Password: password123

Also, the system has other users from someUser2 to someUser15. Odd ones are active, even ones are inactive.

## Testing and documentation
- The UserManagement module has exported Postman collection and Postman environment and can be imported for testing. You can find Postman files at the address **src\UserManagement\Modules\UserManagement.Api\Postman\**.
- Both services, **Identity.Api** and **UserManagement.Api**, have a connected swagger and it can be reached via addresses **http://localhost:5002/swagger** and **http://localhost:5001/swagger**.
**Identity.Api** has unit tests located at address **src\UserManagement\Tests\Identity.ApiTests\**

# Solutions justification

## Microservice-like architecture + Using MediatR and Vertical Slice Architecture
I have never worked with this type of architecture and I wanted to try to implement something similar. And since the task is quite simple, this is a good chance to try approaches that have not been used before.

## Dapper
I knew that the company I was doing the test task for used pure SQL with stored procedures. So I used the most popular micro ORM to transfer "raw" SQL.

## What could have been done better, but since the task was a test, it was decided not to overcomplicate it
- No established migration process - there are a couple of homemade initializers.
- No resource files - there are static hardcoded strings.
- Using sha256 rather than a more serious solution like pbkdf2 with generation and storage of a unique salt for each user.
- Using symmetric key for JWT instead of asymmetric key.
- JWT token, not OAuth 2.0 with refresh token.