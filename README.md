# Teams Bridge
This is a Web API designed to expose a simple endpoint to an internal application and in turn utilize and orchestrate the Microsoft Graph API to create Tasks in a specific Planner and Bucket.
This creates a standardized task based on a task model in the specific Planner Bucket, which is to be passed in the body of the call.
I believe this could be extended to other Teams items besides Planner. Additionally, this uses the REST API versus the Graph SDK, so there are certain limitations I ran into. 
This project also uses the "password" grant_type, which I am sure is not ideal, but Planner does have limitations on what kind of delegation can be used. 
## Disclaimer
I'm sure there are things that could have been done better in this, so use at your own risk. This was an opportunity for me to practice both with creating a .NET Core Web API and utilizing the Microsoft Graph REST API.
This is my first swing at a .NET Core Web API, so if anything is entirely wrong or stupid, forgive me and let me know!

### Prerequisites
You must register your application with Azure App Registrations and provide the approprate delegaed permissions first. 
This will provide you with the ```Client ID``` and ```Tenant ID```, both needed in the configuration. 

Planner requires Delegated Group.ReadWrite.All permissions to create tasks. As of this writing, I couldn't find a way to use Application permissions with Planner.
More information can be found by searching basic Azure application and Graph API setup, but most of it was pulled directly from the Planner API docs.

[Microsoft Planner REST API Documentation](https://docs.microsoft.com/en-us/graph/api/resources/planner-overview?view=graph-rest-1.0)

Additionally, when you register your application on Azure App Registrations, you'll be proivided with a ```Client Secret```, also needed for the config.

A valid Office 365 User was also used in this configuration, complete with a ```Username``` and ```Password``` in the config. 
In this case, I added the user to my specific Team in Teams where my Planner lived. 

I would have liked to use a different method to aquire delegated permissions, but this was a bit of a quick and dirty way.

So to recap, you'll need the following:

Config Setting  | Where From
------------- | -------------
ClientID  | Azure App Registration
TenantID  | Azure App Registration
ClientSecret | Azure App Registration
Username | Office 365 User for Service Account (enter as FQDN)
Password | Password for service account


## Running the API
After editing the configuration in the appsettings.json file, run the API. I set the browser to not launch, but if you choose to it will display "I'm Alive" with a ```GET``` on ```api/Planner``` 
I used Postman to test the API.

In the body of the request, pass the ``PlanID`` and the ``BucketID`` of the Planner/Bucket you'd like to create the Task in. 

```
{
	"PlanID":"yourPlanID",
	"BucketID":"yourBucketID"
}
```
Make sure the content type is ``application\json`` and send a ``POST`` request 
to ``https://localhost:{yourport}/api/Planner``

A successful response will contain the response from the Task Header from Microsoft Graph and you will have created a Task in the specific planner bucket configured in the call.

## How it Works
At Startup, we are creating a Scoped ``TaskService``, which basically handles all of the various calls needed to create a Task.
I chose a Scoped service at startup so that each time our Planner endpoint was hit, it would instantiate a Task Service, which is injected into the ``PlannerController`` at runtime.

So with our ``TaskService`` injected, the ``POST`` will then call our ``TaskService.CreateTask`` method asynchronously and await the response. 
The Planner controller, when ``POST`` is invoked, is expecting to bind the body of the call to our ``Bucket`` model.  This was probably unnecessary, but I like the idea of it for documenting the API.

The ``TaskService`` is injected with the options pattern ``IOptions`` and sets local variables from the ``appsettings.json`` file. 
I chose this because it seemed to have the most flexiblity if we wanted to extend the different configurations used. 

The first thing the ``CreateTask`` method does it tries to get a Delegate Token from Graph. If successful, we will retrn this token and use it in subsequent calls.

Next, we create a ``RestClient``, add our JWT Token as Authentication, create a dynamic object representing some basic Task header level information, serialize it, and call the Graph endpoint
``/v1.0/planner/tasks`` with a ``POST``. If successful, this will create the Task Header in the specific Planner/Bucket. 
After that, we want to add some task Details. To do this, we parse the response and obtain the ``TaskID``, then call a `GET` on `/v1.0/planner/tasks/{taskID}/details`

This step is important, because although we have an `eTag` from our newly created Task, we need to get the most recent `eTag` for our Task Details.
Parse the response from the `GET` to get the current `eTag`, then we call our `UpdateTaskDetails` method passing in the `eTag`, `token`, and `taskID`.

The ``UpdateTaskDetails`` method creates the serialized JSON from a special method available on our `TaskDetails` model. This is where I configured the data needed/desired for the Task and where one would likely extend the system to have multiple versions of tasks.
With that, we call Graph at `/v1.0/planner/tasks/{taskID}/details` with a `PATCH` and our serialized Task data and await a response.
As of this writing, there is a bug in Graph where it will return an OK resule as NoContent, which this handles.

So to recap, the flow is 

~~~
1. Call api/Planner with POST and send the PlanID and BucketID as json body
2. We obtain a token from Graph
3. Create the Task in the specific planner/bucket
4. Call GET on the newly created Task Details to get current eTag
5. PATCH the record with our new TaskDetail information
6. Await the response 
~~~

# Wrapping Up
I didn't deploy this outside of testing, so not sure what else I'm missing in that regard yet. 
I did have a few "gotcha's" along the way that I'll comment on.

First, the Microsoft Graph REST API was documented ok, but there were very few examples on proper use and instead pointed people in the direction of the Graph testing site. This is problematic given that the Planner API doesn't appear to be designed to be used with service accounts. 

Secondly, I only found like 1 example of the SDK in use for Planner, which was a huge factor in me choosing to use the REST API vs. the Graph SDK. I am still unsure if there is a proper way to handle the delegated permissions outside of the password grant.

Third, the Graph request and response schemas make it quite difficult to utilize dynamic C# objects, which is why I opted for complex models decorated with the `JsonProperty` tags. 
It was also not abundently clear that the `eTag` returned from our Task creation was not the same `eTag` required to update the Task details.

Lastly, I'm sure there could be much more elegant response handling especially within my `TaskService`, but this was good enough for testing. 

Thanks for reading!


