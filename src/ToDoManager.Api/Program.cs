
public class TodoManagerApi
{
	private readonly WebApplication _app;
	
	public TodoManagerApi(
		string[] args, 
		Action<IServiceCollection, IConfiguration> options
		)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		options.Invoke(builder.Services, builder.Configuration);

		_app = builder.Build();

		// Configure the HTTP request pipeline.
		
		if (_app.Environment.IsDevelopment())
		{
			_app.UseSwagger();
			_app.UseSwaggerUI();
		}

		_app.UseHttpsRedirection();

		_app.UseAuthorization();

		_app.MapControllers();
	}

	public Task StartAsync()
	{
		return _app.RunAsync();
	}
    
}

