using Identity.Reconcilation.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Reconcilation.Data
{
    public class ConsolidatedContact : IConsolidatedContact
    {
        private readonly ContactContext _contactContext;

        public ConsolidatedContact(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }
        public async Task<ResponseData> GetConsolidatedContact(Contact data)
        {
            var result = await _contactContext.Contacts.Where(p => p.Email == data.Email || p.PhoneNumber == data.PhoneNumber).ToListAsync()
                .ConfigureAwait(false);
            ResponseData responseData = new ResponseData();
            if (data.LinkPrecedence == "primary")
            {
                responseData.primaryContatctId = data.Id;
                responseData.emails = new string[] { data.Email };
                responseData.phoneNumbers = new string[] { data.PhoneNumber };
                responseData.secondaryContactIds = null;
            }
            else
            {
                foreach(var item in result)
                {
                    if(item.LinkPrecedence == "primary")
                    {
                        responseData.primaryContatctId = item.Id;
                    }
                    else
                    {
                        responseData.secondaryContactIds.Append(item.Id);
                    }
                    responseData.emails.Append(item.Email);
                    responseData.phoneNumbers.Append(item.PhoneNumber);
                }
            }
            return responseData;
        }
    }
}
