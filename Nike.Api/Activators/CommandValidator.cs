using FluentValidation;

namespace Nike.Api.Activators
{
    public static class CommandValidator
    {
        public static void Validate(CommandBase command)
        {
            if (command is null)
                throw new ValidationException("Command should not be empty .");

            command.Validate();
        }
    }
}