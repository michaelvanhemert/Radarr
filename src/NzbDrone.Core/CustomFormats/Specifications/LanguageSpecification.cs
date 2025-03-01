using System.Linq;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.CustomFormats
{
    public class LanguageSpecificationValidator : AbstractValidator<LanguageSpecification>
    {
        public LanguageSpecificationValidator()
        {
            RuleFor(c => c.Value).NotEmpty();
            RuleFor(c => c.Value).Custom((value, context) =>
            {
                if (!Language.All.Any(o => o.Id == value))
                {
                    context.AddFailure(string.Format("Invalid Language condition value: {0}", value));
                }
            });
        }
    }

    public class LanguageSpecification : CustomFormatSpecificationBase
    {
        private static readonly LanguageSpecificationValidator Validator = new LanguageSpecificationValidator();

        public override int Order => 3;
        public override string ImplementationName => "Language";

        [FieldDefinition(1, Label = "Language", Type = FieldType.Select, SelectOptions = typeof(LanguageFieldConverter))]
        public int Value { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(ParsedMovieInfo movieInfo)
        {
            var comparedLanguage = movieInfo != null && Name == "Original" && movieInfo.ExtraInfo.ContainsKey("OriginalLanguage")
                ? (Language)movieInfo.ExtraInfo["OriginalLanguage"]
                : (Language)Value;
            return movieInfo?.Languages?.Contains(comparedLanguage) ?? false;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
