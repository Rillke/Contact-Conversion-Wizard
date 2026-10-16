namespace ContactConversionWizard.Core.Text
{
    /// <summary>
    /// How the combined name is built from first name, last name and company.
    /// The numeric values correspond to the entries of the "Combined Name Style" selection.
    /// </summary>
    public enum NameStyle
    {
        LastFirstBracketCompany = 0,         // Lastname Firstname [Company]
        LastCommaFirstBracketCompany = 1,    // Lastname, Firstname [Company]
        LastCommaFirstCommaCompany = 2,      // Lastname, Firstname, Company
        CompanyBracketLastFirst = 3,         // Company [Lastname Firstname]
        CompanyBracketLastCommaFirst = 4,    // Company [Lastname, Firstname]
        CompanyCommaLastCommaFirst = 5,      // Company, Lastname, Firstname
        FirstLastBracketCompany = 6,         // Firstname Lastname [Company]
        FirstCommaLastBracketCompany = 7,    // Firstname, Lastname [Company]
        FirstCommaLastCommaCompany = 8,      // Firstname, Lastname, Company
        CompanyBracketFirstLast = 9,         // Company [Firstname Lastname]
        CompanyBracketFirstCommaLast = 10,   // Company [Firstname, Lastname]
        CompanyCommaFirstCommaLast = 11,     // Company, Firstname, Lastname
        LastFirst = 12,                      // Lastname Firstname
        LastCommaFirst = 13,                 // Lastname, Firstname
        FirstLast = 14,                      // Firstname Lastname
        FirstCommaLast = 15,                 // Firstname, Lastname
    }
}
