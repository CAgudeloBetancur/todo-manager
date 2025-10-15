// See https://aka.ms/new-console-template for more information

using ToDoManager.Api;
using ToDoManager.Application;
using ToDoManager.Infrastructure;

var api = new TodoManagerApi(args, (services, configuration) =>
{
	services
		.AddPresentation()
		.AddApplication()
		.AddInfrastructure(configuration);
});

await api.StartAsync();
