
# MovieRental Exercise - Answered

  

This is my revised version of this README file, alongside the questions there are my considerations.


**The app is throwing an error when we start, please help us. Also, tell us what caused the issue.**

  
The issue was caused by a wrong lifetime definition. The DbContext used by EF Core were using a scoped lifetime, and the RentalFeature service were being registered as a singleton: 
	 
```csharp
	builder.Services.AddEntityFrameworkSqlite().AddDbContext<MovieRentalDbContext>();
	builder.Services.AddSingleton<IRentalFeatures, RentalFeatures>();
```
I decided to register RentalFeatures as a scoped service because it makes sense that for each request I create a DbContext instance.
  
  
**The rental class has a method to save, but it is not async, can you make it async and explain to us what is the difference?**

The difference between an async and a "sync" method is that a "sync" method locks the thread that is is being executed on. For our method __Save()__ it means that the thread would be locked until EF Core finished saving the entry. A good pratice when working with EF Core  (or other I/O bound operations) is to use async/await pattern, which allows the thread to be released and used for other work while it waits for an _awaited_ operation to complete. This make the application more efficient and scalable.

**In the MovieFeatures class, there is a method to list all movies, tell us your opinion about it.**

Syntactically there is nothing wrong with the following: 

```csharp
public List<Movie> GetAll()
{
	return _movieRentalDb.Movies.ToList();
}
//It works!
```

But semantically there are some points that can be improved.
* Async Method and _Async_ suffix - As the previous example, this is an I/O Bound operation, so its a good pratice to use async/await pattern to deal with it. Also, using the _Async_ suffix in the method name makes it more clear that this is an asynchronous method.
* AsNoTracking() usage -  Since the method is a pure lookup, with no changes to be done. We can use the **AsNoTracking()** method to tell EF Core that we don't need to keep track of the changes made on the objects returned by the query. This reduces the memory usage and the work on the ChangeTracker, which usually makes the queries more efficient.
* Interfaces x Concrete - Returning a concrete type is completely okay, but using an abstract type in the signature (IEnumerable or IReadOnlyList for example) can make the signature more idiomatic.

Example of the method after adjustments:
```csharp
public async Task<IEnumerable<Movie>> GetAllAsync()
{
	return await _movieRentalDb
		.Movies
		.AsNoTracking()
		.ToListAsync();
}
```



**No exceptions are being caught in this api, how would you deal with these exceptions?**
I decided to deal with the exceptions by using a middleware that implements the IExceptionHandler interface, and registering it as a global exception handler. This allows me to deal with concerns like logging, formating the response, and ensuring consistent error handling in the entire application.

  
  

## Challenge (PaymentProviders)

For the payment providers, I decided to use an interface to define the behaviour of a __PaymentProvider__ without coupling the __RentalFeatures__ Service to any concrete class. This allows me to extend the beahviour of payment providers by adding new classes that implement the __IPaymentProvider__ interface.

To determine which payment provider should to be used based on the value of the entry, I decided to use the IKeyedServiceProvider, which allows the DI container to resolve the correct implementation at runtime, based on a key (in this case the _PaymentMethod_ property of a Rental object). 
A different approach would be to use a Factory to handle the DI resolution manually, this way I could have a centralized and flexible place to apply more elaborate business rules, the use of logging, or even use other types (like enums) instead of strings to define the Service.

The **IKeyedServiceProvider** was chosed mainly because it was simpler to use and the version of .NET used has native support to keyed services, but wheter the version was older or I needed a more flexible solution I'd probably go for the factory. 


## Improvements

Throughout the resolution of the test I identified some points that I believe that could be improved, but I decided not to do it to keep the solution simple and to resolve it faster, but I decided to mention here as a section.

#### Respository Pattern

I'd use a repository pattern by creating repositories to deal with data handling aspects (querying, insert etc.)
This would allow me to separate the concerns with Data, (repositories), from my business Rules (Services).


#### Request and Response Contracts. 
In the Services we are using the types of the entities as the contracts of request or response
for example:
```csharp
public Rental Save(Rental rental)
{
	//Implementation
}
```
In this case, **Rental** is being used as both the parameter type and return type. This works but having a contract to define what is needed when calling a method and what the client can see in the return may be the best option. 

In fact, I did created a **CreateRentalRequest** type, for the Save() method inside the RentalFeatures service, because it allowed me to define what I wanted to receive from the client. 

```csharp
public record CreateRentalRequest(int DaysRented, int MovieId, int CustomerId, string PaymentMethod = default!);

//_________________
public async Task<Rental> SaveAsync(CreateRentalRequest request) { //implementation }
```

Also, for the result of a service call, one good thing to improve is the result by adding a ServiceResult
For example: 
```csharp
public record ServiceResult<T>(T? Value, bool Success, string Error)
```

This provides a more explicit and consistent way of represent the outcome of service calls, by allowing services to communicate success, errors and metadata in a structured way

Example usage in controller:
```csharp
public async Task<IActionResult> SomeMethod() 
{
		var serviceResult = await service.SomeMethod();
		return serviceResult.Success ? Ok("Ok") : HandleError(serviceResult.Error);
		// Handle error could define which status code return based on the error.
}
```
This pattern helps centralize error handling, makes responses more predictable, and allows the application to map service level errors to proper HTTP status codes.

#### Validation and a better Error Handling 

The API lacks validation. One good improvement would be to add validation to the endpoints, preventing wrong user input that could lead to errors.  
Alongside these validations, the API could benefit from better error handling (even though I added a GlobalExceptionHandler for the exceptions) and a more structured method result, ensuring that the user can clearly understand what went wrong by receiving specific status codes and clear response messages.

#### Testing
One way o ensuring that the API is behaving as expected is by adding test (in this case unit tests).
The methods used in the solution are simple and writing many unit tests for simple behaviours would be a overhead, but I think it is fair to mention that both the functionality and code quality could benefit from unit tests (if it was a growing application)
In our scenario, one specific mathod that could be tested is the one that handles the creation of a rental (since it is the one with relatively more business logic)

Some tests examples could be:
```csharp
public Task RentalFeatures_SaveAsync_IsPaymentOk_CreateRental();
public Task RentalFeatures_SaveAsync_IsPaymentFailed_ReturnNull();
public Task RentalFeatures_SaveAsync_PaymentMethodNotExists_ThrowErrorAsync();
```


