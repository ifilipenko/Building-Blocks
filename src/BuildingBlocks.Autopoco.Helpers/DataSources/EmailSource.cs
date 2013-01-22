using AutoPoco.DataSources;
using AutoPoco.Engine;

namespace BuildingBlocks.Autopoco.Helpers.DataSources
{
    public class EmailSource : DatasourceBase<string>
    {
        private int _index;
        private readonly FirstNameSource _firstNameSource;

        public EmailSource()
        {
            _firstNameSource = new FirstNameSource();
        }

        public override string Next(IGenerationContext context)
        {
            var firstName = _firstNameSource.Next(context);
            return string.Format("{0}{1}@example.com", firstName, ++_index);
        }
    }
}