using Identity.Reconcilation.Models;

namespace Identity.Reconcilation.Data
{
    public interface IConsolidatedContact
    {
        public Task<ResponseData> GetConsolidatedContact(Contact data);
    }
}
