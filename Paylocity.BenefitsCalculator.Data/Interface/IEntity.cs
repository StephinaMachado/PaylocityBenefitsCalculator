
namespace Paylocity.BenefittsCalculator.Data.Interface;

public interface IEntity
{
    public int Id { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }

}
