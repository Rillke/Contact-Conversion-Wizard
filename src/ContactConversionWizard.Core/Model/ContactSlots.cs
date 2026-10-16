using System;

namespace ContactConversionWizard.Core.Model
{
    /// <summary>Access to the home, work and mobile number fields of a contact by type.</summary>
    public static class ContactSlots
    {
        public static string Get(Contact contact, PhoneType type)
        {
            switch (type)
            {
                case PhoneType.Home: return contact.Home;
                case PhoneType.Work: return contact.Work;
                case PhoneType.Mobile: return contact.Mobile;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, "Not a number field.");
            }
        }

        public static void Set(Contact contact, PhoneType type, string number)
        {
            switch (type)
            {
                case PhoneType.Home: contact.Home = number; break;
                case PhoneType.Work: contact.Work = number; break;
                case PhoneType.Mobile: contact.Mobile = number; break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, "Not a number field.");
            }
        }

        /// <summary>
        /// Stores an additional number (e.g. Outlook's "Other" number) in the empty home or work field, starting with the
        /// preferred one, optionally also in the mobile field. Returns false if no field was free.
        /// </summary>
        public static bool PutIntoFreeSlot(Contact contact, string number, TypePreference preference, bool includeMobile = false)
        {
            if (string.IsNullOrEmpty(number)) { return true; }
            if (contact.Home == number || contact.Work == number || contact.Mobile == number) { return true; }

            PhoneType first = preference == TypePreference.Home ? PhoneType.Home : PhoneType.Work;
            PhoneType second = preference == TypePreference.Home ? PhoneType.Work : PhoneType.Home;
            foreach (PhoneType type in includeMobile ? new[] { first, second, PhoneType.Mobile } : new[] { first, second })
            {
                if (Get(contact, type) == "")
                {
                    Set(contact, type, number);
                    return true;
                }
            }
            return false;
        }
    }
}
