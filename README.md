# AweformAPI.Net
This is a simple Aweform API consumer built for Microsoft .Net. For more information about the Aweform API check out the documentation at https://aweform.com/help/api/

## Usage

To get started with the API first grab your Aweform API key from your Account page. Second, create a new instance of the Aweform API object:

```
AweformAPI aweformAPI = new AweformAPI(API_KEY);
```

Now you are ready to start accessing your Aweform data. First let's grab a copy of the current user just to verify that you are who you actually think you are:

```
AweformUser user = aweformAPI.GetMe();
Console.WriteLine(user.Name);
```

With the API you can access several different types of objects, you can list your Workspaces, Forms, FormDefinitions and Responses. Each of these is incapsulated in the corresponding Aweform* classes.

The most common use case is to access Response information, the following code grabs the five most recent Responses regardless of which Form they belong to and dump some key details to the console:

```
List<AweformResponse> responses = aweformAPI.GetResponses(0, 5);

foreach (AweformResponse response in responses) {

  Console.WriteLine(response.Id + " .. " + response.DateInUtc);

  foreach (AweformQuestionAndAnswer questionAndAnswer in response.Answers) {

    Console.WriteLine("- " + questionAndAnswer.Question + ": " + questionAndAnswer.Answer);
  }
}
```
