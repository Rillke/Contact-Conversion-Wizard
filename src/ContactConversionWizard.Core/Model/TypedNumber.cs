using System.Collections.Generic;

namespace ContactConversionWizard.Core.Model
{
    /// <summary>The kind of a phone or fax number in <see cref="Contact.Numbers"/>.</summary>
    public enum NumberKind
    {
        Home,
        Work,
        Mobile,
        HomeFax,
        WorkFax,

        /// <summary>Numbers without (known) type, e.g. FRITZ!Box "other" or vCard <c>TEL;VOICE</c>.</summary>
        Other,
    }

    /// <summary>A number with its kind (immutable).</summary>
    public sealed class TypedNumber
    {
        public TypedNumber(NumberKind kind, string number, bool preferred = false, string sourceType = "")
        {
            Kind = kind;
            Number = number ?? "";
            Preferred = preferred;
            SourceType = sourceType ?? "";
        }

        public NumberKind Kind { get; }

        public string Number { get; }

        /// <summary>Marked as preferred number by the source (FRITZ!Box <c>prio="1"</c>, vCard <c>TYPE=pref</c>).</summary>
        public bool Preferred { get; }

        /// <summary>The type as written in the source, e.g. <c>intern</c> for an internal FRITZ!Box number.</summary>
        public string SourceType { get; }

        /// <summary>The voice number kind of a <see cref="PhoneType"/>.</summary>
        public static NumberKind FromPhoneType(PhoneType type)
        {
            switch (type)
            {
                case PhoneType.Home: return NumberKind.Home;
                case PhoneType.Work: return NumberKind.Work;
                case PhoneType.Mobile: return NumberKind.Mobile;
                default: return NumberKind.Other;
            }
        }

        /// <summary>
        /// The numbers of a contact with their kinds: <see cref="Contact.Numbers"/> if the source provided them,
        /// otherwise the single number fields.
        /// </summary>
        public static IReadOnlyList<TypedNumber> AllOf(Contact contact)
        {
            if (contact.Numbers.Count > 0) { return contact.Numbers; }

            var numbers = new List<TypedNumber>();
            PhoneType preferred = contact.ExplicitPreferred;
            void Add(NumberKind kind, string number, PhoneType type = PhoneType.None)
            {
                if (number != "") { numbers.Add(new TypedNumber(kind, number, type != PhoneType.None && type == preferred)); }
            }
            Add(NumberKind.Home, contact.Home, PhoneType.Home);
            Add(NumberKind.Work, contact.Work, PhoneType.Work);
            Add(NumberKind.Mobile, contact.Mobile, PhoneType.Mobile);
            Add(NumberKind.HomeFax, contact.HomeFax);
            Add(NumberKind.WorkFax, contact.WorkFax);
            return numbers;
        }

        public override string ToString() => Kind + ": " + Number;
    }
}
