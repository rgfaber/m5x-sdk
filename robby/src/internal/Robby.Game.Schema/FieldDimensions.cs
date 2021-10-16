using FluentValidation;
using M5x.DEC.Schema.Common;

namespace Robby.Game.Schema
{

    public static class Helper
    {
        public static void Validate(this FieldDimensions dimensions)
        {
            var v = new FieldDimensions.Validator();
            var res = v.Validate(dimensions);
            if(res.IsValid) return;
            throw new ValidationException(string.Join('\n', res.Errors));
        }
    } 
    
    public record  FieldDimensions : Vector
    {
        public class Validator : AbstractValidator<FieldDimensions>
        {
            public Validator()
            {
                RuleFor(fd => fd.X).GreaterThan(19);
                RuleFor(fd => fd.Y).GreaterThan(19);
                RuleFor(fd => fd.Z).GreaterThan(19);
            }
        }
        
        public FieldDimensions(): base(20,20,20)
        {
        }

        public FieldDimensions(int x, int y, int z) : base(x, y, z)
        {
        }
    }
}