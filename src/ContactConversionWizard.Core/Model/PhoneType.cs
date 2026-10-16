namespace ContactConversionWizard.Core.Model
{
    /// <summary>The kind of a (voice) phone number of a contact.</summary>
    public enum PhoneType
    {
        None = 0,
        Home,
        Work,
        Mobile,
    }

    /// <summary>Whether ambiguous data (fax, address, other numbers) is treated as home or as work data.</summary>
    public enum TypePreference
    {
        Home = 0,
        Work = 1,
    }

    /// <summary>How the VIP flag of a contact is derived from its source data.</summary>
    public enum VipMode
    {
        Never = 0,
        IfNickname = 1,
        IfNotesContainVip = 2,
    }
}
