using Identity.Reconcilation.Data;
using Identity.Reconcilation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Identity.Reconcilation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactContext _contactContext;
        private readonly IConsolidatedContact _consolidatedContact;
        public ContactController(ContactContext contactContext, IConsolidatedContact consolidatedContact)
        {
            _contactContext = contactContext;
            _consolidatedContact = consolidatedContact;
        }

        [HttpGet]
        public async Task<List<Contact>> GetContacts()
        {
            if(_contactContext == null)
            {
                return new List<Contact>();
            }
            return await _contactContext.Contacts.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<Contact> GetContactsById(int id)
        {
            if (_contactContext == null)
            {
                return new Contact();
            }
            return await _contactContext.Contacts.FindAsync(id).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<ResponseData> PostContact([FromBody] PostParameters contact)
        {
            //await _contactContext.Contacts.AddAsync(contact).ConfigureAwait(false);
            //var contactResult = await GetContactsById(contact.Id);
            var isParentContact = await _contactContext.Contacts.FirstOrDefaultAsync(p => p.Email == contact.email || p.PhoneNumber == contact.phoneNumber)
                .ConfigureAwait(false);
            Contact saveResult = new Contact();
            ResponseData responseData = new ResponseData();
            if (isParentContact == null)
            {
                saveResult.Email = contact.email;
                saveResult.PhoneNumber = contact.phoneNumber;
                saveResult.LinkPrecedence = "primary";
                await _contactContext.AddAsync(saveResult);
                await _contactContext.SaveChangesAsync();
                responseData = await _consolidatedContact.GetConsolidatedContact(saveResult);
            } 
            else
            {
                saveResult.Email = contact.email;
                saveResult.PhoneNumber = contact.phoneNumber;
                saveResult.LinkPrecedence = "secondary";
                saveResult.LinkedId = isParentContact.Id;
                await _contactContext.AddAsync(saveResult);
                await _contactContext.SaveChangesAsync();
            }

            var contactResult = new Contact();
            return responseData;
        }
    }
}
