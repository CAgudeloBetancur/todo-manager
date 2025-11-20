using FluentValidation;

namespace ToDoManager.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandValidator :  AbstractValidator<UpdateTagCommand>
{
	public UpdateTagCommandValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Name is required");
		
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Id is required");
	}
};