namespace DonationHub.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Money(decimal amount, string currency) 
        {
            if (amount < 0)
                throw new ArgumentException("Amount can not be less than Zero!",nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required!",nameof(currency));
            currency = currency.Trim().ToUpperInvariant();
            if (currency.Length != 3 || !currency.All(char.IsLetter))
                throw new ArgumentException("Currency must be a 3 letter code!",nameof(currency));
            Amount = amount;
            Currency = currency;
        }
        protected override IEnumerable<object?> GetEqualityComponents() 
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
