namespace Stravaig.ConnOfficer.Domain;

public interface IRawDto<out TDto> : IRawData
{
    public TDto RawDto { get; }
}