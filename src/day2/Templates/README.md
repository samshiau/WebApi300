# Templates


## Installing

In this directory:

```sh
dotnet new install ./Template.Api  
```

## Using

In the directory you want to create the project:

```sh
dotnet new shoppingapi -n Shopping.Api --openApiName=shopping --database=shopping
```


## Updating

If you change the template, when you are done, uninstall it and reinstall.

```sh
dotnet new uninstall .
dotnet new install .
```